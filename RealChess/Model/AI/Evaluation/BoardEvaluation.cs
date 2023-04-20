using RealChess.Model.AI;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.Bitboard.BoardLogic;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.MajorEvaluations;


namespace RealChess.Model.AI.Evaluation
{
    /// <summary>
    /// Class responsible for evaluation of the board
    /// </summary>
    internal static class BoardEvaluation
    {
        public static int EndGameWeight { get; set; }
        private static Board _gameBoard;

        public static double Evaluation { get; set; }
        public static int MaterialCount { get; set; }

        // Sets the game board and players
        public static void SetBoard(Board board)
        {
            _gameBoard = board;

            MajorEvaluations.SetBoard(board);
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

            // The difference between white's pieces and black's pieces (by value)
            double evaluation = EvaluateMaterial();


            // Evaluates the difference board control of white against black's.
            evaluation += EvaluateBoardControl();
            // Evaluates king safety

            evaluation += EvaluateKingSafety();

            double openingWeight = _gameBoard.CurrentPhase == GamePhase.Opening ? 1 : 0.5;
            // Evaluates piece development for both players

            evaluation += EvaluatePieceDevelopment(openingWeight);

            // Evaluates safety of all player's ieces
            evaluation += EvaluatePiecesSafety();

            return evaluation;

        }


        /// <summary>
        /// Evaluates the board position for a specific player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static double EvaluateForPlayer(PieceColor color)
        {
            
            double evaluation = Evaluate();

            evaluation *= color == PieceColor.WHITE ? 1 : -1;


            return evaluation;
        }

        public static int EvaluateMaterial()
        {
            return EvaluatePlayerMaterial(PieceColor.WHITE) - EvaluatePlayerMaterial(PieceColor.BLACK);
        }

        public static int EvaluateBoardControl()
        {
            return EvaluatePlayerControl(PieceColor.WHITE) - EvaluatePlayerControl(PieceColor.BLACK);

        }

        public static int EvaluateKingSafety()
        {
            return EvaluatePlayerKingSafety(PieceColor.WHITE) - EvaluatePlayerKingSafety(PieceColor.BLACK);
        }

        public static double EvaluatePieceDevelopment(double openingWeight)
        {
            return (EvaluatePlayerDevelopment(PieceColor.WHITE) - EvaluatePlayerDevelopment(PieceColor.BLACK))
                * openingWeight;
        }

        public static int EvaluatePiecesSafety()
        {
            return (EvaluatePlayerDanger(PieceColor.BLACK) - EvaluatePlayerDanger(PieceColor.WHITE)) * 20;
        }


    }
}
