using System.Collections.Generic;

namespace POCO
{
  public class RoomInfo
  {
    public int Id { get; set; }
    public int HostId { get; set; }
    public string Name { get; set; }
    public int MaxPlayers { get; set; }
    public List<int> Players { get; } = new List<int>();
    public List<int> Spectators { get; } = new List<int>();
    public RoomState State { get; set; }
  }

  public enum RoomState
  {
    CanHost,
    CanJoin,
    Gaming,
  }
}
