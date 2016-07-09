using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            try
            {

                Log.Write("TICK");
                SaveState(_state);
                await _loop.Tick();
                CopyState();
            }
            catch (Exception ex)
            {
                ex.Log();
                await Task.Delay(250);
            }

            Sleep();
        }

        private void CopyState()
        {
            SetLeaderboard();
            SetInventory();
            SetQueue();
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
            var nextMove = _state.NextMove;
            var message = nextMove != null ?
                $"{nextMove.Mode} Item Scheduled: {nextMove.Item.Name} on '{nextMove.Target ?? Constants.Me}'" :
                "No move scheduled";
            lblNextMove.Text = $"{message} ({remaining}s)";
        }


        private void SetInventory()
        {
            inventory.BeginUpdate();

            var groups = _state.Items.GroupBy(i => i.Name)
                .Select(g => new { Item = g.First(), Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var count = group.Count - _state.MoveQueue.AsEnumerable().Count(q => q.Item.Name == group.Item.Name);
                if (count == 0)
                {
                    groups.RemoveAt(i);
                    i--;
                    continue;
                }



                if (inventory.Items.Count <= i)
                    inventory.Items.Add(new ListViewItem(new[]
                    {
                        count.ToString(), group.Item.Name, group.Item.Description, group.Item.Rarity.ToString()
                    }));
                else
                {
                    var lvi = inventory.Items[i];
                    lvi.SubItems[0].Text = count.ToString();
                    lvi.SubItems[1].Text = group.Item.Name;
                    lvi.SubItems[2].Text = group.Item.Description;
                    lvi.SubItems[3].Text = group.Item.Rarity.ToString();
                }
            }

            while (inventory.Items.Count > groups.Count)
                inventory.Items.RemoveAt(inventory.Items.Count - 1);

            inventory.EndUpdate();
        }

        private void Sleep()
        {
            var nextTick = _state.LastPoints.AddMilliseconds(RulesEngine.PointsRateLimitMS);
            var sleepTime = nextTick - DateTime.UtcNow;
            if (sleepTime.TotalMilliseconds < 0)
                sleepTime = TimeSpan.FromMilliseconds(10);

            //AddMessage("Sleep for " + sleepTime.TotalMilliseconds);
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
            leaderboard.BeginUpdate();

            var leaders = _state.Leaderboard.ToArray();
            for (int i = 0; i < leaders.Length; i++)
            {
                var leader = leaders[i];

                if (leaderboard.Items.Count <= i)
                    leaderboard.Items.Add(new ListViewItem(new string[]
                    {
                        leader.Points.ToString(CultureInfo.InvariantCulture),
                        leader.PlayerName, leader.Effects.StringJoin()
                    }));
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

            leaderboard.EndUpdate();
        }

        public void SetQueue()
        {
            moves.BeginUpdate();

            for (int i = 0; i < _state.MoveQueue.Count; i++)
            {
                var move = _state.MoveQueue[i];
                if (moves.Items.Count <= i)
                    moves.Items.Add(new ListViewItem(new string[] { move.Item.Name, move.Target ?? Constants.Me }));
                else
                {
                    var lvi = moves.Items[i];
                    lvi.SubItems[0].Text = move.Item.Name;
                    lvi.SubItems[1].Text = move.Target ?? Constants.Me;
                }
            }

            while (moves.Items.Count > _state.MoveQueue.Count)
                moves.Items.RemoveAt(moves.Items.Count - 1);

            moves.EndUpdate();
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
            Log.Write("Booted");
            CopyState();
            _timer.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveState(_state);
            Log.Write("Shut down normally");
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
                    Item = _state.Items.First(i => i.Name == item.SubItemText(1)),
                    Mode = ItemMode.Manual,
                    Target = target,
                };

                _state.MoveQueue.Add(move);
                SetNextMoveMessage();
                SetInventory();
                SetQueue();
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var index = moves.SelectedIndex();
            _state.MoveQueue.Swap(index, index - 1);
            SetQueue();
            SetNextMoveMessage();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {

            var index = moves.SelectedIndex();
            _state.MoveQueue.Swap(index, index + 1);
            SetQueue();
            SetNextMoveMessage();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var index = moves.SelectedIndex();
            if (index >= 0)
            {
                _state.MoveQueue.RemoveAt(index);
                SetQueue();
                SetInventory();
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

        public static int SelectedIndex(this ListView list)
        {
            if (list.SelectedIndices.Count > 0)
                return list.SelectedIndices[0];
            return -1;
        }

        public static string SubItemText(this ListViewItem item, int index)
        {
            return item.SubItems[index].Text;
        }
    }


}
