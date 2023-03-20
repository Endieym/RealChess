using RealChess.Model;
using RealChess.Model.ChessPieces;
using RealChess.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Controller
{
    
    internal static class BoardController
    {
        private static Board _gameBoard;

        internal static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        // Updates the data structure on the move of the piece.
        internal static void UpdateBoard(Move move)
        {

            _gameBoard.UpdateBoard(move);

        }

        internal static bool IsInCheck(PieceColor color)
        {
            return _gameBoard.IsKingUnderAttack(color);
        }

        internal static int GetKingPos(PieceColor color)
        {
            return _gameBoard.GetKingPos(color);
        }

        internal static List<Move> GetMovesList(ChessPiece piece)
        {
            return _gameBoard.GetMovesPiece(piece);
        }
        internal static List<Move> GetCapturesList(ChessPiece piece)
        {
            return _gameBoard.GetCapturesPiece(piece);
        }

        internal static ChessPiece GetPieceForSquare(int row, int col)
        {
            ChessPiece chessPiece;
            PieceType pieceType = GetTypeByPos(row, col);
            switch (pieceType)
            {
                case PieceType.PAWN:
                    chessPiece = new Pawn(row, col);
                    break;

                case PieceType.KNIGHT:
                    chessPiece = new Knight(row, col);
                    break;

                case PieceType.BISHOP:
                    chessPiece = new Bishop(row, col);
                    break;

                case PieceType.ROOK:
                    chessPiece = new Rook(row, col);
                    break;

                case PieceType.QUEEN:
                    chessPiece = new Queen(row, col);
                    break;

                case PieceType.KING:
                    chessPiece = new King(row, col);
                    break;

                default:
                    chessPiece = null;
                    break;
            }
            chessPiece.Color = GetColorByPos(row, col);
            return chessPiece;
        }

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
        internal static PieceColor GetColorByPos(int row, int col)
        {
            if (row == 0 || row == 1)
                return PieceColor.BLACK;
            else
                return PieceColor.WHITE;
        }
    }

    
}
