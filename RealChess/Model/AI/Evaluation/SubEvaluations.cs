using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.AI.Evaluation
{
    internal static class SubEvaluations
    {
        private static Board _gameBoard;
        private static Player whitePlayer;
        private static Player blackPlayer;

        // Sets the game board and players
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            whitePlayer = board.GetPlayer1();
            blackPlayer = board.GetPlayer2();
        }


        public static int CountMaterial(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;

            int pieceCount = 0;
            // Counts value of pieces on a specific player
            foreach (var piece in pieces.Values)
            {
                if (piece.Type != PieceType.KING)
                    pieceCount += piece.Value;
            }

            return pieceCount;
        }

        public static int CountBishopPair(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;

            int bishopCount = 0;
            foreach(var piece in pieces.Values)
            {
                if (piece.Type == PieceType.BISHOP)
                    bishopCount++;
            }
            return bishopCount;
        }


    }
}
