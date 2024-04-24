using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WordGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MediaPlayer MediaPlayer = new MediaPlayer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string path = "Assets/audio/soundtrack.mp3";
            MediaPlayer.Open(new Uri(path, UriKind.Relative));
            MediaPlayer.Volume = WordGame.Properties.Settings.Default.SoundtrackVolume;
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            MediaPlayer.Play();
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer.Position = TimeSpan.Zero;
            MediaPlayer.Play();
        }
    }
}
