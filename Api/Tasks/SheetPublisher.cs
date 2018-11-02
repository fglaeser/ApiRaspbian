using ApiRaspbian.Mgmt;
using ApiRaspbian.Model;
using DapperExtensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Infra.Data;
using Infra.Full.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiRaspbian.Tasks
{
  public class SheetPublisher : ITaskObject
  {
    readonly ILogger<SheetPublisher> _logger;
    readonly SettingsManagement _settingsMgmt;
    readonly IDataAccessRegistry _dataAccessRegistry;
    static string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static string ApplicationName = "Google Sheets API .NET Quickstart";

    public TimeSpan? WaitTimeout => null;

    public string TaskName => GetType().Name;

    public IDataAccess DataAccess => _dataAccessRegistry.GetDataAccess();

    public SheetPublisher(ILogger<SheetPublisher> logger, SettingsManagement settingsMgmt, IDataAccessRegistry dataAccessRegistry)
    {
      _logger = logger;
      _settingsMgmt = settingsMgmt;
      _dataAccessRegistry = dataAccessRegistry;
    }

    public async Task StartAsync(CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        // wait for some reading before publish
        await Task.Delay(TimeSpan.FromMinutes(_settingsMgmt.GetSettings().PublishInterval), token);
        try
        {
          await PublishValues(token);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Exception checking temperature.");
        }
      }
    }

    private async Task PublishValues(CancellationToken cancellationToken)
    {
      var temperatures = GetTemperatures();
      UserCredential credential;

      using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
      {
        string credPath = "token.json";
        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            Scopes,
            "user",
            cancellationToken,
            new FileDataStore(credPath, true));
        _logger.LogInformation("Credential file saved to: {0}", credPath);
      }

      // Create Google Sheets API service.
      var service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName,
      });
      string spreadsheetId = "1RCRFRNucJB-fs4jyhIlPMfHnMOIv7xx81h1oeZx0j-g";
      ValueRange valueRange = new ValueRange { Values = new List<IList<object>>() };
      foreach(var temp in temperatures)
      {
        valueRange.Values.Add(new List<object> { temp.Inside, temp.Outside, temp.Timestamp.ToString() });
      }
      var request = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, "A1:C10");
      request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
      await request.ExecuteAsync(cancellationToken);

      var response = await service.Spreadsheets.Values.Get(spreadsheetId, "E4:E14").ExecuteAsync(cancellationToken);
      IList<IList<Object>> values = response.Values;
      if (values != null && values.Count > 0)
      {
        _logger.LogInformation("Updating settings....");
        var settings = _settingsMgmt.GetSettings();
        settings.TargetTemperature = float.Parse(values[0][0].ToString().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
        settings.PollingInterval = int.Parse(values[1][0].ToString());
        settings.PollingSensorsPath = values[2][0].ToString();
        settings.PollingTemperatureFile = values[3][0].ToString();
        settings.SensorInsideId = values[4][0].ToString();
        settings.SensorOutsideId = values[5][0].ToString();
        settings.PinCool = int.Parse(values[6][0].ToString());
        settings.PinHeat = int.Parse(values[7][0].ToString());
        settings.Diff = float.Parse(values[8][0].ToString().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
        settings.PublishInterval = int.Parse(values[9][0].ToString());
        settings.ThermostatInterval = int.Parse(values[10][0].ToString());
        _settingsMgmt.UpdateSettings(settings);
      }
      service.Dispose();
    }

    private IEnumerable<Temperature> GetTemperatures()
    {
      return DataAccess.Query<Temperature>("SELECT temp_in as Inside, temp_out as Outside, created_at as Timestamp FROM temperature ORDER BY created_at DESC LIMIT 10");
    }
  }
}
