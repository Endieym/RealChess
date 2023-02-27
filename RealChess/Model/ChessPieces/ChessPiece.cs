using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    public abstract class ChessPiece
    {
        protected UInt64 bitBoard;
        public ChessPiece()
        {
        }

        public ChessPiece(int row, int col)
        {
            int key = row * Board.SIZE + col;
            this.bitBoard = (UInt64)1 << key;
        }

        public abstract UInt64 GetMoves();
        

        public enum PieceColor { WHITE, BLACK }
        public enum PieceType { PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING }

        public PieceColor Color{ get; set; }
        public PieceType Type{ get; set; }
        
    
        }
    }
