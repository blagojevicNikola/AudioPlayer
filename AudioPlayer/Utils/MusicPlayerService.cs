using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AudioPlayer
{
    public class MusicPlayerService:INotifyPropertyChanged
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader? audioFile;
        private Song? _selectedSong;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Song> List { get; set; } = new ObservableCollection<Song>();
        public Song? SelectedSong { get { return _selectedSong; } set { _selectedSong = value; NotifyPropertyChanged("SelectedSong"); } }
        public MusicPlayerService()
        {
            outputDevice = new WaveOutEvent();
        }

        public double GetVolume()
        {
            return outputDevice.Volume;
        }

        public TimeSpan GetPosition()
        {
            if(audioFile == null)  
                return TimeSpan.Zero;
            return audioFile.CurrentTime;
        }

        public TimeSpan GetFullDuration()
        {
            if (audioFile == null)
                return TimeSpan.Zero;
            return audioFile.TotalTime;
        }

        public void Play()
        {
            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                audioFile.Position = 0;
            }
            outputDevice.Play();

        }

        public void Stop()
        {
            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                outputDevice.Stop();
            }
        }

        public void Pause()
        {
            if (outputDevice.PlaybackState != PlaybackState.Paused)
            {
                outputDevice.Pause();
            }
        }

        public void Open(Song song)
        {
            if (audioFile != null && audioFile.FileName.Equals(song.SongPath))
            {
                return;
            }

            outputDevice.Stop();
            audioFile = new AudioFileReader(song.SongPath);
            outputDevice.Init(audioFile);
        }

        public void PlayNext()
        {
            if(SelectedSong==null)
            {
                return;
            }
            int temp = List.IndexOf(SelectedSong);
            if(temp == -1)
            {
                return;
            }
            if(temp+1<=List.Count-1)
            {
                SelectedSong = List[temp+1];
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
