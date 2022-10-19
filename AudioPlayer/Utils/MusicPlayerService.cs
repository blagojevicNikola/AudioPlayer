using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioPlayer
{
    public class MusicPlayerService:INotifyPropertyChanged
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader? audioFile;
        private Song? _selectedSong;
        private string _endTime = "00:00";
        private string _currentTime = "00:00";
        private double _currentValue = 0;
        private double _maximumValue = 0;
        private DispatcherTimer _timer;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Song> List { get; set; } = new ObservableCollection<Song>();
        public Song? SelectedSong { get { return _selectedSong; } set { _selectedSong = value; NotifyPropertyChanged("SelectedSong"); } }
        public float Volume { get { return outputDevice.Volume; } set { outputDevice.Volume = value; NotifyPropertyChanged("Volume"); } }
        public string CurrentValueString { get { return _currentTime; } set { _currentTime = value; NotifyPropertyChanged("CurrentValueString"); } }
        public double CurrentValue { get { return _currentValue; } set { _currentValue = value; NotifyPropertyChanged("CurrentValue"); } }
        public double MaximumValue { get { return _maximumValue; } set { _maximumValue = value; NotifyPropertyChanged("MaximumValue"); } }
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
                NotifyPropertyChanged("EndValueString");
            }
        }
        public MusicPlayerService()
        {
            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += (sender, args) => stopOnEvent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.9);
            _timer.Tick += (sender, a) => updateTimeAndSlider();
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
                CurrentValue = 0;
                audioFile.Position = 0;
            }
            SelectedSong.IsPlaying = true;
            _timer.Start();
            outputDevice.Play();
        }

        public void Stop()
        {
            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                _timer.Stop();
                outputDevice.Stop();
            }
        }

        public void Pause()
        {
            if (outputDevice.PlaybackState != PlaybackState.Paused)
            {
                _timer.Stop();
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
            MaximumValue = audioFile.TotalTime.TotalSeconds;
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

        private void updateTimeAndSlider()
        {
            CurrentValueString = audioFile!.CurrentTime.ToString("mm\\:ss");
            CurrentValue += 1;
        }

        private void stopOnEvent()
        {
            _timer.Stop();
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
