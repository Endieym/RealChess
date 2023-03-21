﻿using RealChess.Model.ChessPieces;
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
        public bool IsCheck { get; set; }
        public ulong BoardBefore { get; set; }
        public bool DefendsCheck { get; set; }
        public bool IsQueenSideCastle { get; set; }
        public bool IsKingSideCastle { get; set; }

        public enum MoveType
        {
            Normal,
            Capture,
            Check,
            Checkmate,
            Stalemate,
            Castle
        }
        public MoveType Type { get; set; }


        public Move()
        {

        }
        public Move(int endSquare, ChessPiece piece)
        {
            EndSquare = endSquare;
            PieceMoved = piece;
            StartSquare = (int)Math.Log(piece.GetPosition(), 2);
            Type = MoveType.Normal;
        }
    }
}
