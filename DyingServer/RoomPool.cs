using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using POCO;

namespace DyingServer
{
  public static class RoomPool
  {
    private static Dictionary<string, RoomInfo> _idDic = new Dictionary<string, RoomInfo>();

    public static void Add(RoomInfo ri)
    {
      _idDic.Add(ri.RoomId, ri);
    }

    public static void Remove(RoomInfo ri)
    {
      _idDic.Remove(ri.RoomId);
    }

    public static RoomInfo GetById(string rid)
    {
      return _idDic[rid];
    }

    public static IEnumerable<RoomInfo> Enumerate()
    {
      return _idDic.Values;
    }
  }
}