using AudioPlayer.View;
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

namespace AudioPlayer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MusicPlayerService _playerService;
        private string _BeginTime = "--:--";
        private string _EndTime = "--:--";

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<SongViewModel> Playlist { get; set; }
        public string SelectedSongName { get; set; } = "";
        public string BeginTime { get { return _BeginTime; } set { _BeginTime = value; NotifyPropertyChanged("BeginTime"); } }
        public string EndTime { get { return _EndTime; } set { _EndTime = value; NotifyPropertyChanged("EndTime"); } }
        public ICommand AddSongCommand { get; set; }
        public ICommand PlaySongCommand { get; set; }
        public ICommand NextSongCommand { get; set; }
        public ICommand PrevSongCommand { get; set; }
        public MainWindowViewModel()
        {
            _playerService = new MusicPlayerService();
            _playerService.OnLoaded(loadTime);
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
                    _playerService.Open(svm.SongPath);
                    _playerService.Play();
                });
                Playlist.Add(svm); win.Close(); 
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
            Duration dur = _playerService.GetFullDuration();
            EndTime = dur.TimeSpan.Minutes.ToString().PadLeft(2,'0') + ":" + dur.TimeSpan.Seconds.ToString().PadLeft(2, '0'); 
        }

    }
}
