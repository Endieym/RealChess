using static RealChess.Model.Bitboard.BoardLogic;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.AI
{
    /// <summary>
    /// A static class that contains preprocessed tables used for evaluation.
    /// </summary>
    internal static class PreprocessedTables
    {
        // An array of PieceType enum values that represent minor pieces.
        internal static readonly PieceType[] MinorPieces = { PieceType.BISHOP, PieceType.KNIGHT };

        // A table of evaluation scores for each square on the chessboard when occupied by a pawn.
        private static readonly int[] PawnTable = 
            { 0,  0,  0,  0,  0,  0,  0,  0,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
            5,  5, 10, 25, 25, 10,  5,  5,
            0,  0,  0, 20, 20,  0,  0,  0,
            5, -5,-10,  0,  0,-10, -5,  5,
            5, 10, 10,-20,-20, 10, 10,  5,
            0,  0,  0,  0,  0,  0,  0,  0};

        // A table of evaluation scores for each square on the chessboard when occupied by a bishop.
        private static readonly int[] BishopTable =
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        };

        // A table of evaluation scores for each square on the chessboard when occupied by a knight.
        private static readonly int[] KnightTable =
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-30,-30,-30,-30,-30,-30,-50,
        };

        // A table of evaluation scores for each square on the chessboard when occupied by a rook.

        private static readonly int[] RookTable =
        {
              0,  0,  0,  0,  0,  0,  0,  0,
              5, 10, 10, 10, 10, 10, 10,  5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
              0,  0,  0,  5,  5,  3,  0,  0
        };

        // A table of evaluation scores for each square on the chessboard when occupied by a queen.
        private static readonly int[] QueenTable =
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  10,  10,  10,  10,  0,-10,
             -5,  0,  10,  10,  10,  10,  0, -5,
              0,  0,  10,  10,  10,  10,  0, -5,
            -10,  10,  10,  10,  10,  10,  0,-10,
            -10,  0,  10,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };

        // A table of evaluation scores for each square on the chessboard when occupied by a king.
        // In the opening-middle game.
        private static readonly int[] KingMiddleTable =
        {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
             20, 20,  0,  0,  0,  0, 20, 20,
             20, 30, 10,  0,  0, 10, 30, 20
        };

        // A table of evaluation scores for each square on the chessboard when occupied by a king.
        // In the endgame
        private static readonly int[] KingEndTable =
        {
            -50,-40,-30,-20,-20,-30,-40,-50,
            -30,-20,-10,  0,  0,-10,-20,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-30,  0,  0,  0,  0,-30,-30,
            -50,-30,-30,-30,-30,-30,-30,-50
        };

        /// <summary>
        /// Returns a piece-square table for a given piece type and game phase.
        /// </summary>
        /// <param name="type">The type of piece to generate a table for.</param>
        /// <param name="phase">The current phase of the game.</param>
        /// <returns>An array representing the piece-square table for the given piece type and game phase.</returns>
        public static int[] PieceSquareTable(PieceType type, GamePhase phase)
        {
            int[] squareTable = new int[64];
            switch (type)
            {
                case PieceType.PAWN:
                    squareTable = PawnTable;
                    break;

                case PieceType.BISHOP:
                    squareTable = BishopTable;
                    break;

                case PieceType.KNIGHT:
                    squareTable = KnightTable;
                    break;

                case PieceType.ROOK:
                    squareTable = RookTable;
                    break;

                case PieceType.QUEEN:
                    squareTable = QueenTable;
                    break;

                case PieceType.KING:
                    if (phase == GamePhase.Endgame)
                        squareTable = KingEndTable;
                    else 
                        squareTable = KingMiddleTable;
                    break;
            }
            return squareTable;
        }
    }
}
