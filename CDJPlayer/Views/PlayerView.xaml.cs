using CDJPlayer.Models;
using CDJPlayer.Models.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.Storage;
using NAudio.Wave;
using CDJPlayer.Constants;
using System.Diagnostics;

namespace CDJPlayer.Views
{
    public sealed partial class PlayerView : UserControl, IPlayer
    {
        private Player _player;
        private PlayerMode _mode = PlayerMode.Paused;
        private TempoRangeView _tempoRangeView;
        private bool autoCue = false;
        private DispatcherTimer _playerTimer;
        private DispatcherTimer _shortTimeDangerTimer;
        private TempoRange _tempoRangeValue;
        private CuePoint _cuePoint;
        private float _bpm;
        private TimeSpan _pausedTime;
        private TimeSpan _playerTime;
        private TimeSpan _loopStartTime;
        private TimeSpan _loopEndTime;
        private bool _pausedSeek;
        private Double _currentTempo = 1;
        private bool _shortTimeRedColor;
        private bool _isAutoCue;
        private double seekToTime = 0;
        private bool searching = false;
        private SettingsView _settingsView;
        public TimeMode _timeMode = TimeMode.Remaining;
        public Double CurrentTempo { get => _currentTempo; }
        public TempoRange GetTempoRangeValue { get => _tempoRangeValue; }
        public float GetCurrentBPM { get => _bpm; }
        public PlayerState PlayerState { get => _player.PlayerState; }
        public bool PauseSeek { get => _pausedSeek; }

        public PlayerView(Player player, SettingsView settingsView)
        {
            this.InitializeComponent();

            this._player = player;
            this._settingsView = settingsView;

            _tempoRangeValue = TempoRange.PlusMinus10;
            _tempoRangeView = new TempoRangeView();
            tempoRange.Children.Add(_tempoRangeView);
            tempoRange.Visibility = Visibility.Collapsed;

            _cuePoint = new CuePoint();

            _playerTimer = new DispatcherTimer();
            _playerTimer.Interval = TimeSpan.FromMilliseconds(1);
            _playerTimer.Tick += PlayerTimer_Tick;

            _shortTimeDangerTimer = new DispatcherTimer();
            _shortTimeDangerTimer.Interval = TimeSpan.FromSeconds(0.5);
            _shortTimeDangerTimer.Tick += _shortTimeDangerTimer_Tick;

        }

        private void _shortTimeDangerTimer_Tick(object sender, object e)
        {
            if (_shortTimeRedColor)
            {
                sliderTimePositioSong.Background = new SolidColorBrush(Colors.Black);
                _shortTimeRedColor = false;
            }
            else
            {
                sliderTimePositioSong.Background = new SolidColorBrush(Colors.Red);
                _shortTimeRedColor = true;
            }
        }
        private void PlayerTimer_Tick(object sender, object e)
        {
            switch (_mode)
            {
                case PlayerMode.Playing:
                    break;
                case PlayerMode.Paused:
                    break;
                case PlayerMode.Seek:
                    break;
                case PlayerMode.Loop:
                    if (_player.Position >= _loopEndTime)
                    {
                        _player.SetPosition(_loopStartTime);
                    }
                    break;
                default:
                    break;
            }


            // Search CUE point 
            if (_pausedSeek)
            {
                //Debug.WriteLine("PAUSED TIME ", _pausedTime.ToString());
                //Debug.WriteLine("time ", _player.Position.ToString());

                seekToTime = _pausedTime.Add(new TimeSpan(0, 0, 0, 0, Settings.pausedSeekTime)).TotalMilliseconds;

                if (_player.Position.TotalMilliseconds >= seekToTime)
                {
                    _player.SetPosition(_pausedTime);

                }

                if (_timeMode == TimeMode.Remaining)
                    _playerTime = (_player.Duration - _pausedTime);
                else
                    _playerTime = _pausedTime;

            }
            else
            {

            sliderTimePositioSong.Maximum = Double.Parse(_player.Duration.TotalSeconds.ToString());

                if (_timeMode == TimeMode.Remaining)
                    _playerTime = (_player.Duration - _player.Position);
                else
                    _playerTime = _player.Position;

            }
            // Min
            var min = string.Format("{0:mm}", _playerTime).ToCharArray();
            timeTrackMinFirstZero.Text = min[0].ToString();
            timeTrackMinSecondZero.Text = min[1].ToString();
            // Seconds
            var seconds = string.Format("{0:ss}", _playerTime).ToCharArray();
            timeTrackSecondsFirstZero.Text = seconds[0].ToString();
            timeTrackSecondsSecondZero.Text = seconds[1].ToString();
            // Miliseconds
            var miliseconds = string.Format("{0:fff}", _playerTime);
            timeTrackMilisecondsFirstZero.Text = miliseconds[0].ToString();
            timeTrackMilisecondsSecondZero.Text = miliseconds[1].ToString();
            //timeTrackMilisecondsLastZero.Text = miliseconds[2].ToString();

           if (!searching) sliderTimePositioSong.Value = Double.Parse(_player.Position.TotalSeconds.ToString());

            if (Double.Parse(_player.Duration.TotalSeconds.ToString()) - Double.Parse(_player.Position.TotalSeconds.ToString()) <= 40)
            {
                if (!_shortTimeDangerTimer.IsEnabled)
                    _shortTimeDangerTimer.Start();
            }
            else
            {
                sliderTimePositioSong.Background = new SolidColorBrush(Colors.White);
                _shortTimeDangerTimer.Stop();
            }
        }

