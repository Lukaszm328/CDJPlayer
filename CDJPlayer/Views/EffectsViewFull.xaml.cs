using CDJPlayer.Models;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static DjPlayer.Models.EfxMode;

namespace CDJPlayer.Views
{
    public sealed partial class EffectsViewFull : UserControl
    {
        private EffectMode _currentEfxMode = EffectMode.Loop;
        private LoopEfx _loopEfx = new LoopEfx();
        private PlayerView _playerView;
        private DispatcherTimer _flaherTimer;
        private HotCue _hotCueA = new HotCue();
        private HotCue _hotCueB = new HotCue();
        private HotCue _hotCueC = new HotCue();

        public EffectsViewFull(PlayerView playerView)
        {
            this.InitializeComponent();
            this._playerView = playerView;

            _flaherTimer = new DispatcherTimer();
            _flaherTimer.Interval = TimeSpan.FromMilliseconds(300);
            _flaherTimer.Tick += FlasherTimer_Tick;
            _flaherTimer.Start();
        }

        private void FlasherTimer_Tick(object sender, object e)
        {
            if (_loopEfx.LoopInTime >TimeSpan.FromSeconds(0))
            {
                if (_loopEfx.InActive)
                {
                    _loopEfx.InActive = false;
                    loopEffectIN.Opacity = 0.3;
                }
                else
                {
                    _loopEfx.InActive = true;
                    loopEffectIN.Opacity = 1;
                }

            }

            if (_loopEfx.LoopEndTime > TimeSpan.FromSeconds(0))
            {
                if (!_loopEfx.InActive)
                {
                    _loopEfx.OutActive = false;
                    loopEffectOUT.Opacity = 0.3;
                }
                else
                {
                    _loopEfx.OutActive = true;
                    loopEffectOUT.Opacity = 1;
                }
            }

        }

        public void ChangeMode()
        {
            switch (_currentEfxMode)
            {
                case EffectMode.Loop:
                    EffectLoopMode.Background = new SolidColorBrush(Colors.DarkBlue);
                    EffectLoopMode.BorderThickness = new Thickness(0);
                    EffectCueMode.BorderThickness = new Thickness(2);
                    EffectCueMode.Background = new SolidColorBrush(Colors.Blue);
                    _currentEfxMode = EffectMode.HotCue;
                    efxEffectView.Opacity = 0.3f;
                    cueEffectView.Opacity = 1f;
                    loopEffectView.Opacity = 0.3f;
                    LoopHeader.Background = new SolidColorBrush(Colors.Black);
                    LoopText.Foreground = new SolidColorBrush(Colors.White);
                    EfxHeader.Background = new SolidColorBrush(Colors.Black);
                    EfxText.Foreground = new SolidColorBrush(Colors.White);
                    HotCueHeader.Background = new SolidColorBrush(Colors.White);
                    HotCueText.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case EffectMode.Effects:
                    EffectsMode.Background = new SolidColorBrush(Colors.DarkBlue);
                    EffectsMode.BorderThickness = new Thickness(0);
                    EffectLoopMode.BorderThickness = new Thickness(2);
                    EffectLoopMode.Background = new SolidColorBrush(Colors.Blue);
                    _currentEfxMode = EffectMode.Loop;
                    efxEffectView.Opacity = 0.3f;
                    cueEffectView.Opacity = 0.5f;
                    loopEffectView.Opacity = 1f;
                    LoopHeader.Background = new SolidColorBrush(Colors.White);
                    LoopText.Foreground = new SolidColorBrush(Colors.Black);
                    EfxHeader.Background = new SolidColorBrush(Colors.Black);
                    EfxText.Foreground = new SolidColorBrush(Colors.White);
                    HotCueHeader.Background = new SolidColorBrush(Colors.Black);
                    HotCueText.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case EffectMode.HotCue:
                    EffectCueMode.Background = new SolidColorBrush(Colors.DarkBlue);
                    EffectCueMode.BorderThickness = new Thickness(0);
                    EffectsMode.BorderThickness = new Thickness(2);
                    EffectsMode.Background = new SolidColorBrush(Colors.Blue);
                    _currentEfxMode = EffectMode.Effects;
                    efxEffectView.Opacity = 1f;
                    cueEffectView.Opacity = 0.5f;
                    loopEffectView.Opacity = 0.3f;
                    LoopHeader.Background = new SolidColorBrush(Colors.Black);
                    LoopText.Foreground = new SolidColorBrush(Colors.White);
                    EfxHeader.Background = new SolidColorBrush(Colors.White);
                    EfxText.Foreground = new SolidColorBrush(Colors.Black);
                    HotCueHeader.Background = new SolidColorBrush(Colors.Black);
                    HotCueText.Foreground = new SolidColorBrush(Colors.White);
                    break;
                default:
                    break;
            }
        }

        public void SetMode(EffectMode effectMode)
        {
            _currentEfxMode = effectMode - 1;
            ChangeMode();
        }

        public EffectMode GetMode()
        {
            return _currentEfxMode;
        }

        // LOOP
        public void SetBackgroundLoopBtnIN()
        {
            loopEffectIN.Background = new SolidColorBrush(Colors.Orange);
        }

