using CDJPlayer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static DjPlayer.Models.FileBrowserItemType;

namespace CDJPlayer.Views
{
    public sealed partial class FilesBrowserView : UserControl
    {
        private List<StorageFolder> drives = new List<StorageFolder>();
        private List<ListItemDrive> filesBrowserItemsList;
        private Stack stackFilesBrowser = new Stack();
        private StorageFolder currentStoragefolder;
        private int lastPlayedItem = -1;
        private TopPanelView topPanelView;
        private PlayerView playerView;
        private EffectsViewFull effectsView;
        private string playingFilePath;
        private bool musicInfoVisibility = true;
        private MusicInfo musicInfo = new MusicInfo();
        public BrowserViewMode _browserViewMode;

        public FilesBrowserView(TopPanelView topPanelView, PlayerView playerView, EffectsViewFull effectsView)
        {
            this.InitializeComponent();

            this.topPanelView = topPanelView;
            this.playerView = playerView;
            this.effectsView = effectsView;

            musicInfoView.Children.Add(musicInfo);

            filesBrowserItemsList = new List<ListItemDrive>();
            LoadDriveToFileBrowser();
        }

        public List<ListItemDrive> DriveInfoUSBList
        {
            get
            {
                return filesBrowserItemsList;
            }
        }

        public async void LoadDriveToFileBrowser()
        {
            this.DataContext = null;
            filesBrowserItemsList.Clear();
            drives.Clear();

            StorageFolder externalDevicecs = KnownFolders.RemovableDevices;
            var USBdrive = (await externalDevicecs.GetFoldersAsync()).ToList();

            foreach (var driveInfo in USBdrive)
            {
                var allProperties = driveInfo.Properties;
                IEnumerable<string> propertiesToRetrieve = new List<string> { "System.FreeSpace", "System.Capacity" };

                var storageIdProperties = await allProperties.RetrievePropertiesAsync(propertiesToRetrieve);
                string freeSpace = "", totalSpace = "";

                if (!(storageIdProperties["System.FreeSpace"] == null))
                {
                    freeSpace = storageIdProperties["System.FreeSpace"].ToString();
                    totalSpace = storageIdProperties["System.Capacity"].ToString();
                }
                else
                {
                    freeSpace = "0";
                    totalSpace = "0";
                }

                float freeSpaceInt = float.Parse(freeSpace, CultureInfo.InvariantCulture.NumberFormat) / 1024 / 1024 / 1024;
                float total = float.Parse(totalSpace, CultureInfo.InvariantCulture.NumberFormat) / 1024 / 1024 / 1024;
                filesBrowserItemsList.Add(new ListItemDrive { StorageFolder = driveInfo, PicturePath = "/Assets/Images/drive_icon.png", PathDrive = driveInfo.Name, DescriptionDrive = driveInfo.DisplayName.Remove(driveInfo.DisplayName.Length - 4), FreeSpaceDrrive = freeSpaceInt.ToString("F") + " GB Free", TotalSpaceDrive = total.ToString("F") + " GB Total" });
            }

            topPanelView.SetTopSizeTex("[Total of drivers: " + USBdrive.Count());

            this.DataContext = this;
        }

