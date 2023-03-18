using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.BoardOperations;

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

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            ulong movemask = GenerateDiagonals(this.bitBoard, occupied);
            movemask |= GenerateLines(this.bitBoard, occupied);
            return movemask;
        }

       

    }
}
