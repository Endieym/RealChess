using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.BoardOperations;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.Bitboard
{
    internal static class BoardLogic

    {
        // Gets the pieces of a specific player, and returns whether or not the rooks
        // are connected
        public static bool AreRooksConnected(Dictionary<int, ChessPiece> pieces, PieceColor color, ulong occupied)
        {
            // Gets the rook pair
            Tuple<Rook, Rook> rookPair = GetRooks(pieces, color);

            // If a rook is missing (captured) return false
            if (rookPair.Item1 == null || rookPair.Item2 == null)
                return false;

            // Checks if the rooks are connected via bitboard
            if ((rookPair.Item1.GenerateLegalMoves(occupied) & rookPair.Item2.GetPosition()) > 0)
                return true;

            return false;
        }

        // Returns the number of open files on the board
        public static int OpenFiles(Player whitePlayer, Player blackPlayer)
        {
            var whitePieces = whitePlayer.Pieces;
            var blackPieces = blackPlayer.Pieces;

            ulong occupiedMask = 0;
            foreach(var piece in whitePieces)
            {
                if (piece.Value.Type == PieceType.PAWN)
                    occupiedMask |= piece.Value.GetPosition();
            }
            foreach (var piece in blackPieces)
            {
                if (piece.Value.Type == PieceType.PAWN)
                    occupiedMask |= piece.Value.GetPosition();
            }

            int openFiles = 0;
            // Iterate over each file
            for (int i = 0; i < 8; i++)
            {
                ulong fileBitboard = BitboardConstants.AFile << i;
                ulong occupiedSquaresOnFile = fileBitboard & occupiedMask;

                // If there are no pawns on the file, increment the count of open files
                if (occupiedSquaresOnFile == 0)
                {
                    openFiles++;
                }
            }

            return openFiles;
        }

    
    }
}
