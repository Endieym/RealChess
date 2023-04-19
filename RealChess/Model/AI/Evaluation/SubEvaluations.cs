using RealChess.Model.Bitboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.EvaluationConstants;

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

        public static int EvaluateKingPerimeter(PieceColor color, ulong kingPerimeter)
        {
            var player = color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                _gameBoard.GetPlayer2();

            var enemy = color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                _gameBoard.GetPlayer1();

            int kingSafety = 0;

            
            foreach (var piece in player.Pieces.Values)
            {
                if ((piece.GetPosition() & kingPerimeter) > 0)
                {
                    kingSafety += BoardLogic.EvaluatePieceSafety(piece);
                    kingPerimeter ^= piece.GetPosition();
                }
            }

            foreach (var piece in enemy.Pieces.Values)
            {
                if ((piece.GetPosition() & kingPerimeter) > 0)
                {
                    kingSafety -= piece.Value;
                    kingPerimeter ^= piece.GetPosition();
                }
            }

            while (kingPerimeter > 0)
            {
                ulong position = kingPerimeter & ~(kingPerimeter - 1);
                kingSafety += BoardLogic.EvaluateSquareControl(color, position);
                kingPerimeter &= kingPerimeter - 1;
            }

            return kingSafety;

        }

        public static int PawnShield(PieceColor color ,ulong kingPerimeter)
        {
            var playerPieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            int pawnShield = 1;

            ulong pawnPos = (kingPerimeter & KingSidePawns) > 0 ? KingSidePawns : QueenSidePawns;
           
            if (color == PieceColor.BLACK) pawnPos >>= 40;
            pawnPos |= color == PieceColor.WHITE ? pawnPos >> 8 : pawnPos << 8;
            
            foreach (var piece in playerPieces.Values)
            {
                if (piece.Type == PieceType.PAWN && (piece.GetPosition() & kingPerimeter) > 0
                    && (piece.GetPosition() & pawnPos) > 0)
                    pawnShield *= pawnShieldBuff; 
                
            }


            return pawnShield;
        }


    }
}
