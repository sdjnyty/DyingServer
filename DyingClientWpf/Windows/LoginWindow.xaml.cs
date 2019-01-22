using System.Windows;
using POCO;

namespace DyingClientWpf
{
  /// <summary>
  /// Interaction logic for LoginWindow.xaml
  /// </summary>
  public partial class LoginWindow : Window
  {
    public LoginWindow()
    {
      InitializeComponent();
    }

    private async void btnLogin_Click(object sender, RoutedEventArgs e)
    {
      btnLogin.IsEnabled = false;
      var md5 = Util.HashMd5(pwdPassword.Password);
      var lr = await SignalRClient.Login(txxUserId.Text, md5);
      switch (lr.Result)
      {
        case LoginResultKind.Success:
          DialogResult = true;
          Close();
          break;
        case LoginResultKind.Fail:
          MessageBox.Show("用户名或密码不正确");
          break;
        case LoginResultKind.AlreadyLoginedIn:
          MessageBox.Show("用户已经登录，退出后方可再次登录");
          Close();
          break;
      }
      btnLogin.IsEnabled = true;
    }
  }
}
