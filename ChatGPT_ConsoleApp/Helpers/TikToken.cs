using ChatGPT_ConsoleApp.Models;
using Microsoft.Extensions.Configuration;
using SharpToken;

namespace ChatGPT_ConsoleApp.Helpers
{
    public static class TikToken
    {

        public static async Task<int> GetTokens(OpenAIApiModel model, IConfiguration _configuration)
        {
            var encoding = GptEncoding.GetEncoding(_configuration.GetValue<string>("Encoding"));

            int numTokens = 2;
            foreach (var msg in model.Messages)
            {
                numTokens += 4;
                int roleLength = encoding.Encode(msg.Role).Count;
                int valueLenght = encoding.Encode(msg.Content).Count;
                numTokens += valueLenght + roleLength;
            }

            return numTokens;
        }
    }
}
