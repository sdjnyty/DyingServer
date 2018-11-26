using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Injector
{

  public struct SockAddr
  {
    public short Family { get; set; }
    public byte Port1 { get; set; }
    public byte Port2 { get; set; }
    public int IP { get; set; }
    public long Zero { get; set; }
    public override string ToString()
    {
      return $"{new IPAddress(IP)}:{Port1 * 256 + Port2}";
    }
  }
}
