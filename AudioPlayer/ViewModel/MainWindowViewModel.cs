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
        public event PropertyChangedEventHandler? PropertyChanged;
        public MusicPlayerService Service { get { return _service; } }
        public ObservableCollection<SongViewModel> Songs { get; set; } = new ObservableCollection<SongViewModel>();
        public ICommand AddSongCommand { get; set; }
        public ICommand MainCommand { get; set; }
        public ICommand NextSongCommand { get; set; }
        public ICommand PrevSongCommand { get; set; }
        public MainWindowViewModel()
        {
            AddSongCommand = new RelayCommand(addSong);
            MainCommand = new RelayCommand(() =>{});
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
                svm.SelectCommand = new RelayCommand(() => { _service.Open(svm.SongModel); _service.Play(); });
                Songs.Add(svm);
                Service.List.Add(s);
                win.Close(); 
            };
            win.Show();
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
