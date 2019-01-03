using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using EasyHook;
using System.IO;
using System.Net.Sockets;
using System.Net;
using POCO;

namespace DyingClient
{
  public partial class FrmLobby : Form
  {
    private const string URL = "http://47.93.96.30:81/";
    private string _userId;
    private string _roomId;
    private HubConnection _hc;
    internal IHubProxy _hp;
    private string exePath = (string)Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("EXE Path");
    private UdpClient _udpOutgoing;
    private UdpClient _udpIncoming;
    private TcpListener _tcpOutgoing;
    private TcpListener _tcpRec;
    //Tuple<RemoteIp,RemotePort,LocalPort>
    private Dictionary<Tuple<int, int, int>, TcpClient> _tcpIncoming = new Dictionary<Tuple<int, int, int>, TcpClient>();
    private int _vip;
    private BlockingCollection<Tuple<int, int, int, byte[]>> _qUdpOut = new BlockingCollection<Tuple<int, int, int, byte[]>>();
    private BlockingCollection<byte[]> _qRec = new BlockingCollection<byte[]>();
    private CancellationTokenSource _cts;
    private HashSet<IDisposable> _subscribers = new HashSet<IDisposable>();

    public FrmLobby()
    {
      InitializeComponent();
      _hc = new HubConnection(URL);
      Load += Form1_Load;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      btnLogout.Enabled = false;
      btnCreateRoom.Enabled = false;
      btnJoinRoom.Enabled = false;
      btnLeaveRoom.Enabled = false;
      btnCancelRoom.Enabled = false;
      btnRun.Enabled = false;
      btnSpectate.Enabled = false;
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtUserName.Text))
      {
        MessageBox.Show("用户名不能为空");
      }
      else
      {
        btnLogin.Enabled = false;
        _hp = _hc.CreateHubProxy("PlayerHub");
        _subscribers.Add( _hp.On<int, int, int, byte[]>(nameof(OnReceiveFrom), OnReceiveFrom));
        _subscribers.Add( _hp.On<int, int, int>(nameof(OnTcpConnect), OnTcpConnect));
        _subscribers.Add( _hp.On<int, int, int, byte[]>(nameof(OnTcpSend), OnTcpSend));
        _subscribers.Add( _hp.On<string>(nameof(UserLogin), UserLogin));
        _subscribers.Add( _hp.On<string>(nameof(UserLogout), UserLogout));
        _subscribers.Add( _hp.On<string>(nameof(CreateRoom), CreateRoom));
        _subscribers.Add( _hp.On<string, string>(nameof(JoinRoom), JoinRoom));
        _subscribers.Add( _hp.On<string, string>(nameof(SpectateRoom), SpectateRoom));
        _subscribers.Add(_hp.On<string>(nameof(DestroyRoom), DestroyRoom));
        _subscribers.Add(_hp.On<string>(nameof(LeaveRoom), LeaveRoom));
        _subscribers.Add(_hp.On(nameof(RunGame), RunGame));
        _subscribers.Add(_hp.On<string, string>(nameof(Chat), Chat));
        _subscribers.Add(_hp.On<byte[]>("UploadRec", RecReceived));
        _subscribers.Add(_hp.On(nameof(RunSpectate), RunSpectate));
        _subscribers.Add(_hp.On(nameof(EndRec), EndRec));
        await _hc.Start();
        _hc.Closed += _hc_Closed;
        _userId = txtUserName.Text;
        var lr = await _hp.Invoke<LoginResult>("Login", _userId);
        _vip = lr.Vip;
        AppendLine($"登录成功，用户名 {_userId} ;虚拟IP {new IPAddress(_vip)}");
        lbxOnlineUsers.Items.AddRange(lr.OnlineUsers.ToArray());
        btnLogout.Enabled = true;
        btnCreateRoom.Enabled = true;
        btnJoinRoom.Enabled = true;
        btnSpectate.Enabled = true;
      }
    }

    private void _hc_Closed()
    {
      _hc.Closed -= _hc_Closed;
      foreach(var subsc in _subscribers)
      {
        subsc.Dispose();
      }
      _subscribers.Clear();
      AppendLine("已离线");
      lbxOnlineUsers.Items.Clear();
      btnLogin.Enabled = true;
      btnLogout.Enabled = false;
      btnJoinRoom.Enabled = false;
      btnCreateRoom.Enabled = false;
      btnCancelRoom.Enabled = false;
      btnSpectate.Enabled = false;
      btnLogout.Enabled = false;
      lbxRooms.Items.Clear();
      lbxRoomUsers.Items.Clear();
      lbxSpectators.Items.Clear();
    }

    private async Task TcpOut(TcpClient tcp)
    {
      var localPort = ((IPEndPoint)tcp.Client.RemoteEndPoint).Port;
      var stream = tcp.GetStream();
      var buffer = new byte[1024];
      await stream.ReadAsync(buffer, 0, 8);
      var remoteIp = BitConverter.ToInt32(buffer, 0);
      var remotePort = BitConverter.ToInt32(buffer, 4);
      try
      {
        _tcpIncoming.Add(Tuple.Create(remoteIp, remotePort, localPort), tcp);
        await _hp.Invoke("TcpConnect", remoteIp, remotePort, localPort);
        AppendLine($"TcpConnect to={new IPAddress(remoteIp)}:{remotePort} from={localPort}");
        while (true)
        {
          var count = await stream.ReadAsync(buffer, 0, 1024);
          var data = new byte[count];
          Buffer.BlockCopy(buffer, 0, data, 0, count);
          await _hp.Invoke("TcpSend", localPort, remoteIp, remotePort, data);
          AppendLine($"TcpSend to={new IPAddress(remoteIp)}:{remotePort} from={localPort} data={BitConverter.ToString(data)}");
          if (count == 0) break;
        }
      }
      catch (IOException)
      {
      }
      finally
      {
        _tcpIncoming.Remove(Tuple.Create(remoteIp, remotePort, localPort));
        AppendLine($"TcpClient closed local=:{localPort} remote={new IPAddress(remoteIp)}:{remotePort}");
      }
    }

    private void AppendLine(string line)
    {
      rtb.Invoke((Action)(() => rtb.AppendText(line + "\n")));
    }

    private async void OnReceiveFrom(int fromVip, int fromPort, int toPort, byte[] data)
    {
      using (var ms = new MemoryStream(data.Length + 6))
      using (var bw = new BinaryWriter(ms))
      {
        bw.Write(fromVip);
        bw.Write((ushort)fromPort);
        bw.Write(data);
        var data1 = ms.ToArray();
        AppendLine($"OnReceiveFrom from={new IPAddress(fromVip)}:{fromPort} to={toPort} data={BitConverter.ToString(data)}");
        await _udpIncoming.SendAsync(data1, data1.Length, new IPEndPoint(IPAddress.Loopback, toPort));
      }
    }

    private async void OnTcpConnect(int remoteIp, int remotePort, int localPort)
    {
      var tcpIncoming = new TcpClient(AddressFamily.InterNetwork);
      await tcpIncoming.ConnectAsync(IPAddress.Loopback, localPort);
      _tcpIncoming[Tuple.Create(remoteIp, remotePort, localPort)] = tcpIncoming;
      AppendLine($"OnTcpConnect remote={new IPAddress(remoteIp)}:{remotePort} local=:{localPort}");
      var stream = tcpIncoming.GetStream();
      stream.Write(BitConverter.GetBytes(remoteIp), 0, 4);
      stream.Write(BitConverter.GetBytes(remotePort), 0, 4);
      var buffer = new byte[1024];
      try
      {
        while (true)
        {
          var count = await stream.ReadAsync(buffer, 0, 1024);
          var data = new byte[count];
          Buffer.BlockCopy(buffer, 0, data, 0, count);
          await _hp.Invoke("TcpSend", localPort, remoteIp, remotePort, data);
          AppendLine($"TcpSend from=:{localPort} to={new IPAddress(remoteIp)}:{remotePort}");
          if (count == 0) break;
        }
      }
      catch (IOException)
      {
      }
      catch (ObjectDisposedException)
      {

      }
      finally
      {
        _tcpIncoming.Remove(Tuple.Create(remoteIp, remotePort, localPort));
        AppendLine($"TcpClient closed local=:{localPort} remote={new IPAddress(remoteIp)}:{remotePort}");
      }
    }

    private void OnTcpSend(int remoteIp, int remotePort, int localPort, byte[] data)
    {
      if (_tcpIncoming.TryGetValue(Tuple.Create(remoteIp, remotePort, localPort), out var tcpIncoming))
      {
        AppendLine($"OnTcpSend from={new IPAddress(remoteIp)}:{remotePort} to={localPort} data={BitConverter.ToString(data)}");
        if (data.Length == 0)
        {
          tcpIncoming.Close();
        }
        else
        {
          tcpIncoming.GetStream().Write(data, 0, data.Length);
        }
      }
    }

    private async Task UploadRec()
    {
      var tcpClient = new TcpClient(AddressFamily.InterNetwork);
      do
      {
        try
        {
          await Task.Delay(1000);
          await tcpClient.ConnectAsync(IPAddress.Loopback, 53754);
        }
        catch (SocketException)
        {

        }
      } while (!tcpClient.Connected);
      var stream = tcpClient.GetStream();
      await _hp.Invoke("BeginRec");
      var buffer = new byte[2048];
      try
      {
        while (true)
        {
          var count = stream.Read(buffer, 0, buffer.Length);
          if (count == 0) break;
          var upload = new byte[count];
          Array.Copy(buffer, upload, count);
          await _hp.Invoke("UploadRec", upload);
        }
      }
      catch (IOException)
      {

      }
      finally
      {
        tcpClient.Close();
        await _hp.Invoke("EndRec");
      }
    }

    private async Task ProduceUdpOutQ()
    {
      try
      {
        while (true)
        {
          var result = await _udpOutgoing.ReceiveAsync();
          using (var ms = new MemoryStream(result.Buffer))
          using (var sr = new BinaryReader(ms))
          {
            var toIp = sr.ReadInt32();
            var toPort = (int)sr.ReadUInt16();
            var data = sr.ReadBytes((int)ms.Length - 6);
            _qUdpOut.TryAdd(Tuple.Create(toIp, toPort, result.RemoteEndPoint.Port, data));
            AppendLine($"SendTo from={result.RemoteEndPoint} to={new IPAddress(toIp)}:{toPort} data={BitConverter.ToString(data)}");
          }
        }
      }
      catch (SocketException)
      {

      }
    }

    private async Task ConsumeUdpOutQ()
    {
      try
      {
        while (true)
        {
          var item = await Task.Run(() =>
          {
            _qUdpOut.TryTake(out var tup, -1);
            return tup;
          });
          await _hp.Invoke("SendTo", item.Item1, item.Item2, item.Item3, item.Item4);
        }
      }
      catch (SocketException)
      {

      }
    }

    private void UserLogin(string userId)
    {
      lbxOnlineUsers.Invoke((Action)(() =>
     {
       lbxOnlineUsers.Items.Add(userId);
     }));
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
      AppendLine("正在登出...");
      btnJoinRoom.Enabled = false;
      btnCreateRoom.Enabled = false;
      btnCancelRoom.Enabled = false;
      btnSpectate.Enabled = false;
      btnLogout.Enabled = false;
      lbxRooms.Items.Clear();
      lbxRoomUsers.Items.Clear();
      lbxSpectators.Items.Clear();
      _hc.Stop();
    }

    private async void btnCreateRoom_Click(object sender, EventArgs e)
    {
      btnCreateRoom.Enabled = false;
      _roomId = await _hp.Invoke<string>("CreateRoom");
      AppendLine($"创建了房间“{_roomId}”");
      lbxRooms.Items.Add(_roomId);
      lbxRoomUsers.Items.Add(_userId);
      AppendLine($"已进入房间“{_roomId}”");
      btnJoinRoom.Enabled = false;
      btnLeaveRoom.Enabled = false;
      btnCancelRoom.Enabled = true;
      btnRun.Enabled = true;
      btnSpectate.Enabled = false;
    }

    private void CreateRoom(string roomId)
    {
      lbxRooms.Invoke((Action)(() =>
     {
       lbxRooms.Items.Add(roomId);
     }));

    }

    private void UserLogout(string userId)
    {
      lbxOnlineUsers.Invoke((Action)(() => { lbxOnlineUsers.Items.Remove(userId); }));
    }

    private async void btnCancelRoom_Click(object sender, EventArgs e)
    {
      btnCancelRoom.Enabled = false;
      await _hp.Invoke("DestroyRoom");
      btnCreateRoom.Enabled = true;
      btnRun.Enabled = false;
      btnSpectate.Enabled = true;
      AppendLine("已取消房间");
      lbxRooms.Items.Remove(_roomId);
      lbxRoomUsers.Items.Clear();
    }

    private async void btnJoinRoom_Click(object sender, EventArgs e)
    {
      if (lbxRooms.SelectedItem is string roomId)
      {
        btnJoinRoom.Enabled = false;
        btnCreateRoom.Enabled = false;
        btnSpectate.Enabled = false;
        var roomInfo = await _hp.Invoke<RoomInfo>("JoinRoom", roomId);
        _roomId = roomId;
        lbxRoomUsers.Items.AddRange(roomInfo.Players.ToArray());
        lbxSpectators.Items.AddRange(roomInfo.Spectators.ToArray());
        btnLeaveRoom.Enabled = true;
        AppendLine($"已进入房间 “{_roomId}”");
      }
      else
      {
        MessageBox.Show("请选择房间");
      }
    }

    private void JoinRoom(string roomId, string userId)
    {
      if (_roomId == roomId)
      {
        AppendLine($"{userId} 进入了房间");
        lbxRoomUsers.Invoke((Action)(() => { lbxRoomUsers.Items.Add(userId); }));
      }
    }

    private void SpectateRoom(string roomId, string userId)
    {
      if (_roomId == roomId)
      {
        AppendLine($"{userId} 前来围观");
        lbxSpectators.Invoke((Action)(() => { lbxSpectators.Items.Add(userId); }));
      }
    }

    private async void btnLeaveRoom_Click(object sender, EventArgs e)
    {
      btnLeaveRoom.Enabled = false;
      await _hp.Invoke("LeaveRoom");
      _roomId = null;
      btnCreateRoom.Enabled = true;
      btnJoinRoom.Enabled = true;
      btnSpectate.Enabled = true;
      lbxRoomUsers.Items.Clear();
      AppendLine("已离开房间");
    }

    private void LeaveRoom(string userId)
    {
      AppendLine($"{userId} 离开了房间");
      lbxRoomUsers.Invoke((Action)(() => { lbxRoomUsers.Items.Remove(userId); }));
    }

    private void DestroyRoom(string roomId)
    {
      lbxRooms.Invoke((Action)(() => { lbxRooms.Items.Remove(roomId); }));
      if (_roomId == roomId)
      {
        _roomId = null;
        btnLeaveRoom.Enabled = false;
        btnCreateRoom.Enabled = true;
        btnJoinRoom.Enabled = true;
        lbxRoomUsers.Items.Clear();
        AppendLine($"房间 {roomId} 已被取消");
      }
    }

    private async void btnRun_Click(object sender, EventArgs e)
    {
      btnRun.Enabled = false;
      AppendLine("正在启动游戏");
      string channel = null;
      RemoteHooking.IpcCreateServer<Injector.ServerInterface>(ref channel, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
      var dllPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Injector.dll");
      var _ = SetupSockets();
      RemoteHooking.CreateAndInject(Path.Combine(exePath, @"age2_x1\age2_wk.exe"), $"HOST_IP_LAUNCH \"{_userId}\"", 0, dllPath, dllPath, out var pid, channel, dllPath, _vip, _userId);
      var process = Process.GetProcessById(pid);
      AppendLine("游戏已启动");
      await Task.Delay(12000);
      await _hp.Invoke("RunGame");
      var _1 = UploadRec();
      await WaitForExitAsync(process);
      AppendLine("游戏已退出");
      TearDownSockets();
      btnRun.Enabled = true;
    }

    private async Task SetupSockets()
    {
      _udpOutgoing = new UdpClient(123 * 256 + 123);
      _udpIncoming = new UdpClient(122 * 256 + 122);
      var _ = ProduceUdpOutQ();
      _ = ConsumeUdpOutQ();
      _tcpOutgoing = new TcpListener(IPAddress.Loopback, 123 * 256 + 123);
      _tcpOutgoing.Start();
      try
      {
        while (true)
        {
          var tcpOut = await _tcpOutgoing.AcceptTcpClientAsync();
          var _1 = TcpOut(tcpOut);
        }
      }
      catch (SocketException)
      {

      }
    }

    private void TearDownSockets()
    {
      _tcpOutgoing.Stop();
      _udpOutgoing.Close();
      _udpIncoming.Close();
    }

    private static Task WaitForExitAsync(Process process)
    {
      var tcs = new TaskCompletionSource<object>();
      process.EnableRaisingEvents = true;
      process.Exited += (s, e) => tcs.TrySetResult(null);
      return tcs.Task;
    }

    private async void RunGame()
    {
      var hostIp = await _hp.Invoke<int>("GetHostVipByRoomId", _roomId);
      AppendLine("正在启动游戏");
      string channel = null;
      RemoteHooking.IpcCreateServer<Injector.ServerInterface>(ref channel, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
      var dllPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Injector.dll");
      var _ = SetupSockets();
      RemoteHooking.CreateAndInject(Path.Combine(exePath, @"age2_x1\age2_wk.exe"), $"CLIENT_IP_LAUNCH \"{_userId}\" {new IPAddress(hostIp)}", 0, dllPath, dllPath, out var pid, channel, dllPath, _vip, _userId);
      var process = Process.GetProcessById(pid);
      AppendLine("游戏已启动");
      await WaitForExitAsync(process);
      AppendLine("游戏已退出");
      TearDownSockets();
    }

    private async void btnSpectate_Click(object sender, EventArgs e)
    {
      if (lbxRooms.SelectedItem is string roomId)
      {
        btnJoinRoom.Enabled = false;
        btnSpectate.Enabled = false;
        btnCreateRoom.Enabled = false;
        var roomInfo = await _hp.Invoke<RoomInfo>("SpectateRoom", roomId);
        _roomId = roomId;
        lbxRoomUsers.Items.AddRange(roomInfo.Players.ToArray());
        lbxSpectators.Items.AddRange(roomInfo.Spectators.ToArray());
        btnLeaveRoom.Enabled = true;
        AppendLine($"已进入房间 “{_roomId}”");
      }
      else
      {
        MessageBox.Show("请选择房间");
      }
    }

    private async void txxChat_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Return)
      {
        await _hp.Invoke("Chat", txxChat.Text);
        txxChat.Text = "";
      }
    }

    private void Chat(string userId, string message)
    {
      AppendLine($"{userId}: {message}");
    }

    private void RecReceived(byte[] data)
    {
      _qRec.TryAdd(data);
    }

    private async void RunSpectate()
    {
      _tcpRec = new TcpListener(IPAddress.Loopback, 53754);
      _tcpRec.Start();
      var process = Process.Start(Path.Combine(exePath, @"age2_x1\spectate.exe"), "localhost");
      var tc = _tcpRec.AcceptTcpClient();
      _cts = new CancellationTokenSource();
      await Task.Run(() =>
      {
        try
        {
          while (true)
          {
            _qRec.TryTake(out var data, -1, _cts.Token);
            tc.GetStream().Write(data, 0, data.Length);
          }
        }
        catch (TaskCanceledException)
        {
          tc.Close();
        }
      });
    }

    private void EndRec()
    {
      _cts.Cancel();
    }
  }
}
