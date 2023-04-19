using RealChess.Model.AI.Evaluation;
using RealChess.Model.Bitboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static RealChess.Model.Bitboard.BoardLogic;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.AI
{
    internal static class MoveChecker
    {

        public static int MoveBuffer(Move move)
        {
            int buff = 0;

            if (move.IsCapture)
            {
                if (IsGoodCapture(move))
                {
                    buff += move.CapturedPiece.Value * 100;
                }
            }
            if(move.IsKingSideCastle || move.IsQueenSideCastle)
            {
                buff += EvaluationConstants.movePenalty;
            }
            
            return buff;
        }

        // Returns if true a move is good, false if else
        public static bool IsBadCapture(Move move)
        {
            bool flag = false;

            if (IsTrade(move))
                flag= false;

            else if (move.IsCapture && !move.IsPositiveCapture)
                flag = true;

            else if (move.IsPositiveCapture)
                flag = false;
          
            else if (BoardLogic.EvaluatePieceSafety(move.PieceMoved) < 0)
                flag = true;



            return flag;
        }

        public static bool IsGoodCapture(Move move)
        {

            if (move.IsPositiveCapture)
                return true;

            return false;
        }

        public static bool ShouldTrade(Move move)
        {
            double evaluation = BoardEvaluation.Evaluation;

            evaluation *= move.PieceMoved.Color == PieceColor.WHITE ? 1 : -1;

            if (evaluation > 100)
                return true;

            return false;
        }

        public static bool IsTrade(Move move)
        {
            if (move.IsCapture)
            {
                if (move.PieceMoved.Value == move.CapturedPiece.Value)
                    return true;
            }

            return false;
        }

        public static bool IsBadForPhase(Move move, GamePhase phase) 
        { 
            if(phase == GamePhase.Opening)
            {
                if (BoardLogic.RevokesCastlingRights(move))
                    return true;
                

            }
            else if(phase == GamePhase.Middlegame)
            {

            }
            else
            {

            }

            return false;
        
        
        
        }


    }
}
