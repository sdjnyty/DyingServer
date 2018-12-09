using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Injector
{
  public class ServerInterface:MarshalByRefObject
  {
    private StreamWriter _writer;

    public ServerInterface()
    {
      //_writer = new StreamWriter(@"log.log");
      //_writer.AutoFlush = true;
    }

    public void Echo(object obj)
    {
      //_writer.WriteLine(obj);
    }
  }
}
