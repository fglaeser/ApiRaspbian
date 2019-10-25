using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Mgmt
{
  public enum Mode
  {
    Idle = 0,
    Cool,
    Heat
  }
  public class ThermostatManagement
  {
    public Mode CurrentMode { get; set; }
    public ThermostatManagement()
    {

    }
  }
}
