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
        private Song _song;
        public ICommand? SelectCommand { get; set; }

        public SongViewModel(Song model)
        {
            _song = model;
        }
    }
}
