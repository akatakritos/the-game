using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

using Newtonsoft.Json;

using Timer = System.Timers.Timer;

namespace TheGame
{
    public partial class TheGameForm : Form
    {
        private readonly Timer _timer;
        private readonly GameState _state;
        private readonly RulesEngine _rules;
        private readonly GameLoop _loop;

        public TheGameForm()
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.SynchronizingObject = this;
            _timer.AutoReset = false;
            _timer.Interval = 1000;
            _timer.Elapsed += TimerOnElapsed;

            _state = LoadState();
            _rules = new RulesEngine(_state);
            _loop = new GameLoop(_state, _rules);
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Log.Write("TICK");
            SaveState(_state);
            await _loop.Tick();
            CopyState();
            Sleep();
        }

        private void CopyState()
        {
            SetLeaderboard();
            SetInventory();
            CopyMessages();
            SetNextMoveMessage();
        }

        private void CopyMessages()
        {
            foreach (var msg in _state.LastMessages)
                AddMessage(msg);
        }

        private void SetNextMoveMessage()
        {
            var remaining = (int)(_rules.NextItemTime - DateTime.UtcNow).TotalSeconds;
            var message = _state.NextMove != null ?
                $"{_state.NextMove.Mode} Item Scheduled: {_state.NextMove.Item.Name} on '{_state.NextMove.Target}'" :
                "No move scheduled";
            lblNextMove.Text = $"{message} ({remaining}s)";
        }


        private void SetInventory()
        {
            var groups = _state.Items.GroupBy(i => i.Name)
                .Select(g => new { Item = g.First(), Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToArray();

            for (int i = 0; i < groups.Length; i++)
            {
                var group = groups[i];
                if (inventory.Items.Count <= i)
                    inventory.Items.Add(new ListViewItem(new[] { group.Count.ToString(), group.Item.ToString(), group.Item.Rarity.ToString() }));
                else
                {
                    var lvi = inventory.Items[i];
                    lvi.SubItems[0].Text = group.Count.ToString(CultureInfo.InvariantCulture);
                    lvi.SubItems[1].Text = group.Item.ToString();
                    lvi.SubItems[2].Text = group.Item.Rarity.ToString();
                }
            }

            while (inventory.Items.Count > groups.Length)
                inventory.Items.RemoveAt(inventory.Items.Count - 1);
        }

        private void Sleep()
        {
            var nextTick = _state.LastPoints.AddMilliseconds(RulesEngine.PointsRateLimitMS);
            var sleepTime = nextTick - DateTime.UtcNow;
            if (sleepTime.TotalMilliseconds < 0)
                sleepTime = TimeSpan.FromMilliseconds(10);

            AddMessage($"Sleeping for {sleepTime.TotalMilliseconds}ms");
            _timer.Interval = sleepTime.TotalMilliseconds;
            _timer.Start();
        }

        public void AddMessage(string message)
        {
            log.Items.Add(message);
            while (log.Items.Count > 100)
                log.Items.RemoveAt(0);

            log.TopIndex = log.Items.Count - 1;
        }


        private void SetLeaderboard()
        {
            var leaders = _state.Leaderboard.ToArray();
            for (int i = 0; i < leaders.Length; i++)
            {
                var leader = leaders[i];

                if (leaderboard.Items.Count <= i)
                    leaderboard.Items.Add(new ListViewItem(new string[] { leader.Points.ToString(CultureInfo.InvariantCulture), leader.PlayerName, leader.Effects.StringJoin() }));
                else
                {
                    var lvi = leaderboard.Items[i];
                    lvi.SubItems[0].Text = leader.Points.ToString(CultureInfo.InvariantCulture);
                    lvi.SubItems[1].Text = leader.PlayerName;
                    lvi.SubItems[2].Text = leader.Effects.StringJoin();
                }
            }

            if (!_state.OnLeaderboard)
            {
                var lvi = leaderboard.FindItemWithText(Constants.Me);
                if (lvi == null)
                {
                    leaderboard.Items.Add(new ListViewItem(new string[]
                    {
                        _state.Points.ToString(), Constants.Me, _state.Effects.StringJoin()
                    }));
                }
                else
                {
                    lvi.SubItems[0].Text = _state.Points.ToString(CultureInfo.InvariantCulture);
                    lvi.SubItems[1].Text = Constants.Me;
                    lvi.SubItems[2].Text = _state.Effects.StringJoin();
                }
            }
        }



        private static GameState LoadState()
        {
            if (File.Exists("state.json"))
            {
                return JsonConvert.DeserializeObject<GameState>(File.ReadAllText("state.json"));
            }

            return new GameState();
        }

        private static void SaveState(GameState state)
        {
            File.WriteAllText("state.json", JsonConvert.SerializeObject(state));
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            CopyState();
            _timer.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveState(_state);
        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            if (inventory.SelectedItems.Count > 0)
            {
                var target = string.IsNullOrEmpty(txtTarget.Text) ? null : txtTarget.Text;
                var item = inventory.SelectedItem();

                var targetItem = leaderboard.SelectedItem();
                if (targetItem != null)
                    target = targetItem.SubItemText(1);

                var move = new Move()
                {
                    Item = _state.Items.First(i => i.ToString() == item.SubItemText(1)),
                    Mode = ItemMode.Manual,
                    Target = target,
                };

                _state.NextMove = move;
                SetNextMoveMessage();
            }
        }
    }

    public static class FormControlExtensions
    {
        public static ListViewItem SelectedItem(this ListView list)
        {
            if (list.SelectedItems.Count > 0)
                return list.SelectedItems[0];
            return null;
        }

        public static string SubItemText(this ListViewItem item, int index)
        {
            return item.SubItems[index].Text;
        }
    }


}
