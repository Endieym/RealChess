using RealChess.Controller;
using RealChess.Model.Bitboard;
using System;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.EvaluationConstants;

namespace RealChess.Model.AI.Evaluation
{
    /// <summary>
    /// Static class containing methods for evaluating a chess move and choosing the best promotion option.
    /// </summary>
    internal static class MoveEvaluation
    {
        private static Board _gameBoard;

        /// <summary>
        /// Sets the game board instance to be used for evaluation.
        /// </summary>
        /// <param name="board">The game board instance to use.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        /// <summary>
        /// Calculates the evaluation score for a given move.
        /// </summary>
        /// <param name="move">The move to evaluate.</param>
        /// <returns>The evaluation score for the given move.</returns>
        public static double GetEvaluationForMove(Move move)
        {
            var color = move.PieceMoved.Color;

            // Gets evaluation of entire board
            double moveScore = BoardEvaluation.EvaluateForPlayer(color);

            // Evaluates the player's pieces' safety after the move was made
            moveScore += MajorEvaluations.EvaluatePlayerSafety(color);

            // Adds bonuses and penalties according to MoveChecker
            moveScore += MoveChecker.MoveBonus(move);
            moveScore -= MoveChecker.MovePenalty(move);

            // If in "Real" gamemode, add the success rate to the evaluation
            if (GameController.IsReal)
            {
                double successRate = RealBoardController.CalculateSuccess(move) /10;
                
                moveScore += successRate;

                // Evaluate the added morale (en passant, captures, checks since it boosts morale)
                moveScore += MoveChecker.GetAddedMorale(move, successRate/10);
            }
            
            // If the move could lead to a draw or is a draw, evaluate as 0
            if (move.Type == Move.MoveType.Draw ||  MoveChecker.NextMoveAllowsDraw(_gameBoard,color))
                moveScore = 0;

            return moveScore;
        }

        /// <summary>
        /// Chooses the best promotion option for a given move.
        /// </summary>
        /// <param name="move">The move to promote.</param>
        /// <returns>The best promotion move.</returns>
        public static Move ChooseBestPromotion(Move move)
        {
            Move bestPromotion = null;
            double bestPromotionScore = negativeInfinity;

            foreach (PieceType p in Enum.GetValues(typeof(PieceType)))
            {
                if (!(p == PieceType.PAWN || p == PieceType.KING))
                {
                    var piece = BoardController.GetPieceByType(p);
                    piece.Color = move.PieceMoved.Color;

                    var tempMove = BoardController.PromotePiece(move, piece);

                    _gameBoard.MakeTemporaryMove(tempMove);

                    var tempScore = GetEvaluationForMove(tempMove);

                    if (tempMove.Type == Move.MoveType.Checkmate)
                    {
                        bestPromotionScore = infinity;
                        bestPromotion = tempMove;
                    }
                    if (tempScore > bestPromotionScore)
                    {
                        bestPromotion = tempMove;
                        bestPromotionScore = tempScore;
                    }

                    BoardUpdate.UndoPromotion(move);
                    _gameBoard.UndoMove();
                }
            }
            _gameBoard.MakePromotionMove(bestPromotion, bestPromotion.PieceMoved);
            return bestPromotion;
        }
    }
}
