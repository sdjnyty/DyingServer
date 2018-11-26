using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector
{

  public class Message
  {
    public int FromVip { get; set; }
    public short FromPort { get; set; }
    public byte[] Data { get; set; }
  }
}
