using CDJPlayer.Models;
using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using static DjPlayer.Models.FileBrowserItemType;

namespace CDJPlayer.Views
{
    public sealed partial class MusicInfo : UserControl
    {
        public MusicInfo()
        {
            this.InitializeComponent();
        }

        public async void SetMusicInfo(ListItemDrive listItemDrive)
        {
            trackContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            folderContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            driveContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (listItemDrive.ItemType == ItemType.Track)
            {
                MusicProperties musicData = await listItemDrive.StorageFile.Properties.GetMusicPropertiesAsync();
               var image = await listItemDrive.StorageFile.GetThumbnailAsync(ThumbnailMode.MusicView);

                var musicInfo = new MusicInfoModel
                {
                    BPM = listItemDrive.Bpm.ToString(),
                    Year = musicData?.Year.ToString(),
                    Bitrate = musicData?.Bitrate.ToString(),
                    Album = musicData?.Album?.ToString(),
                    Artist = musicData?.Artist?.ToString(),
                    Duration = $"{(int)musicData?.Duration.TotalMinutes}:{musicData?.Duration.Seconds:00}",
                };

                artist.Text = musicInfo.Artist != null ? musicInfo.Artist.ToString() : "empty";
                album.Text = musicInfo.Album != null ? musicInfo.Album.ToString() : "empty";
                year.Text = musicInfo.Year != null ? musicInfo.Year.ToString() : "epmty";
                bpm.Text = musicInfo.BPM != null ? musicInfo.BPM + " bpm" : string.Empty;
                duration.Text = musicInfo.Duration;
                var img = new BitmapImage();
                if (image != null)
                {
                    img.SetSource(image);
                    artwork.Source = img;
                }
                else
                    artwork.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/no_artwork.png", UriKind.Absolute));
            }
            else if (listItemDrive.ItemType == ItemType.Folder)
            {
                trackContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                driveContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                folderContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else if (listItemDrive.ItemType == ItemType.Drive)
            {
                folderContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                trackContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                driveContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
    }
}
