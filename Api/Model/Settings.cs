using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Model
{
  public class Settings
  {
    public string Id { get; set; }
    public int PollingInterval { get; set; }

    public string PollingSensorsPath { get; set; }

    public string PollingTemperatureFile { get; set; }

    public string SensorInsideId { get; set; }

    public string SensorOutsideId { get; set; }

    #region Thermostat

    public float TargetTemperature { get; set; }

    public int PinCool { get; set; }
    
    public int PinHeat { get; set; }

    public float Diff { get; set; }
    #endregion
  }
}
