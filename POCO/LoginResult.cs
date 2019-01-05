using System.Collections.Generic;

namespace POCO
{
  public class LoginResult
  {
    public LoginResultKind Result { get; set; }
    public int Vip { get; set; }
    public List<string> OnlineUsers { get; set; }
    public List<string> Rooms { get; set; }
  }

  public enum LoginResultKind
  {
    Success,
    Fail,
    AlreadyLoginedIn,
  }
}
