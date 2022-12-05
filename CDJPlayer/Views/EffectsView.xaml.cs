using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static DjPlayer.Models.EfxMode;

namespace CDJPlayer.Views
{
    public sealed partial class EffectsView : UserControl
    {
        private EffectMode _currentEfxMode = EffectMode.Loop;
        public EffectsView()
        {
            this.InitializeComponent();

            DispatcherTimer effectOnFlash = new DispatcherTimer();
            effectOnFlash.Interval = TimeSpan.FromMilliseconds(100);
            effectOnFlash.Tick += EffectOnFlash_Tick;
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
                    efxEffectView.Visibility = Visibility.Collapsed;
                    cueEffectView.Visibility = Visibility.Visible;
                    loopEffectView.Visibility = Visibility.Collapsed;
                    break;
                case EffectMode.Effects:
                    EffectsMode.Background = new SolidColorBrush(Colors.DarkBlue);
                    EffectsMode.BorderThickness = new Thickness(0);
                    EffectLoopMode.BorderThickness = new Thickness(2);
                    EffectLoopMode.Background = new SolidColorBrush(Colors.Blue);
                    _currentEfxMode = EffectMode.Loop;
                    efxEffectView.Visibility = Visibility.Collapsed;
                    cueEffectView.Visibility = Visibility.Collapsed;
                    loopEffectView.Visibility = Visibility.Visible;
                    break;
                case EffectMode.HotCue:
                    EffectCueMode.Background = new SolidColorBrush(Colors.DarkBlue);
                    EffectCueMode.BorderThickness = new Thickness(0);
                    EffectsMode.BorderThickness = new Thickness(2);
                    EffectsMode.Background = new SolidColorBrush(Colors.Blue);
                    _currentEfxMode = EffectMode.Effects;
                    efxEffectView.Visibility = Visibility.Visible;
                    cueEffectView.Visibility = Visibility.Collapsed;
                    loopEffectView.Visibility = Visibility.Collapsed;
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
        private void EffectOnFlash_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }

        public void SetActiveEffect(int btnId)
        {
            if (btnId == 1)
                efxEffect1.Background = new SolidColorBrush(Colors.Orange);
            else if (btnId == 2)
                cueEffect2.Background = new SolidColorBrush(Colors.Orange);
            else if (btnId == 3)
                cueEffect3.Background = new SolidColorBrush(Colors.Orange);
        }
    }
}