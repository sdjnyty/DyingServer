using System.Windows;
using System.ComponentModel;

namespace DyingClientWpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class LobbyWindow : Window
  {
    public LobbyWindow()
    {
      InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      var result=MessageBox.Show("确定要退出呆鹰帝国对战平台吗？", "退出", MessageBoxButton.OKCancel);
      if(result== MessageBoxResult.Cancel)
      {
        e.Cancel = true;
      }
      else if(result== MessageBoxResult.OK)
      {
        SignalRClient.Stop();
        Program.Shutdown();
      }
    }
  }
}
