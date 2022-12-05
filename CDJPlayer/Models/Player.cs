using CDJPlayer.Models.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;

namespace CDJPlayer.Models
{
    public class Player : IPlayer
    {
        private StorageFile _storageFile;
        private AudioGraph _audioGraph;
        private AudioFileInputNode _audioFileInputNode;
        private CreateAudioDeviceOutputNodeResult _createAudioDeviceOutputNodeResult;
        private PlayerState _playerState = PlayerState.NotLoaded;

        public PlayerState PlayerState { get => _playerState; }
        public TimeSpan Position { get => _audioFileInputNode.Position; }
        public TimeSpan Duration { get => _audioFileInputNode.Duration; }
        public Double GetTempo { get => _audioFileInputNode.PlaybackSpeedFactor; }

        public Player()
        {
            InitializeAudioGraph();
        }

        private async void InitializeAudioGraph()
        {
            var settings = new AudioGraphSettings(AudioRenderCategory.Media);
            var createResult = await AudioGraph.CreateAsync(settings);
            if (createResult.Status != AudioGraphCreationStatus.Success) return;
            _audioGraph = createResult.Graph;
            _createAudioDeviceOutputNodeResult = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (_createAudioDeviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success) return;
        }

        public async Task Load(ListItemDrive listItemDrive)
        {
            _playerState = PlayerState.Buffering;
            _storageFile = listItemDrive.StorageFile;

            var outputNode = _createAudioDeviceOutputNodeResult.DeviceOutputNode;

            if (_audioFileInputNode != null)
                _audioFileInputNode.RemoveOutgoingConnection(outputNode);

            var fileResult = await _audioGraph.CreateFileInputNodeAsync((listItemDrive.StorageFile));
            if (fileResult.Status != AudioFileNodeCreationStatus.Success) return;
            _audioFileInputNode = fileResult.FileInputNode;
            _audioFileInputNode.AddOutgoingConnection(outputNode);
            _audioFileInputNode.Stop();
            _playerState = PlayerState.Loaded;
        }

        public void Play()
        {
            _audioGraph.Start();
            _audioFileInputNode.Start();
            _playerState = PlayerState.Playing;
        }

        public void PlayInPause()
        {
            _audioGraph.Start();
            _audioFileInputNode.Start();
            _playerState = PlayerState.Paused;
        }

        public void Pause()
        {
             _audioFileInputNode.Stop();
            _playerState = PlayerState.Paused;
        }

        /// <summary>
        /// Seek position player after timeSpan value.
        /// </summary>
        /// <param name="time"></param>
        public void Seek(TimeSpan time)
        {
            if (_playerState != PlayerState.NotLoaded)
            {
                if (PlayerState == PlayerState.Playing || PlayerState == PlayerState.Paused)
                {
                    if ((time + _audioFileInputNode.Position) >= _audioFileInputNode.Duration)
                        _audioFileInputNode.Seek(_audioFileInputNode.Duration);
                    else if ((_audioFileInputNode.Position + time) <= new TimeSpan(0, 0, 0, 0))
                        _audioFileInputNode.Seek(new TimeSpan(0, 0, 0, 0));
                    else
                        _audioFileInputNode.Seek(_audioFileInputNode.Position + time);
                }
                else if (PlayerState == PlayerState.Paused)
                {
                    // kiedy szuka cue
                }
            }
        }

        /// <summary>
        /// Change time position.
        /// </summary>
        /// <param name="timeSpan"></param>
        public void SetPosition(TimeSpan timeSpan)
        {
            if (_audioFileInputNode != null)
            {
                _audioFileInputNode.Seek(timeSpan);
            }
        }

        /// <summary>
        /// Set tempo player in double, default is 1. 
        /// </summary>
        /// <param name="tempo"></param>
        public void SetTempo(double tempo)
        {
            if (_audioFileInputNode != null)
            {
                _audioFileInputNode.PlaybackSpeedFactor = tempo;
            }
        }

        /// <summary>
        /// Return beats per minute. 
        /// </summary>
        /// <returns></returns>
        public async Task<float> GetBPM()
        {
            if (_playerState != PlayerState.NotLoaded)
            {
                var fileProps = await _storageFile.Properties.RetrievePropertiesAsync(null);
                var bpmObj = fileProps["System.Music.BeatsPerMinute"];

                if (bpmObj != null)
                {
                    string a = bpmObj.ToString();
                    int b = Int32.Parse(a);

                    return float.Parse(a);
                }
                else
                {
                    return 000.0f;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}