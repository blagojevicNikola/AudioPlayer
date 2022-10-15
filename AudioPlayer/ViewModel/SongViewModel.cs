using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioPlayer
{
    public class SongViewModel
    {
        public string SongPath { get; set; }
        public string SongName { get; set; }
        public string PlayerName { get; set; }
        public bool IsSelected { get; set; }
        public ICommand SelectCommand { get; set; }

        public SongViewModel(Song model)
        {
            SongPath = model.SongPath;
            SongName = model.SongName;
            PlayerName = model.PlayerName;
            IsSelected = false;
        }
    }
}
