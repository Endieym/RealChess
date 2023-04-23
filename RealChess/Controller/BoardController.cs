using RealChess.Model;
using RealChess.Model.AI;
using RealChess.Model.Bitboard;
using RealChess.Model.ChessPieces;
using System.Collections.Generic;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Controller
{
    /// <summary>
    /// Controller responsible for interacting 
    /// and updating the data structure
    /// </summary>
    internal static class BoardController
    {
        private static Board _gameBoard;

        // Sets the game board for all models using it
        internal static void SetBoard(Board board)
        {
            _gameBoard = board;
            BoardLogic.SetBoard(board);
            BoardUpdate.SetBoard(board);
            ComputerPlay.SetBoard(board);

        }

        /// <summary>
        /// Updates the data structure on the move of the piece.
        /// </summary>
        /// <param name="move">Move made</param>
        internal static void UpdateBoard(Move move)
        {

            _gameBoard.UpdateBoard(move);

        }

        /// <summary>
        /// Checks if a player has any legal moves
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>Returns true if has, false if not</returns>
        internal static bool HasLegalMoves(PieceColor color)
        {
            return _gameBoard.CanLegallyMove(color);
        }


        internal static bool IsInCheck(PieceColor color)
        {
            return _gameBoard.IsKingUnderAttack(color);
        }

        /// <summary>
        /// Gets the king's position for a player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>King position on the board</returns>
        internal static int GetKingPos(PieceColor color)
        {
            return _gameBoard.GetKingPos(color);
        }

        /// <summary>
        /// Makes a promotion move
        /// </summary>
        /// <param name="move">Original pawn move</param>
        /// <param name="piece">Piece chosen as a promotion</param>
        /// <returns>The new promotion move</returns>
        internal static Move PromotePiece(Move move, ChessPiece piece)
        {
            return _gameBoard.MakePromotionMove(move, piece);
        }

        internal static List<Move> GetAllMovesPiece(ChessPiece piece)
        {
            return _gameBoard.GetAllPieceMoves(piece);
        }

        /// <summary>
        /// Gets the possible moves a piece has
        /// </summary>
        /// <param name="piece">Piece chosen</param>
        /// <returns>List of moves</returns>
        internal static List<Move> GetMovesList(ChessPiece piece)
        {
            return _gameBoard.GetLegalMoves(piece);
        }

        /// <summary>
        /// Gets all captures a piece has
        /// </summary>
        /// <param name="piece">Piece chosen</param>
        /// <returns>A list of the captures</returns>
        internal static List<Move> GetCapturesList(ChessPiece piece)
        {
            return _gameBoard.GetCapturesPiece(piece);
        }

        /// <summary>
        /// Creates a piece according to row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>The piece creates</returns>
        internal static ChessPiece GetPieceForSquare(int row, int col)
        {
            ChessPiece chessPiece;
            PieceType pieceType = GetTypeByPos(row, col);

            chessPiece = GetPieceByType(pieceType);

            chessPiece.UpdatePosition(row, col);

            chessPiece.Color = GetColorByPos(row, col);
            return chessPiece;
        }

        /// <summary>
        /// Generates a new piece object according to type
        /// </summary>
        /// <param name="pieceType"></param>
        /// <returns></returns>
        public static ChessPiece GetPieceByType(PieceType pieceType)
        {
            ChessPiece chessPiece;

            switch (pieceType)
            {
                case PieceType.PAWN:
                    chessPiece = new Pawn();
                    break;

                case PieceType.KNIGHT:
                    chessPiece = new Knight();
                    break;

                case PieceType.BISHOP:
                    chessPiece = new Bishop();
                    break;

                case PieceType.ROOK:
                    chessPiece = new Rook();
                    break;

                case PieceType.QUEEN:
                    chessPiece = new Queen();
                    break;

                case PieceType.KING:
                    chessPiece = new King();
                    break;

                default:
                    chessPiece = null;
                    break;
            }

            return chessPiece;
        }

        /// <summary>
        /// Gets the type of the piece (Pawn, Knight, Bishop)
        /// According to the position of the piece (row,col)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>The type of the piece as enum</returns>
        internal static PieceType GetTypeByPos(int row, int col)
        {
           
            if (row == 0 || row == 7)
            {
                if (col == 0 || col == 7)
                    return PieceType.ROOK;

                else if (col == 1 || col == 6)
                    return PieceType.KNIGHT;

                else if (col == 2 || col == 5)
                    return PieceType.BISHOP;

                else if (col == 3)
                    return PieceType.QUEEN;

                else if (col == 4)
                    return PieceType.KING;

            }
            return PieceType.PAWN;
         
        }

        /// <summary>
        /// Gets the color of a piece based on its position(row,col)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>The color as enum</returns>
        internal static PieceColor GetColorByPos(int row, int col)
        {
            if (row == 0 || row == 1)
                return PieceColor.BLACK;
            else
                return PieceColor.WHITE;
        }
    }

    
}
