using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EasyHook;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Injector
{
  public class HookEntryPoint : IEntryPoint
  {
    private string _userId;
    private int _vip;
    private int _pid;
    private string _channel;
    private ServerInterface _server;
    private string _dllPath;
    private BlockingCollection<string> _q = new BlockingCollection<string>();
    private Dictionary<int, SockAddr> _dicTcp = new Dictionary<int, SockAddr>();
    private string _hostName;
    private SockAddr _udpOutgoing = new SockAddr
    {
      Family = (short)AddressFamily.InterNetwork,
      IP = 0x100007f,
      Port1 = 123,
      Port2 = 123,
    };

    [DllImport("kernel32")]
    internal static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

    [DllImport("ws2_32")]
    internal static extern SocketError WSAGetLastError();
    [DllImport("ws2_32")]
    internal static extern void WSASetLastError(SocketError e);

    [DllImport("ws2_32")]
    internal static extern SocketError WSARecvFrom(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, out SockAddr from, out int fromLen, IntPtr overlapped, IntPtr completionRoutine);

    [DllImport("ws2_32")]
    internal static extern SocketError bind(int socket, ref SockAddr addr, int addrLen);

    [DllImport("ws2_32")]
    internal static extern SocketError getsockname(int socket, out SockAddr addr, ref int addrLen);

    [DllImport("ws2_32")]
    internal static extern SocketError connect(int socket, ref SockAddr addr, int addrLen);
    [DllImport("ws2_32")]
    internal static extern int select(int nfds, IntPtr read, ref fd_set write, ref fd_set except, IntPtr timeout);
    [DllImport("ws2_32")]
    internal static extern int send(int socket, IntPtr buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int sendto(int socket, IntPtr buff, int len, int flags, ref SockAddr to, int toLen);

    [DllImport("ws2_32")]
    internal static extern int recvfrom(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen);
    [DllImport("ws2_32")]
    internal static extern int recv(int socket, IntPtr buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int accept(int socket, out SockAddr addr, ref int addrLen);
    [DllImport("wsock32")]
    internal static extern SocketError getpeername(int socket, out SockAddr name, out int nameLen);

    [DllImport("kernel32", CharSet = CharSet.Ansi)]
    internal static extern IntPtr LoadLibraryA(string fileName);
    [DllImport("ws2_32", CharSet = CharSet.Ansi,ExactSpelling =true)]
    internal static extern SocketError gethostname(StringBuilder name, int nameLen);
    [DllImport("ws2_32", CharSet = CharSet.Ansi)]
    internal static extern IntPtr gethostbyname(string name);


    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SendToD(int socket, IntPtr buff, int len, int flags, ref SockAddr to, int toLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SendD(int socket, IntPtr buff, int len, int flags);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int RecvFromD(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    internal delegate bool CreateProcessAD(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, IntPtr startupInfo, out ProcessInformation processInformation);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError WSARecvFromD(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, out SockAddr from, out int fromLen, IntPtr pOverlapped, IntPtr pCompletionRoutine);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError ConnectD(int socket, ref SockAddr addr, int addrLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int AcceptD(int socket, out SockAddr addr, ref int addrLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError GetPeerNameD(int socket, out SockAddr name, out int nameLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr LoadLibraryAD(string fileName);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError GetSockNameD(int socket, out SockAddr addr, ref int addrLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    internal delegate SocketError GetHostNameD(IntPtr name, int nameLen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    internal delegate IntPtr GetHostByNameD(IntPtr name);

    public HookEntryPoint(RemoteHooking.IContext context, string channel, string dllPath, int vip, string userId)
    {
      _channel = channel;
      _server = RemoteHooking.IpcConnectClient<ServerInterface>(channel);
      _pid = NativeAPI.GetCurrentProcessId();
      _server.Echo(_pid);
      _dllPath = dllPath;
      _vip = vip;
      _userId = userId;
    }

    public void Run(RemoteHooking.IContext context, string channel, string dllPath, int vip, string userId)
    {
      var moduleName = Process.GetCurrentProcess().ProcessName;
      var hSendTo = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "sendto"), new SendToD(SendToH), this);
      hSendTo.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hRecvFrom = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recvfrom"), new RecvFromD(RecvFromH), this);
      hRecvFrom.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hCreateProcessA = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "CreateProcessA"), new CreateProcessAD(CreateProcessAH), this);
      hCreateProcessA.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hWSARecvFrom = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "WSARecvFrom"), new WSARecvFromD(WSARecvFromH), this);
      hWSARecvFrom.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hAccept = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "accept"), new AcceptD(AcceptH), this);
      hAccept.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hConnect = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "connect"), new ConnectD(ConnectH), this);
      hConnect.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadLibraryA = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "LoadLibraryA"), new LoadLibraryAD(LoadLibraryAH), this);
      hLoadLibraryA.ThreadACL.SetExclusiveACL(new[] { 0 });
      LoadLibraryA("wsock32");
      var hGetPeerName = LocalHook.Create(LocalHook.GetProcAddress("wsock32", "getpeername"), new GetPeerNameD(GetPeerNameH), this);
      hGetPeerName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetSockName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "getsockname"), new GetSockNameD(GetSockNameH), this);
      hGetSockName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var sb = new StringBuilder();
      gethostname(sb, 50);
      _hostName = sb.ToString();
      var hGetHostName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "gethostname"), new GetHostNameD(GetHostNameH), this);
      hGetHostName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetHostByName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "gethostbyname"), new GetHostByNameD(GetHostByNameH), this);
      hGetHostByName.ThreadACL.SetExclusiveACL(new[] { 0 });
      RemoteHooking.WakeUpProcess();
      Thread.Sleep(Timeout.Infinite);
    }

    private int SendToH(int socket, IntPtr buff, int len, int flags, ref SockAddr to, int toLen)
    {
      if ((to.IP & 0xff) == 10)
      {
        var pBuff = Marshal.AllocHGlobal(len + 6);
        Marshal.WriteInt32(pBuff, to.IP);
        Marshal.WriteByte(pBuff + 4, to.Port2);
        Marshal.WriteByte(pBuff + 5, to.Port1);
        CopyMemory(pBuff + 6, buff, len);
        sendto(socket, pBuff, len + 6, flags, ref _udpOutgoing, toLen);
        Marshal.FreeHGlobal(pBuff);
        return len;

      }
      else
      {
        return sendto(socket, buff, len, flags, ref to, toLen);
      }
    }

    private int RecvFromH(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen)
    {
      var result = recvfrom(socket, buff, len, flags, ref from, ref fromLen);
      if (result > 0 && from.IP == 0x100007f && from.Port1 == 122 && from.Port2 == 122)
      {
        from.IP = Marshal.ReadInt32(buff);
        from.Port2 = Marshal.ReadByte(buff + 4);
        from.Port1 = Marshal.ReadByte(buff + 5);
        CopyMemory(buff, buff + 6, result - 6);
        return result - 6;
      }
      else
      {
        return result;
      }
    }

    private bool CreateProcessAH(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, IntPtr startupInfo, out ProcessInformation processInformation)
    {
      RemoteHooking.CreateAndInject(applicationName, commandLine, 0, _dllPath, _dllPath, out var pid, _channel, _dllPath, _vip, _userId);
      processInformation = new ProcessInformation
      {
        ProcessId = pid,
      };
      return true;
    }

    private SocketError WSARecvFromH(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, out SockAddr from, out int fromLen, IntPtr overlapped, IntPtr completionRoutine)
    {
      var result = WSARecvFrom(socket, ref buffers, bufferCount, out numberOfBytesRecvd, ref flags, out from, out fromLen, overlapped, completionRoutine);
      if (result == 0 && from.IP == 0x100007f && from.Port1 == 122 && from.Port2 == 122)
      {
        from.IP = Marshal.ReadInt32(buffers.Buf);
        from.Port2 = Marshal.ReadByte(buffers.Buf + 4);
        from.Port1 = Marshal.ReadByte(buffers.Buf + 5);
        CopyMemory(buffers.Buf, buffers.Buf + 6, numberOfBytesRecvd - 6);
        numberOfBytesRecvd = numberOfBytesRecvd - 6;
      }
      return result;
    }

    private SocketError ConnectH(int socket, ref SockAddr addr, int addrLen)
    {
      _server.Echo($"Before connect socket={socket} addr={addr}");
      if ((addr.IP & 0xff) == 10)
      {

        var addrProxy = new SockAddr
        {
          Family = addr.Family,
          IP = 0x100007f,
          Port1 = 123,
          Port2 = 123,
        };
        var ret = connect(socket, ref addrProxy, addrLen);
        _server.Echo($"After connect socket={socket} addr={addrProxy} ret={ret}");
        if (ret == SocketError.Success)
        {
          var pBuff = Marshal.AllocHGlobal(6);
          Marshal.WriteInt32(pBuff, addr.IP);
          Marshal.WriteByte(pBuff + 4, addr.Port2);
          Marshal.WriteByte(pBuff + 5, addr.Port1);
          send(socket, pBuff, 6, 0);
          Marshal.FreeHGlobal(pBuff);
        }
        else if (ret == SocketError.SocketError && WSAGetLastError() == SocketError.WouldBlock)
        {
          var write = new fd_set
          {
            Count = 1,
            Socket = socket,
          };
          var except = write;
          select(0, IntPtr.Zero, ref write, ref except, IntPtr.Zero);
          var pBuff = Marshal.AllocHGlobal(6);
          Marshal.WriteInt32(pBuff, addr.IP);
          Marshal.WriteByte(pBuff + 4, addr.Port2);
          Marshal.WriteByte(pBuff + 5, addr.Port1);
          send(socket, pBuff, 6, 0);
          Marshal.FreeHGlobal(pBuff);
          WSASetLastError(SocketError.WouldBlock);
        }
        return ret;
      }
      else
      {
        return connect(socket, ref addr, addrLen);
      }
    }

    private int AcceptH(int socket, out SockAddr addr, ref int addrLen)
    {
      var ret = accept(socket, out addr, ref addrLen);
      _server.Echo($"After accept socket={socket} addr={addr}");
      if (addr.IP == 0x100007f)
      {
        var buff = Marshal.AllocHGlobal(6);
        recv(ret, buff, 6, 0);
        addr.IP = Marshal.ReadInt32(buff);
        addr.Port1 = Marshal.ReadByte(buff + 4);
        addr.Port2 = Marshal.ReadByte(buff + 5);
        _dicTcp[ret] = addr;
        Marshal.FreeHGlobal(buff);
        _server.Echo($"Hook accept socket={socket} addr={addr}");
      }
      return ret;
    }

    private SocketError GetPeerNameH(int socket, out SockAddr name, out int nameLen)
    {
      var ret = getpeername(socket, out name, out nameLen);
      _server.Echo($"After getpeername socket={socket} addr={name}");
      if (ret == SocketError.Success && name.IP == 0x100007f && name.Port1 == 123 && name.Port2 == 123)
      {
        var addr = _dicTcp[socket];
        name.IP = addr.IP;
        name.Port1 = addr.Port1;
        name.Port2 = addr.Port2;
        _server.Echo($"Hook getpeername socket={socket} addr={name}");
      }
      return ret;
    }

    private IntPtr LoadLibraryAH(string fileName)
    {
      return LoadLibraryA(fileName);
    }

    private SocketError GetSockNameH(int socket, out SockAddr addr, ref int addrLen)
    {
      var ret = getsockname(socket, out addr, ref addrLen);
      if (ret == SocketError.Success)
      {
        addr.IP = _vip;
      }
      return ret;
    }

    private SocketError GetHostNameH(IntPtr name, int nameLen)
    {
      var bytes = Encoding.ASCII.GetBytes(_userId.ToString()+"\0");
      Marshal.Copy(bytes, 0, name, bytes.Length);
      return SocketError.Success;
    }

    private IntPtr GetHostByNameH(IntPtr pName)
    {
      var name = Marshal.PtrToStringAnsi(pName);
      IntPtr ret;
      if (name == _userId)
      {
        ret = gethostbyname(_hostName);
        var he =(HostEnt) Marshal.PtrToStructure(ret, typeof(HostEnt));
        Marshal.WriteInt32(Marshal.ReadIntPtr(he.AddrList), _vip);
        Marshal.StructureToPtr(he, ret, false);
      }
      else
      {
        ret = gethostbyname(name);
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
