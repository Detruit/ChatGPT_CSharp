using ChatGPT_ConsoleApp.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace ChatGPT_ConsoleApp.Helpers
{
    public static class RestClients
    {
        public static async Task<object> PostGPT3(OpenAIApiModel model, IConfiguration _configuration)
        {
            RestClient restClient = new(_configuration.GetValue<string>("Gpt-Url"));
            var request = new RestRequest()
                .AddHeader("Authorization", _configuration.GetValue<string>("Gpt-token"))
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(model);
            return await restClient.PostAsync<object>(request);
        }
    }
}
