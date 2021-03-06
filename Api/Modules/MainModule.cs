﻿using ApiRaspbian.Model;
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
        var temps = DataAccess.Query<Temperature>("SELECT temp_in as Inside, temp_out as Outside, created_at as Timestamp FROM temperature WHERE created_at >= datetime('now', '-1 day') order by created_at");
        var last = temps.Last();
        var insideInt = last.Inside != 0f ? last.Inside.ToString("0.00").Split(".")[0] : "0";
        var insideRest = last.Inside != 0f ? last.Inside.ToString("0.00").Split(".")[1] : "0";
        var outsideInt = last.Outside != 0f ? last.Outside.ToString("0.00").Split(".")[0] : "0";
        var outsideRest = last.Outside != 0f ? last.Outside.ToString("0.00").Split(".")[1] : "0";
        return View["index.html", 
          new { Temps = temps.Select(t => new { Inside = t.Inside.ToString(), Outside = t.Outside.ToString(), Timestamp = t.Timestamp.ToString("o") }),
          InsideInt = insideInt, InsideRest = insideRest, OutsideInt = outsideInt, OutsideRest = outsideRest
          }];
      });
    }
  }
}
