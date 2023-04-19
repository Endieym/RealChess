using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
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

        public void UpdatePosition(int key)
        {
            this.bitBoard = (ulong)1 << key;
        }

        public void UpdatePosition(int row, int col)
        {
            int key = row * Board.SIZE + col;
            this.bitBoard = (ulong)1 << key;
        }


        public abstract UInt64 GenerateLegalMoves(ulong occupied);
        
        public bool IsUnderAttack(ulong attackingMask)
        {
            if ((attackingMask & bitBoard) != 0)
                return true;

            return false;
        }
        
        public ulong GetPosition()
        {
            return this.bitBoard;
        }

        public int CompareTo(ChessPiece other)
        {
            if(other == null) return 1;

            return this.Value.CompareTo(other.Value);
        }

        //public virtual List<int> GetMovesList()
        //{
        //    // Initialize the moves list
        //    List<int> list = new List<int>();

        //    ulong movesMask = this.GetMoves();
        //    // checks for legal moves using the moves bitmask
        //    for(int i =0; i < 64; i++)
        //    {
        //        if((movesMask & 1) > 0)
        //            list.Add(i);
        //        // shift by one bit, to check the next bit
        //        movesMask >>= 1;

        //        // If mask is 0, there are no more legal moves
        //        if (movesMask == 0)
        //            break;
        //    }
        //    return list;
        //}

      
        public enum PieceColor { WHITE, BLACK }
        public enum PieceType { PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING }

        public PieceColor Color{ get; set; }
        public virtual PieceType Type{ get; set; }
        
    
        }
    }
