using RealChess.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model
{
    internal static class ComputerPlay
    {
        public static void PlayMove()
        {
            GameController.MovePiece(ChooseBestMove());
        }
        
        public static Move ChooseBestMove()
        {
            return new Move();
        }
    }
}