        /// <summary>
        /// Load audio file from listItemDrive object.
        /// </summary>
        /// <param name="listItemDrive"></param>
        public async Task Load(ListItemDrive listItemDrive)
        {
            // Load data
            await _player.Load(listItemDrive);

            // Reset view
            _cuePoint?.ResetCuePoint();
            notLoadedLabel.Visibility = Visibility.Collapsed;
            waveformSmall.Visibility = Visibility.Visible;
            sliderTimePositioSong.Visibility = Visibility.Visible;
            pointsCanvasBottom.Visibility = Visibility.Visible;
            tempoRange.Visibility = Visibility.Visible;

            // Check BPM value and track id display.
            bmpTrack.Text = String.Format("{0:00.0}", listItemDrive.Bpm);
            _bpm = listItemDrive.Bpm;
            TrackId.Text = listItemDrive.PathDrive;

            // Start player timer for time slider etc.
            ResetCuePoint();
            _playerTimer.Start();

            if (!_isAutoCue)
            {
                Play();
                _mode = PlayerMode.Playing;
            }

            //loadingWaveLabel.Visibility = Visibility.Visible;
            ////await GenerateWaveform(listItemDrive);
            //loadingWaveLabel.Visibility = Visibility.Collapsed;
        }

        private async Task GenerateWaveform(ListItemDrive listItemDrive)
        {
            var storageFile = (StorageFile)listItemDrive.StorageFile;

            var builder = new Mp3FileReaderBase.FrameDecompressorBuilder(wf => new NAudio.FileFormats.Mp3.DmoMp3FrameDecompressor(wf));
            Mp3FileReaderBase file = new Mp3FileReaderBase(storageFile.Path, builder);
        }

        #region Buttons
        public void Play()
        {
            _pausedSeek = false;
            if (!_pausedSeek || _player.PlayerState == PlayerState.Loaded || _player.PlayerState == PlayerState.Stoped || _player.PlayerState == PlayerState.Paused)
            {
                _player.Play();
                playControll.Opacity = 1f;
                pauseControll.Opacity = 0.2f;
                _mode = PlayerMode.Playing;
            }
        }

        public void Pause()
        {
            _player.Pause();
            _mode = PlayerMode.Paused;
            _pausedTime = _player.Position;

            if (!_cuePoint.CueActivate)
            {
                _pausedSeek = true;
                _player.PlayInPause();
                _mode = PlayerMode.Seek;
            }
            pauseControll.Opacity = 1f;
            playControll.Opacity = 0.2f;
        }

        public void PlayPause()
        {
            switch (_player.PlayerState)
            {
                case PlayerState.Playing:
                    if (_pausedSeek)
                    {
                        Play();
                        _mode = PlayerMode.Playing;
                    }
                    else
                    {
                        Pause();
                        _mode = PlayerMode.Paused;
                    }
                    break;
                case PlayerState.Paused:
                    Play();
                    _mode = PlayerMode.Playing;
                    break;
                case PlayerState.Loaded:
                    Play();
                    _mode = PlayerMode.Playing;
                    break;
                default:
                    break;
            }
        }

