using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    public abstract class ChessPiece
    {
        public ChessPiece()
        {
        }

        public ChessPiece(int row, int col)
        {
            
        }

        public enum PieceColor { WHITE, BLACK }
        public enum PieceType { PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING }

        public PieceColor Color{ get; set; }
        public PieceType Type{ get; set; }
        
    
        }
    }
