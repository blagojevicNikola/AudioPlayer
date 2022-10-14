using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioPlayer
{
    public class AddSongWindowViewModel:INotifyPropertyChanged
    {
        private string? _SongPath;
        private string? _SongName;
        private string? _PlayerName;
        public string? SongPath { get { return _SongPath; } set { _SongPath = value; NotifyPropertyChanged("SongPath"); } }
        public string? SongName { get { return _SongName; } set { _SongName = value; NotifyPropertyChanged("SongName"); } }
        public string? PlayerName { get { return _PlayerName; } set { _PlayerName = value; NotifyPropertyChanged("PlayerName"); } }
        public ICommand BrowseCommand { get; set; }
        public ICommand OkCommand { get; set; }
        public EventHandler? OnCloseRequest { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public AddSongWindowViewModel()
        {
            BrowseCommand = new RelayCommand(browseSong);
            OkCommand = new RelayCommand(addSong);
        }

        private void browseSong()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog()==true)
            {
                SongPath = openFileDialog.FileName;
                string[] data = openFileDialog.SafeFileName.Split("\\");
                if(data.Length==0 || data.Length==1)
                {
                    SongName = openFileDialog.SafeFileName;
                    PlayerName = "Unknown";
                }
                else
                {
                    SongName = data[0];
                    PlayerName = data[1];
                }
            }
        }

        private void addSong()
        {
            if(infoIsValid() && OnCloseRequest!=null)
            {
                OnCloseRequest(this, EventArgs.Empty);
            }
        }

        private bool infoIsValid()
        {
            if (File.Exists(SongPath) && !string.IsNullOrEmpty(SongName) && !string.IsNullOrEmpty(PlayerName))
            {
                return true;
            }
            return false;
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));  
        }
    }
}
