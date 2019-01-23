using System.Collections.Generic;

namespace POCO
{
  public class LoginResult
  {
    public SignalRResult Result { get; set; }
    public int Vip { get; set; }
    public IReadOnlyList<UserInfo> OnlineUsers { get; set; }
    public IReadOnlyList<RoomInfo> Rooms { get; set; }
  }

  public enum SignalRResult
  {
    Success,
    BadPassword,
    AlreadyLoginedIn,
    RoomFull,
  }
}