        public async void LoadFoldersAndFilesToFileBrowser(StorageFolder storageFolder)
        {
            this.DataContext = null;
            filesBrowserItemsList.Clear();

            IReadOnlyList<IStorageItem> itemsList = await storageFolder.GetItemsAsync();

            int mp3FilesSize = 0;
            int itemsSize = 0;

            foreach (var item in itemsList)
            {
                // Color = itemsSize % 2 == 0 ? "#22A3E2" : "#106D99"

                if (item is StorageFolder)
                {
                    itemsSize++;
                    filesBrowserItemsList.Add(new ListItemDrive
                    {
                        Id = itemsSize,
                        StorageFolder = (StorageFolder)item,
                        ItemType = ItemType.Folder,
                        PicturePath = "/Assets/Images/folder_ico.png",
                        Color = itemsSize % 2 == 0 ? "#22A3E2" : "#106D99",
                        PathDrive = "",
                        DescriptionDrive = item.Name,
                        FreeSpaceDrrive = "",
                        TotalSpaceDrive = ""
                    });
                }
                else
                {
                    if (((StorageFile)item).FileType == ".mp3")
                    {
                        mp3FilesSize++;
                        itemsSize++;

                        var file = (StorageFile)item;
                        var fileInfo = await file.Properties.RetrievePropertiesAsync(null); // optymalizacja !!!!!!!!!!
                        var fileInfoObj = fileInfo["System.Music.BeatsPerMinute"];
                        var bpm = 000.0f;

                        if (fileInfoObj != null)
                            bpm = Convert.ToSingle(fileInfoObj);

                        filesBrowserItemsList.Add(new ListItemDrive
                        {
                            Id = itemsSize,
                            TotalSpaceDrive = "",
                            FreeSpaceDrrive = "",
                            StorageFile = file,
                            ItemType = ItemType.Track,
                            SongPath = item.Path,
                            PicturePath = "/Assets/Images/track_ico.png",
                            Color = playingFilePath == item.Path ? "Reds" : itemsSize % 2 == 0 ? "#22A3E2" : "#106D99",
                            PathDrive = mp3FilesSize.ToString(),
                            DescriptionDrive = item.Name.Remove(item.Name.Length - 4),
                            Bpm = bpm
                        });
                    }
                }
            }

            if (itemsList.Count == 0)
                filesBrowserItemsList.Add(new ListItemDrive { DescriptionDrive = "Empty folder", ItemType = ItemType.Empty });

            topPanelView.SetTopSizeTex("Total of tracks: " + mp3FilesSize.ToString());

            this.DataContext = this;
        }

