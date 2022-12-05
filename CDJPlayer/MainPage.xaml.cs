using CDJPlayer.Models;
using CDJPlayer.Views;
using Windows.UI.Xaml.Controls;

namespace CDJPlayer
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Player player = new Player();
            TopPanelView topPanelView = new TopPanelView();
            SettingsView settingsView = new SettingsView();
            PlayerView playerView = new PlayerView(player, settingsView);
            settingsView.SetPlayer(playerView);
            EffectsViewFull effectsViewFull = new EffectsViewFull(playerView);
            FilesBrowserView filesBrowserView = new FilesBrowserView(topPanelView, playerView, effectsViewFull);
            //GpioEvents gpioEvents = new GpioEvents(playerView, filesBrowserView, effectsViewFull, settingsView);
            KeyboardInput keyboardInput = new KeyboardInput(playerView, filesBrowserView, effectsViewFull, settingsView);

            titlePanel.Children.Add(topPanelView);
            fileBrowser.Children.Add(filesBrowserView);
            effects.Children.Add(effectsViewFull);
            settings.Children.Add(settingsView);

            this.player.Children.Add(playerView);
        }
    }
}