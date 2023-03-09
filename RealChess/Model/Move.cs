using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model
{
    public class Move
    {
        public ChessPiece PieceMoved { get; set; }
        public ChessPiece CapturedPiece { get; set; }
        public int StartSquare { get; set; }
        public int EndSquare { get; set; }
        public bool IsCapture { get; set; }
        public bool IsEnPassantCapture { get; set; }
        public bool IsPromotion { get; set; }

        public Move()
        {

        }
        public Move(int endSquare, ChessPiece piece)
        {
            EndSquare = endSquare;
            PieceMoved = piece;
        }
    }
}
