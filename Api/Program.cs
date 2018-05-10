using Infra.Full.Configuration;
using Infra.WebHost;
using System;

namespace apiraspbian
{
    public class Program : IServiceStatus
  {
    public ServiceState CurrentState { get; set; }

    public string ServiceName => "ApiRaspbian";

    public string ServiceDisplayName => "ApiRaspbian";

    public string ServiceDescription => "ApiRaspbian";

    public static void Main(string[] args)
    {
      Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", Environment.GetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES") + ";ApiRaspbian");
      WebHostWrapper.Run(args, new Program());
    }

    public void AfterStart()
    {    
    }

    public void AfterStop()
    {
    }

    public void BeforeStart()
    {
    }

    public void BeforeStop()
    {
    }
  }
}
