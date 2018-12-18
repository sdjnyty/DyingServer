using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
  }

  public class RoomInfo
  {
    public string RoomId { get; set; }
    public string HostId { get; set; }
    public List<string> Players { get; } = new List<string>();
  }
}