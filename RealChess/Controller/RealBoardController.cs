using RealChess.Model;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.RealConstants;

namespace RealChess.Controller
{
    internal class RealBoardController
    {
        private static Board _gameBoard;

        internal static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        internal static void UpdateReal(Move move)
        {
            _gameBoard.UpdateReal(move);
        }

        internal static int GetPlayerMorale(PieceColor color)
        {
            return _gameBoard.GetMorale(color);
        }

        internal static int GetPieceMorale(ChessPiece piece)
        {
            if (piece.Type == PieceType.KING)
                return 100;
            int successRate = GetPlayerMorale(piece.Color);

            // Checks if the piece is adjacent to the king, if so, improve morale
            if ((piece.GetPosition() &
                _gameBoard.GetKing(piece.Color).GenerateMovesMask()) > 0)
                successRate += AdjacentToKing;

            var pieces = piece.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            // Checks if the rooks are connected, if so, improve rook morale
            if (piece.Type == PieceType.ROOK && BoardOperations.AreRooksConnected(pieces, piece.Color, _gameBoard.GetBoard()))
                successRate += RooksConnected;

            // Maximum success rate is 100
            if (successRate > 100)
                return 100;
            return successRate;
        }


        internal static int CalculateSuccess(Move move)
        {
            if (move.DefendsCheck || move.IsEnPassantCapture)
                return 100;

            int successRate = GetPieceMorale(move.PieceMoved);
            

            // Maximum success rate is 100
            if (successRate > 100)
                return 100;
            return successRate ;
        }

        internal static bool TryMove(Move move)
        {
 
            Random rnd = new Random();
            double randomNum = rnd.NextDouble();

            return randomNum <= ((float)CalculateSuccess(move) /100);
        }

    }
}
