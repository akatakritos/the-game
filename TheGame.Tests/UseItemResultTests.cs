using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFluent;

using Xunit;

namespace TheGame.Tests
{
    public class UseItemResultTests
    {
        [Theory]
        [InlineData("You found a bonus item! <b338a5d1-acfc-4bbf-b1ef-95a1ab0234b5> | <Blue Shell>")]
        public void FindsBonusItems(string message)
        {
            var subject = new UseItemResult()
            {
                Messages = new List<string>() { message }
            };

            var item = subject.BonusItems.First();
            Check.That(item.Id).IsEqualTo("b338a5d1-acfc-4bbf-b1ef-95a1ab0234b5");
            Check.That(item.Name).IsEqualTo("Blue Shell");
        }
    }
}
