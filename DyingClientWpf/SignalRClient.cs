using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using POCO;

namespace DyingClientWpf
{
  public static class SignalRClient
  {
    private static readonly HubConnection _hc = new HubConnection("http://47.93.96.30:81/");
    private static readonly IHubProxy _hp;
    private static HashSet<IDisposable> _subscribers = new HashSet<IDisposable>();

    public static event EventHandler Closed;

    static SignalRClient()
    {
      _hc.Closed += _hc_Closed;
      _hp=_hc.CreateHubProxy("PlayerHub");
    }

    public static  async Task<Tuple<SignalRResult,int>> Login(string userId,string passwordMd5)
    {
      await _hc.Start();
      return await _hp.Invoke<Tuple<SignalRResult,int>>(nameof(Login), userId, passwordMd5);
    }

    public static void Stop()
    {
      _hc.Stop();
    }

    public static async Task< IReadOnlyList<RoomInfo>> GetRooms()
    {
      return await _hp.Invoke< IReadOnlyList<RoomInfo>>(nameof(GetRooms));
    }

    private static void _hc_Closed()
    {
      Closed?.Invoke(null, EventArgs.Empty);
    }
  }
}
