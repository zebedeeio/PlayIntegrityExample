using Beamable.Common;
using Google.Apis.Auth.OAuth2;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public static class PlayIntegrityController
{


    public static async Task<string> RunPlayIntegrity(string serviceJSON, string integrityToken)
    {


        var credentials = GoogleCredential.FromJson(serviceJSON).CreateScoped("https://www.googleapis.com/auth/playintegrity"); // Ensure the correct scope is used
        var tokenResponse = await credentials.UnderlyingCredential.GetAccessTokenForRequestAsync();



        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse);

        // Construct the JSON payload with the integrity token
        string integrityTokenJson = $"{{\"integrity_token\":\"{integrityToken}\"}}"; // Assuming integrityTokenProvider is the actual token string

        HttpContent content = new StringContent(integrityTokenJson, Encoding.UTF8, "application/json");



        // Send the POST request
        var response = await client.PostAsync("https://playintegrity.googleapis.com/v1/com.zebedee.flappysats:decodeIntegrityToken", content);


        if (response.IsSuccessStatusCode)
        {
            string responseString = await response.Content.ReadAsStringAsync();
            BeamableLogger.Log("integrityResponse " + responseString);
            return responseString;
        }
        else
        {
            // Handle the error or unsuccessful response 

            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }

    }
}