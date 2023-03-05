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
            this.Value = 8;
        }

        public Queen(int row, int col) : base(row, col)
        {
            this.Type = PieceType.QUEEN;
            this.Value = 8;

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
