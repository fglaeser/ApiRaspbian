using ApiRaspbian.Mgmt;
using ApiRaspbian.Tasks;
using Infra.Data;
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
        c.AddSingleton<ITaskObject, Thermostat>();
        c.AddSingleton<ITaskObject, SheetPublisher>();
        c.AddSingleton<IDataAccessRegistry, DataAccessRegistry>();
        c.AddSingleton<SettingsManagement>();
        c.Configure<DataAccessRegistryOptions>(ctx.Configuration.GetSection("DataAccessRegistry"));
      });
    }
  }
}
