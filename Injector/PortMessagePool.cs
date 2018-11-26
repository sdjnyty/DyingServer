using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector
{
  public static class PortMessagePool
  {
    private static readonly Dictionary<short, BlockingCollection<Message>> _dic
      = new Dictionary<short, BlockingCollection<Message>>();
    private static readonly object _lock = new object();

    public static void Add(short port,Message msg)
    {
      lock (_lock)
      {
        if (!_dic.ContainsKey(port))
        {
          _dic.Add(port,new BlockingCollection<Message>());
        }
        _dic[port].Add(msg);
      }
    }

    public static Message Take(short port)
    {
      lock(_lock)
      {
        if (!_dic.ContainsKey(port))
        {
          _dic.Add(port, new BlockingCollection<Message>());
        }
        return _dic[port].Take();
      }
    }
  }
}
