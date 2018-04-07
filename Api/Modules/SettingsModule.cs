using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Modules
{
  public class SettingsModule : Nancy.NancyModule
  {
    public SettingsModule(): base("/api/_settings")
    {
      Get("/", (p) => 
      {
        return "No Settings yet.";
      });
    }
  }
}
