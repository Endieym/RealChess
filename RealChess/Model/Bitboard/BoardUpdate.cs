using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.Bitboard.BoardOperations;
using RealChess.Model.ChessPieces;

namespace RealChess.Model.Bitboard
{
    /// <summary>
    /// The BoardUpdate class provides functionality for updating the state of a chess game board,
    /// including methods for making and undoing moves, captures, promotions, and castling.
    /// </summary>
    internal static class BoardUpdate
    {

        private static Board _gameBoard;

        /// <summary>
        /// Sets the game board.
        /// </summary>
        /// <param name="board">The game board to set.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        /// <summary>
        /// Updates the game board after a move is made, including setting the HasMoved attribute of the moved piece to true,
        /// checking and updating the king's InCheck attribute, updating the position of the moved piece in the data structures,
        /// and removing a captured piece if necessary.
        /// </summary>
        /// <param name="move">The move made.</param>
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

        /// <summary>
        /// Updates the game board according to the move played by updating the position of the piece moved,
        /// removing the previous position, and adding the new position. If the move was a castle move,
        /// it also updates the position of the rook. If the move was a capture, it removes the captured piece from the board.
        /// Finally, it adds the move to the list of all moves and the current board state to the list of all states.
        /// </summary>
        /// <param name="move">The move played</param>
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
            _gameBoard.BitBoard = _gameBoard.BlackBoard | _gameBoard.WhiteBoard;

        }

        /// <summary>
        /// Updates the player's and the enemy's board and dictionary in the case of a captured piece.
        /// </summary>
        /// <param name="move">The capture made.</param>
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


            enemy.DeletePiece(newKey); // Delets from enemy's dictionary
            enemyBoard ^= (ulong)1 << newKey; // Updates enemy's board

            // Sets the boards back
            if (playerColor == PieceColor.WHITE)
                _gameBoard.BlackBoard = enemyBoard;
            else
                _gameBoard.WhiteBoard = enemyBoard;

            _gameBoard.BitBoard = _gameBoard.BlackBoard | _gameBoard.WhiteBoard;

        }

        /// <summary>
        /// Undos the last move made. This method removes the last move made from the gameBoard's list of moves,
        /// the last state made from the gameBoard's list of positions, and calls the UpdateDataStructures method
        /// to undo the last move made. If the move being undone was a capture move, the UndoCapture method is called
        /// to update the gameBoard's data structures accordingly. If the move being undone was a castling move,
        /// this method undos the castling by moving the rook back to its original position.
        /// </summary>
        public static void UndoMove()
        {
            var movesList = _gameBoard.GetAllMoves();
            var positionsList = _gameBoard.GetAllStates();

            int posIndx = positionsList.Count;
            int index = movesList.Count - 1;

            Move oldMove = movesList[index];



            Move undoMove = new Move(oldMove.StartSquare, oldMove.PieceMoved)
            {
                StartSquare = oldMove.EndSquare
            };

            int rookPos = oldMove.StartSquare;

            int oldRookPos;

            if (oldMove.Type == Move.MoveType.Castle)
            {
                rookPos += oldMove.IsKingSideCastle ? 1 : -1;
                oldRookPos = rookPos;

                oldRookPos += oldMove.IsKingSideCastle ? 2 : -3;

                var piece = oldMove.PieceMoved.Color == PieceColor.WHITE ?
                    _gameBoard.WhitePlayer.Pieces[rookPos] :
                    _gameBoard.BlackPlayer.Pieces[rookPos];

                UpdateDataStructures(new Move(oldRookPos, piece));

                ((King)oldMove.PieceMoved).Castled = false;
            }

            UpdateDataStructures(undoMove);

            // Removes the last two moves made (Checked move and Undone move)
            movesList.RemoveAt(index + 1);
            movesList.RemoveAt(index);

            // Removes the last state made
            positionsList.RemoveAt(posIndx);
            positionsList.RemoveAt(posIndx - 1);
            
            if (oldMove.IsCapture)
                UndoCapture(oldMove);

        }

        /// <summary>
        /// Undoes a capture made by a move by adding the captured piece back to the opponent player's dictionary and board.
        /// </summary>
        /// <param name="oldCapture">The move that resulted in the capture.</param>
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

        /// <summary>
        /// This method undoes a promotion made by a move. It replaces the promoted piece back to a pawn and updates the player's list of pieces.
        /// </summary>
        /// <param name="move">The move to undo the promotion for.</param>
        public static void UndoPromotion(Move move)
        {
            move.PieceMoved = new Pawn(move.EndSquare)
            {
                HasMoved = true,
                Color = move.PieceMoved.Color
            };

            var player = move.PieceMoved.Color == PieceColor.WHITE ? _gameBoard.WhitePlayer :
                _gameBoard.BlackPlayer;
            
            player.SwitchPiece(move.EndSquare, move.PieceMoved);

        }
    }
}
