using RealChess.Model.ChessPieces;
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
        public static int EndGameWeight { get; set; }
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


        /// <summary>
        ///  Evaluates the board for a player,      
        ///  0 is a draw,
        ///  a negative number means better position for black,
        ///  a positive means better for white
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int Evaluate()
        {
            // The difference between white's pieces and black's pieces (by value)
            int evaluation = (ValuePieces(PieceColor.WHITE) - ValuePieces(PieceColor.BLACK)) * 100;
          
            
            // Evaluates the difference board control of white against black's.
            evaluation += EvaluateBoardControl(PieceColor.WHITE) - EvaluateBoardControl(PieceColor.BLACK);

            // Evaluates king safety
            
            evaluation += EvaluateKingSafety(PieceColor.WHITE) - EvaluateKingSafety(PieceColor.BLACK);
            
            //foreach (var piece in blackPieces.Values)
            //{
            //    evaluation -= BoardLogic.EvaluateSafety(piece);
            //}

            return evaluation;

        }



        public static int EvaluateForPlayer(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;
            
            int evaluation = Evaluate();

            evaluation *= color == PieceColor.WHITE ? 1 : -1;

            // Evaluates safety of every piece
            foreach (var piece in pieces.Values)
            {
                if (piece.Type != PieceType.PAWN && piece.Type != PieceType.KING)
                {
                    evaluation += BoardLogic.EvaluatePieceSafety(piece) * 2;
                }
            }

            return evaluation;
        }

        // Evaluates control of squares for a specific player
        public static int EvaluateBoardControl(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE? _gameBoard.GetPlayer1().Pieces:
                _gameBoard.GetPlayer2().Pieces;

            int countControl = 0;
            foreach(var piece in pieces.Values)
            {
                if (piece.Type == PieceType.KING) continue;// king shouldn't be active
                countControl += EvaluatePieceMobility(piece, _gameBoard.BitBoard);
            }

            return countControl;
        }

        public static int EvaluatePieceMobility(ChessPiece piece, ulong ocuppied)
        {
            // Gets the possible moves bitmask for the piece
            ulong attacks = piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures() :
                piece.GenerateLegalMoves(ocuppied);

            attacks |= piece.GetPosition();

            int mobility = 0;
            ulong centerControl = BitboardConstants.Center;

            centerControl &= attacks;

            // Center is more valuable square to control
            while (centerControl != 0)
            {
                centerControl &= centerControl - 1;// reset LS1B
                mobility += 2;
            }
            while (attacks != 0)
            {
                attacks &= attacks - 1; // reset LS1B
                mobility++;
            }
            return mobility;
        }

        // Counts pieces and their values of a specific coloured player
        public static int ValuePieces(PieceColor color)
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

        
        public static int EvaluateKingSafety(PieceColor color)
        {
            ulong kingPerimeter = BoardLogic.GetKingPerimeter(color);

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

        

    }
}
