using Windows.UI.Xaml.Controls;
using static DjPlayer.Models.FileBrowserItemType;

namespace CDJPlayer.Views
{
    public sealed partial class TopPanelView : UserControl
    {
        public TopPanelView()
        {
            this.InitializeComponent();
        }

        public void SetTopDescriptionText(string text, ItemType itemType)
        {
            textBoxTrackList.Text = text;
            switch (itemType)
            {
                case ItemType.Drive:
                    icon.Symbol = Symbol.Favorite;
                    break;
                case ItemType.Folder:
                    icon.Symbol = Symbol.Folder;
                    break;
                case ItemType.Track:
                    icon.Symbol = Symbol.Play;
                    break;
                default:
                    icon.Symbol = Symbol.GlobalNavigationButton;
                    break;
            }
        } 

        public void SetTopSizeTex(string text) => textBoxTracksSize.Text = text;
    }
}
