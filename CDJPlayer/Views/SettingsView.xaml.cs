using CDJPlayer.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;


//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace CDJPlayer.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        private int jogTempo = 0;
        private PlayerView _playerView;
        public SettingsView() 
        {
            this.InitializeComponent();
        }

        public void SetPlayer(PlayerView playerView)
        {
            _playerView = playerView;
        }

        private void sliderSeekTime_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Settings.seekTime = e.NewValue;
        }

        private void sliderTimeSeek_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Settings.pausedSeekTime = (int)e.NewValue;
        }

        public void Hide()
        {
            settingsGrid.Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            settingsGrid.Visibility = Visibility.Visible;
        }

        private void sliderJogSpeed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Settings.jogTempo = e.NewValue;
        }

        private void sliderJogSensitiveness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Settings.intervalEncoderInSeek = (int)e.NewValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            _playerView.SetPlayerNumber(rb.Content.ToString());
        }
    }
}
