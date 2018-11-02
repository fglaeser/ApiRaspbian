using DapperExtensions.Mapper;

namespace ApiRaspbian.Model.Mapping
{
  public class SettingsMap : ClassMapper<Settings>
  {
    public SettingsMap()
    {
      Table("settings");
      Map(c => c.Id).Column("id").Key(KeyType.Assigned);
      Map(c => c.PollingInterval).Column("polling_interval"); // Cada cuantos minutos se leen los sensores
      Map(c => c.PollingSensorsPath).Column("polling_sensor_path"); //Path de los sensores
      Map(c => c.PollingTemperatureFile).Column("polling_temperature_file"); // nombre del archivo donde esta la temperatura
      Map(c => c.SensorInsideId).Column("sensor_in_id"); // id del sensor interno
      Map(c => c.SensorOutsideId).Column("sensor_out_id"); // id del sensor externo
      Map(c => c.TargetTemperature).Column("target_temperature"); // Temperatura que se quiere en la heladera
      Map(c => c.PinCool).Column("pin_cool"); //numero de pin
      Map(c => c.PinHeat).Column("pin_heat");
      Map(c => c.Diff).Column("diff");
      Map(c => c.PublishInterval).Column("publish_interval");
      Map(c => c.ThermostatInterval).Column("thermostat_interval");
    }
  }
}
