using System.Net.Http;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Chatter
{
    internal class openai_chatter
    {
        private static readonly HttpClient client = new HttpClient();

        public class Option
        {
            public string Text { get; set; }
        }

        public class TextResponse
        {
            public Option[] Options { get; set; }
        }

        public static string filepath = @"apikey.txt";
        public static string apikey = File.ReadAllText(filepath);
        public static string endpointURL = "https://api.openai.com/v1/completions";
        public static string modelType = "test-davinci-003";
        public static int maxTokens = 256;
        public static double temperature = 1.0f;
        static async Task Main(string[] args)
        {
            await Ask();
        }

        public static async Task Ask()
        {
            Console.WriteLine("Ask ChatGPT something and press enter:");
            string prompt = Console.ReadLine();
            Console.WriteLine("\n" + "Please wait...");

            
            string response = await OpenAIComplete(apikey, endpointURL, modelType, prompt, maxTokens, temperature);
            TextResponse textresponse = JsonConvert.DeserializeObject<TextResponse>(response);
            string text = textresponse.Options[0].Text;
            Console.WriteLine(text);

            //Console.WriteLine(response);

            Console.WriteLine("------------------------------------");

            await Ask();
        }

        public static async Task<string> OpenAIComplete(string apikey, string endpoint, string modelType, string promptmsg, int maxTokens, double temp)
        {
            var requestbody = new
            {
                model = modelType,
                prompt = promptmsg,
                max_tokens = maxTokens,
                temperature = temp
            };

            string jsonPayload = JsonConvert.SerializeObject(requestbody);

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Authorization", $"Bearer {apikey}");
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var httpResponse = await client.SendAsync(request);
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            return responseContent;
        }
    }
}