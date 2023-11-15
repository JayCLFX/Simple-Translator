using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Translator
{
    public class Library
    {
        //Data Structure
        private static Dictionary<string, string> Main_Dictionary = new Dictionary<string, string>();
        private static string Envoirment_Path = Environment.CurrentDirectory + "/" + "data";
        private static string Dictionary_Path = Environment.CurrentDirectory + "/" + "data" + "/" + "save.dat";

        //Keys
        private static string RapidAPI_Key = "";

        //Get Language
        private static string json_language;
        public static dynamic data_language;
        private static string language;

        //Get Desired Language
        public static string converted_language = null;


        public static async Task GetLanguageEvaluation()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://google-translate1.p.rapidapi.com/language/translate/v2/detect"),
                    Headers =
                {
                    { "X-RapidAPI-Key", $"{RapidAPI_Key}" },
                    { "X-RapidAPI-Host", $"google-translate1.p.rapidapi.com" },
                },
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "q", $"{Program.Sentence}" },
                }),
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    object body = await response.Content.ReadAsStringAsync();
                    json_language = body.ToString();
                    data_language = JObject.Parse(json_language);

                    dynamic languageraw = data_language.data.detections[0];
                    language = languageraw[0].language;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static async Task GetTranslation()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://microsoft-translator-text.p.rapidapi.com/BreakSentence?api-version=3.0&Language={converted_language}"),
                Headers =
                {
                    { "X-RapidAPI-Key", $"{RapidAPI_Key}" },
                    { "X-RapidAPI-Host", "microsoft-translator-text.p.rapidapi.com" },
                },
                Content = new StringContent($"{Program.Sentence}")
                {
                    Headers =
                    {
                        ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Youre Translation:  {body}");
                Console.ReadLine(); Program.State();
            }
        }

        public static async Task ConvertLanguage()
        {
            string desired_language = Program.Desired_Language.ToLower();
            converted_language = null;

            switch (desired_language)
            {
                case "afrikaans":
                converted_language = "af"; break;
                case "albanian":
                converted_language = "sq"; break;
                case "amharic":
                converted_language = "am"; break;
                case "arabic":
                converted_language = "ar"; break;
                case "armenian":
                converted_language = "hy"; break;
                case "french":
                converted_language = "fr"; break;
                case "english":
                converted_language = "en"; break;
                case "german":
                converted_language = "de"; break;
            }

            if (converted_language != null)
            {
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("You entered an invalid language who might have not been added!");
                Console.ReadLine(); Program.State();
            }
        }






        public static async Task Initialize_Library()
        {
            if (File.Exists(Dictionary_Path))
            {
                var task = Task.Run((Func<Task>)Library.LoadData);
                task.Wait();
                return;
            }
            else
            {
                var task = Task.Run((Func<Task>)Library.CreateData);
                task.Wait();
                return;
            }
        }

        public static async Task CreateData()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("State youre RapidAPI-Key:\n");
            Console.ForegroundColor = ConsoleColor.White;
            var key = Console.ReadLine(); Console.Clear();

            Directory.CreateDirectory(Envoirment_Path);
            Main_Dictionary.Add("RapidAPI_Key", key);
            string data = JsonConvert.SerializeObject(Main_Dictionary, Formatting.Indented);
            File.WriteAllText(Dictionary_Path, data);

            var task = Task.Run((Func<Task>)Library.LoadData);
            task.Wait();
        }

        public static async Task LoadData()
        {
            try
            {
                string data = File.ReadAllText(Dictionary_Path);
                Main_Dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                foreach (var item in Main_Dictionary)
                {
                    if (item.Key == "RapidAPI_Key") RapidAPI_Key = item.Value;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Fatal Error while loading Keys:  {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine(); CreateData();
            }
        }
    }
}
