using System;

namespace RealChess.Model.ChessPieces
{
    /// <summary>
    /// The abstract class ChessPiece represents the behavior and properties of a chess piece in the game.
    /// </summary>
    public abstract class ChessPiece : IComparable<ChessPiece>
    {
        protected UInt64 bitBoard;
        public bool HasMoved { get; set; }

        public virtual ushort Value { get; set; }
        public ChessPiece()
        {
        }

        public ChessPiece(int row, int col)
        {
            UpdatePosition(row,col);
        }

        public ChessPiece(int key)
        {
            
            this.bitBoard = (UInt64)1 << key;
        }

        /// <summary>
        /// Updates the position of the chess piece with a specified key.
        /// </summary>
        /// <param name="key">The key of the square on which the piece is located.</param>

        public void UpdatePosition(int key)
        {
            this.bitBoard = (ulong)1 << key;
        }

        /// <summary>
        /// Updates the position of the chess piece with a specified row and column.
        /// </summary>
        /// <param name="row">The row on which the piece is located.</param>
        /// <param name="col">The column on which the piece is located.</param>
        public void UpdatePosition(int row, int col)
        {
            int key = row * Board.SIZE + col;
            this.bitBoard = (ulong)1 << key;
        }

        /// <summary>
        /// Generates the legal moves that a chess piece can make given the occupied squares on the board.
        /// </summary>
        /// <param name="occupied">The bitboard representing the occupied squares on the board.</param>
        /// <returns>A bitboard representing the legal moves that the chess piece can make.</returns>
        public abstract UInt64 GenerateLegalMoves(ulong occupied);

        /// <summary>
        /// Determines if a chess piece is under attack by an attacking mask.
        /// </summary>
        /// <param name="attackingMask">The bitboard representing the attacking pieces.</param>
        /// <returns>True if the chess piece is under attack, otherwise false.</returns>
        public bool IsUnderAttack(ulong attackingMask)
        {
            if ((attackingMask & bitBoard) != 0)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the position of the chess piece on the board as a bitboard.
        /// </summary>
        /// <returns>A bitboard representing the position of the chess piece on the board.</returns>
        public ulong GetPosition()
        {
            return this.bitBoard;
        }

        /// <summary>
        /// Compares the value of this chess piece to another, by value attribute
        /// </summary>
        /// <param name="other">The other piece to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public int CompareTo(ChessPiece other)
        {
            if(other == null) return 1;

            return this.Value.CompareTo(other.Value);
        }

        public enum PieceColor { WHITE, BLACK }
        public enum PieceType { PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING }

        public PieceColor Color{ get; set; }
        public virtual PieceType Type{ get; set; }
        
    
        }
    }
