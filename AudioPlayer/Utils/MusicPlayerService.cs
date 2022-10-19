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
        private Mp3FileReader? audioFile;
        private Song? _selectedSong;
        private string _endTime = "00:00";
        private string _currentTime = "00:00";
        private double _currentValue = 0;
        private double _maximumValue = 0;
        private DispatcherTimer _timer;
        private bool _isActive = false;
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
        public bool IsActive { get { return _isActive; } set { _isActive = value; NotifyPropertyChanged("IsActive"); } }

        public MusicPlayerService()
        {
            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += (sender, args) => stopOnEvent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.2);
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
            }
            SelectedSong.IsPlaying = true;
            _timer.Start();
            outputDevice.Play();
            IsActive = true;
        }

        public void Stop()
        {
            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                _timer.Stop();
                outputDevice.Stop();
                IsActive = false;
            }
        }

        public void Pause()
        {
            if (outputDevice.PlaybackState != PlaybackState.Paused)
            {
                _timer.Stop();
                outputDevice.Pause();
                IsActive = false;
            }
        }

        public void Open(Song song)
        {
            if (SelectedSong is not null)
            {
                if(SelectedSong.SongPath.Equals(song.SongPath))
                {
                    return;
                }
                SelectedSong.IsPlaying = false;
            }
            SelectedSong = song;
            outputDevice.Stop();
            _timer.Stop();
            CurrentValue = 0;
            audioFile = new Mp3FileReader(song.SongPath);
            CurrentValueString = "00:00";
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

        public void PlayPrevious()
        {
            if (SelectedSong is null)
            {
                return;
            }
            int temp = List.IndexOf(SelectedSong);
            if (temp == -1)
            {
                return;
            }
            if (temp - 1 >= 0)
            {
                Open(List[temp - 1]);
                Play();
            }
        }

        public void DeleteSong(Song song)
        {
            if (SelectedSong == song)
            {
                this.Stop();
                SelectedSong = null;
                EndValueString = "00:00";
                CurrentValueString = "00:00";
                CurrentValue = 0;
            }
            List.Remove(song);
        }

        private void updateTimeAndSlider()
        {
            CurrentValueString = audioFile!.CurrentTime.ToString("mm\\:ss");
            CurrentValue = audioFile.CurrentTime.TotalSeconds;
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
