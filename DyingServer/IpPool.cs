using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DyingServer
{
  public static class IpPool
  {
    private static readonly byte[] _currentIp = new byte[] { 10, 1, 1, 1 };
    private static readonly HashSet<int> _set = new HashSet<int>();
    private static readonly object _locker = new object();

    public static int AllocateIp()
    {
      lock (_locker)
      {
        for (Next(); ; Next())
        {
          var current = CurrentToInt();
          if (!_set.Contains(current))
          {
            _set.Add(current);
            return current;
          }
        }
      }
    }

    public static void RecycleIp(int ip)
    {
      lock (_locker)
      {
        _set.Remove(ip);
      }
    }

    private static void Next()
    {
      if (_currentIp[3] < 254)
      {
        _currentIp[3]++;
        return;
      }
      else
      {
        _currentIp[3] = 1;
        if (_currentIp[2] < 254)
        {
          _currentIp[2]++;
          return;
        }
        else
        {
          _currentIp[2] = 1;
          if (_currentIp[1] < 254)
          {
            _currentIp[1]++;
            return;
          }
          else
          {
            _currentIp[1] = 1;
            return;
          }
        }
      }
    }

    private static int CurrentToInt()
    {
      return BitConverter.ToInt32(_currentIp, 0);
    }
  }
}