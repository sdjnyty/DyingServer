using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Drawing;

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

    [DllImport("gdi32")]
    internal static extern TA_ GetTextAlign(IntPtr dc);

    [DllImport("gdi32")]
    internal static extern COLORREF GetTextColor(IntPtr dc);

    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern IntPtr LoadLibraryA(string fileName);

    [DllImport("user32",CharSet= CharSet.Ansi,ExactSpelling =true)]
    public static extern int LoadStringA(IntPtr instance, uint id, StringBuilder buffer, int bufferMax);

    [DllImport("ws2_32")]
    public static extern int recv(int socket, IntPtr buff, int len, int flags);

    [DllImport("ws2_32")]
    public static extern int recvfrom(int socket, IntPtr buff, int len, int flags, ref SockAddr from, ref int fromLen);

    [DllImport("ws2_32")]
    public static extern int select(int nfds, IntPtr read, IntPtr write, IntPtr except, IntPtr timeout);

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

  [Flags]
  public enum TA_ : uint
  {
    TA_NOUPDATECP = 0,
    TA_UPDATECP = 1,
    TA_LEFT = 0,
    TA_RIGHT = 2,
    TA_CENTER = 6,
    TA_TOP = 0,
    TA_BOTTOM = 8,
    TA_BASELINE = 24,
    TA_RTLREADING = 256,
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct COLORREF
  {
    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Zero;

    public Color ToColor()
    {
      return Color.FromArgb(Red, Green, Blue);
    }

    public Color ToColor(byte alpha)
    {
      return Color.FromArgb(alpha, Red, Green, Blue);
    }

    public override string ToString()
    {
      return ToColor().ToString();
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Rectangle ToRectangle()
    {
      return new Rectangle(Left, Top, Right - Left, Bottom - Top);
    }

    public RectangleF ToRectangleF()
    {
      return RectangleF.FromLTRB(Left, Top, Right, Bottom);
    }
  }

  [Flags]
  public enum DT_ : uint
  {
    DT_TOP = 0x00000000,
    DT_LEFT = 0x00000000,
    DT_CENTER = 0x00000001,
    DT_RIGHT = 0x00000002,
    DT_VCENTER = 0x00000004,
    DT_BOTTOM = 0x00000008,
    DT_WORDBREAK = 0x00000010,
    DT_SINGLELINE = 0x00000020,
    DT_EXPANDTABS = 0x00000040,
    DT_TABSTOP = 0x00000080,
    DT_NOCLIP = 0x00000100,
    DT_EXTERNALLEADING = 0x00000200,
    DT_CALCRECT = 0x00000400,
    DT_NOPREFIX = 0x00000800,
    DT_INTERNAL = 0x00001000,
    DT_EDITCONTROL = 0x00002000,
    DT_PATH_ELLIPSIS = 0x00004000,
    DT_END_ELLIPSIS = 0x00008000,
    DT_MODIFYSTRING = 0x00010000,
    DT_RTLREADING = 0x00020000,
    DT_WORD_ELLIPSIS = 0x00040000,
    DT_NOFULLWIDTHCHARBREAK = 0x00080000,
    DT_HIDEPREFIX = 0x00100000,
    DT_PREFIXONLY = 0x00200000,
  }
}
