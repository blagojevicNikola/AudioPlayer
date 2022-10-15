using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AudioPlayer
{
    public class MusicPlayerService
    {
        MediaPlayer _mediaPlayer;

        public MusicPlayerService()
        {
            _mediaPlayer = new MediaPlayer();
        }

        public double GetVolume()
        {
            return _mediaPlayer.Volume;
        }

        public TimeSpan GetPosition()
        {
            return _mediaPlayer.Position;
        }

        public Duration GetFullDuration()
        {
            return _mediaPlayer.NaturalDuration;
        }

        public void Play()
        {
            _mediaPlayer.Play();
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
        }

        public void Open(string songPath)
        {
            if(_mediaPlayer.Source==null || !_mediaPlayer.Source.AbsolutePath.Equals(songPath))
            {
                _mediaPlayer.Open(new Uri(songPath));
            }
        }

        public void OnLoaded(Action a)
        {
            _mediaPlayer.MediaOpened += (s, t) => a();
        }
    }
}
