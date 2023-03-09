﻿using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model
{
    internal static class BoardOperations
    {
        
        // Returns the bitmask of the possible en passant.
        // if en passant is not available, returns 0
        public static ulong GenerateEnPassant(Pawn pawn, Move lastMove)
        {
            if (lastMove.PieceMoved.Color == pawn.Color)
                return 0;

            // Checks if the last move was a double pawn move 
            if (lastMove.PieceMoved.Type == PieceType.PAWN &&
                Math.Abs(lastMove.EndSquare - lastMove.StartSquare) == 16)
            {
                // Returns the bitmask for the en passant
                return ((Pawn)lastMove.PieceMoved).GoBack() & pawn.GetCaptures();
            }
            return 0;
        }
        public static ulong FileMask(int file)
        {
            return 0x0101010101010101UL << file;
        }

        public static ulong RankMask(int rank)
        {
            return 0xFFUL << (rank * 8);
        }

        public static int UInt64ToKey(ulong position)
        {
            return (int)Math.Log(position, 2);
        }

        public static ulong GenerateDiagonals(ulong position)
        {
            return 0;

        }
        public static ulong GenerateLines(ulong position, ulong occupied)
        {

            ulong moves = 0;
            moves |= RookMovesInDirection(position, occupied, 0);
            moves |= RookMovesInDirection(position, occupied, 1);
            moves |= RookMovesInDirection(position, occupied, 2);
            moves |= RookMovesInDirection(position, occupied, 3);
            return moves;

        }

        public static ulong RookMovesInDirection(ulong squareIndex, ulong occupiedSquares, int direction)
        {
            //ulong fileMask = FileMask(squareIndex % 8);
            //ulong rankMask = RankMask(squareIndex / 8);

            switch (direction)
            {
                case 0: // North
                    return SlideNorth(squareIndex, occupiedSquares);
                case 1: // East
                    return SlideEast(squareIndex, occupiedSquares);
                case 2: // South
                    return SlideSouth(squareIndex, occupiedSquares);
                case 3: // West
                    return SlideWest(squareIndex, occupiedSquares);
                default:
                    return 0;
            }
        }

        // Generates moves to the north
        public static ulong SlideNorth(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            // while in the board 
            while (square > 0)
            {
                square >>= 8;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) break;
     
            }
            return attacks;
        }


        // Generates moves to the east
        public static ulong SlideEast(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.HFile;
            if ((square & BitboardConstants.HFile) != 0)
                return 0;

            while (square != 0)
            {
                square <<= 1;
                attacks |= square;
                if ((square & blockers) != 0)
                    break;

            }
            // while not reached an occupied square, and not reached the end of the rank

            return attacks;
        }

        // Generates moves to the south
        public static ulong SlideSouth(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            // while in the board 
            while (square > 0)
            {
                square <<= 8;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) break;
            }
            return attacks;
        }

        // Generates moves to the west
        public static ulong SlideWest(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.AFile;
            if ((square & BitboardConstants.AFile) != 0)
                return 0;

            while(square!=0)
            {
                square >>= 1;
                attacks |= square;
                if((square & blockers) != 0)
                     break;

            }
            // while not reached an occupied square, and not reached the end of the rank

            return attacks;
        }
    }
}