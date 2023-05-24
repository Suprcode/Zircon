using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Client.Controls;
using Client.Envir;
using Client.Properties;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public class NPCRollDialog : DXControl
    {
        private readonly DXAnimatedControl _animation;
        private readonly DXImageControl _image;

        private int _currentLoop;
        private int _type;
        private int _result;

        private bool _rolled;
        private bool _rolling;

        public NPCRollDialog()
        {
            Movable = false;
            Sort = true;

            _animation = new DXAnimatedControl
            {
                Parent = this,
                Index = 0,
                LibraryFile = LibraryFile.MiniGames,
                UseOffSet = true,
                Location = new Point(0, 0),
                Visible = true
            };
            _animation.AfterAnimationLoop += (o, e) =>
            {
                switch (_type)
                {
                    case 0: //Die
                        {
                            if (_currentLoop < 5)
                            {
                                _currentLoop++;
                                return;
                            }

                            _image.Visible = true;
                            _animation.Visible = false;
                            _animation.Animated = false;
                            ReturnResult();
                        }
                        break;
                    case 1: //Yut
                        {
                            _image.Visible = true;
                            _animation.Visible = false;
                            _animation.Animated = false;
                            ReturnResult();
                        }
                        break;
                }

            };

            _image = new DXImageControl
            {
                Parent = this,
                Index = 0,
                LibraryFile = LibraryFile.MiniGames,
                UseOffSet = true,
                Location = new Point(0, 0),
                Visible = false
            };
            _image.MouseClick += _image_Click;
        }

        public void Setup(int type, int result, bool autoRoll)
        {
            _type = type;
            _result = result;

            _rolled = false;

            _currentLoop = 0;
            Visible = true;

            switch (type)
            {
                case 0: //Die
                    {
                        Size = new Size(65, 65);
                        Location = new Point((GameScene.ActiveScene.Size.Width / 2) - 38, (GameScene.ActiveScene.Size.Height / 2) - 40);

                        _image.Index = 12;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = true;

                        _animation.Loop = true;
                        _animation.Visible = false;
                        _animation.Animated = false;
                    }
                    break;
                case 1: //Yut
                    {
                        Size = new Size(180, 210);
                        Location = new Point((GameScene.ActiveScene.Size.Width / 2) - 90, (GameScene.ActiveScene.Size.Height / 2) - 65);

                        _image.Index = 100;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = true;

                        _animation.Loop = false;
                        _animation.Visible = false;
                        _animation.Animated = false;
                    }
                    break;
            }
        }

        private void Roll()
        {
            Visible = true;

            _rolling = true;

            switch (_type)
            {
                case 0: //Die
                    {
                        _image.Index = 11 + _result;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = false;

                        _animation.BaseIndex = 20;
                        _animation.LibraryFile = LibraryFile.MiniGames;
                        _animation.FrameCount = 4;
                        _animation.AnimationDelay = TimeSpan.FromMilliseconds(400);
                        _animation.AnimationStart = DateTime.MinValue;
                        _animation.Loop = true;
                        _animation.Visible = true;
                        _animation.Animated = true;

                        DXSoundManager.Play(SoundIndex.RollDice);
                    }
                    break;
                case 1: //Yut
                    {
                        _image.Index = 106 + _result;
                        _image.LibraryFile = LibraryFile.MiniGames;
                        _image.Visible = false;

                        _animation.BaseIndex = 100;
                        _animation.LibraryFile = LibraryFile.MiniGames;
                        _animation.FrameCount = 6;
                        _animation.AnimationDelay = TimeSpan.FromMilliseconds(600);
                        _animation.AnimationStart = DateTime.MinValue;
                        _animation.Loop = false;
                        _animation.Visible = true;
                        _animation.Animated = true;

                        DXSoundManager.Play(SoundIndex.RollYut);
                    }
                    break;
            }
        }

        private void _image_Click(object sender, EventArgs e)
        {
            if (_rolling) return;

            if (_rolled)
            {
                Hide();
                return;
            }

            Roll();
        }

        private void Hide()
        {
            Visible = false;
        }

        private void ReturnResult()
        {
            _rolling = false;
            _rolled = true;

            CEnvir.Enqueue(new C.NPCRollResult());
        }
    }
}
