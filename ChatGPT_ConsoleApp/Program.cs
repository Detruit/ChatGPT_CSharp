using ChatGPT_ConsoleApp.Helpers;
using ChatGPT_ConsoleApp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;

namespace ChatGPT_ConsoleApp
{
    internal class Program
    {
        private static async Task Main()
        {
            IConfiguration _configuration = Configuration();

            string prompter = _configuration.GetValue<string>("Prompt");
            int maxResponseToken = _configuration.GetValue<int>("MaxToken");
            int tokenLimit = _configuration.GetValue<int>("TokenLimit");

            OpenAIApiModel conversation = new();
            conversation.AddMessage(Message: prompter, Role: "system");
            Console.WriteLine();
            Console.WriteLine("App is ready");


            while (true)
            {
                Console.WriteLine();
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                Console.WriteLine();
                conversation.AddMessage(Message: line, Role: "user");

                int conversationHistoryToken = await TikToken.GetTokens(conversation, _configuration);
                //Console.WriteLine("Conversation_Token : " + conversationHistoryToken); Checking Our calculation
                while (conversationHistoryToken + maxResponseToken >= tokenLimit)
                {
                    conversation.Messages.RemoveAt(1);
                    conversationHistoryToken = await TikToken.GetTokens(conversation, _configuration);
                }

                Stopwatch stopwatch = new();
                try
                {
                    stopwatch.Start();
                    object serviceResponse = await RestClients.PostGPT3(conversation, _configuration);
                    stopwatch.Stop();


                    if (serviceResponse == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("There is an error Occured!");
                    }
                    else
                    {
                        string serilized = serviceResponse.ToString();

                        //JObject parsed = JObject.Parse(serilized);

                        //foreach (var pair in parsed)
                        //{
                        //    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                        //}

                        //Upper method is writes to json body to console.

                        Console.WriteLine($"Response Time: {stopwatch.Elapsed.TotalSeconds}" ); // Response Time calculation
                        Console.WriteLine();
                        OpenAIApiResponse res = JsonConvert.DeserializeObject<OpenAIApiResponse>(serilized);
                        
                        conversation.AddMessage(
                            Message: res.Choices.First().Message.Content.TrimStart(),
                            Role: res.Choices.First().Message.Role
                            );

                        Console.WriteLine(res.Choices.First().Message.Content.TrimStart());
                    }

                }
                catch (DeserializationException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.InnerException);
                }
                catch (Exception) { throw; }
            }

            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(conversation));
            Console.WriteLine();
            Console.WriteLine("Press Any Button.");
            Console.ReadKey();

        }

        static IConfiguration Configuration()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            return config;
        }

    }
}