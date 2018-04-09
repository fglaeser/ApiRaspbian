using ApiRaspbian.Tasks;
using Infra.Full.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ApiRaspbian.Startup))]

namespace ApiRaspbian
{
  public class Startup : IHostingStartup
  { 
    public void Configure(IWebHostBuilder builder)
    {
      builder.ConfigureServices((ctx, c) => {
        c.AddSingleton<ITaskObject, Reading>();
        //c.Configure<DataAccessRegistryOptions>(ctx.Configuration.GetSection("DataAccessRegistry"));
      });
    }
  }
}
