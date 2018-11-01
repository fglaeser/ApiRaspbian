using ApiRaspbian.Model;
using Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRaspbian.Mgmt
{
  public class SettingsManagement
  {
    IDataAccessRegistry _dataAccessRegistry;
    public IDataAccess DataAccess => _dataAccessRegistry.GetDataAccess();
    Settings _settings = null;

    public SettingsManagement(IDataAccessRegistry dataAccessRegistry)
    {
      _dataAccessRegistry = dataAccessRegistry;
    }

    public Settings GetSettings()
    {
      if (_settings != null) return _settings;
      _settings = DataAccess.Get<Settings>("main");
      return _settings;
    }

    public void UpdateSettings(Settings settings)
    {
      DataAccess.Update(settings);
      _settings = settings;
    }
  }
}
