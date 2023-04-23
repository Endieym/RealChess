using static RealChess.Model.Bitboard.BoardOperations;

namespace RealChess.Model.ChessPieces
{
    internal class Queen : ChessPiece
    {
        public bool SacrificeBuff { get; set; }
        public override PieceType Type { get; set; } = PieceType.QUEEN;
        public override ushort Value { get; set; } = 9;
        public Queen() { }

        public Queen(int key) : base(key) { }

        /// <summary>
        /// Generates a mask of all possible moves for the queen piece.
        /// </summary>
        /// <returns>A mask of all possible moves for the queen piece.</returns>
        public override ulong GenerateLegalMoves(ulong occupied)
        {
            ulong movemask = GenerateDiagonals(this.bitBoard, occupied);
            movemask |= GenerateLines(this.bitBoard, occupied);
            return movemask;
        }

       

    }
}
