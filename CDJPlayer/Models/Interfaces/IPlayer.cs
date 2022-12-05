using System;
using System.Threading.Tasks;

namespace CDJPlayer.Models.Interfaces
{
    interface IPlayer
    {
        Task Load(ListItemDrive listItemDrive);

        void Play();
        
        void Pause();

        void Seek(TimeSpan time);

        void SetTempo(double tempo);

        void SetPosition(TimeSpan time);

        Task<float> GetBPM();
    }
}