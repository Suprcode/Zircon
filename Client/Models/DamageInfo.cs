using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Library;

namespace Client.Models
{
/*
    public class DamageInfoOld
    {
        public static List<DXLabel> Labels = new List<DXLabel>();

        public string Text;
        public Color ForeColour;
        public Color OutlineColour;

        public DateTime StartTime;
        public TimeSpan Duration;
        public int OffsetX = 25;
        public int OffsetY = 50;
        public bool Shift;

        public DXLabel Label;

        public DamageInfo(int damage)
        {
            Text = damage.ToString("+#,##0;-#,##0");
            StartTime = CEnvir.Now;
            Duration = TimeSpan.FromSeconds(2);
            OutlineColour = Color.Black;

            if (damage <= -500)
                ForeColour = Color.Orange;
            else if (damage <= -100)
                ForeColour = Color.Green;
            else if (damage < 0)
                ForeColour = Color.Red;
            else
                ForeColour = Color.Blue;


            CreateLabel();
        }
        public DamageInfo(string text, Color textColour)
        {
            Text = text;
            ForeColour = textColour;
            StartTime = CEnvir.Now;
            Duration = TimeSpan.FromSeconds(2);
            OutlineColour = Color.Black;

            CreateLabel();
        }

        public void CreateLabel()
        {
            Label = Labels.FirstOrDefault(x => x.Text == Text && x.ForeColour == ForeColour && x.OutlineColour == OutlineColour);

            if (Label != null) return;

            Label = new DXLabel
            {
                Text = Text,
                ForeColour = ForeColour,
                Outline = true,
                OutlineColour = OutlineColour,
                IsVisible = true,
                Font = new Font(Config.FontName, 10),
            };

            Labels.Add(Label);
            Label.Disposing += (o, e) => Labels.Remove(Label);
        }

        public void Draw(int DrawX, int DrawY)
        {
            TimeSpan time = CEnvir.Now - StartTime;

            int x = (int)(time.Ticks / (Duration.Ticks / OffsetX) % OffsetX);
            int y = (int)(time.Ticks / (Duration.Ticks / OffsetY) % OffsetY);

            if (Shift)
                x -= Label.Size.Width - 5;

            Point location = new Point(GameScene.Game.Location.X + DrawX + x + 13 , GameScene.Game.Location.Y + DrawY - y - 30);


           // location.X += (48 - Label.Size.Width) / 2;
            location.Y -= 32  - Label.Size.Height;

            Label.Location = location;

            Label.Draw();
        }

    }*/

    public class DamageInfo
    {
        #region Value

        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                int oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private int _Value;
        public event EventHandler<EventArgs> ValueChanged;
        public virtual void OnValueChanged(int oValue, int nValue)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Block

