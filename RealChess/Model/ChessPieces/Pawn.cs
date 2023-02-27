using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public Pawn()
        {
            this.Type = PieceType.PAWN;

        }

        public Pawn(int row, int col) : base(row, col)
        {
            this.Type = PieceType.PAWN;
            
        }
    }
}
