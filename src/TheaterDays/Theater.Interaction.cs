using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.TheaterDays {
    partial class Theater {

        private void KeyboardStateHandler_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Space:
                    TogglePlayState();
                    break;
            }
        }

        private void KeyboardStateHandler_KeyUp(object sender, KeyEventArgs e) {
        }

        private void BackgroundMedia_PlaybackStopped(object sender, System.EventArgs e) {
            SetIsPlaying(false, stop: true);
        }

        private void TogglePlayState() {
            SetIsPlaying(!_isPlaying, stop: false);
        }

        private void SetIsPlaying(bool isPlaying, bool stop) {
            var bga = this.FindSingleElement<BackgroundVideo>();
            var audio = this.FindSingleElement<AudioController>();

            _isPlaying = isPlaying;

            if (!isPlaying) {
                if (stop) {
                    bga?.Stop();
                    audio?.Music?.Source.Stop();
                } else {
                    bga?.Pause();
                    audio?.Music?.Source.Pause();
                }
            } else {
                if (bga != null) {
                    if (bga.State == MediaState.Stopped) {
                        bga.Play();
                    } else {
                        bga.Resume();
                    }
                }
                audio?.Music?.Source.PlayDirect();
            }

            UpdateStateDisplay();
        }

        private void UpdateStateDisplay() {
            var isPlaying = _isPlaying;

            var helpOverlay = this.FindSingleElement<HelpOverlay>();

            if (helpOverlay != null) {
                helpOverlay.Visible = !isPlaying;
            }
        }

        private bool _isPlaying;

    }
}
