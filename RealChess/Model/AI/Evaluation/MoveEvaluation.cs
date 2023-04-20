using RealChess.Controller;
using RealChess.Model.AI.Evaluation;
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
    internal static class MoveEvaluation
    {
        private static Board _gameBoard;
 

        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        public static double GetEvaluationForMove(Move move)
        {
            var color = move.PieceMoved.Color;

            double moveScore = BoardEvaluation.EvaluateForPlayer(color);

            //if (MoveChecker.IsBadCapture(move))
            //{
            //    moveScore -= (move.PieceMoved.Value * 100);

            //    if (moveScore > 0)
            //        moveScore *= -1;
            //}


            moveScore += MoveChecker.MoveBonus(move);
            moveScore -= MoveChecker.MovePenalty(move);

            if (MoveChecker.IsBadForPhase(move, _gameBoard.CurrentPhase))
                moveScore -= movePenalty;

            if (GameController.IsReal && move.PieceMoved.Type != PieceType.KING)
                moveScore += RealBoardController.CalculateSuccess(move) / 10;

            if (move.Type == Move.MoveType.Draw)
                moveScore = 0;

            return moveScore;
        }

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
