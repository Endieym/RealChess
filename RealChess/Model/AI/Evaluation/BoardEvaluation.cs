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
        public static double Evaluate(GamePhase phase)
        {

            // The difference between white's pieces and black's pieces (by value)
            double evaluation = EvaluateMaterial();

            // Evaluates the difference board control of white against black's.
            evaluation += EvaluateBoardControl();
            // Evaluates king safety

            evaluation += EvaluateKing(phase);

            double openingWeight = phase == GamePhase.Opening ? 1.5 : 1;
            // Evaluates piece development for both players

            evaluation += EvaluatePieceDevelopment(openingWeight);

            // Evaluates safety of all player's pieces
            evaluation += EvaluatePiecesSafety();

            evaluation += EvaluatePawnStructure();

            return evaluation;

        }


        /// <summary>
        /// Evaluates the board position for a specific player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static double EvaluateForPlayer(PieceColor color)
        {
            
            double evaluation = TaperedEval();

            evaluation *= color == PieceColor.WHITE ? 1 : -1;


            return evaluation;
        }

        public static double TaperedEval()
        {
            double phase = PhaseEvaluation.GetPhaseWeight();

            double opening = Evaluate(GamePhase.Opening);
            double endgame = Evaluate(GamePhase.Endgame);

            double eval = ((opening * (256 - phase)) + (endgame * phase)) / 256;

            return eval;
        }

        public static int EvaluateMaterial()
        {
            return EvaluatePlayerMaterial(PieceColor.WHITE) - EvaluatePlayerMaterial(PieceColor.BLACK);
        }

        public static int EvaluateBoardControl()
        {
            return EvaluatePlayerControl(PieceColor.WHITE) - EvaluatePlayerControl(PieceColor.BLACK);

        }

        public static int EvaluateKing(GamePhase phase)
        {
            if(phase == GamePhase.Endgame)
            {
                return EvaluateKingActivity(PieceColor.WHITE) - EvaluateKingActivity(PieceColor.BLACK);
            
            }
            return EvaluatePlayerKingSafety(PieceColor.WHITE) - EvaluatePlayerKingSafety(PieceColor.BLACK);
        }

        public static double EvaluatePieceDevelopment(double openingWeight)
        {
            return (EvaluatePlayerDevelopment(PieceColor.WHITE) - EvaluatePlayerDevelopment(PieceColor.BLACK))
                * openingWeight;
        }

        public static int EvaluatePiecesSafety()
        {
            return (EvaluatePlayerDanger(PieceColor.WHITE) - EvaluatePlayerDanger(PieceColor.BLACK)) * 20;
        }

        public static int EvaluatePawnStructure()
        {
            return EvaluatePlayerPawnStructure(PieceColor.WHITE) - EvaluatePlayerPawnStructure(PieceColor.BLACK);
        }


    }
}
