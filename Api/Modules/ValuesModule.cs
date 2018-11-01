using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Modules
{
  public class ValuesModule : Nancy.NancyModule
  {
    public ValuesModule() : base("api/values")
    {
      Get("/{id}", p => { return "value"; });
    }
  }
}
