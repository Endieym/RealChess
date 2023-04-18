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
    internal static class MoveTranslator
    {
        private static Board _gameBoard;

        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        public static Move CopyMasterMove(PieceColor color, ilf.pgn.Data.Move masterMove)
        {  
            Move move = new Move();
            if(masterMove.OriginSquare != null)
            {
                move.StartSquare = CopySquare(masterMove.OriginSquare);
            }

            move.EndSquare = CopySquare(masterMove.TargetSquare);

            AddProperties(color, masterMove, move);

            return move;


        }

        public static void AddProperties(PieceColor color, ilf.pgn.Data.Move masterMove, Move move)
        {
            var player = color == PieceColor.WHITE ? _gameBoard.WhitePlayer : _gameBoard.WhitePlayer;
            var enemy = color == PieceColor.WHITE ? _gameBoard.BlackPlayer : _gameBoard.BlackPlayer;

            move.PieceMoved = player.Pieces[move.StartSquare];

            if (masterMove.Type == MoveType.Capture)
            {
                move.IsCapture = true;
                move.Type = Move.MoveType.Capture;
                move.CapturedPiece = enemy.Pieces[move.EndSquare];
            }

            if ((bool)masterMove.IsCheck)
            {
                move.Type = Move.MoveType.Check;
                move.IsCheck = true;
            }

        }


        public static int CopySquare(Square square)
        {
            return (int)square.File - 1 + (8 - square.Rank) * 8;
        }


        public static bool SquaresEqual(Square masterSquare, int square)
        {

            return MoveTranslator.CopySquare(masterSquare) == square;

        }

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


        

        public static bool MoveEqual(ilf.pgn.Data.Move masterMove, Move gameMove)
        {
            if (masterMove.OriginSquare != null)
            {
                if (!SquaresEqual(masterMove.OriginSquare, gameMove.StartSquare))
                    return false;
            }

            if (!SquaresEqual(masterMove.TargetSquare, gameMove.EndSquare))
                return false;

            if (masterMove.Piece != null)
            {
                if (!PieceEqual(masterMove.Piece, gameMove.PieceMoved.Type))
                    return false;
            }

            return true;
        }
    }
}
