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
    private const string GROUP_NAME_LOBBY = "Lobby";
    private const int DEFAULT_MAX_PLAYERS = 8;

    public override Task OnConnected()
    {
      return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
      var pi = PlayerInfoPool.GetByCid(Context.ConnectionId);
      pi.UploadingStream?.Close();
      IpPool.RecycleIp(pi.Vip);
      if (pi.State== PlayerState.InRoom)
      {
        var ri = RoomPool.GetById(pi.RoomId);
        if (ri.HostId == pi.Id)
        {

        }
        else
        {
          Clients.Group(ri.Id.ToString()).LeaveRoom(pi.ToUserInfo());
        }
      }
      Clients.Others.UserLogout(pi.Id);
      PlayerInfoPool.Remove(pi);
      return base.OnDisconnected(stopCalled);
    }

    public Tuple<SignalRResult,int> Login(string userName, string passwordMd5)
    {
      var userId = DAL.Login(userName, passwordMd5);
      if (userId == 0)
      {
        return Tuple.Create(SignalRResult.BadPassword, default(int));
      }
      else
      {
        var pi = PlayerInfoPool.GetByUid(userId);
        if (pi == null)
        {
          var vip = IpPool.AllocateIp();
          pi = new PlayerInfo
          {
            Id = userId,
            Name = userName,
            Vip = vip,
            ConnectionId = Context.ConnectionId,
            State = PlayerState.InLobby,
          };
          PlayerInfoPool.Add(pi);
          Clients.Group(GROUP_NAME_LOBBY).UserLogin(pi.ToUserInfo());
          Groups.Add(Context.ConnectionId, GROUP_NAME_LOBBY);
          return Tuple.Create(SignalRResult.Success, pi.Vip);
        }
        else
        {
          return Tuple.Create(SignalRResult.AlreadyLoginedIn, default(int));
        }
      }
    }

    public IReadOnlyList <RoomInfo > GetRooms()
    {
      return RoomPool.Enumerate();
    }

    public IReadOnlyList<UserInfo> GetPlayers()
    {
      return PlayerInfoPool.Enumerate().Select(p => p.ToUserInfo()).ToList();
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

    public async Task<Tuple<SignalRResult, RoomInfo>> JoinRoom(int roomId)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      var ri = RoomPool.GetById(roomId);
      if(ri.Players.Count==0)
        //第一个进入房间的视为创建房间，设为房主
      {
        ri.HostId = fromPlayer.Id;
        ri.Name = fromPlayer.Name + " 的游戏";
        ri.State = RoomState.CanJoin;
        ri.MaxPlayers = DEFAULT_MAX_PLAYERS;
      }

      if (ri.Players.Count < ri.MaxPlayers)
      {
        ri.Players.Add(fromPlayer.Id);
        fromPlayer.RoomId = roomId;
        fromPlayer.State = PlayerState.InRoom;
        Clients.Group(roomId.ToString()).JoinRoom(ri, fromPlayer.ToUserInfo());
        await Groups.Remove(Context.ConnectionId, GROUP_NAME_LOBBY);
        await Groups.Add(fromPlayer.ConnectionId, ri.Id.ToString());
        return Tuple.Create(SignalRResult.Success, ri);
      }
      else
      {
        return Tuple.Create(SignalRResult.RoomFull, ri);
      }
    }

    public async Task<RoomInfo> SpectateRoom(int roomId)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      var ri = RoomPool.GetById(roomId);
      ri.Spectators.Add(fromPlayer.Id);
      Clients.Group(roomId.ToString()).SpectateRoom(ri, fromPlayer.ToUserInfo());
      await Groups.Add(fromPlayer.ConnectionId, roomId.ToString());
      fromPlayer.RoomId = roomId;
      fromPlayer.State = PlayerState.InRoom;
      return ri;
    }

    public async Task LeaveRoom()
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      var ri = RoomPool.GetById(fromPlayer.RoomId);
      if (ri.Players.Contains(fromPlayer.Id))
        //是游戏者退出房间
      {
        ri.Players.Remove(fromPlayer.Id);
        fromPlayer.State = PlayerState.InLobby;
        await Groups.Remove(fromPlayer.ConnectionId, ri.Id.ToString());
        await Groups.Add(fromPlayer.ConnectionId, GROUP_NAME_LOBBY);
        if (ri.HostId == fromPlayer.Id)
        {
          if (ri.Players.Count > 0)
            //是房主退出房间，且还有游戏者，则移交房主
          {
            var newHost = ri.Players[0];
            ri.HostId = newHost;
          }
        }
      }
      Clients.Group(ri.Id.ToString()).LeaveRoom(fromPlayer.ToUserInfo());
    }

    public int GetHostVipByRoomId(int roomId)
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

    public void Chat(string message)
    {
      var fromPlayer = PlayerInfoPool.GetByCid(Context.ConnectionId);
      if (fromPlayer.State == PlayerState.InRoom)
      {
        Clients.Group(fromPlayer.RoomId.ToString()).Chat(fromPlayer.Id, message);
      }
    }

    private static string GetChatGroupName(RoomInfo ri)
    {
      return ri.ToString();
    }
  }

  public interface IPlayerClient
  {
    void Chat(int userId, string message);
    void OnReceiveFrom(int fromIp, int fromPort, int toPort, byte[] data);
    void OnTcpConnect(int fromIp, int fromPort, int toPort);
    void OnTcpSend(int fromIp, int fromPort, int toPort, byte[] data);
    void UserLogin(UserInfo ui);
    void UserLogout(int userId);
    void JoinRoom(RoomInfo ri, UserInfo ui);
    void DestroyRoom(string roomId);
    void LeaveRoom(UserInfo ui);
    void HostStartGame();
    void RunGame();
    void SpectateRoom(RoomInfo ri, UserInfo ui);
    void UploadRec(byte[] data);
    void RunSpectate();
    void EndRec();
  }
}