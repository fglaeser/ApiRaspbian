using ApiRaspbian.Model;
using Infra.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Modules
{
  public class MainModule : Nancy.NancyModule
  {
    IDataAccessRegistry _dataAccessRegistry;
    public IDataAccess DataAccess => _dataAccessRegistry.GetDataAccess();

    public MainModule(ILogger<MainModule> logger, IDataAccessRegistry dataAccessRegistry)
    {
      _dataAccessRegistry = dataAccessRegistry;
      Get("/", x =>
      {
        var temps = DataAccess.Query<Temperature>("SELECT temp_in as Inside, temp_out as Outside, created_at as Timestamp FROM temperature WHERE created_at >= datetime('now', '-1 day')");

        return View["index.html", temps.Select(t => new { Inside = t.Inside.ToString(), Outside = t.Outside.ToString(), Timestamp = t.Timestamp.ToString("o") })];
      });
    }
  }
}
