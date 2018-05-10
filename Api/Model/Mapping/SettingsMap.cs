using DapperExtensions.Mapper;

namespace ApiRaspbian.Model.Mapping
{
  public class SettingsMap : ClassMapper<Settings>
  {
    public SettingsMap()
    {
      Table("settings");
      Map(c => c.Id).Column("id").Key(KeyType.Assigned);
      Map(c => c.PollingInterval).Column("polling_interval");
      Map(c => c.PollingSensorsPath).Column("polling_sensor_path");
      Map(c => c.PollingTemperatureFile).Column("polling_temperature_file");
      Map(c => c.SensorInsideId).Column("sensor_in_id");
      Map(c => c.SensorOutsideId).Column("sensor_out_id");
    }
  }
}
