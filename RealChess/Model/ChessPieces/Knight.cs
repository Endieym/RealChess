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
            this.Value = 3;

        }

        public Knight(int row, int col) : base(row, col)
        {
            this.Type = PieceType.KNIGHT;
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