        // Click FileBrowser item OnClick
        private async void BrowserDrive_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListItemDrive listItemDrive = (ListItemDrive)e.ClickedItem;
            switch (listItemDrive.ItemType)
            {
                case ItemType.Drive:
                    topPanelView.SetTopDescriptionText("[ USB ]", ItemType.Drive);
                    if (listItemDrive.StorageFolder != null)
                    {
                        stackFilesBrowser.Push(listItemDrive.StorageFolder);
                        currentStoragefolder = listItemDrive.StorageFolder;
                        LoadFoldersAndFilesToFileBrowser(listItemDrive.StorageFolder);

                        if (playerView.PlayerState != PlayerState.Playing || playerView.PlayerState != PlayerState.Paused)
                            topPanelView.SetTopDescriptionText(listItemDrive.StorageFolder.Path, ItemType.Folder);
                    }
                    break;
                case ItemType.Folder:
                    topPanelView.SetTopDescriptionText("[ FOLDER ]", ItemType.Folder);
                    if (listItemDrive.StorageFolder != null)
                    {
                        stackFilesBrowser.Push(listItemDrive.StorageFolder);
                        currentStoragefolder = listItemDrive.StorageFolder;
                        LoadFoldersAndFilesToFileBrowser(listItemDrive.StorageFolder);

                        if (playerView.PlayerState != PlayerState.Playing || playerView.PlayerState != PlayerState.Paused)
                            topPanelView.SetTopDescriptionText(listItemDrive.StorageFolder.Path, ItemType.Folder);
                    }
                    break;
                case ItemType.Track:
                    //var listBackup = filesBrowserItemsList;
                    //var backup = fileBrowser.Items[listItemDrive.Id - 1];
                    //var ds = (ListItemDrive)backup;
                    //ds.PicturePath = "/Assets/Images/Play.png";
                    //ds.Color = "Red";
                    //if (lastPlayedItem != -1)
                    //{
                    //    var itemToReset = (ListItemDrive)fileBrowser.Items[lastPlayedItem];
                    //    itemToReset.Color = "";
                    //    itemToReset.PicturePath = "/Assets/Images/track_ico.png";
                    //}
                    //lastPlayedItem = listItemDrive.Id - 1;
                    //fileBrowser.Items.Remove(fileBrowser.Items[listItemDrive.Id]);
                    //fileBrowser.Items.Add(ds);
                    //this.DataContext = null;
                    //listBackup[listItemDrive.Id] = ds;
                    //filesBrowserItemsList = listBackup;
                    //this.DataContext = this;
                    //ListView listView = sender as ListView;

                    //ListViewItem item = (ListViewItem)listView.SelectedItem;

                    //item.Background = new SolidColorBrush(Colors.Orange);
                    //item.Foreground = new SolidColorBrush(Colors.Blue);
                    playingFilePath = listItemDrive.SongPath;
                    topPanelView.SetTopDescriptionText(listItemDrive.StorageFile.DisplayName, ItemType.Track);
                    await playerView.Load(listItemDrive);
                    break;
                case ItemType.Empty:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Back directory
        /// </summary>
        private void BrowserDrive_BeckEvent()
        {
            if (stackFilesBrowser.Count > 0)
            {
                if (currentStoragefolder == stackFilesBrowser.Peek())
                {
                    stackFilesBrowser.Pop();
                    if (stackFilesBrowser.Count == 0)
                        LoadDriveToFileBrowser();
                    else
                        LoadFoldersAndFilesToFileBrowser((StorageFolder)stackFilesBrowser.Pop());
                }
                else
                {
                    LoadFoldersAndFilesToFileBrowser((StorageFolder)stackFilesBrowser.Pop());
                }
            }
            else
            {
                LoadDriveToFileBrowser();
            }
        }

        public void BackFolder()
        {
            BrowserDrive_BeckEvent();
        }

        public async Task LoadNextFile()
        {
            if ((fileBrowser.SelectedIndex + 1) < fileBrowser.Items.Count())
            {
                fileBrowser.SelectedIndex += 1;

                if (((ListItemDrive)fileBrowser.SelectedItem).StorageFile != null)
                    await playerView.Load(((ListItemDrive)fileBrowser.SelectedItem));
            }
        }

        public async Task LoadPreviousFile()
        {
            if ((fileBrowser.SelectedIndex - 1) > 0)
            {
                fileBrowser.SelectedIndex -= 1;

                if (((ListItemDrive)fileBrowser.SelectedItem).StorageFile != null)
                    await playerView.Load(((ListItemDrive)fileBrowser.SelectedItem));
            }
        }

        public void ScrollUpFileBrowserList()
        {
            fileBrowser.Focus(FocusState.Keyboard);
            if ((fileBrowser.SelectedIndex + 1) < fileBrowser.Items.Count())
                fileBrowser.SelectedIndex += 1;
        }

        public void ScrollDownFileBrowserList()
        {
            fileBrowser.Focus(FocusState.Keyboard);
            if (fileBrowser.SelectedIndex > 0)
                fileBrowser.SelectedIndex -= 1;

        }

        public async void LoadSelectedItem()
        {
            fileBrowser.Focus(FocusState.Keyboard);
            await playerView.Load(((ListItemDrive)fileBrowser.SelectedItem));
        }

        public Visibility GetVisibilityState()
        {
            if (fileBrowser.Visibility == Visibility)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public void ChangeMusicInfoVisibility()
        {
            if (musicInfoVisibility)
            {
                musicInfo.Visibility = Visibility.Collapsed;
                musicInfoVisibility = false;
            }else
            {
                musicInfo.Visibility = Visibility.Visible;
                musicInfoVisibility = true;
            }
        }

        private void fileBrowser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ListView).SelectedItem as ListItemDrive;
            if (selectedItem != null)
                musicInfo.SetMusicInfo(selectedItem);
        }

        public void ChangeViewMode(BrowserViewMode browserViewMode)
        {
            switch (browserViewMode)
            {
                case BrowserViewMode.FullView:
                    mainGrid.Height = 290;
                    fileBrowser.Height = 290;
                    musicInfoView.Visibility = Visibility.Collapsed;
                    mainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                    break;
                case BrowserViewMode.FullViewAndInfo:
                    fileBrowser.Height = 290;
                    mainGrid.Height = 290;
                    mainGrid.ColumnDefinitions[1].Width = new GridLength(0.45, GridUnitType.Star);
                    musicInfoView.Visibility = Visibility.Visible;
                    break;
                case BrowserViewMode.SmallView:
                    musicInfoView.Visibility = Visibility.Collapsed;
                    mainGrid.Height = 210;
                    fileBrowser.Height = 210;
                    mainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                    break;
                default:
                    break;
            }
            _browserViewMode = browserViewMode;
        }
    }
}