        public void SetBackgroundLoopBtnOUT()
        {
            loopEffectOUT.Background = new SolidColorBrush(Colors.Orange);
        }

        public void ResetBacgroundLoopBtn()
        {
            loopEffectIN.Background = new SolidColorBrush(Colors.Black);
            loopEffectOUT.Background = new SolidColorBrush(Colors.Black);
        }

        public void Show(bool isShow)
        {
            if (isShow)
                mainGrid.Visibility = Visibility.Visible;
            else
                mainGrid.Visibility = Visibility.Collapsed;
        }

        // HOT CUE
        public void SetActiveCuePoint(int btnId)
        {
            if (btnId == 1)
                cueEffect1.Background = new SolidColorBrush(Colors.Orange);
            else if (btnId == 2)
                cueEffect2.Background = new SolidColorBrush(Colors.Orange);
            else if (btnId == 3)
                cueEffect3.Background = new SolidColorBrush(Colors.Orange);
        }

        public void ResetActiveCuePoints()
        {
            cueEffect1.Background = new SolidColorBrush(Colors.Black);
            cueEffect2.Background = new SolidColorBrush(Colors.Black);
            cueEffect3.Background = new SolidColorBrush(Colors.Black);
        }

        // EFX



        // BUTTONS

        private void Action(EfxButton efxButton)
        {
            switch (_currentEfxMode)
            {
                case EffectMode.Loop:
                    switch (efxButton)
                    {
                        case EfxButton.ButtonA:
                            if (!_loopEfx.IsActive)
                            {
                                _loopEfx.LoopInTime = _playerView.GetPlayerTime();
                            }
                            break;
                            break;
                        case EfxButton.ButtonB:
                            if (_loopEfx.LoopInTime != null && _loopEfx.LoopInTime > TimeSpan.FromMilliseconds(0))
                            {
                                _loopEfx.LoopEndTime = _playerView.GetPlayerTime();
                                _loopEfx.IsActive = true;
                                _playerView.LoopTime(_loopEfx.LoopInTime, _loopEfx.LoopEndTime); ;
                            }
                            break;
                        case EfxButton.ButtonC:
                            if (_loopEfx.IsActive)
                            {
                                _loopEfx.IsActive = false;
                                _loopEfx.LoopEndTime = TimeSpan.FromSeconds(0);
                                _loopEfx.LoopInTime = TimeSpan.FromSeconds(0);
                                _playerView.StopLoop();
                                loopEffectIN.Opacity = 1;
                                loopEffectOUT.Opacity = 1;
                            }

                            break;
                        default:
                            break;
                    }
                    break;
                case EffectMode.Effects:
                    switch (efxButton)
                    {
                        case EfxButton.ButtonA:

                            break;
                        case EfxButton.ButtonB:
                         
                            break;
                        case EfxButton.ButtonC:
                           
                            break;
                        default:
                            break;
                    }
                    break;
                    break;
                case EffectMode.HotCue:
                    switch (efxButton)
                    {
                        case EfxButton.ButtonA:
                            if (!_hotCueA.IsCreated)
                            {
                            _hotCueA.IsCreated=true;
                            _hotCueA.Time = _playerView.GetPlayerTime();
                            cueEffect1.Opacity = 1;
                            SolidColorBrush brush = new SolidColorBrush();
                            brush.Color = Colors.Green;
                            _playerView.DrawPolygonPointOnCanvas(brush,true, "A");
                            }
                            else
                            {
                                _playerView.SetPosition(_hotCueA.Time);
                            }
                            break;
                        case EfxButton.ButtonB:
                            if (!_hotCueB.IsCreated)
                            {
                                _hotCueB.IsCreated = true;
                                _hotCueB.Time = _playerView.GetPlayerTime();
                                cueEffect2.Opacity = 1;
                                SolidColorBrush brush = new SolidColorBrush();
                                brush.Color = Colors.Green;
                                _playerView.DrawPolygonPointOnCanvas(brush, true, "B");
                            }
                            else
                            {
                                _playerView.SetPosition(_hotCueB.Time);
                            }
                            break;
                        case EfxButton.ButtonC:
                            if (!_hotCueC.IsCreated)
                            {
                                _hotCueC.IsCreated = true;
                                _hotCueC.Time = _playerView.GetPlayerTime();
                                cueEffect3.Opacity = 1;
                                SolidColorBrush brush = new SolidColorBrush();
                                brush.Color = Colors.Green;
                                _playerView.DrawPolygonPointOnCanvas(brush, true, "C");
                            }
                            else
                            {
                                _playerView.SetPosition(_hotCueC.Time);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                    break;
                default:
                    break;
            }
        }

        public void PressButtonA()
        {
            Action(EfxButton.ButtonA);
        }

        public void PressButtonB()
        {
            Action(EfxButton.ButtonB);
        }

        public void PressButtonC()
        {
            Action(EfxButton.ButtonC);
        }

        private void efxModeLoop_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _currentEfxMode = EffectMode.Effects;
            ChangeMode();
        }

        private void efxModeHotCue_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _currentEfxMode = EffectMode.Loop;
            ChangeMode();
        }

        private void efxModeEffects_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _currentEfxMode = EffectMode.HotCue;
            ChangeMode();
        }
    }
}