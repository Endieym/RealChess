using RealChess.Model.Bitboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.AI
{
    internal static class MoveChecker
    {
        // Returns if true a move is good, false if else
        public static bool IsGoodMove(Move move)
        {
            if (move.IsCapture && !move.IsPositiveCapture)
                return false;
            
            else if(move.PieceMoved.Type == ChessPieces.ChessPiece.PieceType.QUEEN)
            {
                if (BoardLogic.EvaluateSafety(move.PieceMoved) < 0)
                    return false;
            }
            else if (BoardLogic.EvaluateSafety(move.PieceMoved) < 0)
                return false;


            return true;
        }
    }
}
