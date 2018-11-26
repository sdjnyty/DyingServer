using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DyingServer
{
  public static class PlayerInfoPool
  {
    private static readonly Dictionary<int, PlayerInfo> _vipDic = new Dictionary<int, PlayerInfo>();
    private static readonly Dictionary<string, PlayerInfo> _cidDic = new Dictionary<string, PlayerInfo>();
    public static void Add(PlayerInfo pi)
    {
      _vipDic.Add(pi.Vip, pi);
      _cidDic.Add(pi.ConnectionId, pi);
    }

    public static void Remove(PlayerInfo pi)
    {
      _vipDic.Remove(pi.Vip);
      _cidDic.Remove(pi.ConnectionId);
    }

    public static PlayerInfo GetByVip(int vip)
    {
      return _vipDic[vip];
    }

    public static PlayerInfo GetByCid(string cid)
    {
      return _cidDic[cid];
    }
  }

  public class PlayerInfo
  {
    public string UserId { get; set; }
    public string ConnectionId { get; set; }
    public int Vip { get; set; }
  }
}