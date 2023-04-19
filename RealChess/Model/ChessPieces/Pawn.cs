using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public override PieceType Type { get; set; } = PieceType.PAWN;
        public override ushort Value { get; set; } = 1;
        public Pawn() { }
        
        public Pawn(int key): base(key) { }

        public Pawn(int row, int col) : base(row, col) { }

        public override ulong GenerateLegalMoves(ulong occupied)
        {
            ulong movesMask = GenerateMovesMask();
            // The bit representing the double move (2 squares above)
            ulong doubleMask = this.Color == PieceColor.WHITE ? movesMask & movesMask >> 8:
                movesMask & movesMask << 8;
            
            // Seperates the single move from the double move
            ulong singleMask = movesMask ^ doubleMask;
            
            // If square above is blocked, reset the legal moves
            if((singleMask & occupied) > 0)
            {
                return 0;

            }
            
            // Returns the moves mask minus colliding pieces
            return movesMask & (~occupied);

            
        }

        // Returns the bitboard representing one square back from where the pawn is
        public ulong GoBack()
        {
            return this.Color == PieceColor.WHITE ? this.bitBoard << 8 :
                this.bitBoard >> 8;
        }

        public ulong GetCaptures()
        {
            // The bitmask which represents the move
            UInt64 moveMask = this.bitBoard;

            // Checks for the color of the chess piece
            // and generates moves accordingly, also checks if pawn has moved
            // once in the game, if not, pawn can move two squares

            moveMask = this.Color == PieceColor.WHITE ? moveMask >> 8 : moveMask <<8;

            // Checks if the pawn is not on the A file and can move west
            if ((moveMask & BitboardConstants.AFile) == 0)
            {
                moveMask >>= 1;
                // Checks if the pawn is not on the H file and can move east
                if ((moveMask & BitboardConstants.GFile) == 0) moveMask |= moveMask << 2;

            }
            else
            {
                moveMask <<= 1;
            }

            return moveMask;
            
        
        }

        public ulong GenerateMovesMask()
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

            return singleMoveMask | doubleMoveMask;
        }
    }
}
