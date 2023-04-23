using static RealChess.Model.Bitboard.BoardOperations;

namespace RealChess.Model.ChessPieces
{
    internal class Rook : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.ROOK;
        public override ushort Value { get; set; } = 5;
        public Rook() { }

        public Rook(int key) : base(key) { }

        /// <summary>
        /// Generates a mask of all possible moves for the rook piece.
        /// </summary>
        /// <returns>A mask of all possible moves for the rook piece.</returns>
        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateLines(this.bitBoard, occupied);
                
        }
    }
}
