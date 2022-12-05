using CDJPlayer.Models;
using Windows.UI.Xaml.Controls;

namespace CDJPlayer.Views
{
    public sealed partial class TempoRangeView : Page
    {
        private TempoRange _currentTempoRangeMode = TempoRange.PlusMinus10;
        public TempoRangeView()
        {
            this.InitializeComponent();
        }

        public void ChangeTempoRange()
        {
            switch (_currentTempoRangeMode)
            {
                case TempoRange.PlusMinus10:
                    tempoRange10.Opacity = 0.4;
                    tempoRange16.Opacity = 1;
                    tempoRange100.Opacity = 0.4;
                    _currentTempoRangeMode = TempoRange.PlusMinus16;
                    break;
                case TempoRange.PlusMinus16:
                    tempoRange10.Opacity = 0.4;
                    tempoRange16.Opacity = 0.4;
                    tempoRange100.Opacity = 1;
                    _currentTempoRangeMode = TempoRange.PlusMinus100;

                    break;
                case TempoRange.PlusMinus100:
                    tempoRange10.Opacity = 1;
                    tempoRange16.Opacity = 0.4;
                    tempoRange100.Opacity = 0.4;
                    _currentTempoRangeMode = TempoRange.PlusMinus10;
                    break;
                default:
                    tempoRange10.Opacity = 1;
                    tempoRange16.Opacity = 0.4;
                    tempoRange100.Opacity = 0.4;
                    _currentTempoRangeMode = TempoRange.PlusMinus16;
                    break;
            }
        }

        public TempoRange GetTempoRange => _currentTempoRangeMode;
    }
}