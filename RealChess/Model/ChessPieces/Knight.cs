using static RealChess.Model.BitboardConstants;

namespace RealChess.Model.ChessPieces
{
    internal class Knight : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.KNIGHT;
        public override ushort Value { get; set; } = 3;
        public Knight() { }

        public Knight(int key) : base(key) { }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateMovesMask();
        }

        /// <summary>
        /// Generates a mask of all possible moves for the knight piece.
        /// </summary>
        /// <returns>A mask of all possible moves for the knight piece.</returns>
        public ulong GenerateMovesMask()
        {
            ulong attacks = 0;
            // Check if knight is not on A-file and can move to the left
            if ((bitBoard & AFile) == 0) 
            {
                attacks |= bitBoard >> 17; // move 2 up, 1 left
                attacks |= bitBoard << 15; // move 2 down, 1 left

                // check if knight is not on B-file and can move two squares to the left
                
                    if ((bitBoard & BFile) == 0)
                    {
                    attacks |= bitBoard >> 10; // move 1 up, 2 left
                    attacks |= bitBoard << 6; // move 1 down, 2 left
                    }
            }
 
            // check if knight is not on H-file and can move to the right
            if ((bitBoard & HFile) == 0 ) 
            {
                attacks |= bitBoard >> 15; // move 2 up, 1 right
                attacks |= bitBoard << 17; // move 2 down, 1 right

                // check if knight is not on G-file and can move two squares to the right
                if ((bitBoard & GFile) == 0) 
                {
                    attacks |= bitBoard >> 6; // move 1 up, 2 right
                    attacks |= bitBoard << 10; // move 1 down, 2 right

                }
            }
            return attacks;
        }

    }
}
