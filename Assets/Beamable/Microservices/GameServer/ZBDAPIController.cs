using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Beamable.Server;
using Beamable.Common;

public static class ZBDAPIController
{
    public static async Task<SendToUsernameResponse> SendToUsername(string username, int amount, string description, string apikey)
    {

        SendToUsernamePayload request = new SendToUsernamePayload();
        request.gamertag = username;
        request.amount = (amount * 1000) + "";
        request.description = description;

        string jsonData = JsonConvert.SerializeObject(request);

        BeamableLogger.Log("send to user name payload: " + jsonData);

        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");


        HttpClient httpClient = new HttpClient();
        string url = "https://api.zebedee.io/v0/gamertag/send-payment";

        httpClient.DefaultRequestHeaders.Add("apikey", apikey);


        var response = await httpClient.PostAsync(url, content);
        SendToUsernameResponse sendResponse = new SendToUsernameResponse();
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            BeamableLogger.Log("responseBody", responseBody);
            sendResponse = JsonConvert.DeserializeObject<SendToUsernameResponse>(responseBody);
            return sendResponse;
        }
        else
        {
            BeamableLogger.Log("error sending to username");
            sendResponse.success = false;
            try
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                sendResponse.message = responseBody;

            }
            catch (Exception e)
            {
                sendResponse.message = e.ToString();
            }

            return sendResponse;
        }
    }

}


