using System.Collections.Generic;

namespace POCO
{
  public class RoomInfo
  {
    public string RoomId { get; set; }
    public string HostId { get; set; }
    public List<string> Players { get; } = new List<string>();
    public List<string> Spectators { get; } = new List<string>();
  }
}
