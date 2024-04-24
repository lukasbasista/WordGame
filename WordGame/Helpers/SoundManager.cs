using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WordGame.Helpers
{
    public static class SoundManager
    {
        private static MediaPlayer mediaPlayer = new MediaPlayer();

        public static void PlayClickSound()
        {
            string path = "Assets/audio/click2.mp3";
            mediaPlayer.Open(new Uri(path, UriKind.Relative));
            mediaPlayer.Volume = WordGame.Properties.Settings.Default.SoundEffectVolume;
            mediaPlayer.Play();
        }
        public static void PlayKickSound()
        {
            string path = "Assets/audio/ball.mp3";
            mediaPlayer.Open(new Uri(path, UriKind.Relative));
            mediaPlayer.Volume = WordGame.Properties.Settings.Default.SoundEffectVolume;
            mediaPlayer.Play();
        }
    }
}
