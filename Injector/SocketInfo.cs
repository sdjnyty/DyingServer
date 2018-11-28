using System.Net;

namespace Injector
{
  public class SocketInfo
  {
    public int Socket { get; set; }
    public IPAddress LocalIp { get; set; }
    public int LocalPort { get; set; }
    public IPAddress RemoteIp { get; set; }
    public int RemotePort { get; set; }

    public override string ToString()
    {
      return $"{Socket} {LocalIp}:{LocalPort}->{RemoteIp}:{RemotePort}";
    }
  }
}
