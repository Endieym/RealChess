using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    internal class Knight : ChessPiece
    {
        public Knight()
        {
            this.Type = PieceType.KNIGHT;

        }

        public override ulong GetMoves()
        {
            throw new NotImplementedException();
        }
    }
}
