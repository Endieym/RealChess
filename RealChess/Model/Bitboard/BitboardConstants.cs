
namespace RealChess.Model
{

    /// <summary>
    /// A static class containing various constants related to bitboards used in the chess engine.
    /// </summary>
    internal static class BitboardConstants
    {
        public const ulong NotAFile = 0xfefefefefefefefeUL;
        public const ulong NotHFile = 0x7f7f7f7f7f7f7f7fUL;

        public const ulong AFile = 0x101010101010101UL;
        public const ulong HFile = 0x8080808080808080UL;
        public const ulong BFile = 0x202020202020202;
        public const ulong GFile = 0x4040404040404040;

        public const ulong RankOne = 0xff00000000000000;
        public const ulong RankEight = 0x00000000000000ff;
        public const ulong WhiteQueenSide = 0xe00000000000000;
        public const ulong WhiteKingSide = 0x6000000000000000;
        public const ulong BlackQueenSide = 0xe;
        public const ulong BlackKingSide = 0x60;

        public const ulong lightSquares = 0xAA55AA55AA55AA55;
        public const ulong darkSquares = ~lightSquares;

        public const int blackQueenRook = 0;
        public const int blackKingRook = 7;
        public const int whiteQueenRook = 56;
        public const int whiteKingRook = 63;
        
    }
}