        public bool Block
        {
            get { return _Block; }
            set
            {
                if (_Block == value) return;

                bool oldValue = _Block;
                _Block = value;

                OnBlockChanged(oldValue, value);
            }
        }
        private bool _Block;
        public event EventHandler<EventArgs> BlockChanged;
        public virtual void OnBlockChanged(bool oValue, bool nValue)
        {
            BlockChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region Miss

        public bool Miss
        {
            get { return _Miss; }
            set
            {
                if (_Miss == value) return;

                bool oldValue = _Miss;
                _Miss = value;

                OnMissChanged(oldValue, value);
            }
        }
        private bool _Miss;
        public event EventHandler<EventArgs> MissChanged;
        public virtual void OnMissChanged(bool oValue, bool nValue)
        {
            MissChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Critical

        public bool Critical
        {
            get { return _Critical; }
            set
            {
                if (_Critical == value) return;

                bool oldValue = _Critical;
                _Critical = value;

                OnCriticalChanged(oldValue, value);
            }
        }
        private bool _Critical;
        public event EventHandler<EventArgs> CriticalChanged;
        public virtual void OnCriticalChanged(bool oValue, bool nValue)
        {
            CriticalChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public static MirLibrary Library;

        
        public DateTime StartTime { get; set; } = CEnvir.Now;
        public TimeSpan AppearDelay { get; set; } = TimeSpan.FromMilliseconds(500);
        public TimeSpan ShowDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan HideDelay { get; set; } = TimeSpan.FromMilliseconds(250);

        public int BlueWidth = 9, RedWidth = 9, GreenWidth = 11, OrangeWidth = 13, WhiteWidth = 20;
        public int BlueIndex = 71, RedIndex = 72, GreenIndex = 73, OrangeIndex = 74, WhiteIndex = 75;

        static DamageInfo()
        {
            CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out Library);
        }

        public int DrawHeight = 80;
        public int DrawY { get; set; }

        public bool Visible = true;
        public int Bottom => DrawY - 20;
        public float Opacity;

        public void Process(DamageInfo previous)
        {
            if (Library == null) return;

            TimeSpan visibleTime = CEnvir.Now - StartTime;

            int oldY = DrawY;

            if (visibleTime < AppearDelay)
            {
                decimal percent = visibleTime.Ticks/(decimal)AppearDelay.Ticks;

                if (DrawY != 0)
                {
                    
                }


                DrawY = (int) (DrawHeight * percent);

                Opacity = (float)(percent * 3);

            }
            else if (visibleTime < AppearDelay + ShowDelay)
            {
                DrawY = DrawHeight;
                Opacity = 1;
            }
            else if (visibleTime < AppearDelay + ShowDelay + HideDelay)
            {
                //20 Travel distance?
                visibleTime -= AppearDelay + ShowDelay;
                decimal percent = visibleTime.Ticks / (decimal)HideDelay.Ticks;

                DrawY = DrawHeight + (int) (40*percent);
                Opacity = 1 - (float) (percent);
            }
            else
            {
                Visible = false;
                return;
            }

            if (previous != null && previous.Visible)
                DrawY = Math.Min(DrawY, previous.Bottom);

            if (oldY != DrawY)
            GameScene.Game.MapControl.TextureValid = false;
        }

        public void Draw(int drawX, int drawY)
        {
            if (Library == null) return;


            drawY -= DrawY + 20;

            drawX += 24;
            Size size;
            if (Value == 0)
            {
                if (Miss)
                {
                    size = Library.GetSize(76);
                    drawX -= size.Width / 2;

                    Library.Draw(76, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                }
                else
                if (Block)
                {
                    size = Library.GetSize(77);
                    drawX -= size.Width / 2;

                    Library.Draw(77, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                }

                //Block
            }
            else
            {
                string text = Value.ToString("+#0;-#0");


               
                int index;
                int width;
                if (Value <= -1000)
                {
                    //White
                    index = WhiteIndex;
                    width = WhiteWidth;
                }
                else if (Value <= -500)
                {
                    //Orange
                    index = OrangeIndex;
                    width = OrangeWidth;
                }
                else if (Value <= -100)
                {
                    //Green
                    index = GreenIndex;
                    width = GreenWidth;
                }
                else if (Value < 0)
                {
                    //Red
                    index = RedIndex;
                    width = RedWidth;
                }
                else
                {
                    //Blue
                    index = BlueIndex;
                    width = BlueWidth;
                }
                drawX -= width * text.Length / 2;

                if (Critical && Value < 0)
                {
                    size = Library.GetSize(78);
                    drawX -= size.Width / 2;

                    Library.Draw(78, drawX, drawY, Color.White, false, Opacity, ImageType.Image);
                    drawX += size.Width + 5;
                }
                
                size = Library.GetSize(index);

                for (int i = 0; i < text.Length; i++)
                {
                    int number;

                    if (!int.TryParse(text[i].ToString(), out number))
                    {
                        if (text[i] == '+')
                            number = 10;
                        else if (text[i] == '-')
                            number = 11;
                        else continue;
                    }


                    Library.Draw(index, drawX, drawY, Color.White, new Rectangle(width*number, 0, Math.Min(size.Width - width*number, width), size.Height), Opacity, ImageType.Image);
                    drawX += width;
                }

            }


        }


    }
}
