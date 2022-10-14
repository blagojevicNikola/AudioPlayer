using AudioPlayer.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioPlayer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Song> Playlist { get; set; }
        public ICommand AddSongCommand { get; set; }
        public ICommand PlaySongCommand { get; set; }
        public ICommand NextSongCommand { get; set; }
        public ICommand PrevSongCommand { get; set; }
        public MainWindowViewModel()
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            songs.Add(new Song("putanja", "prva pjesma", "izvodjac"));
            songs.Add(new Song("putanja", "druga pjesma", "izvodjac"));
            songs.Add(new Song("putanja", "treca pjesma", "izvodjac"));
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
            vm.OnCloseRequest += (a, b) => { Playlist.Add(new Song(vm.SongPath, vm.SongName, vm.PlayerName)); win.Close(); };
            win.Show();
        }

    }
}
