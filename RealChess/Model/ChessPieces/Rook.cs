using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.Bitboard.BoardOperations;

namespace RealChess.Model.ChessPieces
{
    internal class Rook : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.ROOK;
        public override ushort Value { get; set; } = 5;
        public Rook() { }

        public Rook(int key) : base(key) { }

        public Rook(int row, int col) : base(row, col) { }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            return GenerateLines(this.bitBoard, occupied);
                
        }

        public ulong GenerateMovesMask()
        {
            throw new NotImplementedException();
        }
    }
}
