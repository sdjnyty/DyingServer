using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using EasyHook;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;

namespace Injector
{
  public class HookEntryPoint : IEntryPoint
  {
    private const int LOCALHOST = 0x100007f;
    private string _userId;
    private int _vip;
    private int _pid;
    private string _channel;
    private ServerInterface _server;
    private string _dllPath;
    private string _hostName;
    private ConcurrentDictionary<int, SocketInfo> _dicSockets = new ConcurrentDictionary<int, SocketInfo>();
    private string _moduleName;
    private SockAddr _udpOutgoing = new SockAddr
    {
      Family = (short)AddressFamily.InterNetwork,
      IP = LOCALHOST,
      Port1 = 123,
      Port2 = 123,
    };
    private SockAddr _tcpOutgoing = new SockAddr
    {
      Family = (short)AddressFamily.InterNetwork,
      IP = LOCALHOST,
      Port1 = 123,
      Port2 = 123,
    };

    public HookEntryPoint(RemoteHooking.IContext context, string channel, string dllPath, int vip, string userId)
    {
      _channel = channel;
      _server = RemoteHooking.IpcConnectClient<ServerInterface>(channel);
      _pid = NativeAPI.GetCurrentProcessId();
      _dllPath = dllPath;
      _vip = vip;
      _userId = userId;
    }

