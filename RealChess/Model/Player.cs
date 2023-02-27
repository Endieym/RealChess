using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using static RealChess.Controller.BoardController;

namespace RealChess.Model
{
    public class Player
    {
        Dictionary<int, ChessPiece> pieces;

        public Dictionary<int, ChessPiece> Pieces 
        {
            get { return pieces; } 
        }
        public Player(bool white)
        {
            pieces = new Dictionary<int, ChessPiece>();
            if (white) init(Board.SIZE - 1, Board.SIZE - 2);
            else       init(0, 1);

        }

        private void init(int row, int rowPawns)
        {
            for (int col = 0; col < Board.SIZE; col++)
            {
                int keyPawn = rowPawns * Board.SIZE + col;
                int keyPiece = row * Board.SIZE + col;
                pieces.Add(keyPawn, GetPieceForSquare(row, col));
                pieces.Add(keyPiece, GetPieceForSquare(rowPawns, col));

            }
        }
    }
}