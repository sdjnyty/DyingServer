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
      if (pi.RoomId != null)
      {
        var ri = RoomPool.GetById(pi.RoomId);
        if (ri.HostId == pi.UserId)
        {
          Clients.All.DestroyRoom(ri.RoomId);
          RoomPool.Remove(ri);
        }
        else
        {
          Clients.Group(ri.RoomId).LeaveRoom(pi.UserId);
        }
      }
      Clients.Others.UserLogout(pi.UserId);
      PlayerInfoPool.Remove(pi);
      return base.OnDisconnected(stopCalled);
    }

    public void Broadcast(string message)
    {
      Clients.All.Receive(message);
    }

    public LoginResult Login(string userId)
    {
      var vip = IpPool.AllocateIp();
      var pi = new PlayerInfo
      {
        UserId = userId,
        Vip = vip,
        ConnectionId = Context.ConnectionId,
      };
      PlayerInfoPool.Add(pi);
      Clients.Others.UserLogin(userId);
      return new LoginResult
      {
        Vip = vip,
        OnlineUsers = PlayerInfoPool.Enumerate().Select(p => p.UserId).ToList(),
      };
    }

    public void SendTo(int toIp, int toPort, int fromPort, byte[] data)
    {
      var toPlayer = PlayerInfoPool.GetByVip(toIp);
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Client(toPlayer.ConnectionId).OnReceiveFrom(fromPlayer.Vip, fromPort, toPort, data);
    }

    public void TcpConnect(int toIp, int toPort, int fromPort)
    {
      var toPlayer = PlayerInfoPool.GetByVip(toIp);
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Client(toPlayer.ConnectionId).OnTcpConnect(fromPlayer.Vip, fromPort, toPort);
    }

    public void TcpSend(int fromPort, int toIp, int toPort, byte[] data)
    {
      var toPlayer = PlayerInfoPool.GetByVip(toIp);
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Client(toPlayer.ConnectionId).OnTcpSend(fromPlayer.Vip, fromPort, toPort, data);
    }

    public void BeginRec()
    {
      var gameId = (int)HttpContext.Current.Application["GameId"];
      gameId++;
      HttpContext.Current.Application["GameId"] = gameId;
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.GameId = gameId;
      var path = HttpContext.Current.Server.MapPath($@"~\app_data\{gameId}.mgz");
      fromPlayer.UploadingStream = File.Create(path);
    }

    public async Task UploadRec(int pos, byte[] data)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      await fromPlayer.UploadingStream.WriteAsync(data, 0, data.Length);
    }

    public void EndRec()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.UploadingStream?.Close();
    }

    public async Task< string> CreateRoom()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      var roomId = fromPlayer.UserId + " 的房间";
      var ri = new RoomInfo
      {
        HostId = fromPlayer.UserId,
        RoomId = roomId,
      };
      ri.Players.Add(fromPlayer.UserId);
      RoomPool.Add(ri);
      fromPlayer.RoomId = roomId;
      await Groups.Add(fromPlayer.ConnectionId, roomId);
      Clients.Others.CreateRoom(roomId);
      return roomId;
    }

    public void DestroyRoom()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Others.DestroyRoom(fromPlayer.RoomId);
      var ri = RoomPool.GetById(fromPlayer.RoomId);
      RoomPool.Remove(ri);
      fromPlayer.RoomId = null;
    }

    public List<string> JoinRoom(string roomId)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.RoomId = roomId;
      var ri = RoomPool.GetById(roomId);
      ri.Players.Add(fromPlayer.UserId);
      Clients.Group(roomId).JoinRoom(roomId, fromPlayer.UserId);
      Groups.Add(fromPlayer.ConnectionId, roomId);
      return ri.Players;
    }

    public void LeaveRoom()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      var ri = RoomPool.GetById(fromPlayer.RoomId);
      ri.Players.Remove(fromPlayer.UserId);
      Groups.Remove(fromPlayer.ConnectionId, ri.RoomId);
      Clients.Group(ri.RoomId).LeaveRoom(fromPlayer.UserId);
      fromPlayer.RoomId = null;
    }
  }

  public interface IPlayerClient
  {
    void Receive(string message);
    void OnReceiveFrom(int fromIp, int fromPort, int toPort, byte[] data);
    void OnTcpConnect(int fromIp, int fromPort, int toPort);
    void OnTcpSend(int fromIp, int fromPort, int toPort, byte[] data);
    void UserLogin(string userId);
    void UserLogout(string userId);
    void CreateRoom(string roomId);
    void JoinRoom(string roomId,string userId);
    void DestroyRoom(string roomId);
    void LeaveRoom(string userId);
  }

  public class LoginResult
  {
    public int Vip { get; set; }
    public List<string> OnlineUsers { get; set; }
  }
}