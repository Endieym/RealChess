using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    internal class Bishop : ChessPiece
    {
        public Bishop()
        {
            this.Type = PieceType.BISHOP;
            this.Value = 3;

        }

        public Bishop(int row, int col) : base(row, col)
        {
            this.Type = PieceType.BISHOP;
            this.Value = 3;
        }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            throw new NotImplementedException();
        }

        public ulong GenerateMovesMask()
        {
            throw new NotImplementedException();
        }

       
    }
}
