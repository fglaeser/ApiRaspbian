using ApiRaspbian.Mgmt;
using ApiRaspbian.Model;
using ApiRaspbian.Requests;
using Infra.Data;
using Nancy;
using Nancy.ModelBinding;

namespace ApiRaspbian.Modules
{
  public class SettingsModule : Nancy.NancyModule
  {
    readonly SettingsManagement _settingsMgmt;

    public SettingsModule(SettingsManagement settingsMgmt): base("/api/_settings")
    {
      _settingsMgmt = settingsMgmt;
      Get("/", (p) => 
      {
        var settings = _settingsMgmt.GetSettings();
        return Negotiate.WithModel(new SettingsRequest
        {
          PollingInterval = settings.PollingInterval,
          PollingSensorsPath = settings.PollingSensorsPath, 
          PollingTemperatureFile = settings.PollingTemperatureFile,
          SensorInsideId = settings.SensorInsideId,
          SensorOutsideId = settings.SensorOutsideId, 
          TargetTemperature = settings.TargetTemperature, 
          PinCool = settings.PinCool,
          PinHeat = settings.PinHeat,
          Diff = settings.Diff
        });
      });

      Put("/", (p) =>
      {
        var req = this.Bind<SettingsRequest>();
        var settings = _settingsMgmt.GetSettings();
        settings.PollingInterval = req.PollingInterval == default(int) ? settings.PollingInterval : req.PollingInterval;
        settings.PollingSensorsPath = req.PollingSensorsPath ?? settings.PollingSensorsPath;
        settings.PollingTemperatureFile = req.PollingTemperatureFile ?? settings.PollingTemperatureFile;
        settings.SensorInsideId = req.SensorInsideId ?? settings.SensorInsideId;
        settings.SensorOutsideId = req.SensorOutsideId ?? settings.SensorOutsideId;
        settings.TargetTemperature = req.TargetTemperature == default(float) ? settings.TargetTemperature : req.TargetTemperature;
        settings.PinCool = req.PinCool == default(int) ? settings.PinCool : req.PinCool;
        settings.PinHeat = req.PinHeat == default(int) ? settings.PinHeat : req.PinHeat;
        settings.Diff = req.Diff == default(float) ? settings.Diff : req.Diff;
        settingsMgmt.UpdateSettings(settings);
        return Negotiate.WithStatusCode(HttpStatusCode.OK);
      });
    }
  }
}
