﻿using AudioPlayer.View;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AudioPlayer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MusicPlayerService _service = new MusicPlayerService();
        private string _BeginTime = "--:--";
        private string _EndTime = "--:--";
        private SongViewModel? _songViewModel;
        private DispatcherTimer timer;
        private double _currentDuration = 0;
        private double _maximumDuration = 1;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<SongViewModel> Playlist { get; set; }
        public string BeginTime { get { return _BeginTime; } set { _BeginTime = value; NotifyPropertyChanged("BeginTime"); } }
        public string EndTime { get { return _EndTime; } set { _EndTime = value; NotifyPropertyChanged("EndTime"); } }
        public double CurrentDuration { get { return _currentDuration; } set { _currentDuration = value; NotifyPropertyChanged("CurrentDuration"); } }
        public double MaximumDuration { get { return _maximumDuration; } set { _maximumDuration = value; NotifyPropertyChanged("MaximumDuration"); } }
        public SongViewModel? SelectedSong
        {
            get { return _songViewModel; }
            set { 
                _songViewModel = value; 
                NotifyPropertyChanged("SelectedSong");
                if(_songViewModel!= null && _songViewModel.SongPath!=null)
                {
                    timer.Start();
                }
            }
        }
        public ICommand AddSongCommand { get; set; }
        public ICommand PlaySongCommand { get; set; }
        public ICommand NextSongCommand { get; set; }
        public ICommand PrevSongCommand { get; set; }
        public MainWindowViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, a) => updateTime();
            ObservableCollection<SongViewModel> songs = new ObservableCollection<SongViewModel>();
            songs.Add(new SongViewModel(new Song("putanja", "prva pjesma", "izvodjac")));
            songs.Add(new SongViewModel(new Song("putanja", "druga pjesma", "izvodjac")));
            songs.Add(new SongViewModel(new Song("putanja", "treca pjesma", "izvodjac")));
            Playlist = songs;
            AddSongCommand = new RelayCommand(addSong);
            PlaySongCommand = new RelayCommand(() => { });
            NextSongCommand = new RelayCommand(() => { });
            PrevSongCommand = new RelayCommand(() => { });
        }

        private void addSong()
        {
            AddSongWindowViewModel vm = new AddSongWindowViewModel();
            AddSongWindow win = new AddSongWindow();
            win.DataContext = vm;   
            vm.OnCloseRequest += (a, b) => {
                Song s = new Song(vm.SongPath, vm.SongName, vm.PlayerName);
                SongViewModel svm = new SongViewModel(s);
                svm.SelectCommand = new RelayCommand(() => {
                    SelectedSong = svm;
                    _service.Open(svm.SongPath);
                    loadTime();
                    playSong();
                });
                Playlist.Add(svm); 
                win.Close(); 
            };
            win.Show();
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void loadTime()
        {
            BeginTime = "00:00";
            CurrentDuration = 0;
            MaximumDuration = _service.GetFullDuration().TotalSeconds;
            EndTime = _service.GetFullDuration().Minutes.ToString().PadLeft(2, '0') + ":" + _service.GetFullDuration().Seconds.ToString().PadLeft(2, '0');

        }

        private void updateTime()
        {
            CurrentDuration = _service.GetPosition().TotalSeconds;
        }

        private void playSong()
        {
            _service.Play();
        }

    }
}
