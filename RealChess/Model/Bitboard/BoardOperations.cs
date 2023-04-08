using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.Bitboard
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

        // Returns a tuple of two rooks from the player's pieces (only rooks which haven't moved)
        public static Tuple<Rook, Rook> GetRooksCastle(Dictionary<int, ChessPiece> pieces, PieceColor color)
        {
            var queenKey = color == PieceColor.WHITE? BitboardConstants.whiteQueenRook:
                BitboardConstants.blackQueenRook;
            var kingKey = color == PieceColor.WHITE ? BitboardConstants.whiteKingRook :
                BitboardConstants.blackKingRook;
            ChessPiece queenSide, kingSide;
            Rook firstRook = null, secondRook = null;

            if (pieces.TryGetValue(queenKey, out queenSide))
            {
                if(queenSide.Type == PieceType.ROOK)
                    firstRook = (Rook)pieces[queenKey];

            }

            if (pieces.TryGetValue(kingKey, out kingSide))
            {
                if (kingSide.Type == PieceType.ROOK)
                    secondRook = (Rook)pieces[kingKey];

            }

            return new Tuple<Rook, Rook>(firstRook, secondRook);

        }

        // Returns a tuple of two rooks from the player's pieces
        public static Tuple<Rook, Rook> GetRooks(Dictionary<int, ChessPiece> pieces, PieceColor color)
        {
            
            ChessPiece queenSide, kingSide;
            Rook firstRook = null, secondRook = null;

           
            foreach(var piece in pieces)
            {
                
                if(piece.Value.Type == PieceType.ROOK)
                {
                    if (firstRook == null)
                        firstRook = (Rook)piece.Value;
                    else
                        secondRook = (Rook)piece.Value;
                }

            }

            return new Tuple<Rook, Rook>(firstRook, secondRook);



        }

       


        // Returns the bitmask of a possible castle.
        // if a castle is not available, returns 0
        public static Tuple<ulong,ulong> GenerateCastle(King king, Dictionary<int,ChessPiece> pieces, ulong blocked)
        {

            ulong queenSide = 0;
            ulong kingSide = 0;
            
            if (!king.HasMoved && !king.InCheck)
            {
                // Gets the pair of rooks from the player's pieces
                Tuple<Rook, Rook> rookPair = GetRooksCastle(pieces, king.Color);
                 if (king.Color == PieceColor.WHITE)
                {
                    if (rookPair.Item1 != null && !rookPair.Item1.HasMoved &&
                        (BitboardConstants.WhiteQueenSide & blocked) == 0)
                    {
                        queenSide |= king.GetPosition() >> 2;
                    }
                    if (rookPair.Item2 != null && !rookPair.Item2.HasMoved &&
                        (BitboardConstants.WhiteKingSide & blocked) == 0)
                    {
                        kingSide |= king.GetPosition() << 2;

                    }
                }
                else if (king.Color == PieceColor.BLACK)
                {
                    if (rookPair.Item1 != null && !rookPair.Item1.HasMoved &&
                        (BitboardConstants.BlackQueenSide & blocked) == 0)
                    {
                        queenSide |= king.GetPosition() >> 2;
                    }
                    if (rookPair.Item2 != null &&!rookPair.Item2.HasMoved &&
                        (BitboardConstants.BlackKingSide & blocked) == 0)
                    {
                        kingSide |= king.GetPosition() << 2;

                    }
                }
            }
            
            
            return new Tuple<ulong,ulong>(queenSide,kingSide);
        }


        public static ulong FileMask(int file)
        {
            return 0x0101010101010101UL << file;
        }

        public static ulong RankMask(int rank)
        {
            return 0xFFUL << (rank * 8);
        }

        public static PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;

        }

        public static int UInt64ToKey(ulong position)
        {
            return (int)Math.Log(position, 2);
        }

        public static ulong GenerateDiagonals(ulong position, ulong occupied)
        {
            ulong moves = 0;
            moves |= BishopMovesInDirection(position, occupied, 0);
            moves |= BishopMovesInDirection(position, occupied, 1);
            moves |= BishopMovesInDirection(position, occupied, 2);
            moves |= BishopMovesInDirection(position, occupied, 3);
            return moves;

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

        public static ulong BishopMovesInDirection(ulong squareIndex, ulong occupiedSquares, int direction)
        {
            

            switch (direction)
            {
                case 0: // NorthEast
                    return SlideNorthEast(squareIndex, occupiedSquares);
                case 1: // SouthEast
                    return SlideSouthEast(squareIndex, occupiedSquares);
                case 2: // SouthWes
                    return SlideSouthWest(squareIndex, occupiedSquares);
                case 3: // NorthWest
                    return SlideNorthWest(squareIndex, occupiedSquares);
                default:
                    return 0;
            }
        }

        public static ulong RookMovesInDirection(ulong squareIndex, ulong occupiedSquares, int direction)
        {
            

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


        // Generates moves to the north west
        public static ulong SlideNorthWest(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.AFile;
            if ((square & BitboardConstants.AFile) != 0)
                return 0;
            // while in the board 
            while (square > 0)
            {
                square >>= 9;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) break;

            }
            return attacks;
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

        // Generates moves to the north east
        public static ulong SlideNorthEast(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.HFile;
            if ((square & BitboardConstants.HFile) != 0)
                return 0;
            // while in the board 
            while (square > 0)
            {
                square >>= 7;
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

        // Generates moves to the south east
        public static ulong SlideSouthEast(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.HFile;
            if ((square & BitboardConstants.HFile) != 0)
                return 0;
            // while in the board 
            while (square > 0)
            {
                square <<= 9;
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

        // Generates moves to the south west
        public static ulong SlideSouthWest(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.AFile;
            if ((square & BitboardConstants.AFile) != 0)
                return 0;
            // while in the board 
            while (square > 0)
            {
                square <<= 7;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) break;

            }
            return attacks;
        }
    }
}
