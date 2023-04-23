using System;
using static RealChess.Model.Bitboard.BoardOperations;

namespace RealChess.Model.ChessPieces
{
    internal class Bishop : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.BISHOP;
        public override ushort Value { get; set; } = 3;
        public Bishop() { }
        public Bishop(int key) : base(key) { }

        /// <summary>
        /// Generates a mask of all possible moves for the bishop piece.
        /// </summary>
        /// <returns>A mask of all possible moves for the bishop piece.</returns>
        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateDiagonals(this.bitBoard, occupied);
        }
    }
}
