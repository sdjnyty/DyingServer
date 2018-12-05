using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace Injector
{
  public static class DllImports
  {
    [DllImport("ws2_32")]
    public static extern int accept(int socket, out SockAddr addr, ref int addrLen);

    [DllImport("ws2_32")]
    public static extern SocketError bind(int socket, ref SockAddr addr, int addrLen);

    [DllImport("ws2_32")]
    public static extern SocketError closesocket(int socket);

    [DllImport("ws2_32")]
    public static extern SocketError connect(int socket, ref SockAddr addr, int addrLen);

    [DllImport("kernel32")]
    public static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

    [DllImport("ws2_32", CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr gethostbyname(string name);

    [DllImport("ws2_32", CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern SocketError gethostname(StringBuilder name, int nameLen);

    [DllImport("wsock32")]
    public static extern SocketError getpeername(int socket, out SockAddr name, out int nameLen);

    [DllImport("ws2_32")]
    public static extern SocketError getsockname(int socket, out SockAddr addr, ref int addrLen);

    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr LoadLibraryA(string fileName);

    [DllImport("ws2_32")]
    public static extern int recv(int socket, IntPtr buff, int len, int flags);

    [DllImport("ws2_32")]
    public static extern int recvfrom(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen);

    [DllImport("ws2_32")]
    public static extern int select(int nfds, IntPtr read, ref fd_set write, ref fd_set except, IntPtr timeout);

    [DllImport("ws2_32")]
    public static extern int send(int socket, IntPtr buff, int len, int flags);

    [DllImport("ws2_32")]
    public static extern int sendto(int socket, IntPtr buff, int len, int flags, ref SockAddr to, int toLen);

    [DllImport("ws2_32")]
    public static extern int socket(AddressFamily af, SocketType type, ProtocolType protocol);

    [DllImport("ws2_32")]
    public static extern SocketError WSAGetLastError();

    [DllImport("ws2_32")]
    public static extern SocketError WSARecv(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, IntPtr overlapped, IntPtr completionRoutine);

    [DllImport("ws2_32")]
    public static extern SocketError WSARecvFrom(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesRecvd, ref int flags, out SockAddr from, out int fromLen, IntPtr overlapped, IntPtr completionRoutine);

    [DllImport("ws2_32")]
    public static extern int WSASend(int socket, ref WSABUF buffers, int bufferCount, out int numberOfBytesSent, int flags, IntPtr overlapped, IntPtr completionRoutine);

    [DllImport("ws2_32")]
    public static extern void WSASetLastError(SocketError e);
  }
}
