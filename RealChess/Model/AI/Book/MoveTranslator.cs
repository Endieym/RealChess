using ilf.pgn.Data;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.AI.Book
{
    /// <summary>
    /// The MoveTranslator class is responsible for translating moves between the ilf.pgn format
    /// and the game's internal format.
    /// </summary>
    internal static class MoveTranslator
    {
        private static Board _gameBoard;

        /// <summary>
        /// Sets the current game board.
        /// </summary>
        /// <param name="board">The Board object representing the current state of the game board.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        /// <summary>
        /// Creates a new Move by copying information from a master move from the book.
        /// </summary>
        /// <param name="color">The color of the piece making the move.</param>
        /// <param name="masterMove">The master move from the book to copy information from.</param>
        /// <returns>A new Move object with information copied from the master move.</returns>
        public static Move CopyMasterMove(PieceColor color, ilf.pgn.Data.Move masterMove)
        {  

            Move move = new Move();
            
            // Copy the origin square if it exists in the annotation
            if (masterMove.OriginSquare != null)            
                move.StartSquare = CopySquare(masterMove.OriginSquare);
            
            // Copy the target square if it exists in the annotation
            if(masterMove.TargetSquare != null)
                move.EndSquare = CopySquare(masterMove.TargetSquare);

            // Add other properties of the move, such as Checks, Captures, castling etc
            AddProperties(color, masterMove, move);

            return move;
        }

        /// <summary>
        /// Adds properties to a Move object based on the given master move and color.
        /// </summary>
        /// <param name="color">The color of the player making the move.</param>
        /// <param name="masterMove">The master move to copy properties from.</param>
        /// <param name="move">The Move object to add properties to.</param>
        public static void AddProperties(PieceColor color, ilf.pgn.Data.Move masterMove, Move move)
        {
            // Get the player and enemy objects based on the color
            var player = color == PieceColor.WHITE ? _gameBoard.WhitePlayer : _gameBoard.WhitePlayer;
            var enemy = color == PieceColor.WHITE ? _gameBoard.BlackPlayer : _gameBoard.BlackPlayer;

            // Check if the master move is a king side castle
            if (masterMove.ToString().Equals("O-O"))
            {
                move.IsKingSideCastle = true;
                move.Type = Move.MoveType.Castle;
                move.StartSquare = _gameBoard.GetKingPos(color);
                move.EndSquare = move.StartSquare + 2;
            }

            // Check if the master move is a queen side castle
            else if (masterMove.ToString().Equals("O-O-O"))
            {
                move.IsQueenSideCastle = true;
                move.Type = Move.MoveType.Castle;
                move.StartSquare = _gameBoard.GetKingPos(color);
                move.EndSquare = move.StartSquare - 2;
            }

            move.PieceMoved = player.Pieces[move.StartSquare];

            // Check if the master move is a capture move
            if (masterMove.Type == MoveType.Capture)
            {
                move.IsCapture = true;
                move.Type = Move.MoveType.Capture;
                move.CapturedPiece = enemy.Pieces[move.EndSquare];
            }


            // Check if the master move results in check
            if ((bool)masterMove.IsCheck)
            {
                move.Type = Move.MoveType.Check;
                move.IsCheck = true;
            }
        }

        /// <summary>
        /// Copies a book square to its integer representation on the board.
        /// </summary>
        /// <param name="square">The square from the library to copy.</param>
        /// <returns>The key position of the square on the board.</returns>
        public static int CopySquare(Square square)
        {
            return (int)square.File - 1 + (8 - square.Rank) * 8;
        }


        /// <summary>
        /// Compares a master square with a key integer of a square on the chess board.
        /// </summary>
        /// <param name="masterSquare">The master square to compare.</param>
        /// <param name="square">The integer representation of the square to compare.</param>
        /// <returns>True if the master square and regular square are equal, otherwise false.</returns>
        public static bool SquaresEqual(Square masterSquare, int square)
        {
            return MoveTranslator.CopySquare(masterSquare) == square;
        }

        /// <summary>
        /// Determines whether the given PGN piece type is equal to the corresponding chess piece type.
        /// </summary>
        /// <param name="masterPiece">The PGN piece type to compare.</param>
        /// <param name="piece">The chess piece type to compare.</param>
        /// <returns>True if the pieces are equal, false otherwise.</returns>
        public static bool PieceEqual(ilf.pgn.Data.PieceType? masterPiece, ChessPiece.PieceType piece)
        {
            switch (masterPiece)
            {
                case ilf.pgn.Data.PieceType.Pawn:
                    return piece == ChessPiece.PieceType.PAWN;

                case ilf.pgn.Data.PieceType.Bishop:
                    return piece == ChessPiece.PieceType.BISHOP;

                case ilf.pgn.Data.PieceType.Knight:
                    return piece == ChessPiece.PieceType.KNIGHT;

                case ilf.pgn.Data.PieceType.Rook:
                    return piece == ChessPiece.PieceType.ROOK;

                case ilf.pgn.Data.PieceType.Queen:
                    return piece == ChessPiece.PieceType.QUEEN;

                case ilf.pgn.Data.PieceType.King:
                    return piece == ChessPiece.PieceType.KING;

            }
            return false;
        }



        /// <summary>
        /// Compares a given masterMove with a gameMove to determine if they are the same
        /// </summary>
        /// <param name="masterMove">The master move to compare against</param>
        /// <param name="gameMove">The game move to compare with</param>
        /// <returns>True if the two moves are equal, false otherwise</returns>
        public static bool MoveEqual(ilf.pgn.Data.Move masterMove, Move gameMove)
        {
            if (masterMove.OriginSquare != null)
            {
                if (!SquaresEqual(masterMove.OriginSquare, gameMove.StartSquare))
                    return false;
            }
            if(masterMove.TargetSquare != null)
            {
                if (!SquaresEqual(masterMove.TargetSquare, gameMove.EndSquare))
                    return false;
            }
            
            if (masterMove.Piece != null)
            {
                if (!PieceEqual(masterMove.Piece, gameMove.PieceMoved.Type))
                    return false;
            }

            return true;
        }
    }
}
