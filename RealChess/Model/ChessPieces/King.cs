using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    internal class King : ChessPiece
    {
        public King()
        {
            this.Type = PieceType.KING;
            this.Value = 9999;

        }

        public King(int row, int col) : base(row, col)
        {
            this.Type = PieceType.KING;
            this.Value = 9999;
        }

        public override ulong GenerateLegalMoves(ulong movesMask, ulong occupied)
        {
            throw new NotImplementedException();
        }

        public override ulong GenerateMovesMask()
        {
            throw new NotImplementedException();
        }

        
    }
}
