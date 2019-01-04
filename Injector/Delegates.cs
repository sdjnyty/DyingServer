using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Text;

namespace Injector
{
  public static class Delegates
  {
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int Accept(int socket, out SockAddr addr, ref int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError Bind(int socket, ref SockAddr addr, int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError CloseSocket(int socket);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError Connect(int socket, ref SockAddr addr, int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi,CharSet= CharSet.Ansi)]
    public delegate IntPtr CreateFontIndirectA(ref LOGFONT lf);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate bool CreateProcessA(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string
currentDirectory, IntPtr startupInfo, out ProcessInformation processInformation);

    [UnmanagedFunctionPointer(CallingConvention.Winapi,CharSet= CharSet.Ansi)]
    public delegate int DrawTextA(IntPtr dc, string pStr, int count, ref RECT rect, DT_ format);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate IntPtr GetHostByName(IntPtr name);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate SocketError GetHostName(IntPtr name, int nameLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError GetPeerName(int socket, out SockAddr name, out int nameLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError GetSockName(int socket, out SockAddr addr, ref int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr LoadLibraryA(string fileName);

    [UnmanagedFunctionPointer(CallingConvention.Winapi,CharSet= CharSet.Ansi)]
    public delegate int LoadStringA(IntPtr instance, uint id, IntPtr buffer, int bufferMax);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int Recv(int socket, IntPtr buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int RecvFrom(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int Send(int socket, IntPtr buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int SendTo(int socket, IntPtr buff, int len, int flags, ref SockAddr to, int toLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int Socket(AddressFamily af, SocketType type, ProtocolType protocol);

    [UnmanagedFunctionPointer( CallingConvention.Winapi,CharSet= CharSet.Ansi)]
    public delegate bool TextOutA(IntPtr dc, int xStart, int yStart, string pStr, int strLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError WSARecv(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, IntPtr pOverlapped, IntPtr pCompletionRoutine);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate SocketError WSARecvFrom(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, out SockAddr from, out int fromLen, IntPtr pOverlapped, IntPtr pCompletionRoutine);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int WSASend(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesSent, int flags, IntPtr overlapped, IntPtr completionRoutine);
  }
}