        public void CuePressed()
        {
            switch (_player.PlayerState)
            {
                case PlayerState.NotLoaded:
                    break;
                case PlayerState.Loaded:
                    if (_cuePoint?.CuePointStatus == CuePointStatus.Created)
                    {
                        Seek(_cuePoint.CuePointTime);
                    }
                    else
                    {
                        // Search auto cue point
                        if (_isAutoCue)
                        {
                            ResetCuePoint();
                            _cuePoint?.SetCuePointTime(new TimeSpan(0, 0, 0, 0, Settings.autoCueTime));
                            DrawCuePoint();
                        }
                        else
                        {
                            _cuePoint?.SetCuePointTime(_pausedTime);
                        }
                    }
                    break;
                case PlayerState.Buffering:
                    break;
                case PlayerState.Playing:
                    if (_pausedSeek && !_cuePoint.CueActivate)
                    {
                        ResetCuePoint();
                        _cuePoint.SetCuePointTime(_pausedTime);
                        DrawCuePoint();
                        Pause();
                        _pausedSeek = false;
                    }
                    else if (_cuePoint?.CuePointStatus == CuePointStatus.Created && !_cuePoint.CueActivate)
                    {
                        _cuePoint.CueActivate = true;
                        SetPosition(_cuePoint.CuePointTime);
                        pauseControll.Opacity = 1f;
                        playControll.Opacity = 0.2f;
                    }
                    else
                    {
                        _pausedSeek = false;
                    }
                    break;
                case PlayerState.Paused:
                    if (_cuePoint.CuePointStatus == CuePointStatus.Empty)
                    {
                        _cuePoint.SetCuePointTime(_pausedTime);
                        DrawCuePoint();
                    }
                    else if (_player.Position != _cuePoint.CuePointTime)
                    {
                        ResetCuePoint();
                        if (_pausedSeek)
                        {
                        _cuePoint.SetCuePointTime(_pausedTime);
                        }
                        else
                        {
                            _cuePoint.SetCuePointTime(_player.Position);
                        }
                        DrawCuePoint();
                    }
                    _pausedSeek = false;
                    _player.Play();
                    break;
                case PlayerState.Stoped:
                    if (_player.Position != _cuePoint.CuePointTime)
                    {
                        ResetCuePoint();
                        _cuePoint.SetCuePointTime(_player.Position);
                        DrawCuePoint();
                    }
                    _player.Play();
                    _mode = PlayerMode.Playing;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Cue button.
        /// </summary>
        public void CueReleased()
        {
            if (_cuePoint?.CuePointStatus == CuePointStatus.Created)
            {
                _player.Pause();
                _mode = PlayerMode.Paused;
                SetPosition(_cuePoint.CuePointTime);
                _cuePoint.CueActivate = false;
            }
        }

        #endregion

        /// <summary>
        /// Seek player position. 
        /// </summary>
        /// <param name="time"></param>
        public void Seek(TimeSpan time)
        {
            if (_player.PlayerState != PlayerState.NotLoaded)
                _player.Seek(time);
        }

        /// <summary>
        /// Set tempo player.
        /// </summary>
        /// <param name="tempo"></param>
        public void SetTempo(double tempo)
        {
            _player.SetTempo(tempo);
            _currentTempo = 1 + tempo;

            // Set current BPM value on UI
            bmpTrack.Text = String.Format("{0:00.0}", (_bpm * (100 + (tempo * 100)) / 100));
        }

        public void AddTempo(double tempoToAdd)
        {
            if (_player.PlayerState != PlayerState.NotLoaded)
                _player.SetTempo(_player.GetTempo + tempoToAdd);
        }

        public void jogTempo(double tempo)
        {
            if (_player.PlayerState != PlayerState.NotLoaded)
                _player.SetTempo(CurrentTempo + tempo);
        }

        public Task<float> GetBPM()
        {
            throw new NotImplementedException();
        }

        public double GetTempo()
        {
            return _player.GetTempo;
        }

        public void setTempoValue(double value, double tempo)
        {
            tempoTrack.Text = string.Format("{0:N1}%", Math.Round(value, 1));
            _currentTempo = 1 + tempo;
            _player.SetTempo(1 + tempo);

            bmpTrack.Text = String.Format("{0:00.0}", (_bpm * (100 + (tempo * 100)) / 100));
        }

        public void ChangeTempoRange()
        {
            _tempoRangeView.ChangeTempoRange();
            _tempoRangeValue = _tempoRangeView.GetTempoRange;
        }

        #region Draw Cue's
        private void DrawCuePoint()
        {
            double xValRatio = sliderTimePositioSong.Value / (sliderTimePositioSong.Maximum - sliderTimePositioSong.Minimum); //gets % the thumb is across the slider
            double thumbPosX = xValRatio * sliderTimePositioSong.Width - 1; //gets the absolute position

            PointCollection pointsBottom = new PointCollection();
            pointsBottom.Add(new Windows.Foundation.Point(thumbPosX, 0));
            pointsBottom.Add(new Windows.Foundation.Point(thumbPosX - 5, 10));
            pointsBottom.Add(new Windows.Foundation.Point(thumbPosX + 5, 10));
            pointsBottom.Add(new Windows.Foundation.Point(thumbPosX, 0));

            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.Yellow;

            DrawPolygonPointOnCanvas(brush, true, "");
            cuePointCanvasBottom.Points = pointsBottom;
        }

        public void DrawPolygonPointOnCanvas(SolidColorBrush color, bool top, string text)
        {
            double xValRatio = sliderTimePositioSong.Value / (sliderTimePositioSong.Maximum - sliderTimePositioSong.Minimum); //gets % the thumb is across the slider
            double thumbPosX = xValRatio * sliderTimePositioSong.Width - 1; //gets the absolute position

            PointCollection points = new PointCollection();

            Polygon polygon1 = new Polygon();

            if (top)
            {
                points.Add(new Windows.Foundation.Point(thumbPosX, 10));
                points.Add(new Windows.Foundation.Point(thumbPosX + 5, 0));
                points.Add(new Windows.Foundation.Point(thumbPosX - 5, 0));
                points.Add(new Windows.Foundation.Point(thumbPosX, 10));
            }
            else
            {
                points.Add(new Windows.Foundation.Point(thumbPosX, 0));
                points.Add(new Windows.Foundation.Point(thumbPosX - 5, 10));
                points.Add(new Windows.Foundation.Point(thumbPosX + 5, 10));
                points.Add(new Windows.Foundation.Point(thumbPosX, 0));
            }

            polygon1.Points = points;
            polygon1.Fill = color;
            pointsCanvasTop.Children.Add(polygon1);

            if (!string.IsNullOrEmpty(text))
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = text;
                textBlock.FontSize = 15;
                Canvas.SetLeft(textBlock, thumbPosX + 5);
                Canvas.SetTop(textBlock, -5);
                textBlock.Foreground = color;
                pointsCanvasTop.Children.Add(textBlock);
            }
        }
        #endregion

        private void ResetCuePoint()
        {
            pointsCanvasTop.Children.Clear();
            cuePointCanvasTop.Points = null;
            cuePointCanvasBottom.Points = null;
        }

        public void Forward(TimeSpan time)
        {
            if (_pausedSeek)
            {
                if (_pausedTime + time < TimeSpan.FromMilliseconds(0))
                {
                    _pausedTime = TimeSpan.FromMilliseconds(0);
                }
                else
                {
                    _pausedTime = _pausedTime - time;
                }
            }
            if (_timeMode == TimeMode.Remaining)
            _playerTime += time;
            else
            _playerTime -= time;
           
            _player.Seek(time);
        }

        public void Rewind(TimeSpan time)
        {
            _player.Seek(time);

            if (_pausedSeek)
                _pausedTime = _pausedTime - time;
        }

        public void SetPosition(TimeSpan time)
        {
            _player.SetPosition(time);
        }

        public void SetAutoCue()
        {
            _isAutoCue = !_isAutoCue;

            if (_isAutoCue)
            {
                autCueControll.Opacity = 1f;
            }
            else
            {
                autCueControll.Opacity = 0.2f;
            }
        }

        public void setSeekPosition(JogTurn turn)
        {
            switch (turn)
            {
                case JogTurn.Left:
                    Forward(TimeSpan.FromMilliseconds(Settings.seekTime));

                    break;
                case JogTurn.Right:
                    Forward(TimeSpan.FromMilliseconds(-Settings.seekTime));
                    break;
                default:
                    break;
            }
        }

        private void sliderTimePositioSong_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            searching = false;
            _player.SetPosition(TimeSpan.FromMilliseconds(sliderTimePositioSong.Value));
        }

        public TimeSpan GetPlayerTime()
        {
            return _player.Position;
        }

        public void LoopTime(TimeSpan loopStartTime, TimeSpan loopEndTime)
        {
            _loopStartTime = loopStartTime;
            _loopEndTime = loopEndTime;
            _mode = PlayerMode.Loop;
        }

        public void StopLoop()
        {
            _mode = PlayerMode.Playing;
        }

        private void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _settingsView.Show();
        }

        public void SetPlayerNumber(string number)
        {
            playerNumber.Text = number;
        }
    }
}