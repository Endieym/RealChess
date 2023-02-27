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

        }

        public King(int row, int col) : base(row, col)
        {
            this.Type = PieceType.KING;

        }

        public override ulong GetMoves()
        {
            throw new NotImplementedException();
        }
    }
}
