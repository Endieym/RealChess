using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    internal class King : ChessPiece
    {
        public bool InCheck { get; set; }
        public King()
        {
            this.Type = PieceType.KING;
            this.Value = 9999;

        }

        public King(int row, int col) : base(row, col)
        {
            this.Type = PieceType.KING;
            this.Value = 9999;
        }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateMovesMask();
        }

        public ulong GenerateMovesMask()
        {
            // Directions are for white, but still the same just
            // opposite for black
            UInt64 moveMask = this.bitBoard;
            ulong north = moveMask << 8;
            ulong south = moveMask >> 8;
            ulong east = (moveMask & 0x7f7f7f7f7f7f7f7fUL) >> 1;
            ulong west = (moveMask & 0xfefefefefefefefeUL) << 1;
            ulong northeast = (moveMask & 0x7f7f7f7f7f7f7f7fUL) << 7;
            ulong northwest = (moveMask & 0xfefefefefefefefeUL) << 9;
            ulong southeast = (moveMask & 0x7f7f7f7f7f7f7f7fUL) >> 9;
            ulong southwest = (moveMask & 0xfefefefefefefefeUL) >> 7;

            moveMask = north | south | east | west | northeast | northwest | southeast | southwest;

            return moveMask;

        }


    }
}
