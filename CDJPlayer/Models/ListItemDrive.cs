using DjPlayer.Models;
using Windows.Storage;

namespace CDJPlayer.Models
{
    public class ListItemDrive
    {
        public int Id { get; set; }
        public FileBrowserItemType.ItemType ItemType { get; set; }
        public StorageFile StorageFile { get; set; }
        public StorageFolder StorageFolder { get; set; }
        public string Color { get; set; }
        public string SongPath { get; set; }
        public string PicturePath { get; set; }
        public string PathDrive { get; set; }
        public string DescriptionDrive { get; set; }
        public string TotalSpaceDrive { get; set; }
        public string FreeSpaceDrrive { get; set; }
        public float  Bpm { get; set; }
    }
}
