using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.BitboardConstants;

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
            return GenerateMovesMask();
        }

        public ulong GenerateMovesMask()
        {
            ulong attacks = 0;
            // Check if knight is not on A-file and can move to the left
            if ((bitBoard & AFile) == 0) 
            {
                attacks |= bitBoard >> 17; // move 2 up, 1 left
                attacks |= bitBoard << 15; // move 2 down, 1 left

                // check if knight is not on B-file and can move two squares to the left
                
                    if ((bitBoard & BFile) == 0)
                    {
                    attacks |= bitBoard >> 10; // move 1 up, 2 left
                    attacks |= bitBoard << 6; // move 1 down, 2 left
                    }

            }
 
            // check if knight is not on H-file and can move to the right
            if ((bitBoard & HFile) == 0 ) 
            {
                attacks |= bitBoard >> 15; // move 2 up, 1 right
                attacks |= bitBoard << 17; // move 2 down, 1 right

                // check if knight is not on G-file and can move two squares to the right
                if ((bitBoard & GFile) == 0) 
                {
                    attacks |= bitBoard >> 6; // move 1 up, 2 right
                    attacks |= bitBoard << 10; // move 1 down, 2 right

                }

            }
            
            return attacks;
        }

    }
}
