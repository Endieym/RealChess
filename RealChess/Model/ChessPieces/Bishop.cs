using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.BoardOperations;

namespace RealChess.Model.ChessPieces
{
    internal class Bishop : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.BISHOP;
        public override ushort Value { get; set; } = 3;
        public Bishop() { }

        public Bishop(int key) : base(key) { }

        public Bishop(int row, int col) : base(row, col) { }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateDiagonals(this.bitBoard, occupied);
        }

        public ulong GenerateMovesMask()
        {
            throw new NotImplementedException();
        }

       
    }
}
