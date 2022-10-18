using NAudio.Utils;
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
        private string _endTime = "00:00";
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Song> List { get; set; } = new ObservableCollection<Song>();
        public Song? SelectedSong { get { return _selectedSong; } set { _selectedSong = value; NotifyPropertyChanged("SelectedSong"); } }
        public float Volume { get { return outputDevice.Volume; } set { outputDevice.Volume = value; NotifyPropertyChanged("Volume"); } }
        public string CurrentValueString { get {
                if (audioFile is null)
                {
                    return "00:00";
                }
                return audioFile.CurrentTime.ToString("mm\\:ss");
            } }
        public string EndValueString { 
            get
            {
                if (audioFile is null)
                {
                    return "00:00";
                }
                return _endTime;
            }
            set
            {
                _endTime = value;  
            }
        }
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
            if(audioFile is null)  
                return TimeSpan.Zero;
            return audioFile.CurrentTime;
        }

        public TimeSpan GetFullDuration()
        {
            if (audioFile is null)
                return TimeSpan.Zero;
            return audioFile.TotalTime;
        }

        public void Play()
        {
            if(audioFile is null || SelectedSong is null)
            {
                return;
            }
            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                audioFile.Position = 0;
            }
            SelectedSong.IsPlaying = true;
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
            if(SelectedSong is not null)
            {
                SelectedSong.IsPlaying = false;
            }
            SelectedSong = song;
            outputDevice.Stop();
            audioFile = new AudioFileReader(song.SongPath);
            EndValueString = audioFile.TotalTime.ToString("mm\\:ss");
            outputDevice.Init(audioFile);
        }

        public void PlayNext()
        {
            if(SelectedSong is null)
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
                Open(List[temp + 1]);
                Play();
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
