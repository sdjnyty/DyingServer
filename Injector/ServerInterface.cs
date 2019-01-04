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

    public event EventHandler Port47624Bind;
    public event EventHandler Port53754Bind;

    public ServerInterface()
    {
      _writer = new StreamWriter(@"DyingClient.log");
      _writer.AutoFlush = true;
    }

    public void OnPort47624Bind()
    {
      Port47624Bind?.Invoke(this, EventArgs.Empty);
    }

    public void OnPort53754Bind()
    {
      Port53754Bind?.Invoke(this, EventArgs.Empty);
    }

    public void Echo(object obj)
    {
     _writer.WriteLine(obj);
    }
  }
}
