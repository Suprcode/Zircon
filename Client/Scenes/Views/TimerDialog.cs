using Client.Controls;
using Client.Envir;
using Client.Properties;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public sealed class TimerDialog : DXControl
    {
        private bool _timerStarted = false;
        private int _timerCounter = 0;
        private DateTime _timerTime;

        private readonly DXAnimatedControl _eggTimer = null;
        private readonly DXImageControl _1000 = null;
        private readonly DXImageControl _100 = null;
        private readonly DXImageControl _10 = null;
        private readonly DXImageControl _1 = null;
        private readonly DXImageControl _colon = null;
        private readonly int _libraryOffset = 6580;

        private readonly List<ClientTimer> ActiveTimers = new ();
        private ClientTimer CurrentTimer = null;

        public TimerDialog()
        {
            PassThrough = true;
            Size = new Size(120, 100);
            Movable = false;
            Sort = true;

            _eggTimer = new DXAnimatedControl
            {
                Index = 960,
                FrameCount = 6,
                AnimationDelay = TimeSpan.FromMilliseconds(333),
                LibraryFile = LibraryFile.GameInter,
                Parent = this,
                Location = new Point(23, 0),
                PassThrough = true,
                UseOffSet = true,
                Animated = true,
                Loop = false,
                Opacity = 1F
            };

            _1000 = new DXImageControl
            {
                Parent = this,
                Index = _libraryOffset + 0,
                LibraryFile = LibraryFile.GameInter,
                PassThrough = true,
                UseOffSet = true,
                Location = new Point(0, 70),
                Visible = false
            };

            _100 = new DXImageControl
            {
                Parent = this,
                Index = _libraryOffset + 0,
                LibraryFile = LibraryFile.GameInter,
                PassThrough = true,
                UseOffSet = true,
                Location = new Point(25, 70),
                Visible = false
            };

            _colon = new DXImageControl
            {
                Parent = this,
                Index = _libraryOffset + 10,
                LibraryFile = LibraryFile.GameInter,
                PassThrough = true,
                UseOffSet = true,
                Location = new Point(50, 70),
                Visible = false
            };

            _10 = new DXImageControl
            {
                Parent = this,
                Index = _libraryOffset + 0,
                LibraryFile = LibraryFile.GameInter,
                PassThrough = true,
                UseOffSet = true,
                Location = new Point(75, 70),
                Visible = false
            };

            _1 = new DXImageControl
            {
                Parent = this,
                Index = _libraryOffset + 0,
                LibraryFile = LibraryFile.GameInter,
                PassThrough = true,
                UseOffSet = true,
                Location = new Point(100, 70),
                Visible = false
            };
        }

        public override void Process()
        {
            base.Process();

            var timer = GetBestTimer();

            if (timer != null)
            {
                if (timer != CurrentTimer || timer.Refresh)
                {
                    CurrentTimer = timer;
                    CurrentTimer.Refresh = false;

                    _timerStarted = true;
                    _timerTime = CEnvir.Now.AddSeconds(1);
                    _timerCounter = (int)(CurrentTimer.RelativeTime - CEnvir.Now).TotalSeconds;

                    UpdateTimeGraphic();
                }
            }

            if (CurrentTimer == null || _timerStarted == false || CEnvir.Now < _timerTime) return;

            _timerCounter--;
            _timerTime = CEnvir.Now.AddSeconds(1);

            if (_timerCounter < 0 && _eggTimer != null)
            {
                _1000.Visible = _100.Visible = _10.Visible = _1.Visible = false;
                _eggTimer.Loop = false;
                _timerStarted = false;

                ActiveTimers.Remove(CurrentTimer);
                CurrentTimer = null;
                return;
            }

            UpdateTimeGraphic();
        }

        private ClientTimer GetBestTimer()
        {
            return ActiveTimers.OrderBy(x => x.RelativeTime).FirstOrDefault();
        }

        public ClientTimer GetTimer(string key)
        {
            return ActiveTimers.FirstOrDefault(x => x.Key == key);
        }

        public void AddTimer(S.SetTimer p)
        {
            if (p.Seconds <= 0)
            {
                ExpireTimer(p.Key);
                return;
            }

            var currentTimer = GetTimer(p.Key);

            if (currentTimer != null)
            {
                currentTimer.Update(p.Seconds, p.Type);
                return;
            }

            ActiveTimers.Add(new ClientTimer(p.Key, p.Seconds, p.Type));
        }

        public void ExpireTimer(string key)
        {
            var timer = ActiveTimers.FirstOrDefault(x => x.Key == key);

            if (timer != null)
            {
                timer.RelativeTime = DateTime.MinValue;

                if (timer == CurrentTimer)
                {
                    _timerCounter = 0;
                }
            }
        }

        private void UpdateTimeGraphic()
        {
            TimeSpan ts = new TimeSpan(0, 0, _timerCounter);

            if (ts.Hours > 0)
            {
                _1000.Index = _libraryOffset + (ts.Hours / 10);
                _100.Index = _libraryOffset + (ts.Hours % 10);
                _10.Index = _libraryOffset + (ts.Minutes / 10);
                _1.Index = _libraryOffset + (ts.Minutes % 10);
            }
            else
            {
                _1000.Index = _libraryOffset + (ts.Minutes / 10);
                _100.Index = _libraryOffset + (ts.Minutes % 10);
                _10.Index = _libraryOffset + (ts.Seconds / 10);
                _1.Index = _libraryOffset + (ts.Seconds % 10);
            }

            Visible = true;
            _1000.Visible = _100.Visible = _10.Visible = _1.Visible = true;

            switch (CurrentTimer.Type)
            {
                default:
                case 0:
                    _eggTimer.Visible = false;
                    _eggTimer.Index = 960;
                    break;
                case 1:
                    _eggTimer.Visible = true;
                    _eggTimer.Index = 960;
                    break;
                case 2:
                    _eggTimer.Visible = true;
                    _eggTimer.Index = 440;
                    break;
            }

            _eggTimer.Loop = true;
        }
    }

    public class ClientTimer
    {
        public string Key;
        public byte Type;
        public int Seconds;

        public DateTime RelativeTime;
        public bool Refresh;

        public ClientTimer(string key, int time, byte type)
        {
            Key = key;
            Update(time, type);
        }

        public void Update(int time, byte type)
        {
            Seconds = time;
            Type = type;

            RelativeTime = CEnvir.Now.AddSeconds(Seconds);
            Refresh = true;
        }
    }
}
