using Newtonsoft.Json;

namespace ApiRaspbian.Requests
{
  public class SettingsRequest
  {
    [JsonProperty("polling.interval")]
    public int PollingInterval { get; set; }

    [JsonProperty("polling.sensors_path")]
    public string PollingSensorsPath { get; set; }

    [JsonProperty("polling.temperature_file")]
    public string PollingTemperatureFile { get; set; }

    [JsonProperty("sensor.inside_id")]
    public string SensorInsideId { get; set; }

    [JsonProperty("sensor.outside_id")]
    public string SensorOutsideId { get; set; }

    [JsonProperty("thermostat.target_temperature")]
    public float TargetTemperature { get; set; }

    [JsonProperty("thermostat.pin_cool")]
    public int PinCool { get; set; }

    [JsonProperty("thermostat.pin_heat")]
    public int PinHeat { get; set; }

    [JsonProperty("thermostat.diff")]
    public float Diff { get; set; }

  }
}
