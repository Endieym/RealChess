using RealChess.Model.Bitboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static RealChess.Model.Bitboard.BoardLogic;

namespace RealChess.Model.AI
{
    internal static class MoveChecker
    {
        // Returns if true a move is good, false if else
        public static bool IsGoodMove(Move move)
        {
            if (move.IsCapture && !move.IsPositiveCapture)
                return false;

            else if (move.IsPositiveCapture)
                return true;
          
            else if (BoardLogic.EvaluatePieceSafety(move.PieceMoved) < 0)
                return false;

            

            return true;
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
