using Windows.UI.Xaml.Media.Imaging;

namespace CDJPlayer.Models
{
    internal class MusicInfoModel
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string BPM { get; set; }
        public string Bitrate { get; set; }
        public string Duration { get; set; }
        public BitmapImage Artwork { get; set; }
    }
}
