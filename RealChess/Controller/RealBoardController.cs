using RealChess.Model;
using RealChess.Model.Bitboard;
using RealChess.Model.ChessPieces;
using System;
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

        // Balances the morale when getting too far away
        internal static void BalancePlayers()
        {
            var player1 = _gameBoard.GetPlayer1();
            var player2 = _gameBoard.GetPlayer2();

            if (player1.Morale > player2.Morale + 15)
            {
                player1.Morale -= (player2.Morale / 10);
                player2.Morale += player2.Morale / 10;
            }
            else if (player2.Morale > player1.Morale + 15)
            {
                player1.Morale -= (player2.Morale / 10);
                player2.Morale += player2.Morale / 10;
            }

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
            if (piece.Type == PieceType.ROOK && BoardLogic.AreRooksConnected(pieces, piece.Color, _gameBoard.GetBoard()))
                successRate += RooksConnected;

            int openFiles = BoardLogic.OpenFiles(_gameBoard.GetPlayer1(), _gameBoard.GetPlayer2());

            // Bishop gets buffed when the board is more open
            if (piece.Type == PieceType.BISHOP)
                successRate += Positionbuff * openFiles;

            // Knight gets buffed when the board is more closed
            else if (piece.Type == PieceType.KNIGHT)
                successRate += Positionbuff * (8 - openFiles);
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
