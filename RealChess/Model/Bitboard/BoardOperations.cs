using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.Bitboard
{
    internal static class BoardOperations
    {

        /// <summary>
        /// Returns the bitmask of the possible en passant for a pawn.
        /// </summary>
        /// <param name="pawn">The pawn for which to generate the en passant bitmask.</param>
        /// <param name="lastMove">The last move made in the game.</param>
        /// <returns>The bitmask of the possible en passant.</returns>
        /// <remarks>If en passant is not available, returns 0.</remarks>
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

        /// <summary>
        /// Returns a tuple of two rooks from the player's pieces that haven't moved, given the dictionary of pieces and the color of the player.
        /// </summary>
        /// <param name="pieces">A dictionary containing the player's pieces.</param>
        /// <param name="color">The color of the player whose pieces are being examined.</param>
        /// <returns>A tuple containing two rooks from the player's pieces.
        /// If the player doesn't have two unmoved rooks,
        /// the respective tuple elements will be null.</returns>
        public static Tuple<Rook, Rook> GetRooksCastle(Dictionary<int, ChessPiece> pieces, PieceColor color)
        {
            var queenKey = color == PieceColor.WHITE ? BitboardConstants.whiteQueenRook :
                BitboardConstants.blackQueenRook;
            var kingKey = color == PieceColor.WHITE ? BitboardConstants.whiteKingRook :
                BitboardConstants.blackKingRook;
            Rook firstRook = null, secondRook = null;

            if (pieces.TryGetValue(queenKey, out ChessPiece queenSide))
            {
                if (queenSide.Type == PieceType.ROOK)
                    firstRook = (Rook)pieces[queenKey];
            }
            if (pieces.TryGetValue(kingKey, out ChessPiece kingSide))
            {
                if (kingSide.Type == PieceType.ROOK)
                    secondRook = (Rook)pieces[kingKey];
            }
            return new Tuple<Rook, Rook>(firstRook, secondRook);
        }

        /// <summary>
        /// Returns a tuple of two rooks from the player's pieces, regardless of whether they have moved or not.
        /// </summary>
        /// <param name="pieces">A dictionary of the player's pieces with their corresponding squares.</param>
        /// <param name="color">The color of the player's pieces.</param>
        /// <returns>A tuple containing two Rook objects representing the player's rooks.</returns>
        public static Tuple<Rook, Rook> GetRooks(Dictionary<int, ChessPiece> pieces, PieceColor color)
        {
            Rook firstRook = null, secondRook = null;

            foreach (var piece in pieces)
            {

                if (piece.Value.Type == PieceType.ROOK)
                {
                    if (firstRook == null)
                        firstRook = (Rook)piece.Value;
                    else
                        secondRook = (Rook)piece.Value;
                }
            }

            return new Tuple<Rook, Rook>(firstRook, secondRook);
        }




        /// <summary>
        /// Returns the bitmask of a possible castle.
        /// If a castle is not available, returns a tuple of two empty bitmasks.
        /// </summary>
        /// <param name="king">The King chess piece.</param>
        /// <param name="pieces">A dictionary of all pieces on the chess board.</param>
        /// <param name="blocked">A bitboard representing all occupied squares on the chess board.</param>
        /// <returns>A tuple of two bitmasks representing the possible castles 
        /// on the queen side and king side respectively.</returns>
        public static Tuple<ulong, ulong> GenerateCastle(King king, Dictionary<int, ChessPiece> pieces, ulong blocked)
        {
            ulong queenSide = 0;
            ulong kingSide = 0;

            if (king.HasMoved || king.InCheck)
            {
                return new Tuple<ulong, ulong>(queenSide, kingSide);
            }

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
                if (rookPair.Item2 != null && !rookPair.Item2.HasMoved &&
                    (BitboardConstants.BlackKingSide & blocked) == 0)
                {
                    kingSide |= king.GetPosition() << 2;
                }
            }

            return new Tuple<ulong, ulong>(queenSide, kingSide);
        }

        /// <summary>
        /// Determines whether two chess pieces are on the same color square or not.
        /// </summary>
        /// <param name="pieceA">The first chess piece.</param>
        /// <param name="pieceB">The second chess piece.</param>
        /// <returns>True if both chess pieces are on the same color square, false otherwise.</returns>
        public static bool OnSameColor(ChessPiece pieceA, ChessPiece pieceB)
        {
            if (pieceA is null || pieceB is null)
                return false;

            var posA = pieceA.GetPosition();
            var posB = pieceB.GetPosition();

            bool sameColor = false;

            if ((posA & BitboardConstants.lightSquares) > 0 && (posB & BitboardConstants.lightSquares) > 0)
                sameColor = true;

            else if((posA & BitboardConstants.darkSquares) > 0 && (posB & BitboardConstants.darkSquares) > 0)
                sameColor = true;

            return sameColor;
        }

        /// <summary>
        /// Returns a list of all the pawns of a given color on the board.
        /// </summary>
        /// <param name="board">The chess board object.</param>
        /// <param name="color">The color of the pawns to be retrieved.</param>
        /// <returns>A list of all the pawns of the given color on the board.</returns>
        public static List<Pawn> GetAllPawns(Board board, PieceColor color)
        {
            var pieces = (color == PieceColor.WHITE ? board.GetPlayer1() :
                board.GetPlayer2()).Pieces.Values.ToList();

            List<Pawn> pawns = pieces.Where(p => p.Type == PieceType.PAWN)
                         .Select(p => (Pawn)p)
                         .ToList();

            return pawns;
        }


        /// <summary>
        /// Converts the given row and column numbers to a string in chess format (e.g. "a1", "h7", etc.).
        /// </summary>
        /// <param name="row">The row number (0-7) of the square.</param>
        /// <param name="col">The column number (0-7) of the square.</param>
        /// <returns>A string representing the position in chess format.</returns>
        public static string GetPositionString(int row, int col)
        {
            // Convert the row and column to their corresponding letter and number, respectively
            var colLetter = (char)('a' + col);
            var rowNumber = 8 - row;

            // Concatenate the letter and number to create the position string
            return $"{colLetter}{rowNumber}";
        }

        /// <summary>
        /// Returns the position in string for a given chess piece.
        /// </summary>
        /// <param name="color">The color of the chess piece</param>
        /// <param name="type">The type of the chess piece</param>
        /// <param name="key">The key value of the chess piece on the board</param>
        /// <returns>The position string</returns>
        public static string GetPositionStringByPos(PieceColor color,PieceType type, int key)
        {
            int row = key / 8;
            int col = key % 8;

            char colorChar = color == PieceColor.WHITE ? 'W' : 'B';

            return $"{colorChar}{type.ToString().Substring(0, 1)}{GetPositionString(row, col)}";
        }

        /// <summary>
        /// Returns the position of a chess piece in chess format, such as Ra1, Ka5, etc.
        /// </summary>
        /// <param name="piece">The chess piece for which to get the position.</param>
        /// <returns>A string representing the position of the chess piece in chess format.</returns>
        public static string GetPiecePositionString(ChessPiece piece)
        {
            int key = (int)Math.Log(piece.GetPosition(),2);

            return GetPositionStringByPos(piece.Color, piece.Type, key);
        }


        /// <summary>
        /// Generates a string representing the current state of the chess board by concatenating the position strings of all pieces on the board.
        /// </summary>
        /// <param name="board">The current state of the chess board</param>
        /// <returns>A string representing the current state of the chess board</returns>
        public static string GetBoardStateString(Board board)
        {
            var whitePieces = board.WhitePlayer.Pieces;
            var blackPieces = board.BlackPlayer.Pieces;

            string boardState = "";

            foreach (var piece in blackPieces.Values)
                boardState += GetPiecePositionString(piece);

            foreach (var piece in whitePieces.Values)
                boardState += GetPiecePositionString(piece);

            return boardState;
        }

        /// <summary>
        /// Returns the board state after making a move in string format, where each piece is represented by its position in chess format (Ra1, Ka5, etc.).
        /// </summary>
        /// <param name="move">The move to be made.</param>
        /// <param name="board">The current board state.</param>
        /// <returns>The board state after making the move in string format.</returns>
        public static string GetBoardStateAfterMove(Move move, Board board)
        {
            string boardState = GetBoardStateString(board);
            var pieceMoved = move.PieceMoved;

            string pieceString = GetPiecePositionString(pieceMoved);
            string pieceStringAfterMove = GetPositionStringByPos(pieceMoved.Color, pieceMoved.Type, move.EndSquare);

            return boardState.Replace(pieceString, pieceStringAfterMove);
        }


        /// <summary>
        /// Checks if a chess piece is blocked by a given bit mask.
        /// </summary>
        /// <param name="piece">The chess piece to check.</param>
        /// <param name="bitmask">The bit mask to check against.</param>
        /// <returns>True if the chess piece is blocked by the bit mask, false otherwise.</returns>
        public static bool IsBlockedByPieces(ChessPiece piece, ulong bitmask)
        {
            ulong pieceMask = piece.GetPosition();

            bool onSameFile = false;

            while (pieceMask > 0 && !onSameFile)
            {
                onSameFile = IsBlocked(piece.Color, pieceMask, bitmask);
                pieceMask &= pieceMask - 1;
            }
            return onSameFile;
        }

        /// <summary>
        /// Checks if the given position is blocked by the given bitmask, in the same file as the position.
        /// </summary>
        /// <param name="pieceColor">The color of the chess piece.</param>
        /// <param name="piecePosition">The position of the chess piece in a bitboard format.</param>
        /// <param name="bitmask">The bitboard representing the blockages.</param>
        /// <returns>True if the position is blocked, otherwise false.</returns>
        public static bool IsBlocked(PieceColor pieceColor, ulong piecePosition, ulong bitmask)
        {
            int file = UInt64ToKey(piecePosition) % 8;

            if ((FileMask(file) & bitmask) == 0)
                return false;

            ulong path = pieceColor == PieceColor.WHITE ? SlideNorth(piecePosition, 0) :
                SlideSouth(piecePosition, 0);

            if ((path & bitmask) > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns the rank (row) of the chess piece on the board.
        /// </summary>
        /// <param name="piece">The chess piece to get the rank of.</param>
        /// <returns>The rank of the chess piece.</returns>
        public static int GetRank(ChessPiece piece)
        {
            return UInt64ToKey(piece.GetPosition()) / 8;
        }

        /// <summary>
        /// Gets the file index of the given chess piece's position.
        /// </summary>
        /// <param name="piece">The chess piece whose position's file is to be retrieved.</param>
        /// <returns>The file index (0-7) of the given chess piece's position.</returns>
        public static int GetFile(ChessPiece piece)
        {
            return UInt64ToKey(piece.GetPosition()) % 8;
        }

        /// <summary>
        /// Calculates the distance (in squares) between two chess pieces.
        /// </summary>
        /// <param name="pieceA">The first piece.</param>
        /// <param name="pieceB">The second piece.</param>
        /// <returns>The number of squares between the two pieces (in a straight line).</returns>
        public static int GetDistancePieces(ChessPiece pieceA, ChessPiece pieceB)
        {
            int pieceARank = BoardOperations.GetRank(pieceA);
            int pieceAFile = BoardOperations.GetFile(pieceA);

            int pieceBRank = BoardOperations.GetRank(pieceB);
            int pieceBFile = BoardOperations.GetFile(pieceB);

            int dstBetweenPiecesRank = Math.Abs(pieceARank - pieceBRank);
            int dstBetweenPiecesFile = Math.Abs(pieceAFile - pieceBFile);

            return dstBetweenPiecesRank + dstBetweenPiecesFile;
        }

        /// <summary>
        /// Returns an bitmask representing the entire file
        /// </summary>
        /// <param name="file">The file index (0 to 7) to get the mask for.</param>
        /// <returns>The bitmask</returns>
        public static ulong FileMask(int file)
        {
            return 0x0101010101010101UL << file;
        }

        /// <summary>
        /// Returns an bitmask representing the entire rank
        /// </summary>
        /// <param name="file">The rank index (0 to 7) to get the mask for.</param>
        /// <returns>The bitmask</returns>
        public static ulong RankMask(int rank)
        {
            return 0xFFUL << (rank * 8);
        }

        /// <summary>
        /// Returns the opposite color of the input color.
        /// </summary>
        /// <param name="color">The color of the piece.</param>
        /// <returns>The opposite color of the input color.</returns>
        public static PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;
        }

        /// <summary>
        /// Convert a position represented by a 64-bit unsigned integer to its corresponding key.
        /// </summary>
        /// <param name="position">The position to convert</param>
        /// <returns>The corresponding key</returns>
        public static int UInt64ToKey(ulong position)
        {
            return (int)Math.Log(position, 2);
        }

        /// <summary>
        /// Generates a bitboard representing the positions of a list of pieces.
        /// </summary>
        /// <param name="pieces">A list of chess pieces</param>
        /// <returns>A bitboard with bits set in positions corresponding to the positions of the pieces</returns>
        public static ulong GetPiecesPositions(List<ChessPiece> pieces)
        {
            ulong bitboard = 0;

            foreach (var piece in pieces)
            {
                bitboard |= piece.GetPosition();
            }

            return bitboard;
        }

        /// <summary>
        /// Generates a bitboard with all the possible diagonal moves from a given position.
        /// </summary>
        /// <param name="position">The position to generate the moves from.</param>
        /// <param name="occupied">The bitboard representing the positions of all the occupied squares on the board.</param>
        /// <returns>A bitboard representing all the possible diagonal moves from the given position.</returns>
        public static ulong GenerateDiagonals(ulong position, ulong occupied)
        {
            ulong moves = 0;
            moves |= BishopMovesInDirection(position, occupied, 0);
            moves |= BishopMovesInDirection(position, occupied, 1);
            moves |= BishopMovesInDirection(position, occupied, 2);
            moves |= BishopMovesInDirection(position, occupied, 3);
            return moves;
        }

        /// <summary>
        /// Generates all possible moves of a rook at the given position on the board
        /// based on the pieces that are currently occupying the board.
        /// </summary>
        /// <param name="position">The position of the rook on the board represented as a bitboard.</param>
        /// <param name="occupied">A bitboard representing the positions of all pieces on the board.</param>
        /// <returns>A bitboard with all possible moves of the rook at the given position.</returns>
        public static ulong GenerateLines(ulong position, ulong occupied)
        {
            ulong moves = 0;
            moves |= RookMovesInDirection(position, occupied, 0);
            moves |= RookMovesInDirection(position, occupied, 1);
            moves |= RookMovesInDirection(position, occupied, 2);
            moves |= RookMovesInDirection(position, occupied, 3);
            return moves;
        }

        /// <summary>
        /// Returns a bitboard with all possible bishop moves in a given direction from the given square.
        /// </summary>
        /// <param name="squareIndex">The index of the square the bishop is on.</param>
        /// <param name="occupiedSquares">A bitboard representing the positions of all occupied squares on the board.</param>
        /// <param name="direction">An integer representing the direction the bishop should move in.
        /// 0 = NorthEast, 1 = SouthEast, 2 = SouthWest, 3 = NorthWest</param>
        /// <returns>A bitboard with all possible bishop moves in the given direction from the given square.</returns>
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

        /// <summary>
        /// Returns a bitboard with all possible rook moves in a given direction from the given square.
        /// </summary>
        /// <param name="squareIndex">The index of the square the rook is on.</param>
        /// <param name="occupiedSquares">A bitboard representing the positions of all occupied squares on the board.</param>
        /// <param name="direction">An integer representing the direction the rook should move in.
        /// 0 = North, 1 = East, 2 = South, 3 = West</param>
        /// <returns>A bitboard with all possible rook moves in the given direction from the given square.</returns>
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
        

        /// <summary>
        /// Generates moves to the north west direction for a given square and a set of occupied squares (blockers).
        /// </summary>
        /// <param name="square">The mask of the square to generate moves for.</param>
        /// <param name="blockers">A bitboard containing the mask occupied squares.</param>
        /// <returns>A bitboard containing the generated moves.</returns>
        public static ulong SlideNorthWest(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.AFile;
            if ((square & BitboardConstants.AFile) != 0)
                return 0;
            bool reachedOccupied = false;

            // while in the board 
            while (square > 0 && !reachedOccupied)
            {
                square >>= 9;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) reachedOccupied = true;

            }
            return attacks;
        }

        /// <summary>
        /// Generates all possible moves to the north for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the north for the given square.</returns>
        public static ulong SlideNorth(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            bool reachedOccupied = false;

            // while in the board 
            while (square > 0 && !reachedOccupied)
            {
                square >>= 8;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) reachedOccupied = true;

            }
            return attacks;
        }

        /// <summary>
        /// Generates all possible moves to the north east for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the north east for the given square.</returns>
        public static ulong SlideNorthEast(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.HFile;
            if ((square & BitboardConstants.HFile) != 0)
                return 0;
            bool reachedOccupied = false;

            // while in the board 
            while (square > 0 && !reachedOccupied)
            {
                square >>= 7;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) reachedOccupied = true;

            }
            return attacks;
        }


        /// <summary>
        /// Generates all possible moves to the east for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the east for the given square.</returns>
        public static ulong SlideEast(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.HFile;
            if ((square & BitboardConstants.HFile) != 0)
                return 0;
            bool reachedOccupied = false;

            while (square != 0 && !reachedOccupied)
            {
                square <<= 1;
                attacks |= square;
                if ((square & blockers) != 0)
                    reachedOccupied = true;

            }
            // while not reached an occupied square, and not reached the end of the rank

            return attacks;
        }

        /// <summary>
        /// Generates all possible moves to the south for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the south for the given square.</returns>
        public static ulong SlideSouth(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            bool reachedOccupied = false;

            // while in the board 
            while (square > 0 && !reachedOccupied) 
            {
                square <<= 8;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) reachedOccupied = true;
            }
            return attacks;
        }

        /// <summary>
        /// Generates all possible moves to the south east for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the south east for the given square.</returns>
        public static ulong SlideSouthEast(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.HFile;
            if ((square & BitboardConstants.HFile) != 0)
                return 0;
            bool reachedOccupied = false;

            // while in the board 
            while (square > 0 && !reachedOccupied)
            {
                square <<= 9;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) reachedOccupied = true;

            }
            return attacks;
        }

        /// <summary>
        /// Generates all possible moves to the west for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the west for the given square.</returns>
        public static ulong SlideWest(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.AFile;
            if ((square & BitboardConstants.AFile) != 0)
                return 0;
            bool reachedOccupied = false;

            while (square != 0 && !reachedOccupied)
            {
                square >>= 1;
                attacks |= square;
                if ((square & blockers) != 0)
                    reachedOccupied = true;

            }
            // while not reached an occupied square, and not reached the end of the rank

            return attacks;
        }

        /// <summary>
        /// Generates all possible moves to the south west for a given square.
        /// </summary>
        /// <param name="square">The starting square.</param>
        /// <param name="blockers">A bitboard representing the squares that are occupied.</param>
        /// <returns>A bitboard representing all possible moves to the south west for the given square.</returns>

        public static ulong SlideSouthWest(ulong square, ulong blockers)
        {
            ulong attacks = 0;
            blockers |= BitboardConstants.AFile;
            if ((square & BitboardConstants.AFile) != 0)
                return 0;

            bool reachedOccupied = false;
            // while in the board 
            while (square > 0 && !reachedOccupied)
            {
                square <<= 7;
                attacks |= square;

                // while not reached an occupied square
                if ((square & blockers) != 0) reachedOccupied = true;

            }
            return attacks;
        }
    }
}
