using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TheGame;

namespace LogAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var isUtc = true;
            var state = JsonConvert.DeserializeObject<GameState>(File.ReadAllText("state.json"));
            var files = Directory.GetFiles(@"logs-utc\");
            var guidRegex = new Regex(@"[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}");
            var timestampRegex = new Regex(@"\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d");
            foreach (var file in files)
            {
                Console.WriteLine($"Searching {file}");
                using (var reader = new StreamReader(File.OpenRead(file)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var matches = guidRegex.Matches(line);
                        foreach (Match match in matches)
                        {
                            var timestamp = timestampRegex.Match(line);
                            if (timestamp.Success)
                            {
                                var date = DateTime.SpecifyKind(DateTime.Parse(timestamp.Value), isUtc ? DateTimeKind.Utc : DateTimeKind.Local);
                                var item = state.Items.FirstOrDefault(i => i.Id == match.Value);
                                if (item != null)
                                {
                                    item.ItemAcquired = date;
                                    Console.WriteLine($"Updating {match.Value} {item.Name} with acuired time of {date}");
                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in state.Items.Where(i => i.ItemAcquired == DateTime.MinValue))
            {
                Console.WriteLine($"Could not find timestamp for {item.Id} - {item.Name}");
            }
            var count = state.Items.Count(i => i.ItemAcquired > DateTime.MinValue);
            File.WriteAllText("state-output.json", JsonConvert.SerializeObject(state));
            Console.WriteLine($"Updated {count} items.");
            Console.ReadKey();
        }
    }
}
