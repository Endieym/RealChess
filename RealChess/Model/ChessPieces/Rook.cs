using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.BoardOperations;

namespace RealChess.Model.ChessPieces
{
    internal class Rook : ChessPiece
    {
        public Rook()
        {
            this.Type = PieceType.ROOK;
            this.Value = 5;

        }

        public Rook(int row, int col) : base(row, col)
        {
            this.Type = PieceType.ROOK;
            this.Value = 5;
        }

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
