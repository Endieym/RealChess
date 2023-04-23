using RealChess.Model.ChessPieces;
using System;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model
{
    /// <summary>
    /// Represents a single move in a chess game.
    /// Contains various properties, original  & target squares, pieceMoved, pieceCaptured,
    /// Move type, is check, is capture
    /// </summary>
    public class Move
    {
        public ChessPiece PieceMoved { get; set; }
        public ChessPiece CapturedPiece { get; set; }
        public PieceType PromotedPiece { get; set; }

        public int StartSquare { get; set; }
        public int EndSquare { get; set; }
        public bool IsCapture { get; set; }
        public bool IsEnPassantCapture { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCheck { get; set; }
        public bool DefendsCheck { get; set; }
        public bool IsQueenSideCastle { get; set; }
        public bool IsKingSideCastle { get; set; }
        public bool IsPositiveCapture { get; set; }
        public bool IsDrawByRepetiton { get; set; }
        public bool IsDrawByDeadPosition { get; set; }
        public bool IsStalemate { get; set; }

        /// <summary>
        /// Enum representing move types.
        /// </summary>
        public enum MoveType
        {
            Normal,
            Capture,
            Check,
            Checkmate,
            Draw,
            Castle
        }
        public MoveType Type { get; set; }

        public Move() { }
   
        public Move(int endSquare, ChessPiece piece)
        {
            EndSquare = endSquare;
            PieceMoved = piece;
            StartSquare = (int)Math.Log(piece.GetPosition(), 2);
            Type = MoveType.Normal;
        }
    }
}
