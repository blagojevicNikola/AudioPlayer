using AudioPlayer.View;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AudioPlayer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MusicPlayerService _service;
        public event PropertyChangedEventHandler? PropertyChanged;
        public MusicPlayerService Service { get { return _service; } private set { _service = value; } }
        public ObservableCollection<SongViewModel> Songs { get; set; } = new ObservableCollection<SongViewModel>();
        public bool IsBeingDragged { get; set; }
        public ICommand AddSongCommand { get; set; }
        public ICommand MainCommand { get; set; }
        public ICommand NextSongCommand { get; set; }
        public ICommand PrevSongCommand { get; set; }
        public ICommand UpdatePositionCommand { get; set; }
        public ICommand PauseUpdateCommand { get; set; }
        public ICommand ResumeUpdateCommand { get; set; }
        public MainWindowViewModel()
        {
            AddSongCommand = new RelayCommand(addSong);
            MainCommand = new RelayCommand(pausePlay);
            NextSongCommand = new RelayCommand(nextSong);
            PrevSongCommand = new RelayCommand(previousSong);
            UpdatePositionCommand = new RelayCommand(updatePosition);
            PauseUpdateCommand = new RelayCommand(pauseUpdate);
            ResumeUpdateCommand = new RelayCommand(resumeUpdate);
            //string currDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<string> fajlovi = Directory.GetFiles("C:\\Users\\nikol\\source\\repos\\AudioPlayer\\AudioPlayer\\bin\\Debug\\net6.0-windows").ToList();
            if (fajlovi.Any(s => s.EndsWith("plejer.json")))
            {
                string content = File.ReadAllText("plejer.json");
                MusicPlayerService? deserializedPlayer = JsonSerializer.Deserialize<MusicPlayerService>(content);
                if (deserializedPlayer != null)
                {   
                    if (deserializedPlayer.IsActive)
                    {
                        deserializedPlayer.IsActive = false;
                    }
                    if (deserializedPlayer.SelectedSong != null)
                    {
                        Song? temp = deserializedPlayer.List.ToList().Find(s => s.IsPlaying == true);
                        if(temp is not null)
                        {
                            deserializedPlayer.SelectedSong = null;
                            deserializedPlayer.Open(temp);
                        }
                        Service = deserializedPlayer;
                    }
                    _service = deserializedPlayer;
                    foreach (Song s in Service.List)
                    {
                        SongViewModel svm = new SongViewModel(s);
                        svm.SelectCommand = new RelayCommand(() => { Service.Open(svm.SongModel); Service.Play(); });
                        svm.DeleteCommand = new RelayCommand(() => { Service.DeleteSong(s); Songs.Remove(svm); });
                        Songs.Add(svm);
                    }
                }
            }
            else
            {
                _service = new MusicPlayerService();
            }
        }

        private void addSong()
        {
            AddSongWindowViewModel vm = new AddSongWindowViewModel();
            AddSongWindow win = new AddSongWindow();
            win.DataContext = vm;   
            vm.OnCloseRequest += (a, b) => {
                Song s = new Song(vm.SongPath, vm.SongName, vm.PlayerName);
                SongViewModel svm = new SongViewModel(s);
                svm.SelectCommand = new RelayCommand(() => { Service.Open(svm.SongModel); Service.Play(); });
                svm.DeleteCommand = new RelayCommand(() => { Service.DeleteSong(s); Songs.Remove(svm); });
                Songs.Add(svm);
                Service.List.Add(s);
                win.Close(); 
            };
            win.Show();
        }

        private void pausePlay()
        {
            if(Service.IsActive)
            {
                Service.Pause();
            }
            else
            {
                Service.Play();
            }

        }

        private void updatePosition()
        {
            Service.UpdatePosition();
            Debug.WriteLine("Update Position");
        }
        private void nextSong()
        {
            var obj = JsonSerializer.Serialize(Service);
            File.WriteAllText("plejer.json", obj);
            Service.PlayNext();
            
        }

        private void previousSong()
        {
            Service.PlayPrevious();
        }

        private void pauseUpdate()
        {
            Service.PuseUpdate();
            Debug.WriteLine("Pause Position");

        }

        private void resumeUpdate()
        {
            Service.ResumeUpdate();
            Debug.WriteLine("Resume Position");

        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
