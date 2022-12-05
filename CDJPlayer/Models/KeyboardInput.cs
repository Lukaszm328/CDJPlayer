using CDJPlayer.Views;
using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using CDJPlayer.Constants;

namespace CDJPlayer.Models
{
    internal class KeyboardInput
    {
        private PlayerView _playerView;
        private FilesBrowserView _filesBrowserView;
        private SettingsView _settings;
        private EffectsViewFull _effectsViewFull;

        public KeyboardInput(PlayerView playerView, FilesBrowserView filesBrowserView, EffectsViewFull effectsViewFull, SettingsView settings)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            this._playerView = playerView;
            this._filesBrowserView = filesBrowserView;
            this._settings = settings;
            this._effectsViewFull = effectsViewFull;
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            CoreVirtualKeyStates keyState = sender.GetAsyncKeyState(args.VirtualKey);
            if ((int)keyState == 3)
            {
                // KeyState is locked and pressed.
                // Whenever this happens it means the event fired when I actaually pressed this key.
                // Handle event.
            }
            else if ((int)keyState == 2)
            {
                // KeyState is locked but not pressed. How if it's not caps lock?
                // When this happens it's an unwanted duplicate of the last keystroke.
                // Do not handle event.
            }
            else if ((int)keyState == 0)
            {
                if (args.VirtualKey == VirtualKey.C)
                {
                    _playerView.CueReleased();
                }
                // Key state is None?!? How can a key that isn't currently down fire a KeyDown event?
                // This is a phantom delayed rection of a missed event from two keystrokes ago.
                // Do not handle event.
            }
        }

        private async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            CoreVirtualKeyStates keyState = sender.GetAsyncKeyState(args.VirtualKey);

            if ((int)keyState == 3)
            {
                if (args.VirtualKey == VirtualKey.Number1)
                {
                    _effectsViewFull.PressButtonA();   
                }                

                if (args.VirtualKey == VirtualKey.Number2)
                {
                    _effectsViewFull.PressButtonB();   
                }

                if (args.VirtualKey == VirtualKey.Number3)
                {
                    _effectsViewFull.PressButtonC();   
                }

                if (args.VirtualKey == VirtualKey.Number4)
                {
                    _effectsViewFull.ChangeMode();
                }

                if (args.VirtualKey == VirtualKey.Number5)
                {
                    _filesBrowserView.ChangeViewMode(BrowserViewMode.FullView);
                    _effectsViewFull.Show(false);
                }

                if (args.VirtualKey == VirtualKey.Number6)
                {
                    _filesBrowserView.ChangeViewMode(BrowserViewMode.FullViewAndInfo);
                    _effectsViewFull.Show(false);
                }

                if (args.VirtualKey == VirtualKey.Number7)
                {
                    _filesBrowserView.ChangeViewMode(BrowserViewMode.SmallView);
                    _effectsViewFull.Visibility = Visibility.Visible;
                    _effectsViewFull.Show(true);
                }

                if (args.VirtualKey == VirtualKey.W)
                {
                    await _filesBrowserView.LoadNextFile();
                }

                if (args.VirtualKey == VirtualKey.Tab)
                {
                    _settings.Show();
                }

                if (args.VirtualKey == VirtualKey.Q)
                {
                    await _filesBrowserView.LoadPreviousFile();
                }

                if (args.VirtualKey == VirtualKey.C)
                {
                    _playerView.CuePressed();
                }

                if (args.VirtualKey == VirtualKey.P)
                {
                    _playerView.PlayPause();
                }
                if (args.VirtualKey == VirtualKey.U)
                {
                    _filesBrowserView.BackFolder();
                }
                if (args.VirtualKey == VirtualKey.M)
                {
                    _playerView.Forward(TimeSpan.FromMilliseconds(-10));
                }
                if (args.VirtualKey == VirtualKey.N)
                {
                   _playerView.Forward(TimeSpan.FromMilliseconds(10));
                }
                if (args.VirtualKey == VirtualKey.H)
                {
                    _filesBrowserView.ScrollUpFileBrowserList();
                }
                if (args.VirtualKey == VirtualKey.Y)
                {
                    if (_playerView._timeMode == TimeMode.Remaining)
                        _playerView._timeMode = TimeMode.Elapsed;
                    else
                        _playerView._timeMode = TimeMode.Remaining;
                }
                if (args.VirtualKey == VirtualKey.J)
                {
                    _filesBrowserView.ScrollDownFileBrowserList();
                }
                if (args.VirtualKey == VirtualKey.A)
                {
                    _playerView.ChangeTempoRange();
                }
                if (args.VirtualKey == VirtualKey.F)
                {
                    _filesBrowserView.ChangeMusicInfoVisibility();
                }
                if (args.VirtualKey == VirtualKey.Z)
                {
                    if (_playerView.PlayerState == PlayerState.Playing && !_playerView.PauseSeek)
                    {
                        _playerView.AddTempo(-Settings.jogTempo);
                    }
                    else if (_playerView.PlayerState == PlayerState.Paused || _playerView.PauseSeek)
                    {
                        _playerView.Forward(TimeSpan.FromMilliseconds(-Settings.jogSeekSpeed));
                    }
                }
                if (args.VirtualKey == VirtualKey.X)
                {
                    if (_playerView.PlayerState == PlayerState.Playing && !_playerView.PauseSeek)
                    {
                        _playerView.AddTempo(Settings.jogTempo);
                    }
                    else if (_playerView.PlayerState == PlayerState.Paused || _playerView.PauseSeek)
                    {
                        _playerView.Forward(TimeSpan.FromMilliseconds(Settings.jogSeekSpeed));
                    }
                }
                // KeyState is locked and pressed.
                // Whenever this happens it means the event fired when I actaually pressed this key.
                // Handle event.
            }
            //else if ((int)keyState == 2)
            //{
            //    // KeyState is locked but not pressed. How if it's not caps lock?
            //    // When this happens it's an unwanted duplicate of the last keystroke.
            //    // Do not handle event.
               
            //}
            //else if ((int)keyState == 0)
            //{
            //    // Key state is None?!? How can a key that isn't currently down fire a KeyDown event?
            //    // This is a phantom delayed rection of a missed event from two keystrokes ago.
            //    // Do not handle event.
            //    if (args.VirtualKey == VirtualKey.X)
            //    {
            //        if (_playerView.PlayerState == PlayerState.Playing && !_playerView.PauseSeek)
            //        {
            //            _playerView.AddTempo(-Settings.jogSeekSpeed);
            //        }
            //    }

            //    if (args.VirtualKey == VirtualKey.Z)
            //    {
            //        if (_playerView.PlayerState == PlayerState.Playing && !_playerView.PauseSeek)
            //        {
            //            _playerView.AddTempo(Settings.jogSeekSpeed);
            //        }
            //    }
            //}
        }
    }
}
