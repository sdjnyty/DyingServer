using System.Collections.Generic;
using System.IO;
using POCO;

namespace DyingServer
{
  public static class PlayerInfoPool
  {
    private static readonly Dictionary<int, PlayerInfo> _vipDic = new Dictionary<int, PlayerInfo>();
    private static readonly Dictionary<string, PlayerInfo> _cidDic = new Dictionary<string, PlayerInfo>();
    private static readonly Dictionary<int, PlayerInfo> _uidDic = new Dictionary<int, PlayerInfo>();

    public static void Add(PlayerInfo pi)
    {
      _vipDic.Add(pi.Vip, pi);
      _cidDic.Add(pi.ConnectionId, pi);
      _uidDic.Add(pi.Id, pi);
    }

    public static void Remove(PlayerInfo pi)
    {
      _vipDic.Remove(pi.Vip);
      _cidDic.Remove(pi.ConnectionId);
      _uidDic.Remove(pi.Id);
    }

    public static PlayerInfo GetByVip(int vip)
    {
      return _vipDic.TryGetValue(vip, out var ret) ? ret : null;
    }

    public static PlayerInfo GetByCid(string cid)
    {
      return _cidDic.TryGetValue(cid, out var ret) ? ret : null;
    }

    public static PlayerInfo GetByUid(int uid)
    {
      return _uidDic.TryGetValue(uid, out var ret) ? ret : null;
    }

    public static IEnumerable<PlayerInfo> Enumerate()
    {
      return _cidDic.Values;
    }
  }

  public class PlayerInfo
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string ConnectionId { get; set; }
    public int Vip { get; set; }
    public int RoomId { get; set; }
    public int GameId { get; set; }
    public FileStream UploadingStream { get; set; }
    public PlayerState State { get; set; }

    public UserInfo ToUserInfo()
    {
      return new UserInfo
      {
        Id = Id,
        Name = Name,
      };
    }
  }

  public enum PlayerState
  {
    InLobby,
    InRoom,
    InGame,
  }
}