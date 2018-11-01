using ApiRaspbian.Mgmt;
using ApiRaspbian.Model;
using Infra.Data;
using Infra.Full.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiRaspbian.Tasks
{
  public class Reading : ITaskObject
  {
    ILogger<Reading> _logger;
    IDataAccessRegistry _dataAccessRegistry;
    SettingsManagement _settingsMgmt;
    public IDataAccess DataAccess => _dataAccessRegistry.GetDataAccess();
    public TimeSpan? WaitTimeout => null;

    public string TaskName => GetType().Name;

    private Settings _settings;

    public Reading(ILogger<Reading> logger, IDataAccessRegistry dataAccessRegistry, SettingsManagement settingsMgmt)
    {
      _logger = logger;
      _dataAccessRegistry = dataAccessRegistry;
      _settingsMgmt = settingsMgmt;
    }

    public async Task StartAsync(CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        StoreTemperature();
        await Task.Delay(TimeSpan.FromMinutes(_settings.PollingInterval), token).ConfigureAwait(false);
      }

      return;
      //ID Sensor/28-000008e36946
                //28-000008e28be0
    }

    private void StoreTemperature()
    {
      _settings = _settingsMgmt.GetSettings();
      var tempIn = GetTemperatureFromFile(Path.Combine(_settings.PollingSensorsPath, _settings.SensorInsideId, _settings.PollingTemperatureFile));
      var tempOut = GetTemperatureFromFile(Path.Combine(_settings.PollingSensorsPath, _settings.SensorOutsideId, _settings.PollingTemperatureFile));
      DataAccess.Insert(new Temperature
      {
        Inside = tempIn,
        Outside = tempOut,
        Timestamp = DateTime.Now
      });
      _logger.LogInformation($"Temp IN : {tempIn} , Temp OUT {tempOut}");
    }
    private float GetTemperatureFromFile(string filePath)
    {
      if (!File.Exists(filePath)) return 0.0f;
      var lines = File.ReadAllLines(filePath);
      var secondLine = lines[1];
      var temperatureData = secondLine.Split(" ")[9];
      var temperature = temperatureData.Split("=")[1];
      return float.Parse(temperature) / 1000;
    }

  }
}
