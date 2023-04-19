using RealChess.Model.AI;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.Bitboard.BoardLogic;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.EvaluationConstants;
using RealChess.Model.AI.Evaluation;

namespace RealChess.Model.Bitboard
{
    /// <summary>
    /// Class responsible for evaluation of the board
    /// </summary>
    internal static class BoardEvaluation
    {
        public static int EndGameWeight { get; set; }
        private static Board _gameBoard;
        private static Player whitePlayer;
        private static Player blackPlayer;

        public static double Evaluation { get; set; }

        // Sets the game board and players
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            whitePlayer = board.GetPlayer1();
            blackPlayer = board.GetPlayer2();
            SubEvaluations.SetBoard(board);
        }


        /// <summary>
        ///  Evaluates the board for a player,      
        ///  0 is a draw,
        ///  a negative number means better position for black,
        ///  a positive means better for white
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static double Evaluate()
        {
            var whitePieces = _gameBoard.GetPlayer1().Pieces.Values;
            var blackPieces = _gameBoard.GetPlayer2().Pieces.Values;
            
            
            // The difference between white's pieces and black's pieces (by value)
            double evaluation = EvaluateMaterial(PieceColor.WHITE) - EvaluateMaterial(PieceColor.BLACK);
          
            
            // Evaluates the difference board control of white against black's.
            evaluation += EvaluateBoardControl(PieceColor.WHITE) - EvaluateBoardControl(PieceColor.BLACK);

            // Evaluates king safety
            
            evaluation += EvaluateKingSafety(PieceColor.WHITE) - EvaluateKingSafety(PieceColor.BLACK);

            double openingWeight = _gameBoard.CurrentPhase == GamePhase.Opening ? 1 : 0.5;
            // Evaluates piece development for both players
            evaluation += (EvaluatePieceDevelopment(PieceColor.WHITE) - EvaluatePieceDevelopment(PieceColor.BLACK))* openingWeight;

            // Evaluates safety of all player's ieces
            evaluation += EvaluatePiecesSafety(PieceColor.WHITE) - EvaluatePiecesSafety(PieceColor.BLACK);
            //foreach (var piece in whitePieces)
            //{
            //    evaluation += BoardLogic.EvaluatePieceSafety(piece);
            //}

            //foreach (var piece in blackPieces)
            //{
            //    evaluation -= BoardLogic.EvaluatePieceSafety(piece);
            //}

            return evaluation;

        }


        /// <summary>
        /// Evaluates the board position for a specific player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static double EvaluateForPlayer(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;
            
            double evaluation = Evaluate();

            evaluation *= color == PieceColor.WHITE ? 1 : -1;

            //// Evaluates safety of every piece
            //foreach (var piece in pieces.Values)
            //{
            //    if (piece.Type != PieceType.PAWN && piece.Type != PieceType.KING)
            //    {
            //        evaluation += BoardLogic.EvaluatePieceSafety(piece) * 4;
            //    }
            //}

            return evaluation;
        }

        /// <summary>
        /// Evaluates control of squares for a specific player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
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

            return countControl ;
        }

        /// <summary>
        /// Function which evaluates piece mobility
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="ocuppied">Ocuppied board</param>
        /// <returns></returns>
        public static int EvaluatePieceMobility(ChessPiece piece, ulong ocuppied)
        {
            // Gets the possible moves bitmask for the piece
            ulong attacks = piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures() :
                piece.GenerateLegalMoves(ocuppied);

            attacks |= piece.GetPosition();

            int mobility = 0;
            ulong centerControl = Center;

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

        /// <summary>
        /// Counts pieces and their values of a specific coloured player
        /// </summary>
        /// <param name="color">player color</param>
        /// <returns>Count (value) of pieces</returns>
        public static int EvaluateMaterial(PieceColor color)
        {
            int MaterialEvaluation = 0;

            MaterialEvaluation += SubEvaluations.CountMaterial(color) * 100;

            if (SubEvaluations.CountBishopPair(color) >= 2)
                MaterialEvaluation += BishopPairBuff;

            return MaterialEvaluation;
        }

        /// <summary>
        /// Evaluates the safety of all pieces of a player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The number evaluation</returns>
        public static int EvaluatePiecesSafety(PieceColor color)
        {
            int safety = 0;

            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            // Evaluates safety of every piece
            foreach (var piece in pieces.Values)
            {
                if (piece.Type != PieceType.PAWN && piece.Type != PieceType.KING)
                {
                    safety += BoardLogic.EvaluatePieceSafety(piece);
                }
            }
            return safety;

        }

        /// <summary>
        /// Evaluates the safety of the king of a player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
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


        /// <summary>
        /// Evaluates the development a player has with their pieces
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static int EvaluatePieceDevelopment(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            int development = 0;
            GamePhase currentPhase = _gameBoard.CurrentPhase;

            foreach(var piece in pieces.Values) 
            {
               if(currentPhase == GamePhase.Opening)
                {
                    if (piece.Type != PieceType.PAWN &&
                        piece.Type != PieceType.KNIGHT &&
                        piece.Type != PieceType.BISHOP
                        ) continue;

                }
                int index = (int)Math.Log(piece.GetPosition(),2);
                if (piece.Color == PieceColor.BLACK)
                    index = 63 - index;
                development += PreprocessedTables.PieceSquareTable(piece.Type, currentPhase)[index];

            }
            return development;
        }
    }
}
