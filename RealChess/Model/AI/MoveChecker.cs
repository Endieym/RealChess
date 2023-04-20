using RealChess.Model.AI.Evaluation;
using RealChess.Model.Bitboard;
using RealChess.Model.ChessPieces;
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

        public static double MoveBonus(Move move)
        {
            double buff = 0;

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

            if(IsTrade(move))
                buff += TradeBuff(move);
            
            return buff;
        }

        public static int MovePenalty(Move move)
        {
            int debuff = 0;
            

            debuff += HangingPenalty(move);

            return debuff;
        }

        public static int HangingPenalty(Move move)
        {
            var hangingPiece = HangingValue(move.PieceMoved.Color);

            int debuff = 0;  

            if (hangingPiece == null)
                return 0;

            debuff += hangingPiece.Value * 100;

            if (move.IsCapture && hangingPiece.Value <= move.CapturedPiece.Value)
                debuff = 0;

            else if (hangingPiece.Equals(move.PieceMoved))
            {
                if (move.IsPositiveCapture || IsTrade(move))
                    debuff = 0;
            }
            
            return debuff;
        }

        //// Returns if true a move is good, false if else
        //public static bool IsBadCapture(Move move)
        //{
        //    bool flag = false;

        //    if (IsTrade(move))
        //        flag= false;

        //    else if (move.IsCapture && !move.IsPositiveCapture)
        //        flag = true;

        //    else if (move.IsPositiveCapture)
        //        flag = false;
          
        //    else if (BoardLogic.EvaluatePieceSafety(move.PieceMoved) < 0)
        //        flag = true;



        //    return flag;
        //}

        
        public static ChessPiece HangingValue(PieceColor color)
        {
            var hangingList = BoardLogic.GetHangingPieces(color);

            return hangingList.Max();

        }



        //public static bool HangsPieces(Move move)
        //{
        //    int PiecesSafety = MajorEvaluations.EvaluatePlayerSafety(move.PieceMoved.Color); 

        //    if (PiecesSafety < 0)
        //        return true;


        //    if (PieceMovedSafety < 0)
        //    {
        //        if (move.IsPositiveCapture || MoveChecker.IsTrade(move))
        //            moveScore += PieceMovedSafety * -1;

        //    }

        //    return false;

        //}

        public static bool IsGoodCapture(Move move)
        {

            if (move.IsPositiveCapture)
                return true;

            return false;
        }

        public static double TradeBuff(Move move)
        {
            double evaluation = BoardEvaluation.EvaluateMaterial();

            double tradeBuff = 0;

            evaluation *= move.PieceMoved.Color == PieceColor.WHITE ? 1 : -1;

            if(evaluation > 100)
            {
                tradeBuff += EvaluationConstants.movePenalty * (evaluation / 100);
            }

            return Math.Min(move.PieceMoved.Value * 100 - 20, tradeBuff);
                
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
