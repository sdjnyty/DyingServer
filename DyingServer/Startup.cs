using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DyingServer.Startup))]

namespace DyingServer
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      var config=new Microsoft.AspNet.SignalR.HubConfiguration
      {
        EnableDetailedErrors = true,
      };
      app.MapSignalR(config);
    }
  }
}
