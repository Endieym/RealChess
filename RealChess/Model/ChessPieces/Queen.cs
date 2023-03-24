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

        public override PieceType Type { get; set; } = PieceType.QUEEN;
        public override ushort Value { get; set; } = 8;
        public Queen() { }

        public Queen(int key) : base(key) { }

        public Queen(int row, int col) : base(row, col) { }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            ulong movemask = GenerateDiagonals(this.bitBoard, occupied);
            movemask |= GenerateLines(this.bitBoard, occupied);
            return movemask;
        }

       

    }
}
