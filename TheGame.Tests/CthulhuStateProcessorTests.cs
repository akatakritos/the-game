using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFluent;

using Xunit;

namespace TheGame.Tests
{
    public class CthulhuStateProcessorTests
    {
        [Fact]
        public void StripsCrazinessFromMessages()
        {
            var state = new GameState();
            state.LastMessages.Add("F̘̙͉̘̺̞̳̥̕r̸̩͖̪̹̱̱̯͘͞ͅó̯̥̩̪͉͍ḿ̵͈̗̖̣̲ͅ ̨̳̱͉̣̞t̶͎̮̝͎̝͈͚̘͞͡h͔͈̥̙̟̫͚ę͎͍̪̤̰͇̤ ̝͍̲̫̳͍̰̩͘d͍̳̝̩̤̞͘͠e̢̹̼̹̬̗̘͓̦̞͢͡p̪̖̟̪̳̗̙̕t̫̙͓̣͙͢h҉̸̛͔͚͓̠̝͇ͅs̰̳̟͙͜ ̴̰͞o̳f̨̢͓̱ͅ ͍̖̹̯͝t̙̟̜̘̦ͅh̺̯̀e͏̴̠̠̰̠͉̻̯̞ͅ ͈̣̮̬̘̮̯͘͢a͓͜͝b͕̼̭̜͖̟̣̟͇͢͡y̱͕̲̬͚͝s̱͓̯̪̟̬̬̰͠s̷҉҉͇͖̬̹̗͓̙̠,̞̮̼̙̻̘̯̗ ̺̣̲̤t͙͈͈̙̀͘h̸̺̥e̲͍ ̟̱̩̥̝͜ͅg̢̧͙͍̳r̗̮͟e͇̹̻a͡҉͚͕̱̟̞͡ͅț͇̤̟͎̳͜͟͞ͅ ͓̜̮̭̙͞b̶̧͈̫̺͇̹̝͉ḛ͈̼̥̟ą̩̺͙̭̘ș̞̲̲̳͝͞t̥̖̼͈̗͎̭͠͡ ̸̲͓̹͈̪r͎̩̰̼͔̜͈͉̦͡i̤̭͎̰͉͈͓̱s̬͇̀̀͘e̵̶̹s̸̡̫̞̱̱̱̝̹̞ ͈͙̤̼̞͜͟a͔̜̘͖̥̖̣̞ņ̤͔̱̞̼̠̥͢d͈͈̩̼ ͉̳̻͞g̷͏̻͖̠r̲̖̰̳͇̣͔̞͠a͏̞̪̻̫͔n̶̨̙̭͇͕̜̺̱̙t̼̟̗̯̝s̡̝̹̤̹ ̲̰͚͉̣̺̬̩͢͜a̢͓͚̱̺̮̱͝ ̩͚́d̶̪͈̗̤̞̗̥͜͝a̲͓͉̪̘͜ṟ̀k̴̮̰̥̦̩͓͍͍̱̕ ̵̣̜̥͖̭͉͜b̨̬̻̺͉̼̘͡ọ̜͓͉̰̞͔͜o͈̞͎̝̳̥̞ͅn͏̮͔,͓̹̖̱͙͓ ͏͍͕̭̮͢b̵̢͔͓̺̞͔̟̗r͖͈̭͙̫͖͓i̖͇̪̺̬̰͜͢͠ͅn̸̳͚̬̞̳͟g̵̶͖͍̭ì͖̺̗̬̟̰͎n̯͖̲͘g̸̞̩̣̤͘͢ ̫͖̞̯y̮͚̺͇̳̝͇̯̺͜o͞͏̴͖̙̩̲̙̰̣̩u̵̡͙͘r҉̶̜̳̞̞͙ ̶̠̱̩͚͎͇͕͎͇t͏̱̮̯o͙̣͓̤̩͙͓̙t̹̦͡ą̥̻̝͔̀͝l̷̮͓̺̺̣̫̙̤̯͜ ҉̶̼͓͔̮͟ţ̴͖̳̬̘̀ơ̡̮̟̗̹͕̠̪͜");

            CthulhuStateProcessor.ProcessState(state);

            Check.That(state.LastMessages[0]).IsEqualTo("From the depths of the abyss, the great beast rises and grants a dark boon, bringing your total to");
        }
    }
}