    public void Run(RemoteHooking.IContext context, string channel, string dllPath, int vip, string userId)
    {
      _moduleName = Process.GetCurrentProcess().ProcessName;
      var hSendTo = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "sendto"), new Delegates.SendTo(SendTo), this);
      hSendTo.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hRecvFrom = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recvfrom"), new Delegates.RecvFrom(RecvFrom), this);
      hRecvFrom.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hCreateProcessA = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "CreateProcessA"), new Delegates.CreateProcessA(CreateProcessA), this);
      hCreateProcessA.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hWSARecvFrom = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "WSARecvFrom"), new Delegates.WSARecvFrom(WSARecvFrom), this);
      hWSARecvFrom.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hAccept = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "accept"), new Delegates.Accept(Accept), this);
      hAccept.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hConnect = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "connect"), new Delegates.Connect(Connect), this);
      hConnect.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadLibraryA = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "LoadLibraryA"), new Delegates.LoadLibraryA(LoadLibraryA), this);
      hLoadLibraryA.ThreadACL.SetExclusiveACL(new[] { 0 });
      DllImports.LoadLibraryA("wsock32");
      var hGetPeerName = LocalHook.Create(LocalHook.GetProcAddress("wsock32", "getpeername"), new Delegates.GetPeerName(GetPeerName), this);
      hGetPeerName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetSockName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "getsockname"), new Delegates.GetSockName(GetSockName), this);
      hGetSockName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var sb = new StringBuilder();
      DllImports.gethostname(sb, 50);
      _hostName = sb.ToString();
      var hGetHostName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "gethostname"), new Delegates.GetHostName(GetHostName), this);
      hGetHostName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetHostByName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "gethostbyname"), new Delegates.GetHostByName(GetHostByName), this);
      hGetHostByName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hSend = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "send"), new Delegates.Send(Send), this);
      hSend.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hWSASend = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "WSASend"), new Delegates.WSASend(WSASend), this);
      hWSASend.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hWSARecv = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "WSARecv"), new Delegates.WSARecv(WSARecv), this);
      hWSARecv.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hRecv = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recv"), new Delegates.Recv(Recv), this);
      hRecv.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hSocket = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "socket"), new Delegates.Socket(Socket), this);
      hSocket.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hBind = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "bind"), new Delegates.Bind(Bind), this);
      hBind.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hCloseSocket = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "closesocket"), new Delegates.CloseSocket(CloseSocket), this);
      hCloseSocket.ThreadACL.SetExclusiveACL(new[] { 0 });
      RemoteHooking.WakeUpProcess();
      Thread.Sleep(Timeout.Infinite);
    }

    private int Accept(int socket, out SockAddr addr, ref int addrLen)
    {
      var ret = DllImports.accept(socket, out addr, ref addrLen);
      if (addr.IP == LOCALHOST)
      {
        var buff = Marshal.AllocHGlobal(8);
        var count = 0;
        while (count < 8)
        {
          count += DllImports.recv(ret, buff+count, 8 - count, 0);
        }
        addr.IP = Marshal.ReadInt32(buff);
        var remotePort = Marshal.ReadInt32(buff + 4);
        addr.Port1 = (byte)(remotePort >>8);
        addr.Port2 = (byte)remotePort;
        var siListen = _dicSockets[socket];
        var siAccept = new SocketInfo
        {
          Socket = ret,
          LocalIp = siListen.LocalIp,
          LocalPort = siListen.LocalPort,
          RemoteIp = new IPAddress(addr.IP),
          RemotePort = remotePort,
        };
        _dicSockets[ret] = siAccept;
        Marshal.FreeHGlobal(buff);
        _server.Echo($"{_moduleName} accept {socket} {siAccept}");
      }
      return ret;
    }

    private SocketError Bind(int socket, ref SockAddr addr, int addrLen)
    {
      var ret = DllImports.bind(socket, ref addr, addrLen);
      if (ret == SocketError.Success && _dicSockets.TryGetValue(socket, out var si))
      {
        si.LocalIp = new IPAddress(addr.IP);
        if (addr.Port1 == 0 && addr.Port2 == 0)
        {
          var addr1 = new SockAddr();
          var addrLen1 = 16;
          DllImports.getsockname(socket, out addr1, ref addrLen1);
          si.LocalPort = addr1.Port1 * 256 + addr1.Port2;
        }
        else
        {
          si.LocalPort = addr.Port1 * 256 + addr.Port2;
        }
        _server.Echo($"{_moduleName} bind {si}");
      }
      return ret;
    }

    private SocketError CloseSocket(int socket)
    {
      if (_dicSockets.TryRemove(socket, out var si))
      {
        _server.Echo($"{_moduleName} closesocket {si}");
      }
      return DllImports.closesocket(socket);
    }

    private SocketError Connect(int socket, ref SockAddr addr, int addrLen)
    {
      SocketError ret;
      var si = _dicSockets[socket];
      si.RemoteIp = new IPAddress(addr.IP);
      si.RemotePort = addr.Port1 * 256 + addr.Port2;
      _server.Echo($"{_moduleName} connect {si}");
      if ((addr.IP & 0xff) == 10)
      {
        ret = DllImports.connect(socket, ref _tcpOutgoing, addrLen);
        if (ret == SocketError.Success)
        {
          var pBuff = Marshal.AllocHGlobal(8);
          Marshal.WriteInt32(pBuff, addr.IP);
          Marshal.WriteInt32(pBuff + 4, addr.Port1 * 256 + addr.Port2);
          DllImports.send(socket, pBuff, 8, 0);
          Marshal.FreeHGlobal(pBuff);
        }
        else if (ret == SocketError.SocketError && DllImports.WSAGetLastError() == SocketError.WouldBlock)
        {
          var write = new fd_set
          {
            Count = 1,
            Socket = socket,
          };
          var except = write;
          DllImports.select(0, IntPtr.Zero, ref write, ref except, IntPtr.Zero);
          var pBuff = Marshal.AllocHGlobal(8);
          Marshal.WriteInt32(pBuff, addr.IP);
          Marshal.WriteInt32(pBuff + 4, addr.Port1 * 256 + addr.Port2);
          DllImports.send(socket, pBuff, 8, 0);
          Marshal.FreeHGlobal(pBuff);
          DllImports.WSASetLastError(SocketError.WouldBlock);
        }
      }
      else
      {
        ret = DllImports.connect(socket, ref addr, addrLen);
      }
      return ret;
    }

    private bool CreateProcessA(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, IntPtr startupInfo, out ProcessInformation processInformation)
    {
      RemoteHooking.CreateAndInject(applicationName, commandLine, 0, _dllPath, _dllPath, out var pid, _channel, _dllPath, _vip, _userId);
      processInformation = new ProcessInformation
      {
        ProcessId = pid,
      };
      return true;
    }

    private IntPtr GetHostByName(IntPtr pName)
    {
      var name = Marshal.PtrToStringAnsi(pName);
      IntPtr ret;
      if (name == _userId)
      {
        ret = DllImports.gethostbyname(_hostName);
        var he = (HostEnt)Marshal.PtrToStructure(ret, typeof(HostEnt));
        Marshal.WriteInt32(Marshal.ReadIntPtr(he.AddrList), _vip);
        Marshal.StructureToPtr(he, ret, false);
      }
      else
      {
        ret = DllImports.gethostbyname(name);
      }
      return ret;
    }

    private SocketError GetHostName(IntPtr name, int nameLen)
    {
      var bytes = Encoding.ASCII.GetBytes(_userId.ToString() + "\0");
      Marshal.Copy(bytes, 0, name, bytes.Length);
      return SocketError.Success;
    }

    private SocketError GetPeerName(int socket, out SockAddr name, out int nameLen)
    {
      var ret = DllImports.getpeername(socket, out name, out nameLen);
      if (ret == SocketError.Success && name.IP == LOCALHOST)
      {
        var si = _dicSockets[socket];
        name.IP =BitConverter.ToInt32(si.RemoteIp.GetAddressBytes(), 0);
        name.Port1 = (byte)(si.RemotePort >> 8);
        name.Port2 = (byte)si.RemotePort;
        _server.Echo($"{_moduleName} getpeername {si}");
      }
      return ret;
    }

    private SocketError GetSockName(int socket, out SockAddr addr, ref int addrLen)
    {
      var ret = DllImports.getsockname(socket, out addr, ref addrLen);
      if (ret == SocketError.Success)
      {
        addr.IP = _vip;
      }
      return ret;
    }

    private IntPtr LoadLibraryA(string fileName)
    {
      return DllImports.LoadLibraryA(fileName);
    }

    private int Recv(int socket, IntPtr buff, int len, int flags)
    {
      var ret = DllImports.recv(socket, buff, len, flags);
      if (_dicSockets.TryGetValue(socket, out var si))
      {
        if (ret == 0)
        {
          _server.Echo($"{_moduleName} recv {si} closed by remote");
          _dicSockets.TryRemove(socket, out var _);
        }
        else
        {
          _server.Echo($"{_moduleName} recv {si} len={ret}");
        }
      }
      return ret;
    }

    private int RecvFrom(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen)
    {
      var result = DllImports.recvfrom(socket, buff, len, flags, ref from, ref fromLen);
      if (result > 0 && from.IP == LOCALHOST && from.Port1 == 122 && from.Port2 == 122)
      {
        from.IP = Marshal.ReadInt32(buff);
        from.Port2 = Marshal.ReadByte(buff + 4);
        from.Port1 = Marshal.ReadByte(buff + 5);
        DllImports.CopyMemory(buff, buff + 6, result - 6);
        _server.Echo($"{_moduleName} recvfrom from={from} len={result}");
        return result - 6;
      }
      else
      {
        return result;
      }
    }

    private int Send(int socket, IntPtr buff, int len, int flags)
    {
      if (_dicSockets.TryGetValue(socket, out var si))
      {
        _server.Echo($"{_moduleName} send {si} len={len}");
      }
      return DllImports.send(socket, buff, len, flags);
    }

    private int SendTo(int socket, IntPtr buff, int len, int flags, ref SockAddr to, int toLen)
    {
      if ((to.IP & 0xff) == 10)
      {
        var pBuff = Marshal.AllocHGlobal(len + 6);
        Marshal.WriteInt32(pBuff, to.IP);
        Marshal.WriteByte(pBuff + 4, to.Port2);
        Marshal.WriteByte(pBuff + 5, to.Port1);
        DllImports.CopyMemory(pBuff + 6, buff, len);
        DllImports.sendto(socket, pBuff, len + 6, flags, ref _udpOutgoing, toLen);
        _server.Echo($"{_moduleName} sendto to={to} len={len}");
        Marshal.FreeHGlobal(pBuff);
        return len;

      }
      else
      {
        return DllImports.sendto(socket, buff, len, flags, ref to, toLen);
      }
    }

    private int Socket(AddressFamily af, SocketType type, ProtocolType protocol)
    {
      var ret = DllImports.socket(af, type, protocol);
      if (type == SocketType.Stream)
      {
        var si = new SocketInfo
        {
          Socket = ret,
        };
        _dicSockets[ret] = si;
        _server.Echo($"{_moduleName} socket {si}");
      }
      return ret;
    }

    private SocketError WSARecv(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, IntPtr overlapped, IntPtr completionRoutine)
    {
      var ret = DllImports.WSARecv(socket, ref buffers, bufferCount, out numberOfBytesRecvd, ref flags,  overlapped, completionRoutine);
      if (_dicSockets.TryGetValue(socket, out var si))
      {
        if (numberOfBytesRecvd == 0)
        {
          _server.Echo($"{_moduleName} WSARecv {si} closed by remote");
          _dicSockets.TryRemove(socket, out var _);
        }
        else
        {
          _server.Echo($"{_moduleName} WSARecv {si} len={numberOfBytesRecvd}");
        }
      }
      return ret;
    }

    private SocketError WSARecvFrom(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, out SockAddr from, out int fromLen, IntPtr overlapped, IntPtr completionRoutine)
    {
      var result = DllImports.WSARecvFrom(socket, ref buffers, bufferCount, out numberOfBytesRecvd, ref flags, out from, out fromLen, overlapped, completionRoutine);
      if (result == 0 && from.IP == LOCALHOST && from.Port1 == 122 && from.Port2 == 122)
      {
        from.IP = Marshal.ReadInt32(buffers.Buf);
        from.Port2 = Marshal.ReadByte(buffers.Buf + 4);
        from.Port1 = Marshal.ReadByte(buffers.Buf + 5);
        DllImports.CopyMemory(buffers.Buf, buffers.Buf + 6, numberOfBytesRecvd - 6);
        numberOfBytesRecvd = numberOfBytesRecvd - 6;
        _server.Echo($"{_moduleName} WSARecvFrom from={from} len={numberOfBytesRecvd}");
      }
      return result;
    }

    private int WSASend(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesSent, int flags, IntPtr overlapped, IntPtr completionRoutine)
    {
      var ret = DllImports.WSASend(socket, ref buffers, bufferCount, out numberOfBytesSent, flags, overlapped, completionRoutine);
      if (_dicSockets.TryGetValue(socket, out var si))
      {
        _server.Echo($"{_moduleName} WSASend {si} len={numberOfBytesSent}");
      }
      return ret;
    }

  }

  [StructLayout(LayoutKind.Sequential)]
  public struct ProcessInformation
  {
    public IntPtr hProcess;
    public IntPtr hThread;
    public int ProcessId;
    public int ThreadId;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct WSABUF
  {
    public int Len;
    public IntPtr Buf;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct fd_set
  {
    public int Count;
    public int Socket;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct TimeVal
  {
    public int Sec;
    public int Usec;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct HostEnt
  {
    public string Name;
    public IntPtr Aliases;
    public short AddrType;
    public short Length;
    public IntPtr AddrList;
  }
}
