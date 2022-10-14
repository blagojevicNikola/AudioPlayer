using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer
{
    public class Song
    {
        public string SongPath { get; set; }
        public string SongName { get; set; }
        public string PlayerName { get; set; }

        public Song(string songPath, string songName, string playerName)
        {
            SongPath = songPath;
            SongName = songName;
            PlayerName = playerName;
        }
    }
}
