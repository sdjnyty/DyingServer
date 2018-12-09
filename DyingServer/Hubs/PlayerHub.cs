using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.IO;

namespace DyingServer.Hubs
{
  public class PlayerHub : Hub<IPlayerClient>
  {
    public override Task OnConnected()
    { 
      return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
      var pi = PlayerInfoPool.GetByCid(Context.ConnectionId);
      pi.UploadingStream?.Close();
      IpPool.RecycleIp(pi.Vip);
      Clients.All.Receive($"{pi. UserId} left.");
      return base.OnDisconnected(stopCalled);
    }

    public void Broadcast(string message)
    {
      Clients.All.Receive(message);
    }

    public void Login(string userId)
    {
      var vip = IpPool.AllocateIp();
      var pi = new PlayerInfo
      {
        UserId = userId,
        Vip = vip,
        ConnectionId = Context.ConnectionId,
      };
      PlayerInfoPool.Add(pi);
      Clients.Caller.OnLogin(vip);
      Clients.All.Receive($"{userId} joined.");
    }

    public void SendTo(int toIp,int toPort,int fromPort,byte[] data)
    {
      var toPlayer = PlayerInfoPool.GetByVip(toIp);
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Client(toPlayer.ConnectionId).OnReceiveFrom(fromPlayer.Vip,fromPort,toPort, data);
    }
    
    public void TcpConnect(int toIp,int toPort,int fromPort)
    {
      var toPlayer = PlayerInfoPool.GetByVip(toIp);
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Client(toPlayer.ConnectionId).OnTcpConnect(fromPlayer.Vip, fromPort, toPort);
    }

    public void TcpSend(int fromPort,int toIp,int toPort,byte[] data)
    {
      var toPlayer = PlayerInfoPool.GetByVip(toIp);
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Client(toPlayer.ConnectionId).OnTcpSend(fromPlayer.Vip, fromPort, toPort, data);
    }

    public void BeginRec()
    {
      var gameId=(int)HttpContext.Current.Application["GameId"];
      gameId++;
      HttpContext.Current.Application["GameId"] = gameId;
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.GameId = gameId;
      var path=HttpContext.Current.Server.MapPath($@"~\app_data\{gameId}.mgz");
      fromPlayer.UploadingStream = File.Create(path);
    }

    public async Task UploadRec(int pos,byte[] data)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      await fromPlayer.UploadingStream.WriteAsync(data, 0, data.Length);
    }

    public void EndRec()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.UploadingStream?.Close();
    }
  }

  public interface IPlayerClient
  {
    void OnLogin(int vip);
    void Receive(string message);
    void OnReceiveFrom(int fromIp,int fromPort, int toPort, byte[] data);
    void OnTcpConnect(int fromIp, int fromPort, int toPort);
    void OnTcpSend(int fromIp,int fromPort, int toPort, byte[] data);
  }
}