using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public Pawn()
        {
            this.Type = PieceType.PAWN;

        }

        public Pawn(int row, int col) : base(row, col)
        {
            this.Type = PieceType.PAWN;
            
        }
        public ulong GetCaptures()
        {
            // The bitmask which represents the move
            UInt64 moveMask = this.bitBoard;

            // Checks for the color of the chess piece
            // and generates moves accordingly, also checks if pawn has moved
            // once in the game, if not, pawn can move two squares
            if(this.Color == PieceColor.WHITE)
            {
                moveMask >>= 8;
                moveMask = moveMask <<1;
                moveMask |= moveMask >>2;
            }
            return moveMask;
            
        
        }
            


        public override ulong GetMoves()
        {
            // The bitmask which represents the move
            UInt64 moveMask = this.bitBoard;

            // Checks for the color of the chess piece
            // and generates moves accordingly, also checks if pawn has moved
            // once in the game, if not, pawn can move two squares
            ulong doubleMoveMask = 0;
            ulong singleMoveMask = this.Color == PieceColor.WHITE ? moveMask >> 8:
                moveMask << 8;

            if (!HasMoved)
            {
                doubleMoveMask = this.Color == PieceColor.WHITE ? moveMask >> 16 :
                moveMask << 16;
            }

            
            //if (this.Color == PieceColor.WHITE)
            //{
            //    // Goes up in the bitboard
            //    moveMask >>= 8;
            //    if (!HasMoved)
            //        moveMask |= moveMask >> 8;
            //}
            //else
            //{
            //    // Goes down in the bitboard
            //    moveMask <<= 8;
            //    if (!HasMoved)
            //        moveMask |= moveMask << 8;

            //}


            return singleMoveMask | doubleMoveMask;
        }
    }
}
