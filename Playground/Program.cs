using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            const string json = "[{\"Name\":\"Pandora's Box\",\"Id\":\"9654774f-bfd6-4675-9996-4cdf6f96780d\",\"Description\":\"What's in here? OH NOES!\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Pandora's Box\",\"Id\":\"a29f9f13-4d7f-4e82-a085-62b3d7940d00\",\"Description\":\"What's in here? OH NOES!\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"c7d9ee91-4cd2-472d-bb0b-c89cc3e5777d\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"a4345f1a-5437-485e-8e3b-89b1c02a611d\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"e30eee47-03bb-4f9e-a702-3daf618c8142\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Gold Ring\",\"Id\":\"1949f740-77df-4ad2-8706-015cb0a66e3a\",\"Description\":\"Shiny ring that provides temporary protection from damage.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Gold Ring\",\"Id\":\"26edeee3-215a-48d4-a7b8-3ad834e6c83f\",\"Description\":\"Shiny ring that provides temporary protection from damage.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Blue Shell\",\"Id\":\"26e3ccc3-02cf-4b45-a6b2-db0db8de41b4\",\"Description\":\"Mighty Koopa shell that displaces the leader of the pack.\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Gold Ring\",\"Id\":\"c7e98bbc-e124-4cd2-958e-748f4196478e\",\"Description\":\"Shiny ring that provides temporary protection from damage.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Pandora's Box\",\"Id\":\"d1a20ddf-39f7-484d-b0fb-646720569133\",\"Description\":\"What's in here? OH NOES!\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Pandora's Box\",\"Id\":\"9d48c329-759c-46d5-a689-af038c903508\",\"Description\":\"What's in here? OH NOES!\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"fe223899-36a6-4ac4-8bf7-fda778dddee6\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Pandora's Box\",\"Id\":\"ef57680a-e529-4901-bd03-8e0d7cb3c88b\",\"Description\":\"What's in here? OH NOES!\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Varia Suit\",\"Id\":\"bfd4ddf1-f53d-480a-9f28-6d7148daf355\",\"Description\":\"Sleek pant suit offering significant protection from foes.\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Gold Ring\",\"Id\":\"46d3c1d8-acc0-450e-ad23-b6a764eb4a5e\",\"Description\":\"Shiny ring that provides temporary protection from damage.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"c0342ff2-ce71-4069-aa69-50b63780a5be\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"5480df44-5b91-4232-a269-0b902bbe7ff4\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Gold Ring\",\"Id\":\"6b87208a-7ad9-4a0c-83b6-da68fa84684b\",\"Description\":\"Shiny ring that provides temporary protection from damage.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Get Over Here\",\"Id\":\"231b3c50-c9db-474d-8a78-a74c9e34afdc\",\"Description\":\"Chance to cuddle up to your opponent. Serious snuggles.\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Varia Suit\",\"Id\":\"ca510995-7ca9-4d50-a7ea-0af0a421eed7\",\"Description\":\"Sleek pant suit offering significant protection from foes.\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"47a746c2-a9ac-4049-bd94-0ba4140650cd\",\"Description\":\"Bonus Item\",\"Rarity\":0,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"43d82b9c-8ffb-451b-bdce-2e99e0cbc1a5\",\"Description\":\"Gordon Freeman's weapon of choice.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"5bf6bde0-bb87-4aff-bed6-ce2371b3ffdf\",\"Description\":\"Gordon Freeman's weapon of choice.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Fire Flower\",\"Id\":\"33899aa4-d4f3-4a02-91c6-aaf338b3c4b3\",\"Description\":\"Throw balls of actual fire. Literally.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Hard Knuckle\",\"Id\":\"44d85b9d-46f5-4654-824a-dd786710e929\",\"Description\":\"Mega Man face punch.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Cardboard Box\",\"Id\":\"8cc5e6e9-6c74-4f48-acb3-ce686acd8270\",\"Description\":\"Shhh. You can't see me.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"ec562258-5cbe-497f-9c1c-98c449fc0315\",\"Description\":\"Gordon Freeman's weapon of choice.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"d5652150-200c-4531-9a44-d61fdb6cb09b\",\"Description\":\"Gordon Freeman's weapon of choice.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"46aeba90-cc97-4917-8016-20e4ca1ca0a5\",\"Description\":\"Gordon Freeman's weapon of choice.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Charizard\",\"Id\":\"bf72bafb-5823-4fcb-94fa-018c5a6cbf9e\",\"Description\":\"It's hard to be a diamond in a rhinestone world.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Charizard\",\"Id\":\"4c0fe727-1448-46c3-9357-3e9834bd8347\",\"Description\":\"It's hard to be a diamond in a rhinestone world.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Gold Ring\",\"Id\":\"1c547ab9-d3c4-4588-82d2-c206377f6089\",\"Description\":\"Shiny ring that provides temporary protection from damage.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Leisure Suit\",\"Id\":\"2f3f05fb-c9bc-4afb-94fc-97a0b0575eb4\",\"Description\":\"Winning this game is about as easy as holding onto a mud wrestler!\",\"Rarity\":3,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Charizard\",\"Id\":\"5ffb41e9-da04-411f-8d45-bab11c36e527\",\"Description\":\"It's hard to be a diamond in a rhinestone world.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Cardboard Box\",\"Id\":\"4872a553-2bae-4145-9b95-bfe489365808\",\"Description\":\"Shhh. You can't see me.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"a6f937ce-79d8-435e-96ba-c713db559760\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"d45ab25a-24c5-451a-98e0-9787f8a0fc4a\",\"Description\":\"Adorable suit that provides temporary protection from attacks.\",\"Rarity\":2,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Red Shell\",\"Id\":\"ddfe5036-a401-4c65-bdc4-e057276ddf4b\",\"Description\":\"Toss ahead of you to unsuspecting victim.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Red Shell\",\"Id\":\"011a32d6-1d75-4730-ae2b-bfadb15b3cd3\",\"Description\":\"Toss ahead of you to unsuspecting victim.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Buster Sword\",\"Id\":\"44314589-893e-4f2d-8785-851112650039\",\"Description\":\"Save Aeris!! ... too soon?\",\"Rarity\":3,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Red Shell\",\"Id\":\"02db0503-1fe0-4014-b578-22384af671df\",\"Description\":\"Toss ahead of you to unsuspecting victim.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Carbuncle\",\"Id\":\"1f770f41-50e2-49cc-a99f-16650bf909d6\",\"Description\":\"Fairy creature that uses its red eye to temporarily reflect harm.\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Star\",\"Id\":\"f49b1b53-5e2b-4438-bfe8-7a61ad927962\",\"Description\":\"A celestial object that creates temporary invincibility.\",\"Rarity\":3,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Tanooki Suit\",\"Id\":\"35327e1f-e921-4e17-bb33-e47c94bfe8b2\",\"Description\":\"Bonus Item\",\"Rarity\":0,\"IsOffensive\":false,\"IsDefensive\":true,\"IsPowerup\":false},{\"Name\":\"Hadouken\",\"Id\":\"b4a8554b-1ae0-4ee1-8c79-9a23e93e0d3a\",\"Description\":\"Burst of energy that pulses through the target and results in temporary after shocks.\",\"Rarity\":2,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Red Shell\",\"Id\":\"5871d27e-dd53-4d2e-993e-77f92dffff5a\",\"Description\":\"Toss ahead of you to unsuspecting victim.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"5aecfb72-c91a-4e32-8fb4-f360c711e343\",\"Description\":\"Bonus Item\",\"Rarity\":0,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"6b4dac68-b68e-4d65-a4a0-5200969de89a\",\"Description\":\"Bonus Item\",\"Rarity\":0,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"15690ce0-e814-432a-8c38-c247341939e8\",\"Description\":\"Bonus Item\",\"Rarity\":0,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"837cb996-87b3-42c2-8516-5269e462c8c7\",\"Description\":\"Bonus Item\",\"Rarity\":0,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false},{\"Name\":\"Crowbar\",\"Id\":\"f9594654-83d0-4557-92f7-c84235da3fc8\",\"Description\":\"Gordon Freeman's weapon of choice.\",\"Rarity\":1,\"IsOffensive\":true,\"IsDefensive\":false,\"IsPowerup\":false}]";
            var items = JsonConvert.DeserializeObject<List<GameItem>>(json);

            while (true)
            {
                Console.Clear();
                List<Move> moveQueue = new List<Move>();

                var groups = items.GroupBy(i => i.Name)
                    .Select(g => new { Item = g.First(), Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .ToArray();

                for (int i = 0; i < groups.Length; i++)
                {
                    var group = groups[i];
                    Console.WriteLine($"{i}: {group.Count}x {group.Item.Name} ({group.Item.Rarity})");
                }

                var s = Console.ReadLine();

                if (s == "q")
                {
                    return;
                }
                else if (s.StartsWith("?", StringComparison.InvariantCultureIgnoreCase))
                {
                    var pieces = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var choice = int.Parse(pieces[1]);
                    Console.WriteLine(groups[choice].Item);
                    Console.ReadKey(true);
                }
                else if (s.StartsWith("a", StringComparison.InvariantCultureIgnoreCase))
                {
                    var pieces = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int choice = int.Parse(pieces[0]);
                    string target = pieces.Length > 1 ? pieces[1] : null;

                    moveQueue.Add(new Move()
                    {
                        Item = groups[choice].Item,
                        Target = target,
                        Mode = ItemMode.Manual
                    });
                }
                else
                {
                    Console.WriteLine("Does not compute");
                    Console.ReadKey(true);
                }

            }
        }
    }

    public class GameItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Rarity { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }
    }

    public class Move
    {
        public GameItem Item { get; set; }
        public string Target { get; set; }
        public ItemMode Mode { get; set; }
    }

    public enum ItemMode
    {
        Manual,
        Automatic

    }
}
