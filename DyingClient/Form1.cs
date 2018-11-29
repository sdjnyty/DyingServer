using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using EasyHook;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace DyingClient
{
  public partial class Form1 : Form
  {
    private const string URL = "http://47.93.96.30:81/";
    //private const string URL = "http://localhost:58985/";
    private string _userId;
    private HubConnection _hc;
    private IHubProxy _hp;
    private string exePath = (string)Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("EXE Path");
    private UdpClient _udpOutgoing = new UdpClient(123 * 256 + 123);
    private UdpClient _udpIncoming = new UdpClient(122 * 256 + 122);
    private TcpListener _tcpOutgoing = new TcpListener(IPAddress.Loopback, 123 * 256 + 123);
    //Tuple<RemoteIp,RemotePort,LocalPort>
    private Dictionary<Tuple<int, int, int>, TcpClient> _tcpIncoming = new Dictionary<Tuple<int, int, int>, TcpClient>();
    private int _vip;

    public Form1()
    {
      InitializeComponent();
      _hc = new HubConnection(URL);
      Load += Form1_Load;
    }

    private async void Form1_Load(object sender, EventArgs e)
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
            await _hp.Invoke("SendTo", toIp, toPort, result.RemoteEndPoint.Port, data);
            AppendLine($"SendTo from={result.RemoteEndPoint} to={new IPAddress(toIp)}:{toPort} data={BitConverter.ToString(data)}");
          }
        }
      }
      catch (ObjectDisposedException)
      {

      }
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
      _hp = _hc.CreateHubProxy("PlayerHub");
      _hp.On<int>(nameof(OnLogin), OnLogin);
      _hp.On<int, int, int, byte[]>(nameof(OnReceiveFrom), OnReceiveFrom);
      _hp.On<int, int, int>(nameof(OnTcpConnect), OnTcpConnect);
      _hp.On<int, int, int, byte[]>(nameof(OnTcpSend), OnTcpSend);
      await _hc.Start();
      _userId = Guid.NewGuid().ToString();
      await _hp.Invoke("Login", _userId);
      _tcpOutgoing.Start();
      while (true)
      {
        var tcpClient = await _tcpOutgoing.AcceptTcpClientAsync();
        var _ = Task.Run(async () => await TcpTask(tcpClient));
      }
    }

    private async Task TcpTask(TcpClient tcp)
    {
      var localPort = ((IPEndPoint)tcp.Client.RemoteEndPoint).Port;
      var stream = tcp.GetStream();
      var buffer = new byte[1024];
      stream.Read(buffer, 0, 8);
      var remoteIp = BitConverter.ToInt32(buffer, 0);
      var remotePort = BitConverter.ToInt32(buffer, 4);
      try
      {
        _tcpIncoming.Add(Tuple.Create(remoteIp, remotePort, localPort), tcp);
        await _hp.Invoke("TcpConnect", remoteIp, remotePort, localPort);
        AppendLine($"TcpConnect to={new IPAddress(remoteIp)}:{remotePort} from={localPort}");
        while (true)
        {
          var count = stream.Read(buffer, 0, 1024);
          if (count == 0) break;
          var data = new byte[count];
          Buffer.BlockCopy(buffer, 0, data, 0, count);
          await _hp.Invoke("TcpSend", localPort, remoteIp, remotePort, data);
          AppendLine($"TcpSend to={new IPAddress(remoteIp)}:{remotePort} from={localPort} data={BitConverter.ToString(data)}");
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

    private void btnDisconnect_Click(object sender, EventArgs e)
    {
      _hc.Stop();
      _udpOutgoing.Close();
      _udpIncoming.Close();
    }

    private void OnLogin(int vip)
    {
      _vip = vip;
      AppendLine(new IPAddress(vip).ToString());
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
      var stream = tcpIncoming.GetStream();
      stream.Write(BitConverter.GetBytes(remoteIp), 0, 4);
      stream.Write(BitConverter.GetBytes(remotePort), 0, 4);
      var buffer = new byte[1024];
      await Task.Run(async () =>
      {
        try
        {
          while (true)
          {
            var count = stream.Read(buffer, 0, 1024);
            if (count == 0) break;
            var data = new byte[count];
            Buffer.BlockCopy(buffer, 0, data, 0, count);
            await _hp.Invoke("TcpSend", localPort, remoteIp, remotePort, data);
            AppendLine($"TcpSend from=:{localPort} to={new IPAddress(remoteIp)}:{remotePort}");
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
      });
    }

    private void OnTcpSend(int remoteIp, int remotePort, int localPort, byte[] data)
    {
      if (_tcpIncoming.TryGetValue(Tuple.Create(remoteIp, remotePort, localPort), out var tcpIncoming))
      {
        AppendLine($"OnTcpSend from={new IPAddress(remoteIp)}:{remotePort} to={localPort} data={BitConverter.ToString(data)}");
        tcpIncoming.GetStream().Write(data, 0, data.Length);
      }
    }

    private void btnRunHost_Click(object sender, EventArgs e)
    {
      string channel = null;
      RemoteHooking.IpcCreateServer<Injector.ServerInterface>(ref channel, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
      var dllPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Injector.dll");
      RemoteHooking.CreateAndInject(Path.Combine(exePath, @"age2_x1\age2_wk.exe"), "HOST_IP_LAUNCH \"hostName\"",
              0, dllPath, dllPath, out var pid, channel, dllPath, _vip, _userId);
    }

    private void btnRunClient_Click(object sender, EventArgs e)
    {
      string channel = null;
      RemoteHooking.IpcCreateServer<Injector.ServerInterface>(ref channel, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
      var dllPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Injector.dll");
      RemoteHooking.CreateAndInject(Path.Combine(exePath, @"age2_x1\age2_wk.exe"), "CLIENT_IP_LAUNCH \"clientName\" " + txt.Text,
              0, dllPath, dllPath, out var pid, channel, dllPath, _vip, _userId);
    }
  }
}
