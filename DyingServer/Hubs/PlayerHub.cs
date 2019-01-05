using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.IO;
using POCO;

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

    public LoginResult Login(string userId,string passwordMd5)
    {
      var pi = PlayerInfoPool.GetByUid(userId);
      if (pi == null)
      {
        var success = DAL.Login(userId, passwordMd5);
        if (success)
        {
          var vip = IpPool.AllocateIp();
          pi = new PlayerInfo
          {
            UserId = userId,
            Vip = vip,
            ConnectionId = Context.ConnectionId,
          };
          PlayerInfoPool.Add(pi);
          Clients.Others.UserLogin(userId);
          return new LoginResult
          {
            Result = LoginResultKind.Success,
            Vip = pi.Vip,
            OnlineUsers = PlayerInfoPool.Enumerate().Select(p => p.UserId).ToList(),
            Rooms = RoomPool.Enumerate().Select(ri => ri.RoomId).ToList(),
          };
        }
        else
        {
          return new LoginResult
          {
            Result = LoginResultKind.Fail,
          };
        }
      }
      else
      {
        return new LoginResult
        {
          Result =  LoginResultKind.AlreadyLoginedIn,
        };
      }
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
      Clients.Group($"{fromPlayer.RoomId}:spec").RunSpectate();
    }

    public async Task UploadRec(byte[] data)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Group($"{fromPlayer.RoomId}:spec").UploadRec(data);
      await fromPlayer.UploadingStream.WriteAsync(data, 0, data.Length);
    }

    public void EndRec()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.UploadingStream?.Close();
      Clients.Group($"{fromPlayer.RoomId}:spec").EndRec();
    }

    public async Task<string> CreateRoom()
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
      await Groups.Add(fromPlayer.ConnectionId, $"{roomId}:player");
      Clients.Others.CreateRoom(roomId);
      return roomId;
    }

    public async Task DestroyRoom()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.Others.DestroyRoom(fromPlayer.RoomId);
      var ri = RoomPool.GetById(fromPlayer.RoomId);
      RoomPool.Remove(ri);
      await Groups.Remove(fromPlayer.ConnectionId, ri.RoomId);
      await Groups.Remove(fromPlayer.ConnectionId, $"{ri.RoomId}:player");
      fromPlayer.RoomId = null;
    }

    public async Task<RoomInfo> JoinRoom(string roomId)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.RoomId = roomId;
      var ri = RoomPool.GetById(roomId);
      ri.Players.Add(fromPlayer.UserId);
      Clients.Group(roomId).JoinRoom(roomId, fromPlayer.UserId);
      await Groups.Add(fromPlayer.ConnectionId, roomId);
      await Groups.Add(fromPlayer.ConnectionId, $"{roomId}:player");
      return ri;
    }

    public async Task LeaveRoom()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      var ri = RoomPool.GetById(fromPlayer.RoomId);
      ri.Players.Remove(fromPlayer.UserId);
      await Groups.Remove(fromPlayer.ConnectionId, ri.RoomId);
      if (ri.Players.Contains(fromPlayer.UserId))
      {
        await Groups.Remove(fromPlayer.ConnectionId, $"{ri.RoomId}:player");
      }
      else if (ri.Spectators.Contains(fromPlayer.UserId))
      {
        await Groups.Remove(fromPlayer.ConnectionId, $"{ri.RoomId}:spec");
      }
      Clients.Group(ri.RoomId).LeaveRoom(fromPlayer.UserId);
      fromPlayer.RoomId = null;
    }

    public int GetHostVipByRoomId(string roomId)
    {
      return PlayerInfoPool.GetByUid(RoomPool.GetById(roomId).HostId).Vip;
    }

    public void HostStartGame()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.OthersInGroup($"{fromPlayer.RoomId}:player").HostStartGame();
    }

    public void RunGame()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      Clients.OthersInGroup($"{fromPlayer.RoomId}:player").RunGame();
    }

    public async Task<RoomInfo> SpectateRoom(string roomId)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      fromPlayer.RoomId = roomId;
      var ri = RoomPool.GetById(roomId);
      ri.Spectators.Add(fromPlayer.UserId);
      Clients.Group(roomId).SpectateRoom(roomId, fromPlayer.UserId);
      await Groups.Add(fromPlayer.ConnectionId, roomId);
      await Groups.Add(fromPlayer.ConnectionId, $"{roomId}:spec");
      return ri;
    }

    public void Chat(string message)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      if (fromPlayer.RoomId != null)
      {
        Clients.Group(fromPlayer.RoomId).Chat(fromPlayer.UserId, message);
      }
    }
  }

  public interface IPlayerClient
  {
    void Chat(string userId, string message);
    void OnReceiveFrom(int fromIp, int fromPort, int toPort, byte[] data);
    void OnTcpConnect(int fromIp, int fromPort, int toPort);
    void OnTcpSend(int fromIp, int fromPort, int toPort, byte[] data);
    void UserLogin(string userId);
    void UserLogout(string userId);
    void CreateRoom(string roomId);
    void JoinRoom(string roomId, string userId);
    void DestroyRoom(string roomId);
    void LeaveRoom(string userId);
    void HostStartGame();
    void RunGame();
    void SpectateRoom(string roomId, string userId);
    void UploadRec(byte[] data);
    void RunSpectate();
    void EndRec();
  }
}