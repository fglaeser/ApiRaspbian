using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Model
{
  public class Temperature
  {
    public float Inside { get; set; }
    public float Outside { get; set; }
    public DateTime Timestamp { get; set; }
  }
}
