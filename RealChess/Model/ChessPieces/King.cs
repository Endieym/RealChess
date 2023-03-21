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
            ulong east = 0, west = 0, northeast = 0, northwest = 0,
                southeast = 0, southwest = 0;
            // Directions are for white, but still the same just
            // opposite for black
            UInt64 moveMask = this.bitBoard;
            ulong north = moveMask << 8;
            ulong south = moveMask >> 8;

            // Checks if king is not on the A file, so he can move west
            if((this.bitBoard & BitboardConstants.AFile) == 0)
            {
                west = (moveMask & 0xfefefefefefefefeUL) << 1;
                northwest = (moveMask & 0xfefefefefefefefeUL) << 9;
                southwest = (moveMask & 0xfefefefefefefefeUL) >> 7;

            }
            // Checks if king is not on the H file, so he can move east
            if ((this.bitBoard & BitboardConstants.AFile) == 0)
            {
                east = (moveMask & 0x7f7f7f7f7f7f7f7fUL) >> 1;
                northeast = (moveMask & 0x7f7f7f7f7f7f7f7fUL) << 7;
                southeast = (moveMask & 0x7f7f7f7f7f7f7f7fUL) >> 9;

            }
            moveMask = north | south | east | west | northeast | northwest | southeast | southwest;

            return moveMask;

        }


    }
}
