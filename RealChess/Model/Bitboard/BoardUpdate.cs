using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.Bitboard.BoardOperations;
using RealChess.Model.ChessPieces;

namespace RealChess.Model.Bitboard
{
    internal static class BoardUpdate
    {

        private static Board _gameBoard;

        // Sets the game board
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }


        public static void UpdateBoard(Move move)
        {
            // Checks if the piece has moved before, if not, change the attribute
            if (!move.PieceMoved.HasMoved)
                move.PieceMoved.HasMoved = true;

            var oppositeColor = GetOppositeColor(move.PieceMoved.Color);

            // Checks the king
            if (move.IsCheck)
            {
                _gameBoard.GetKing(oppositeColor).InCheck = true;
            }

            // Removes the check from the king
            if (move.DefendsCheck)
            {
                _gameBoard.GetKing(move.PieceMoved.Color).InCheck = false;
            }

            // Updates position of piece moved
            UpdateDataStructures(move);

            // If the move was a capture, remove the piece
            if (move.IsCapture)
                UpdateCaptures(move);

        }

        // Updates the board class according to the move played
        public static void UpdateDataStructures(Move move)
        {
            var playerColor = move.PieceMoved.Color;
            
            
            var oldKey = move.StartSquare;
            var newKey = move.EndSquare;
            ulong oldBitmask = (ulong)1 << oldKey;
            ulong newBitmask = (ulong)1 << newKey;

            Player player;
            ulong playerBoard;
           

            if(playerColor == PieceColor.WHITE)
            {
                player = _gameBoard.WhitePlayer;
                playerBoard = _gameBoard.WhiteBoard;
            }
            else
            {
                player = _gameBoard.BlackPlayer;
                playerBoard = _gameBoard.BlackBoard;
            }

            player.UpdatePiece(oldKey, newKey); // Updates the piece position in the player dictionary
            
            playerBoard ^= oldBitmask; // Removes previous position
            playerBoard |= newBitmask; // Adds new position

            if (move.IsKingSideCastle)
            {
                player.UpdatePiece(newKey + 1, oldKey + 1);
                playerBoard ^= newBitmask << 1; // Removes old rook position
                playerBoard |= oldBitmask << 1; // Adds new rook position
                ((King)move.PieceMoved).Castled = true;
            }

            else if (move.IsQueenSideCastle)
            {
                player.UpdatePiece(newKey - 2, oldKey - 1);
                playerBoard ^= newBitmask >> 2; // Removes old rook position
                playerBoard |= oldBitmask >> 1; // Adds new rook position
                ((King)move.PieceMoved).Castled = true;

            }

            if (playerColor == PieceColor.WHITE)
                _gameBoard.WhiteBoard = playerBoard;
            else
                _gameBoard.BlackBoard = playerBoard;


            _gameBoard.GetAllMoves().Add(move);
            _gameBoard.GetAllStates().Add(GetBoardStateString(_gameBoard));

        }

        // Updates player's dictionaries in the case of a captured piece
        public static void UpdateCaptures(Move move)
        {
            var playerColor = move.PieceMoved.Color;
            var newKey = move.EndSquare;

            Player player, enemy;
            ulong enemyBoard;

            // Sets the variables for player and enemy
            if (playerColor == PieceColor.WHITE)
            {
                player = _gameBoard.WhitePlayer;
                enemy = _gameBoard.BlackPlayer;
                enemyBoard = _gameBoard.BlackBoard;
            }
            else
            {
                player = _gameBoard.BlackPlayer;
                enemy = _gameBoard.WhitePlayer;
                enemyBoard = _gameBoard.WhiteBoard;

            }

            // If move is an en passant, needs to go back (or forward)
            // one square from the end square to get the captured square
            
            if (move.IsEnPassantCapture)
                newKey += playerColor == PieceColor.WHITE? 8: -8;


            player.AddCapture(enemy.Pieces[newKey]);
            enemy.DeletePiece(newKey); // Delets from enemy's dictionary
            enemyBoard ^= (ulong)1 << newKey; // Updates enemy's board

            // Sets the boards back
            if (playerColor == PieceColor.WHITE)
                _gameBoard.BlackBoard = enemyBoard;
            else
                _gameBoard.WhiteBoard = enemyBoard;

        }

        // Undos the last move made
        public static void UndoMove()
        {
            var movesList = _gameBoard.GetAllMoves();
            var positionsList = _gameBoard.GetAllStates();

            int posIndx = positionsList.Count;
            int index = movesList.Count - 1;

            Move oldMove = movesList[index];

            Move undoMove = new Move(oldMove.StartSquare, oldMove.PieceMoved);
            if (oldMove.IsKingSideCastle)
            {
                int pos = oldMove.StartSquare + 1;
                var piece = oldMove.PieceMoved.Color == PieceColor.WHITE ?
                    _gameBoard.WhitePlayer.Pieces[pos] :
                    _gameBoard.BlackPlayer.Pieces[pos];

                UpdateDataStructures(new Move(pos + 2, piece));

            }
            if (oldMove.IsQueenSideCastle)
            {
                int pos = oldMove.StartSquare - 1;
                var piece = oldMove.PieceMoved.Color == PieceColor.WHITE ?
                    _gameBoard.WhitePlayer.Pieces[pos] :
                    _gameBoard.BlackPlayer.Pieces[pos];

                UpdateDataStructures(new Move(pos - 3, piece));

            }

            UpdateDataStructures(undoMove);

            // Removes the last moves made (Checked move and Undone move)
            movesList.RemoveAt(index + 1);
            movesList.RemoveAt(index);

            // Removes the last state made
            positionsList.RemoveAt(posIndx);
            positionsList.RemoveAt(posIndx - 1);
            
            if (oldMove.IsCapture)
                UndoCapture(oldMove);

        }

        // Undoes a capture made by a move
        public static void UndoCapture(Move oldCapture)
        {
            var playerColor = oldCapture.PieceMoved.Color;
            var newKey = oldCapture.EndSquare;
            ChessPiece oldPiece = oldCapture.CapturedPiece;

            if (oldCapture.IsEnPassantCapture)
                newKey += playerColor == PieceColor.WHITE ? 8 : -8;

            if (playerColor == PieceColor.WHITE)
            {

                _gameBoard.BlackPlayer.AddPiece(oldPiece, newKey);
                _gameBoard.BlackBoard |= (ulong)1 << newKey;

            }
            else
            {
                _gameBoard.WhitePlayer.AddPiece(oldPiece, newKey);
                _gameBoard.WhiteBoard |= (ulong)1 << newKey;
            }


        }

    }
}
