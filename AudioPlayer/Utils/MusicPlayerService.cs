using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AudioPlayer
{
    public class MusicPlayerService
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader? audioFile;
        public MusicPlayerService()
        {
            outputDevice = new WaveOutEvent();
        }

        public double GetVolume()
        {
            return outputDevice.Volume;
        }

        public TimeSpan GetPosition()
        {
            if(audioFile == null)  
                return TimeSpan.Zero;
            return audioFile.CurrentTime;
        }

        public TimeSpan GetFullDuration()
        {
            if (audioFile == null)
                return TimeSpan.Zero;
            return audioFile.TotalTime;
        }

        public void Play()
        {
            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                audioFile.Position = 0;
            }
            outputDevice.Play();

        }

        public void Stop()
        {
            if (outputDevice.PlaybackState != PlaybackState.Stopped)
            {
                outputDevice.Stop();
            }
        }

        public void Pause()
        {
            if (outputDevice.PlaybackState != PlaybackState.Paused)
            {
                outputDevice.Pause();
            }
        }

        public void Open(string songPath)
        {
            if(audioFile==null || audioFile.FileName!=songPath)
            {
                outputDevice.Stop();
                audioFile = new AudioFileReader(songPath);
                outputDevice.Init(audioFile);
            }
        }



    }
}
