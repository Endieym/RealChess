using RealChess.Model.ChessPieces;
using System.Collections.Generic;
using static RealChess.Controller.BoardController;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.RealConstants;
namespace RealChess.Model
{
    /// <summary>
    /// Represents a player in a chess game, with a dictionary of chess pieces.
    /// </summary>
    public class Player
    {
        Dictionary<int, ChessPiece> pieces;

        int kingPos;
        public int Morale { get; set; }

        public Dictionary<int, ChessPiece> Pieces 
        {
            get { return pieces; } 
        }
       
        public Player(bool white)
        {
            pieces = new Dictionary<int, ChessPiece>();
            if (white) Init(Board.SIZE - 1, Board.SIZE - 2);
            else       Init(0, 1);

        }

        /// <summary>
        /// Returns the king piece of the player.
        /// </summary>
        /// <returns>The king piece of the player.</returns>
        internal King GetKing()
        {
            return (King)this.pieces[kingPos];
        }

        /// <summary>
        /// Returns the position of the king piece in the dictionary.
        /// </summary>
        /// <returns>The position of the king piece in the dictionary.</returns>
        internal int GetKingPos()
        {
            return kingPos;
        }

        /// <summary>
        /// Returns true if the player's king is in check, false if else.
        /// </summary>
        /// <param name="attacks">The bitboard of the enemy's possible attacks.</param>
        /// <returns>True if the player's king is in check, false if else.</returns>
        internal bool InCheck(ulong attacks)
        {
            return this.GetKing().IsUnderAttack(attacks);
        }

        /// <summary>
        /// Returns the bitboard of the player's current possible attacks.
        /// </summary>
        /// <param name="enemySquares">The bitboard of the enemy's pieces.</param>
        /// <param name="occuppied">The bitboard of all occupied squares.</param>
        /// <returns>The bitboard of the player's current possible attacks.</returns>

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

        /// <summary>
        /// Returns the bitboard of the player's current possible moves, excluding friendly squares.
        /// </summary>
        /// <param name="friendlySquares">Bitboard of friendly squares</param>
        /// <param name="occupied">Bitboard of occupied squares</param>
        /// <returns>Bitboard of possible moves</returns>
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


        /// <summary>
        /// Switches the piece in the dictionary at the given key to the new piece.
        /// </summary>
        /// <param name="key">The key to the piece to be switched.</param>
        /// <param name="piece">The new piece to switch to.</param>
        public void SwitchPiece(int key, ChessPiece piece)
        {
            this.pieces.Remove(key);
            this.pieces.Add(key, piece);
        }


        /// <summary>
        /// Deletes the piece at the given key from the dictionary.
        /// </summary>
        /// <param name="key">The key to the piece to be deleted.</param>
        public void DeletePiece(int key)
        {
            this.pieces.Remove(key);
        }

        /// <summary>
        /// Updates the key of a piece in the dictionary.
        /// </summary>
        /// <param name="oldKey">The old key of the piece to be updated.</param>
        /// <param name="newKey">The new key of the piece to be updated.</param>
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

        /// <summary>
        /// Adds morale points for a successful En Passant capture.
        /// </summary>
        public void EnPassant()
        {
            // The max morale is 100.
            if (Morale + EnPassantMorale > 100)
            {
                Morale = 100;
                return;
            }
            Morale += EnPassantMorale;
        }


        /// <summary>
        /// Increases morale by a constant for a successful capture.
        /// </summary>
        public void IncreaseMorale()
        {
            // The max morale is 100.
            if (Morale + SuccessfullCapture > 100)
            {
                Morale = 100;
                return;
            }
            Morale += SuccessfullCapture;
        }

        /// <summary>
        /// Decreases morale by a constant for a capture.
        /// </summary>
        public void DecreaseMorale()
        {
            // The minimum morale is 50.
            if (Morale - Captured < 50)
            {
                Morale = 50;
                return;
            }
            Morale -= Captured;
        }

        /// <summary>
        /// Adds a piece to the dictionary at the given key.
        /// </summary>
        /// <param name="piece">The piece to add.</param>
        /// <param name="key">The key to add the piece at.</param>
        public void AddPiece(ChessPiece piece, int key)
        {
            this.pieces.Add(key, piece);
        }

        /// <summary>
        /// Initializes the dictionary by adding pieces at their starting positions.
        /// </summary>
        /// <param name="row">The row of the non-pawn pieces.</param>
        /// <param name="rowPawns">The row of the pawns.</param>
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