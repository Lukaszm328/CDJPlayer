using CDJPlayer.Views;
using DjPlayer.Model;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.UI.Xaml;
using System;
using Windows.UI.Core;
using CDJPlayer.Constants;

namespace CDJPlayer.Models
{
    class GpioEvents
    {
        PlayerView _playerView;
        FilesBrowserView _filesBrowserView;
        EffectsViewFull _effectsView;
        SettingsView _settingsView;

        public GpioEvents(PlayerView playerView, FilesBrowserView filesBrowserView, EffectsViewFull effectsView, SettingsView settingsView)
        {
            this._playerView = playerView;
            this._filesBrowserView = filesBrowserView;
            this._effectsView = effectsView;
            this._settingsView = settingsView;
            InitializeAndOpenAllPinsForPlayer();
            ConnectToI2cInterfacePitch();
        }

        #region GPIO initialized

        #region INITIALIZE GPIO PIN's
        // Pin's GPIO
        // BUTTON's
        private GpioPin buttonPlayPause;
        private GpioPin buttonCue;
        private GpioPin buttonNext;
        private GpioPin buttonBack;
        private GpioPin buttonSearchPlus;
        private GpioPin buttonSearchMinus;
        private GpioPin buttonJet;
        private GpioPin buttonZip;
        private GpioPin buttonWah;
        private GpioPin buttonHold;
        private GpioPin buttonAutoCue;
        private GpioPin buttonRemoveDisc;
        private GpioPin buttonTempo;
        private GpioPin buttonBrowserEncoder;
        // LED's
        private GpioPin ledPlayPause;
        private GpioPin ledCue;
        // ADS Alert
        private GpioPin alertADS;
        // JOG ROTARY ENCODER
        private GpioPin jogA;
        private GpioPin jogB;
        // BROWSER ENCODER
        private GpioPin encoderA;
        private GpioPin encoderB;

        #endregion

        private GpioController GPIO;
        private GPIO_PINS schemeGPIO;
        private bool holdSearchButton;

