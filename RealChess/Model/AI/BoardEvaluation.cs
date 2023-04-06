using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.Bitboard
{
    internal static class BoardEvaluation
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

        // Evaluates the board for a player,
        // 0 is a draw,
        // a negative number means better position for black,
        // a positive means better for white
        public static int Evaluate(PieceColor color)
        {
            // The difference between white's pieces and black's pieces (by value)
            int evaluation = (CountPieces(PieceColor.WHITE) - CountPieces(PieceColor.BLACK)) * 100;
            var blackPieces = blackPlayer.Pieces;
            var whitePieces = whitePlayer.Pieces;
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;

            // Evaluates the differnece board control of white against black's.
            evaluation += EvaluateBoardControl(PieceColor.WHITE) - EvaluateBoardControl(PieceColor.BLACK);
            
            evaluation *= color == PieceColor.WHITE ? 1 : -1;
            
            foreach (var piece in pieces.Values)
            {
                if(piece.Type != PieceType.PAWN && piece.Type != PieceType.KING)
                {
                    evaluation += BoardLogic.EvaluateSafety(piece) * 2;
                }
            }

            

            //foreach (var piece in blackPieces.Values)
            //{
            //    evaluation -= BoardLogic.EvaluateSafety(piece);
            //}


            return evaluation;
        }

        // Evaluates control of squares for a specific player
        public static int EvaluateBoardControl(PieceColor color)
        {
            ulong attacks =  _gameBoard.GetPlayerAttacksAndOcuppied(color);
            int countControl = 0;
            ulong centerControl = BitboardConstants.Center;

            centerControl &= attacks;

            while (centerControl != 0)
            {
                centerControl &= centerControl - 1;// reset LS1B
                countControl += 5;
            }
            while (attacks != 0)
            {

                attacks &= attacks - 1; // reset LS1B
                countControl++;


            }

            return countControl;
        }


        // Counts pieces and their values of a specific coloured player
        public static int CountPieces(PieceColor color)
        {
            int pieceCount = 0;

            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;

            // Counts value of pieces on a specific player
            foreach(var piece in pieces.Values)
            {
                
                pieceCount += piece.Value;
            }

           
            
            return pieceCount;

        }

    }
}
