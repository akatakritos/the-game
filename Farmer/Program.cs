using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TheGame;

namespace Farmer
{
    class Program
    {
        public class Bot
        {
            public Bot(string id)
            {
                Id = id;
            }

            public readonly string Id;
        }

        public class BotState
        {
            public List<GameItem> Items { get; set; } = new List<GameItem>();
        }

        static void Main(string[] args)
        {
            var state = new BotState();
            if (File.Exists("state.json"))
            {
                state = JsonConvert.DeserializeObject<BotState>(File.ReadAllText("state.json", Encoding.UTF8));
            }

            var guids = new Bot[]
            {
                new Bot("b65b2432-ad07-48ae-912b-04844d653d98"),
                new Bot("5201ce2c-d9c4-4568-8305-99042ee3db42"),
                new Bot("5c346433-6633-4fed-b0cd-8ab24d681591"),
                new Bot("d97eca43-34fd-4fe6-af82-dab26696bf93"),
                new Bot("4d87c6a8-68b5-4719-afc6-e258f63737f2"),
                new Bot("3fa6d329-8f5b-43c8-b4eb-fc573f795a88"),
                new Bot("04accba4-2ad2-45fb-bc52-3ea8afc51c1f"),
                new Bot("e8a92552-7de4-4093-9893-5f81ca04b169"),
                new Bot("35536c89-c3fc-4341-83f9-7d483a738fc6"),
                new Bot("ab1f60b9-e1c1-4871-94ce-a9ff5d7d12ab"),
                new Bot("0af13640-9915-4aab-a1e2-cf82f7b62c7a"),
                new Bot("50245b7e-7597-4988-9255-1957b96c224f"),
                new Bot("f2af81f7-f1cd-4f4b-bb96-96fba236e58c"),
                new Bot("63719d53-456b-4e3d-b464-26048daca441"),
                new Bot("bf5d2e62-0378-4cdd-8039-374715dfad2f"),
                new Bot("01275865-e6da-4bcc-8b4b-725e8038ef03"),
                new Bot("980fdf31-c099-4059-82be-98667050b5f8"),
                new Bot("43dac400-e926-45cf-8036-92d469a82a95"),
                new Bot("80342b3e-128d-484e-85c2-223f51e192e2"),
                new Bot("c1718872-0852-4587-abcc-6ca9012dfe88"),
            };

            while (true)
            {
                try
                {
                    foreach (var bot in guids)
                    {
                        var client = new HttpClient();
                        client.BaseAddress = new Uri("http://thegame.nerderylabs.com:1337");
                        client.DefaultRequestHeaders.Add("apiKey", bot.Id);
                        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                        var response = client.PostAsync("/points", null).Result.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(response);
                        var obj = JsonConvert.DeserializeObject<PollResponse>(response);
                        if (obj.Item != null)
                        {
                            state.Items.AddRange(obj.ExtractItems());
                        }
                    }

                    Console.WriteLine(state.Items.Count);

                    File.WriteAllText("state.json", JsonConvert.SerializeObject(state, Formatting.Indented), Encoding.UTF8);
                    Thread.Sleep(1000);
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }
        }
    }
}
