using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Data
{
    // Total = 12 bytes (10m pixels about 120MB)
    public class PixelHistoryEntry
    {
        // 4 bytes
        public int Id { get; set; } // id of the placement

        // 4 bytes
        public short X { get; set; } 
        public short Y { get; set; }

        // 1 byte
        public byte UserId { get; set; } // if needed Expand this to short

        // 3 bytes
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
