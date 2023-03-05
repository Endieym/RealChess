using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using static RealChess.Controller.BoardController;

namespace RealChess.Model
{
    public class Player
    {
        Dictionary<int, ChessPiece> pieces;
        List<ChessPiece> captures;
        public Dictionary<int, ChessPiece> Pieces 
        {
            get { return pieces; } 
        }
        public List<ChessPiece> Captures
        {
            get { return captures; }
        }
        public Player(bool white)
        {
            pieces = new Dictionary<int, ChessPiece>();
            if (white) Init(Board.SIZE - 1, Board.SIZE - 2);
            else       Init(0, 1);

            captures = new List<ChessPiece>();

        }

        // Deletes a piece of a specific key from the dictionary
        public void DeletePiece(int key)
        {
            this.pieces.Remove(key);
        }

        // Updates a pieces' location (key) in the dictionary
        public void UpdatePiece(int oldKey, int newKey)
        {
            var piece = pieces[oldKey];
            piece.UpdatePosition(newKey);
            this.pieces.Remove(oldKey);
            this.pieces.Add(newKey, piece);
        }

        // Adds a captured piece to the list of captured pieces
        public void AddCapture(ChessPiece pieceCaptured)
        {
            captures.Add(pieceCaptured);
        }

        // Initializes the dictionary
        private void Init(int row, int rowPawns)
        {
            for (int col = 0; col < Board.SIZE; col++)
            {
                int keyPawn = rowPawns * Board.SIZE + col;
                int keyPiece = row * Board.SIZE + col;
                pieces.Add(keyPawn, GetPieceForSquare(rowPawns, col));
                pieces.Add(keyPiece, GetPieceForSquare(row, col));

            }
            
        }
    }
}