using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using static RealChess.Controller.BoardController;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model
{
    public class Player
    {
        Dictionary<int, ChessPiece> pieces;
        List<ChessPiece> captures;

        int kingPos;
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

        // Returns the king piece of the player
        internal King GetKing()
        {
            return (King)this.pieces[kingPos];
        }

        internal int GetKingPos()
        {
            return kingPos;
        }

        // Returns true if the player's king is in check, false if else
        internal bool InCheck(ulong attacks)
        {
            return this.GetKing().IsUnderAttack(attacks);
        }
        
        // Returns the bitboard of the player's current possible attacks
        public ulong GetAttacks(ulong enemySquares, ulong occuppied)
        {
            ulong attacks = 0;
            foreach (var value in pieces.Values)
            {
                attacks |= value.Type == PieceType.PAWN ? ((Pawn)value).GetCaptures() :
                value.GenerateLegalMoves(occuppied);
            }
            
            attacks ^= ~enemySquares;
            return attacks;
            
        }

        public ulong GetMoves(ulong friendlySquares, ulong ocuppied)
        {
            ulong attacks = 0;
            foreach (var value in pieces.Values)
            {
                attacks += value.Type == PieceType.PAWN ? ((Pawn)value).GetCaptures() :
                value.GenerateLegalMoves(ocuppied);
            }
            attacks ^= friendlySquares;
            return attacks;

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

            // Checks if the piece is a king, if so, update king position for player
            if (piece.Type == ChessPiece.PieceType.KING)
                this.kingPos = newKey;

            // Updates position for the individual piece's bitboard
            piece.UpdatePosition(newKey);
            this.pieces.Remove(oldKey);
            this.pieces.Add(newKey, piece);
        }

        // Adds a captured piece to the list of captured pieces
        public void AddCapture(ChessPiece pieceCaptured)
        {
            captures.Add(pieceCaptured);
        }

        // Adds a chess piece to the dictionary at a given key
        public void AddPiece(ChessPiece piece, int key)
        {
            this.pieces.Add(key, piece);
        }

        // Removes the last captured piece 
        public void RemoveCapture()
        {
            captures.RemoveAt(captures.Count - 1);
        }

        // Initializes the dictionary
        private void Init(int row, int rowPawns)
        {
            for (int col = 0; col < Board.SIZE; col++)
            {
                int keyPawn = rowPawns * Board.SIZE + col;
                int keyPiece = row * Board.SIZE + col;
                pieces.Add(keyPawn, GetPieceForSquare(rowPawns, col));
                ChessPiece pieceToAdd = GetPieceForSquare(row, col);
                pieces.Add(keyPiece, pieceToAdd);

                // Checks if the added piece is a king, if so, init king position
                if (pieceToAdd.Type == ChessPiece.PieceType.KING)
                    this.kingPos = keyPiece;

            }
            
        }
    }
}