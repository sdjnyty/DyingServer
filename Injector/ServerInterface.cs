using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector
{
  public class ServerInterface:MarshalByRefObject
  {
    public void Echo(object obj)
    {
      Console.WriteLine(obj);
    }
  }
}
