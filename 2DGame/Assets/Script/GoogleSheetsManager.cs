using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class GoogleSheetsManager : MonoBehaviour
{
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private static readonly string ApplicationName = "Unity Ranking System";
    private static readonly string SheetId = "あなたのスプレッドシートID";
    private static readonly string SheetName = "Sheet1";

    private SheetsService service;

    public async Task InitializeService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(Path.Combine(Application.streamingAssetsPath, "service-account.json"), FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        Debug.Log("Google Sheets API Initialized.");
    }

    public async Task AppendRankingEntry(string playerNumber, float time)
    {
        var range = $"{SheetName}!A:C";
        var valueRange = new ValueRange();
        valueRange.Values = new System.Collections.Generic.List<IList<object>> {
            new List<object> { System.DateTime.Now.ToString(), playerNumber, time.ToString("F2") }
        };

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

        var response = await appendRequest.ExecuteAsync();
        Debug.Log("Data appended to Google Sheet.");
    }
}
