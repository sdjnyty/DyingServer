﻿using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using EasyHook;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Drawing;
using SharpDX.DirectWrite;
using SharpDX;
using SharpDX.Direct2D1;

namespace Injector
{
  public class HookEntryPoint : IEntryPoint
  {
    private readonly StringFormat SF_Bottom = new StringFormat { LineAlignment = StringAlignment.Far };
    private readonly StringFormat SF_Right = new StringFormat { Alignment = StringAlignment.Far };

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
    private SharpDX.DirectWrite.Factory _factory = new SharpDX.DirectWrite.Factory();
    private RenderingParams _rp;
    private string _lastTextOutString;

    public HookEntryPoint(RemoteHooking.IContext context, string channel, string dllPath, int vip, string userId)
    {
      _channel = channel;
      _server = RemoteHooking.IpcConnectClient<ServerInterface>(channel);
      _pid = NativeAPI.GetCurrentProcessId();
      _dllPath = dllPath;
      _vip = vip;
      _userId = userId;
      _rp = new RenderingParams(_factory);
    }

    public void Run(RemoteHooking.IContext context, string channel, string dllPath, int vip, string userId)
    {
      _moduleName = Process.GetCurrentProcess().ProcessName;
      DllImports.LoadLibraryA("wsock32");
      DllImports.LoadLibraryA("ws2_32");
      var hTextOut = LocalHook.Create(LocalHook.GetProcAddress("gdi32", "TextOutA"), new Delegates.TextOutA(TextOutA), this);
      hTextOut.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hDrawTextA = LocalHook.Create(LocalHook.GetProcAddress("user32", "DrawTextA"), new Delegates.DrawTextA(DrawTextA), this);
      //hDrawTextA.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hLoadString = LocalHook.Create(LocalHook.GetProcAddress("user32", "LoadStringA"), new Delegates.LoadStringA(LoadStringA), this);
      //hLoadString.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hCreateFontIndirectA = LocalHook.Create(LocalHook.GetProcAddress("gdi32", "CreateFontIndirectA"), new Delegates.CreateFontIndirectA(CreateFontIndirectA), this);
      //hCreateFontIndirectA.ThreadACL.SetExclusiveACL(new[] { 0 });
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
          var ret1 = DllImports.recv(ret, buff + count, 8 - count, 0);
          if (ret1 == -1 && DllImports.WSAGetLastError() == SocketError.WouldBlock)
          {
            var read = new fd_set
            {
              Count = 1,
              Socket = ret,
            };
            var pRead = Marshal.AllocHGlobal(8);
            Marshal.StructureToPtr(read, pRead, true);
            DllImports.select(0, pRead, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            Marshal.FreeHGlobal(pRead);
            ret1 = DllImports.recv(ret, buff + count, 8 - count, 0);
          }
          count += ret1;
        }
        addr.IP = Marshal.ReadInt32(buff);
        var remotePort = Marshal.ReadInt32(buff + 4);
        addr.Port1 = (byte)(remotePort >> 8);
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
          if (addr.Port1 == 0xba && addr.Port2 == 0x8)
          {
            _server.OnPort47624Bind();
          }
          else if (addr.Port1 == 0xd1 && addr.Port2 == 0xfa)
          {
            _server.OnPort53754Bind();
          }
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
          var pWrite = Marshal.AllocHGlobal(8);
          Marshal.StructureToPtr(write, pWrite, true);
          DllImports.select(0, IntPtr.Zero, pWrite, IntPtr.Zero, IntPtr.Zero);
          Marshal.FreeHGlobal(pWrite);
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

    private IntPtr CreateFontIndirectA(ref LOGFONT lf)
    {
      _server.Echo($"{lf.FaceName}\t{lf.Quality}");
      lf.Quality = FontQuality.ANTIALIASED_QUALITY;
      return DllImports.CreateFontIndirectA(ref lf);
    }

    private int DrawTextA(IntPtr dc, string str, int count, ref RECT rect, DT_ format)
    {
      var g = Graphics.FromHdc(dc);
      var font = System.Drawing.Font.FromHdc(dc);
      var rectF = rect.ToRectangleF();
      var color = DllImports.GetTextColor(dc).ToColor();
      StringFormat sf;
      if (format.HasFlag(DT_.DT_BOTTOM))
      {
        sf = SF_Bottom;
      }
      else if (format.HasFlag(DT_.DT_RIGHT))
      {
        sf = SF_Right;
      }
      else
      {
        sf = StringFormat.GenericDefault;
      }
      g.DrawString(str, font, new SolidBrush(color), rectF, sf);
      return (int)rectF.Height;
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
      var bytes = Encoding.Default.GetBytes(_userId.ToString() + "\0");
      Marshal.Copy(bytes, 0, name, bytes.Length);
      return SocketError.Success;
    }

    private SocketError GetPeerName(int socket, out SockAddr name, out int nameLen)
    {
      var ret = DllImports.getpeername(socket, out name, out nameLen);
      if (ret == SocketError.Success && name.IP == LOCALHOST)
      {
        var si = _dicSockets[socket];
        name.IP = BitConverter.ToInt32(si.RemoteIp.GetAddressBytes(), 0);
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
      if (fileName == "dpnathlp.dll")
      {
        return IntPtr.Zero;
      }
      else
      {
        return DllImports.LoadLibraryA(fileName);
      }
    }

    private int LoadStringA(IntPtr instance, uint id, IntPtr buffer, int bufferMax)
    {
      var sb = new StringBuilder(bufferMax);
      var ret = DllImports.LoadStringA(instance, id, sb, bufferMax);
      if (sb.ToString() == "宋体")
      {
        sb.Remove(0, sb.Length);
        sb.Append("微软雅黑");
      }
      var bytes = Encoding.GetEncoding("gb2312").GetBytes(sb.ToString());
      _server.Echo($"{sb}\t{bufferMax}\t{ret}\t{bytes.Length}");
      Marshal.Copy(bytes, 0, buffer, bytes.Length);
      Marshal.WriteByte(buffer, bytes.Length, 0);

      return ret;
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

    private bool TextOutA(IntPtr dc, int xStart, int yStart, string str, int strLen)
    {
      if (str == _lastTextOutString) return true;

      _lastTextOutString = str;
      Color color;
      using (var g = Graphics.FromHdc(dc))
      {
        color = DllImports.GetTextColor(dc).ToColor();
        if (color.ToArgb() == Color.Black.ToArgb())
        {
          color = Color.DarkGray;
        }
      }
      using (var font = System.Drawing.Font.FromHdc(dc))
      using (var tf = new TextFormat(_factory, font.Name, font.Size))
      using (var tl = new TextLayout(_factory, str, tf, float.MaxValue, float.MaxValue))
      using (var brt = _factory.GdiInterop.CreateBitmapRenderTarget(dc, 1920, 1080))
      using (var gtr = new GdiTextRenderer(brt, _rp,color))
      {
        tl.Draw(gtr, 0, 0);
        var r = gtr.BlackBoxRect;
        var width = r.Right - r.Left;
        _server.Echo($"{xStart},{yStart} {str} {r.Left},{r.Top}-{r.Right},{r.Bottom}");
        var height = r.Bottom - r.Top;
        DllImports.TransparentBlt(dc, xStart, yStart, width,height , brt.MemoryDC, 0, 0, width,height,Color.Black.ToArgb());
      }

      //StringFormat sf;
      //if (align.HasFlag(TA_.TA_BOTTOM))
      //{
      //  sf = SF_Bottom;
      //}
      //else if (align.HasFlag(TA_.TA_RIGHT))
      //{
      //  sf = SF_Right;
      //}
      //else
      //{
      //  sf = StringFormat.GenericDefault;
      //}
      //g.DrawString(str, font, new SolidBrush(color), point, sf);
      return true;
    }

    private SocketError WSARecv(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, IntPtr overlapped, IntPtr completionRoutine)
    {
      var ret = DllImports.WSARecv(socket, ref buffers, bufferCount, out numberOfBytesRecvd, ref flags, overlapped, completionRoutine);
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

  public class GdiTextRenderer : TextRendererBase
  {
    private SharpDX.DirectWrite.BitmapRenderTarget _brt;
    private RenderingParams _rp;
    private Color _color;

    public SharpDX.Mathematics.Interop.RawRectangle BlackBoxRect { get; set; }

    public GdiTextRenderer(SharpDX.DirectWrite.BitmapRenderTarget brt, RenderingParams rp,Color color)
    {
      _brt = brt;
      _rp = rp;
      _color = color;
    }

    public override Result DrawGlyphRun(object clientDrawingContext, float baselineOriginX, float baselineOriginY, MeasuringMode measuringMode, GlyphRun glyphRun, GlyphRunDescription glyphRunDescription, ComObject clientDrawingEffect)
    {
      _brt.DrawGlyphRun(baselineOriginX, baselineOriginY, measuringMode, glyphRun, _rp, new SharpDX.Mathematics.Interop.RawColorBGRA(_color.B, _color.G, _color.R, _color.A),out var rect);
      BlackBoxRect = rect;
      return Result.Ok;
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
