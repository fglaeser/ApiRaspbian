using ApiRaspbian.Mgmt;
using ApiRaspbian.Model;
using Infra.Data;
using Infra.Full.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace ApiRaspbian.Tasks
{
  public enum Mode
  {
    Idle = 0,
    Cool,
    Heat

  }
  public class Thermostat : ITaskObject
  {
    ILogger<Thermostat> _logger;
    SettingsManagement _settingsMgmt;
    IDataAccessRegistry _dataAccessRegistry;
    public TimeSpan? WaitTimeout => null;

    public string TaskName => GetType().Name;

    public Mode CurrentMode { get; set; }
 
    public IDataAccess DataAccess => _dataAccessRegistry.GetDataAccess();

    public Thermostat(ILogger<Thermostat> logger, SettingsManagement settingsMgmt, IDataAccessRegistry dataAccessRegistry)
    {
      _logger = logger;
      _settingsMgmt = settingsMgmt;
      _dataAccessRegistry = dataAccessRegistry;
    }

    public async Task StartAsync(CancellationToken token)
    {
      while(!token.IsCancellationRequested)
      {
        // Wait for some reading before check
        await Task.Delay(TimeSpan.FromMinutes(_settingsMgmt.GetSettings().PublishInterval), token);
        try
        {
          await Check();
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Exception checking temperature.");
        }
      }
      // gracefully shutdown
      await CoolOff();
      await HeatOff();
    }

    private async Task Check()
    {
      var temp = GetLastTemperature().Outside;
      var target = _settingsMgmt.GetSettings().TargetTemperature;
      var diff = _settingsMgmt.GetSettings().Diff;
      _logger.LogInformation("Checking Temp. Current Mode {0}. Temp {1} - Target {2} - Diff {3}", CurrentMode.ToString(), temp, target, diff);
      switch (CurrentMode)
      {
        case Mode.Idle:
          await CheckIdle(temp, target, diff);
          break;
        case Mode.Cool:
          await CheckCool(temp, target);
          break;
        case Mode.Heat:
          await CheckHeat(temp, target);
          break;
      }
    }

    private async Task CheckIdle(float temp, float target, float diff)
    {
      if (temp >= (target + diff))
      {
        await CoolOn();
        return;
      }

      if(temp <= ( target - diff))
      {
        await HeatOn();
      }
    }

    private async Task CheckCool(float temp, float target)
    {
      if (temp <= target)
        await CoolOff();
    }

    private async Task CheckHeat(float temp, float target)
    {
      if (temp >= target)
        await HeatOff();
    }

    private Temperature GetLastTemperature()
    {
      return DataAccess.Query<Temperature>("SELECT temp_in as Inside, temp_out as Outside, created_at as Timestamp FROM temperature ORDER BY created_at DESC LIMIT 1")
        .FirstOrDefault() ?? new Temperature();
    }

    private async Task CoolOn()
    {
      _logger.LogInformation("Setting Cool ON");
      CurrentMode = Mode.Cool;
      await WritePin(_settingsMgmt.GetSettings().PinCool, true);
    }

    private async Task CoolOff()
    {
      _logger.LogInformation("Setting Cool OFF");
      CurrentMode = Mode.Idle;
      await WritePin(_settingsMgmt.GetSettings().PinCool, false);
    }

    private async Task HeatOn()
    {
      _logger.LogInformation("Setting Heat ON");
      CurrentMode = Mode.Heat;
      await WritePin(_settingsMgmt.GetSettings().PinHeat, true);
    }

    private async Task HeatOff()
    {
      _logger.LogInformation("Setting Heat OFF");
      CurrentMode = Mode.Idle;
      await WritePin(_settingsMgmt.GetSettings().PinHeat, false);
    }

    private async Task WritePin(int pin, bool value)
    {
      var relayPin = Pi.Gpio[pin];
      relayPin.PinMode = GpioPinDriveMode.Output;
      await relayPin.WriteAsync(value);
    }
  }
}
