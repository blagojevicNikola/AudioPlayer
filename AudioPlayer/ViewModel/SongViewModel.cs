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
        public ICommand? SelectCommand { get; set; }
        public Song SongModel { get; private set; }
        public SongViewModel(Song model)
        {
            SongModel = model;
        }
    }
}
