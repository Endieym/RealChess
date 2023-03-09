using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model
{
    internal static class BitboardConstants
    {
        public const ulong NotAFile = 0xfefefefefefefefeUL;
        public const ulong NotHFile = 0x7f7f7f7f7f7f7f7fUL;
        public const ulong AFile = 0x101010101010101UL;
        public const ulong HFile = 0x8080808080808080UL;
    }
}
