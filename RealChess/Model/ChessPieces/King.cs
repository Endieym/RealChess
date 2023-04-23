using System;

namespace RealChess.Model.ChessPieces
{
    internal class King : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.KING;
        public override ushort Value { get; set; } = 999;
        public bool Castled { get; set; }
        public bool InCheck { get; set; }

        public King() { }
        
        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateMovesMask();
        }

        /// <summary>
        /// Generates a mask of all possible moves for the king piece.
        /// </summary>
        /// <returns>A mask of all possible moves for the king piece.</returns>
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
                west = moveMask >> 1;
                northwest = moveMask  >> 9;
                southwest = moveMask << 7;

            }
            // Checks if king is not on the H file, so he can move east
            if ((this.bitBoard & BitboardConstants.HFile) == 0)
            {
                east = moveMask << 1;
                northeast = moveMask >> 7;
                southeast = moveMask << 9;

            }
            moveMask = north | south | east | west | northeast | northwest | southeast | southwest;

            return moveMask;
        }
    }
}
