using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridle.IO
{
    public enum ByteOrder
    {
        None = 0,
        /// <summary>
        /// 0x0123 = 01 23
        /// </summary>
        BigEndian = 1,
        /// <summary>
        /// Aka "big endian".
        /// </summary>
        MostLeast = 1,
        /// <summary>
        /// 0x0123 = 23 01
        /// </summary>
        LittleEndian = 2,
        /// Aka "little endian".
        LeastMost = 2,
    }
}
