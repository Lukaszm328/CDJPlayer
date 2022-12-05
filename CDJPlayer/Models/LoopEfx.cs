using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDJPlayer.Models
{
    internal class LoopEfx
    {
        public bool IsActive { get; set; }
        public bool InActive { get; set; }
        public bool OutActive { get; set; }
        public TimeSpan LoopInTime { get; set; }
        public TimeSpan LoopEndTime { get; set; }
    }
}
