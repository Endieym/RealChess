using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    internal class Queen : ChessPiece
    {
        public Queen()
        {
            this.Type = PieceType.QUEEN;

        }

        public Queen(int row, int col) : base(row, col)
        {
            this.Type = PieceType.QUEEN;

        }

        public override ulong GetMoves()
        {
            throw new NotImplementedException();
        }
    }
}
