using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DyingClientWpf
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup(object sender, StartupEventArgs e)
    {
     Program. LoginWindow = new LoginWindow();
      var result=Program.LoginWindow.ShowDialog();
      if(result==true)
      {
        Program.LobbyWindow = new LobbyWindow();
        Program.LobbyWindow.Show();
      }
      else
      {
        Shutdown();
      }
    }
  }

  public static class Program
  {
    public static LoginWindow LoginWindow;
    public static LobbyWindow LobbyWindow;

    public static void Shutdown()
    {
      Application.Current.Shutdown();
    }
  }
}