        public void InitializeAndOpenAllPinsForPlayer()
        {
            // Initiazlize GPIO
            var GPIO = GpioController.GetDefault();
            schemeGPIO = new GPIO_PINS();

            // If GPIO no connected
            if (GPIO == null)
            {
                return;
            }

            TimeSpan timeSpan = new TimeSpan();
            timeSpan = TimeSpan.FromMilliseconds(10);

            // BUTTONS
            buttonPlayPause = GPIO.OpenPin(schemeGPIO.PLAY_PAUSE_BUTTON_PIN);
            buttonPlayPause.DebounceTimeout = TimeSpan.FromMilliseconds(5);
            if (buttonPlayPause.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonPlayPause.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonPlayPause.SetDriveMode(GpioPinDriveMode.Input);
            buttonPlayPause.ValueChanged += ButtonPlayPause_ValueChanged;

            buttonCue = GPIO.OpenPin(schemeGPIO.CUE_BUTTON_PIN);
            buttonCue.DebounceTimeout = TimeSpan.FromMilliseconds(5);
            if (buttonCue.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonCue.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonCue.SetDriveMode(GpioPinDriveMode.Input);
            buttonCue.ValueChanged += ButtonCue_ValueChanged;

            buttonNext = GPIO.OpenPin(schemeGPIO.NEXT_BUTTON_PIN);
            buttonNext.DebounceTimeout = timeSpan;
            if (buttonNext.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonNext.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonNext.SetDriveMode(GpioPinDriveMode.Input);
            buttonNext.ValueChanged += ButtonNext_ValueChanged;

            buttonBack = GPIO.OpenPin(schemeGPIO.BACK_BUTTON_PIN);
            buttonBack.DebounceTimeout = timeSpan;
            if (buttonBack.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonBack.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonBack.SetDriveMode(GpioPinDriveMode.Input);
            buttonBack.ValueChanged += ButtonBack_ValueChanged;

            buttonSearchPlus = GPIO.OpenPin(schemeGPIO.SEARCH_PLUS_BUTTON_PIN);
            buttonSearchPlus.DebounceTimeout = timeSpan;
            if (buttonSearchPlus.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonSearchPlus.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonSearchPlus.SetDriveMode(GpioPinDriveMode.Input);
            buttonSearchPlus.ValueChanged += ButtonSearchPlus_ValueChanged;

            buttonSearchMinus = GPIO.OpenPin(schemeGPIO.SEARCH_MINUS_BUTTON_PIN);
            buttonSearchMinus.DebounceTimeout = timeSpan;
            if (buttonSearchMinus.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonSearchMinus.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonSearchMinus.SetDriveMode(GpioPinDriveMode.Input);
            buttonSearchMinus.ValueChanged += ButtonSearchMinus_ValueChanged;

            buttonJet = GPIO.OpenPin(schemeGPIO.JET_BUTTON_PIN);
            buttonJet.DebounceTimeout = timeSpan;
            if (buttonJet.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonJet.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonJet.SetDriveMode(GpioPinDriveMode.Input);
            buttonJet.ValueChanged += ButtonJet_ValueChanged;

            buttonWah = GPIO.OpenPin(schemeGPIO.WAH_BUTTON_PIN);
            buttonWah.DebounceTimeout = timeSpan;
            if (buttonWah.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonWah.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonWah.SetDriveMode(GpioPinDriveMode.Input);
            buttonWah.ValueChanged += ButtonWah_ValueChanged;

            buttonZip = GPIO.OpenPin(schemeGPIO.ZIP_BUTTON_PIN);
            buttonZip.DebounceTimeout = timeSpan;
            if (buttonZip.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonZip.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonZip.SetDriveMode(GpioPinDriveMode.Input);
            buttonZip.ValueChanged += ButtonZip_ValueChanged;

            buttonHold = GPIO.OpenPin(schemeGPIO.HOLD_BUTTON);
            buttonHold.DebounceTimeout = timeSpan;
            if (buttonHold.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonHold.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonHold.SetDriveMode(GpioPinDriveMode.Input);
            buttonHold.ValueChanged += ButtonHold_ValueChanged;

            buttonRemoveDisc = GPIO.OpenPin(schemeGPIO.REMOVE_DISC_BUTTON_PIN);
            buttonRemoveDisc.DebounceTimeout = timeSpan;
            if (buttonRemoveDisc.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonRemoveDisc.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonRemoveDisc.SetDriveMode(GpioPinDriveMode.Input);
            buttonRemoveDisc.ValueChanged += ButtonRemoveDisc_ValueChanged;

            buttonAutoCue = GPIO.OpenPin(schemeGPIO.AUTOCUE_BUTTON_PIN);
            buttonAutoCue.DebounceTimeout = timeSpan;
            if (buttonAutoCue.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonAutoCue.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonAutoCue.SetDriveMode(GpioPinDriveMode.Input);
            buttonAutoCue.ValueChanged += ButtonAutoCue_ValueChanged;

            buttonTempo = GPIO.OpenPin(schemeGPIO.TEMPO_BUTTON_PIN);
            buttonTempo.DebounceTimeout = timeSpan;
            if (buttonTempo.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonTempo.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonTempo.SetDriveMode(GpioPinDriveMode.Input);
            buttonTempo.ValueChanged += ButtonTempo_ValueChanged;

            //buttonBrowserEncoder = GPIO.OpenPin(schemeGPIO.BTN_BROWSER_ENCODER_PIN);
            //buttonBrowserEncoder.DebounceTimeout = timeSpan;
            //if (buttonBrowserEncoder.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            //    buttonBrowserEncoder.SetDriveMode(GpioPinDriveMode.InputPullUp);
            //else
            //    buttonBrowserEncoder.SetDriveMode(GpioPinDriveMode.Input);
            //buttonBrowserEncoder.ValueChanged += buttonBrowserEncoder_ValueChanged;

            // LED's
            ledPlayPause = GPIO.OpenPin(schemeGPIO.PLAY_PAUSE_LED_PIN);
            ledPlayPause.Write(GpioPinValue.High);
            ledPlayPause.SetDriveMode(GpioPinDriveMode.Output);
            ledCue = GPIO.OpenPin(schemeGPIO.CUE_LED_PIN);
            ledCue.Write(GpioPinValue.High);
            ledCue.SetDriveMode(GpioPinDriveMode.Output);

            // JOG
            jogA = GPIO.OpenPin(schemeGPIO.DT_JOG_PIN);
            jogA.SetDriveMode(GpioPinDriveMode.InputPullUp);
            jogA.ValueChanged += EncoderRotary_ValueChange;

            jogB = GPIO.OpenPin(schemeGPIO.CLK_JOG_PIN);
            jogB.SetDriveMode(GpioPinDriveMode.InputPullUp);
            jogB.ValueChanged += EncoderRotary_ValueChange;

            // BROWSER ENCODER
            //encoderA = GPIO.OpenPin(schemeGPIO.DT_BROWSER_ENCODER_PIN);
            //encoderA.SetDriveMode(GpioPinDriveMode.InputPullUp);
            //encoderA.ValueChanged += BrowserEncoderRotary_ValueChange;

            //encoderB = GPIO.OpenPin(schemeGPIO.CLK_BROWSER_ENCODER_PIN);
            //encoderB.SetDriveMode(GpioPinDriveMode.InputPullUp);
            //encoderB.ValueChanged += BrowserEncoderRotary_ValueChange;

        }

        #endregion

        #region GPIO Evnts for buttons 

        bool playPressed = false, cuePressed = false;

        // Button PLAY/PAUSE
        private void ButtonPlayPause_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    if (!cuePressed)
                        _playerView.PlayPause();

                    playPressed = true;
                }
                else
                {
                    playPressed = false;
                }
            });
        }

        // Button CUE
        private void ButtonCue_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _playerView.CuePressed();
                    cuePressed = true;
                }
                else
                {
                    if (!playPressed)
                        _playerView.CueReleased();
                    cuePressed = false;
                }
            });
        }

        // Buuton NEXT SONG
        private void ButtonNext_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _filesBrowserView.LoadNextFile(); // sprawdzić bez async
                }
            });
        }

        // Button BACK SONG
        private void ButtonBack_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _filesBrowserView.LoadPreviousFile();
                }
            });
        }

        private void ButtonSearchPlus_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                _playerView.Seek(TimeSpan.FromSeconds(5));
            }
        }

        private void ButtonSearchMinus_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                _playerView.Seek(TimeSpan.FromSeconds(-5));
            }
        }

        // BACK Button
        private void ButtonRemoveDisc_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _filesBrowserView.BackFolder();
                }
            });
        }

        // EFX BUTTON 1
        private void ButtonJet_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _effectsView.PressButtonA();
                }
            });
        }

        // EFX BUTTON 2
        private void ButtonZip_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _effectsView.PressButtonB();
                }
            });
        }

        // EFX BUTTON 3
        private void ButtonWah_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _effectsView.PressButtonC();
                }
            });
        }

        // EFX MODE
        DispatcherTimer timerModeView = new DispatcherTimer();
        private void ButtonHold_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                timerModeView.Interval = TimeSpan.FromMilliseconds(1000);
                timerModeView.Tick += timerModeView_Tick;

                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    timerModeView.Start();
                    switch (_filesBrowserView._browserViewMode)
                    {
                        case BrowserViewMode.FullView:
                            _filesBrowserView.ChangeViewMode(BrowserViewMode.FullViewAndInfo);
                            _effectsView.Show(false);
                            break;
                        case BrowserViewMode.FullViewAndInfo:
                            _filesBrowserView.ChangeViewMode(BrowserViewMode.SmallView);
                            _effectsView.Show(true);
                            break;
                        case BrowserViewMode.SmallView:
                            _filesBrowserView.ChangeViewMode(BrowserViewMode.FullView);
                            _effectsView.Show(false);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    timerModeView.Stop();
                }
            });
        }
        private void timerModeView_Tick(object sender, object e)
        {
            _effectsView.ChangeMode();
        }

        // AUTO CUE 
        DispatcherTimer timerAutoCue = new DispatcherTimer();
        private void ButtonAutoCue_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                timerAutoCue.Interval = TimeSpan.FromMilliseconds(2000);
                timerAutoCue.Tick += timerAutoCue_Tick;

                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    timerAutoCue.Start();
                    if (_playerView._timeMode == TimeMode.Remaining)
                        _playerView._timeMode = TimeMode.Elapsed;
                    else
                        _playerView._timeMode = TimeMode.Remaining;
                }
                else
                {
                    timerAutoCue.Stop();
                }
            });
        }

        private void timerAutoCue_Tick(object sender, object e)
        {
            _playerView.SetAutoCue();
            timerAutoCue.Stop();
        }

        // MASTER TEMPO
        private void ButtonTempo_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _playerView.ChangeTempoRange();
                }
            });
        }

        #endregion

        #region Rotary encoder JOG

        private GpioPinValue encoderALast = GpioPinValue.Low;

        private void EncoderRotary_ValueChange(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            GpioPinValue encoderA = jogA.Read();

            if ((encoderALast == GpioPinValue.High) && (encoderA == GpioPinValue.Low))
            {
                if (jogB.Read() == GpioPinValue.Low)
                {
                    EncoderTurnRight();
                }
                else
                {
                    EncoderTurnLeft();
                }
            }
            encoderALast = encoderA;
        }

        int countL = 0;

        private void EncoderTurnLeft()
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (holdSearchButton)
                {
                    _playerView.Rewind(TimeSpan.FromSeconds(-Settings.jogSeekSpeed));
                }
                else if (_playerView.PlayerState == PlayerState.Playing && !_playerView.PauseSeek)
                {
                    _playerView.jogTempo(-Settings.jogTempo);
                }
                else if (_playerView.PlayerState == PlayerState.Paused && _playerView.PauseSeek)
                {
                    // SEARCH CUE MODE
                    countL++;
                    if (countL > Settings.intervalEncoderInSeek)
                    {
                        _playerView.setSeekPosition(JogTurn.Left);
                        countL = 0;
                    }
                }
                else if (_playerView.PlayerState == PlayerState.Paused)
                {
                    _playerView.Forward(TimeSpan.FromSeconds(-1));
                }
            });
        }

        int countR = 0;
        private void EncoderTurnRight()
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (holdSearchButton)
                {
                    _playerView.Forward(TimeSpan.FromSeconds(Settings.jogSeekSpeed));
                }
                else if (_playerView.PlayerState == PlayerState.Playing)
                {
                    _playerView.jogTempo(Settings.jogTempo);
                }
                else if (_playerView.PlayerState == PlayerState.Paused && _playerView.PauseSeek)
                {
                    // SEARCH CUE MODE
                    countR++;
                    if (countR > Settings.intervalEncoderInSeek)
                    {
                        _playerView.setSeekPosition(JogTurn.Right);
                        countR = 0;
                    }
                }
                else if (_playerView.PlayerState == PlayerState.Paused)
                {
                    _playerView.Forward(TimeSpan.FromSeconds(1));
                }
            });
        }

        #endregion

        #region Browser encoder

        private GpioPinValue encALast = GpioPinValue.Low;
        private void BrowserEncoderRotary_ValueChange(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            GpioPinValue A = encoderA.Read();

            if ((encALast == GpioPinValue.High) && (A == GpioPinValue.Low))
            {
                if (encoderB.Read() == GpioPinValue.Low)
                {
                    // Right 
                    _filesBrowserView.ScrollDownFileBrowserList();
                }
                else
                {
                    // Left
                    _filesBrowserView.ScrollUpFileBrowserList();
                }
            }
            encALast = A;
        }

        // BROWSER ENCODER BUTTON
        private void buttonBrowserEncoder_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    _filesBrowserView.LoadSelectedItem();
                }
            });
        }

        #endregion

        #region I2c Tempo Value

        I2cDevice i2CDevice;

        DispatcherTimer timerI2c = new DispatcherTimer();

        // Get PITCH value timer
        public async void ConnectToI2cInterfacePitch()
        {
            // Initializing the ADS 1115
            var i2CSettings = new I2cConnectionSettings(0x48)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Shared
            };

            var i2C1 = I2cDevice.GetDeviceSelector("I2C1");

            var devices = await DeviceInformation.FindAllAsync(i2C1);

            i2CDevice = await I2cDevice.FromIdAsync(devices[0].Id, i2CSettings);

            // Write in the config register. 0xc4 0Xe0 = ‭1100010011100000‬
            // = listen to A0, default gain, continuous conversion, 860 SPS, assert after one conversion (= READY signal) 
            // see http://www.adafruit.com/datasheets/ads1115.pdf p18 for details
            // Ecrit dans le registre config. 0xc4 0Xe0 = ‭1100010011100000‬
            // = ecouter A0, gain par défaut, conversion continue, 860 SPS, signal READY après chaque conversion 
            // see http://www.adafruit.com/datasheets/ads1115.pdf p18 for details
            i2CDevice.Write(new byte[] { 0x01, 0xc0, 0xe0 }); // zmiana 010 na 000 0xc0 +- 6v , 0xc4 +-2v
                                                              // Configure the Lo_thresh (0x02) and Hi_Thresh (0x03) registers so the READY signal will be sent
                                                              // Configure les registres Lo_thresh (0x02) et Hi_Thresh (0x03) pour que le signal READY soit envoyé
            i2CDevice.Write(new byte[] { 0x02, 0x00, 0x00 });
            i2CDevice.Write(new byte[] { 0x03, 0xff, 0xff });

            // Get times
            timerI2c.Interval = TimeSpan.FromMilliseconds(500);
            timerI2c.Tick += timerI2c_Tick;
            timerI2c.Start();

            //alertADS = GPIO.OpenPin(schemeGPIO.ADS_ALERT_PIN);
            //alertADS.ValueChanged += AlertADS_ValueChanged;
        }

        double currentVoltage, rate, volt, temp, maxVoltage = 5.00, zeroVoltage = 2.50, minVoltage = 0.0; // zero - 2.53 V
        int value;
        byte[] bytearray = new byte[2];

        private void timerI2c_Tick(object sender, object e)
        {
            // Read conversion register
            i2CDevice.WriteRead(new byte[] { 0x0 }, bytearray);

            // Convert to int16
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytearray);

            value = BitConverter.ToInt16(bytearray, 0);

            // Voltage = (value * gain)/372767
            volt = (value * 6.114) / 32767.0;

            // Temperature = (volt - 0.5) * 100
            temp = (volt - .5) * 100;

            var voltToTransform = Math.Round(Double.Parse(string.Format("{0:.00}", volt)), 2);

            var tempoRangeValue = 10;

            switch (_playerView.GetTempoRangeValue)
            {
                case TempoRange.PlusMinus10:
                    tempoRangeValue = 10;
                    break;
                case TempoRange.PlusMinus16:
                    tempoRangeValue = 16;
                    break;
                case TempoRange.PlusMinus100:
                    tempoRangeValue = 100;
                    break;
                default:
                    break;
            }

            // Change TEMPO if change voltage
            if (voltToTransform != currentVoltage)
            {
                if (voltToTransform < zeroVoltage)
                {
                    voltToTransform = -(tempoRangeValue - ((voltToTransform * tempoRangeValue) / zeroVoltage));
                    rate = voltToTransform / 100;
                }
                else if (voltToTransform == zeroVoltage || voltToTransform == 2.48 || voltToTransform == 2.50)
                {
                    voltToTransform = 0;
                    rate = 0;
                }
                else if (voltToTransform > zeroVoltage)
                {
                    voltToTransform = (((voltToTransform) / (maxVoltage / 2)) * tempoRangeValue) - tempoRangeValue;
                    rate = voltToTransform / 100;
                }

                _playerView.setTempoValue(voltToTransform, rate);
                currentVoltage = voltToTransform;
            }
        }
        #endregion
    }
}