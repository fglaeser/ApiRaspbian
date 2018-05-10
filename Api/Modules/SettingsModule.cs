using ApiRaspbian.Model;
using ApiRaspbian.Requests;
using Infra.Data;
using Nancy;
using Nancy.ModelBinding;

namespace ApiRaspbian.Modules
{
  public class SettingsModule : Nancy.NancyModule
  {
    IDataAccessRegistry _dataAccessRegistry;
    public IDataAccess DataAccess => _dataAccessRegistry.GetDataAccess();

    public SettingsModule(IDataAccessRegistry dataAccessRegistry): base("/api/_settings")
    {
      _dataAccessRegistry = dataAccessRegistry;

      Get("/", (p) => 
      {
        var settings = DataAccess.Get<Settings>("main");
        return Negotiate.WithModel(new SettingsRequest
        {
          PollingInterval = settings.PollingInterval,
          PollingSensorsPath = settings.PollingSensorsPath, 
          PollingTemperatureFile = settings.PollingTemperatureFile,
          SensorInsideId = settings.SensorInsideId,
          SensorOutsideId = settings.SensorOutsideId
        });
      });

      Put("/", (p) =>
      {
        var req = this.Bind<SettingsRequest>();
        var settings = DataAccess.Get<Settings>("main");
        settings.PollingInterval = req.PollingInterval == default(int) ? settings.PollingInterval : req.PollingInterval;
        settings.PollingSensorsPath = req.PollingSensorsPath ?? settings.PollingSensorsPath;
        settings.PollingTemperatureFile = req.PollingTemperatureFile ?? settings.PollingTemperatureFile;
        settings.SensorInsideId = req.SensorInsideId ?? settings.SensorInsideId;
        settings.SensorOutsideId = req.SensorOutsideId ?? settings.SensorOutsideId;
        DataAccess.Update(settings);
        return Negotiate.WithStatusCode(HttpStatusCode.OK);
      });
    }
  }
}
