using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Model.Mapping
{
  public class TemperatureMap : ClassMapper<Temperature>
  {
    public TemperatureMap()
    {
      Table("temperature");
      Map(c => c.Inside).Column("temp_in");
      Map(c => c.Outside).Column("temp_out");
      Map(c => c.Timestamp).Column("created_at");
    }
  }
}
