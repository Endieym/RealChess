using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.Bitboard.BoardOperations;
using RealChess.Model.Bitboard;
using static RealChess.Model.Bitboard.BoardLogic;
using RealChess.Model.AI.Book;

namespace RealChess.Model
{
    /// <summary>
    /// Represents the chess board sized 8*8
    /// </summary>
    public class Board
    {
        public const int SIZE = 8;

        internal Player WhitePlayer {
            set { player1 = value; }
            get { return this.player1; }
        }
        internal Player BlackPlayer {
            set { player2 = value; }
            get { return this.player2; }
        }

        internal ulong BlackBoard {
            set { blackBoard = value; }
            get { return blackBoard; }
        }
        internal ulong WhiteBoard {
            set { whiteBoard = value; }
            get { return whiteBoard; }
        }
        internal ulong BitBoard {
            set { bitBoard = value; }
            get { return bitBoard; }
        }


        Player player1;
        Player player2;
        UInt64 bitBoard;
        UInt64 whiteBoard;
        UInt64 blackBoard;

        List<Move> movesList;
        List<string> positions;
        

        /// <summary>
        /// Constructor for Board class, initialize
        /// </summary>
        public Board()
        {
            this.player1 = new Player(true);
            this.player2 = new Player(false);
            player1.Morale = 70;
            player2.Morale = 70;
            this.bitBoard = 0;
            this.whiteBoard = 0;
            this.blackBoard = 0;

            movesList = new List<Move>();
            positions = new List<string>();

            foreach (var item in player1.Pieces)
            {
                this.whiteBoard |= (ulong)1 << item.Key;
            }
            
            foreach (var item in player2.Pieces)
            {
                this.blackBoard |= (ulong)1 << item.Key;
            }
            this.bitBoard = whiteBoard | blackBoard;
        }

        /// <summary>
        /// Gets the morale of a specific colored player.
        /// </summary>
        /// <param name="color">The color of the player whose morale is being retrieved.</param>
        /// <returns>The morale value of the specified player.</returns>
        public int GetMorale(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.Morale : player2.Morale;
        }


        /// <summary>
        /// Checks if a king of specific color is under attack.
        /// </summary>
        /// <param name="color">The color of the king to check.</param>
        /// <returns>True if the king of the specified color is under attack, false otherwise.</returns>
        public bool IsKingUnderAttack(PieceColor color)
        {
            ulong attackingSquares = GetPlayerAttack(color);

            return color == PieceColor.WHITE ? player1.InCheck(attackingSquares) :
                player2.InCheck(attackingSquares);
        }

        /// <summary>
        /// Gets the king piece of a specific colored player.
        /// </summary>
        /// <param name="color">The color of the player whose king is to be retrieved.</param>
        /// <returns>The king piece of the specified player.</returns>
        internal King GetKing(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.GetKing() :
                player2.GetKing();
        }

        /// <summary>
        /// Function to return the position of the king of a specific color
        /// </summary>
        /// <param name="color"></param>
        /// <returns>The position of the king as an integer</returns>
        public int GetKingPos(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.GetKingPos() :
                player2.GetKingPos();
        }

        /// <summary>
        /// Calculates and returns the attack bitboard for the specified player's pieces of the given color.
        /// </summary>
        /// <param name="color">The color of the player to get the attack bitboard for.</param>
        /// <returns>The attack bitboard of the specified player's pieces.</returns>
        public ulong GetPlayerAttack(PieceColor color)
        {
            return color == PieceColor.WHITE ? player2.GetAttacks(whiteBoard, bitBoard) :
               player1.GetAttacks(blackBoard, bitBoard);
        }

        /// <summary>
        /// Returns the possible move positions of a player of a given color.
        /// </summary>
        /// <param name="color">The color of the player.</param>
        /// <returns>An unsigned 64-bit integer representing the bitboard of possible moves.</returns>
        public ulong GetPlayerMove(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.GetMoves(whiteBoard, bitBoard) :
               player2.GetMoves(blackBoard, bitBoard);
        }

        /// <summary>
        /// Updates the board according to a move made and updates the movesList and bookReader.
        /// </summary>
        /// <param name="move">The move to be made.</param>  
        public void UpdateBoard(Move move)
        {

            BoardUpdate.UpdateBoard(move);
            BookReader.UpdateGames(movesList);
        }

        /// <summary>
        /// This method creates a promotion move from a regular pawn move and a chosen promotion piece.
        /// </summary>
        /// <param name="move">The original pawn move</param>
        /// <param name="chessPiece">The piece chosen for promotion</param>
        /// <returns>A new move object representing the promotion move</returns>
        public Move MakePromotionMove(Move move, ChessPiece chessPiece)
        {
            var endKey = move.EndSquare;
            var beforeKey = move.StartSquare;

            // Switch the pawn to the promotion piece
            if (chessPiece.Color == PieceColor.WHITE)
                player1.SwitchPiece(beforeKey, chessPiece);

            else
                player2.SwitchPiece(beforeKey, chessPiece);

            // Create the promotion move

            Move promotion = new Move(endKey, chessPiece)
            {
                StartSquare = move.StartSquare,
                IsCapture = move.IsCapture,
                DefendsCheck = move.DefendsCheck,
                CapturedPiece = move.CapturedPiece,
                IsPositiveCapture = true,
                IsPromotion = true,
                PromotedPiece = chessPiece.Type
            };

            // Checks for special properties of the move
            // Check, Checkmate or draw
            CheckMove(promotion);

            return promotion;
        }

        /// <summary>
        /// This method gets all the moves of a given chess piece, including legal moves and captures.
        /// It also checks each move for special properties such as check, checkmate or draw.
        /// </summary>
        /// <param name="piece">The chess piece to get the moves for</param>
        /// <returns>A list of all possible moves for the given chess piece</returns>

        public List<Move> GetAllPieceMoves(ChessPiece piece)
        {
            List<Move> allMoves = GetLegalMoves(piece).Concat(GetCapturesPiece(piece)).ToList();

            foreach(Move move in allMoves)
            {
                CheckMove(move);
            }

            return allMoves;
        }


        /// <summary>
        /// Generates all legal moves a piece has
        /// </summary>
        /// <param name="piece">The chess piece to generate legal moves for</param>
        /// <returns>A list of all legal moves the chess piece can make</returns>
        public List<Move> GetLegalMoves(ChessPiece piece)
        {
            ulong finalMoves = piece.GenerateLegalMoves(this.bitBoard);
            // Initialize the moves list
            List<Move> list = new List<Move>();
            
            // Removes moves over occupied squares
            finalMoves &= (~bitBoard);

            
            // checks for legal moves using the moves bitmask
            while (finalMoves != 0)
            {
                // determine bit index, also referred as BitScan
                int bitIndex = (int)Math.Log(finalMoves & ~(finalMoves - 1), 2);
                Move newMove = new Move(bitIndex, piece);
                if (IsMoveLegal(newMove))
                {                 
                    if (IsKingUnderAttack(piece.Color))
                        newMove.DefendsCheck = true;

                    if (piece.Type == PieceType.PAWN && IsMovePromotion(piece.Color, finalMoves))
                        newMove.IsPromotion = true;

                    list.Add(newMove);
                }
                
                finalMoves &= finalMoves - 1; // reset LS1B
            }

            // Adds castling (if possible)
            if (piece.Type == PieceType.KING)
            {
                var pieces = piece.Color == PieceColor.WHITE ? player1.Pieces : player2.Pieces;
                ulong boardAndEnemyMoves = bitBoard;
                boardAndEnemyMoves |= GetPlayerMove(GetOppositeColor(piece.Color));

                var Castle = GenerateCastle((King)piece, pieces, boardAndEnemyMoves);
                ulong castleQueen = Castle.Item1;
                ulong castleKing = Castle.Item2;

                // Add queen-side castling move
                if (castleQueen > 0)
                {

                    Move castleMove = new Move((int)Math.Log(castleQueen, 2), piece)
                    {
                        IsQueenSideCastle = true,
                        Type = Move.MoveType.Castle
                    };

                    list.Add(castleMove);
                }

                // Add king-side castling move
                if (castleKing > 0)
                {
                    Move castleMove = new Move((int)Math.Log(castleKing, 2), piece)
                    {
                        IsKingSideCastle = true,
                        Type = Move.MoveType.Castle

                    };
                    list.Add(castleMove);
                }
            }
            return list;
        }

        /// <summary>
        /// Getter for the bitboard
        /// </summary>
        /// <returns></returns>
        public ulong GetBoard()
        {
            return this.bitBoard;
        }

        /// <summary>
        /// Returns a list of all moves made until now
        /// </summary>
        /// <returns>List of all moves made until now</returns>
        public List<Move> GetAllMoves()
        {
            return movesList;
        }

        /// <summary>
        /// Returns the list of all positions achieved until now
        /// </summary>
        /// <returns>The list of string positions</returns>
        public List<string> GetAllStates()
        {
            return positions;
        }


        /// <summary>
        /// Returns the attack bitmask for a specific piece
        /// </summary>
        /// <param name="piece">The piece whose attack bitmask is returned</param>
        /// <returns>The attacks mask of the piece</returns>
        public ulong GetAttacksMask(ChessPiece piece)
        {
            return piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures():
                piece.GenerateLegalMoves(bitBoard);
        }

        /// <summary>
        /// Checks if a move is legal ie the king is not under attack after the move
        /// </summary>
        /// <param name="newMove">The move being checked</param>
        /// <returns>True if the move is legal, false otherwise</returns>
        public bool IsMoveLegal(Move newMove)
        {
            MakeTemporaryMove(newMove);

            if(newMove.PieceMoved.Type == PieceType.KING && newMove.PieceMoved.Color == PieceColor.WHITE)
            {
                Console.Write("Hey");
            }
            bool result = IsKingUnderAttack(newMove.PieceMoved.Color);

            UndoMove();
            return !result;
        }

        /// <summary>
        /// Makes a temporary move on the board 
        /// (only data without graphics)
        /// </summary>
        /// <param name="move">The move being played</param>
        public void MakeTemporaryMove(Move move)
        {
            BoardUpdate.UpdateDataStructures(move);
            if (move.IsCapture)
                BoardUpdate.UpdateCaptures(move);
            bitBoard = blackBoard | whiteBoard;

        }

        /// <summary>
        /// Checks if a move results in checking the enemy king,
        /// checkmate or a draw
        /// </summary>
        /// <param name="newMove">The move being checked</param>
        public void CheckMove(Move newMove)
        {
            MakeTemporaryMove(newMove);
                        
            bool result = IsKingUnderAttack(GetOppositeColor(newMove.PieceMoved.Color));

            var enemyCanMove = CanLegallyMove(GetOppositeColor(newMove.PieceMoved.Color));
            if (result)
            {
                newMove.IsCheck = true;
                // Checks the king

                
                GetKing(GetOppositeColor(newMove.PieceMoved.Color)).InCheck = true;
                
                // If the enemy king is in check and the player has
                // no legal moves, the original check is a checkmate
                if (!enemyCanMove)
                    newMove.Type = Move.MoveType.Checkmate;
                else
                    newMove.Type = Move.MoveType.Check;

            }

            CheckDraw(newMove, enemyCanMove);

            // Undoes the temporary move made
            if (result)
                GetKing(GetOppositeColor(newMove.PieceMoved.Color)).InCheck = false;

            UndoMove();
        }

        /// <summary>
        /// Checks if a draw has occurred by threefold repetition,
        /// stalemate or dead position
        /// </summary>
        /// <param name="move">The move that triggered the draw check</param>
        /// <param name="enemyCanMove">A boolean representing whether the enemy player can make a move or not</param>
        private void CheckDraw(Move move, bool enemyCanMove)
        {

            if (BoardLogic.IsThreefoldRepetition(GetBoardStateString(this), positions))
            {
                move.IsDrawByRepetiton = true;
                move.Type = Move.MoveType.Draw;
            }
            else if (!move.IsCheck && !enemyCanMove)
            {
                move.IsStalemate = true;
                move.Type = Move.MoveType.Draw;
            }
            else if (IsDeadPosition())
            {
                move.IsDrawByDeadPosition = true;
                move.Type = Move.MoveType.Draw;
            }
        }

        /// <summary>
        /// Checks if the given bitboard for the mask results in a promotion
        /// </summary>
        /// <param name="color">The color of the piece that is making the move</param>
        /// <param name="movePosition">The position of the move represented as a bitboard</param>
        /// <returns>True if the move results in a promotion, false otherwise</returns>
        public bool IsMovePromotion(PieceColor color ,ulong movePosition)
        {
            movePosition &= color == PieceColor.WHITE ? BitboardConstants.RankEight:
                BitboardConstants.RankOne;

            return movePosition > 0;

        }

        /// <summary>
        /// Undoes the last move made
        /// </summary>
        public void UndoMove()
        {
            BoardUpdate.UndoMove();
            // Updates bitboard to before move
            bitBoard = blackBoard | whiteBoard;

        }

        /// <summary>
        /// Checks if a player has any legal moves
        /// </summary>
        /// <param name="color">player color</param>
        /// <returns>True if has legal moves, false otherwise</returns>
        public bool CanLegallyMove(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE?
                player1.Pieces.ToDictionary(entry => entry.Key,
                entry => entry.Value):
                player2.Pieces.ToDictionary(entry => entry.Key,
                entry => entry.Value);
            
            foreach (var piece in pieces.Values)
            {
                if (GetCapturesPiece(piece).Count > 0)
                    return true;
                if (GetLegalMoves(piece).Count > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Generates a list of all possible captures for a given chess piece.
        /// </summary>
        /// <param name="piece">The piece to generate captures for.</param>
        /// <returns>The list of possible captures.</returns>
        public List<Move> GetCapturesPiece(ChessPiece piece)
        {
            List<Move> captureList = new List<Move>();
            // Gets the masks for the captures, according to piece type
            ulong movesMask = GetAttacksMask(piece);

            // Checks colliding squares with enemy
            movesMask &= piece.Color == PieceColor.WHITE ? blackBoard : whiteBoard;
            ulong enPassantMask = 0;

            // Adds en passant (if possible)
            if(movesList.Count > 3 && piece.Type == PieceType.PAWN)
            { 
                Move lastMove = movesList[movesList.Count - 1];
                enPassantMask |= GenerateEnPassant((Pawn)piece,lastMove);
                if (enPassantMask > 0)
                {
                    Move enPassant = new Move((int)Math.Log(enPassantMask, 2), piece)
                    {
                        IsEnPassantCapture = true,
                        IsCapture = true
                    };
                    var capturedPieceIndex = piece.Color == PieceColor.WHITE ? (int)Math.Log(enPassantMask, 2) + 8 :
                        (int)Math.Log(enPassantMask, 2) - 8;

                    if (piece.Color == PieceColor.WHITE)
                        enPassant.CapturedPiece = player2.Pieces[capturedPieceIndex];
                    else
                        enPassant.CapturedPiece = player1.Pieces[capturedPieceIndex];

   
                    // Checks if the move is legal;
                    // meaning that the king isn't in under attack after the move
                    if (IsMoveLegal(enPassant))
                        captureList.Add(enPassant);

                    movesMask &= ~(enPassantMask);
                }
            }

            while (movesMask != 0)
            {
                // determine bit index, also referred as BitScan
                int bitIndex = (int)Math.Log(movesMask & ~(movesMask - 1), 2);
                Move newMove = new Move(bitIndex, piece)
                {
                    IsCapture = true,
                    Type = Move.MoveType.Capture
                };

                if (piece.Color == PieceColor.WHITE)
                    newMove.CapturedPiece = player2.Pieces[bitIndex];
                else
                    newMove.CapturedPiece = player1.Pieces[bitIndex];

                // Checks if the move is legal;
                // meaning that the king isn't in under attack after the move
                if (IsMoveLegal(newMove))
                {
                    if (IsKingUnderAttack(piece.Color))
                        newMove.DefendsCheck = true;


                    // Checks if the capture results in a promotion
                    if (piece.Type == PieceType.PAWN && IsMovePromotion(piece.Color, movesMask))
                        newMove.IsPromotion = true;

                    // Checks if the capture is a positive capture
                    if (BoardLogic.GetCaptureValue(newMove.CapturedPiece, piece) > 0)
                        newMove.IsPositiveCapture = true;

                    captureList.Add(newMove);

                }
                movesMask &= movesMask - 1; // reset LS1B
            }
            return captureList;           
        }

        /// <summary>
        /// Returns a list of every single legal move/capture a player has (by color)
        /// </summary>
        /// <param name="color">The color of the player</param>
        /// <returns>A list of all the possible moves and captures for the player</returns>
        public List<Move> GetAllPlayerMoves(PieceColor color)
        {
            List<Move> allMoves = new List<Move>();

            var pieces = GetPieces(color);

            // Iterates over every piece, and adds all moves to the general list
            foreach (var piece in pieces.ToList())
            {
                GetAllPieceMoves(piece).ForEach(item => allMoves.Add(item));

            }
            
            return allMoves;
        }

        /// <summary>
        /// Generates a list of all the legal non-capture moves for a given player color.
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>List of legal non-capture moves</returns>
        public List<Move> GetAllNonCaptureMoves(PieceColor color)
        {
            List<Move> moves = new List<Move>();

            var pieces = GetPieces(color);

            // Iterates over every piece, and adds all moves to the general list
            foreach (var piece in pieces.ToList())
            {
                GetLegalMoves(piece).ForEach(item => moves.Add(item));
            }

            return moves;
        }
         
        /// <summary>
        /// Gets the first player (white)
        /// </summary>
        /// <returns>White player object</returns>
        public Player GetPlayer1()
        {
            return this.player1;
        }

        /// <summary>
        /// Gets the second player (black)
        /// </summary>
        /// <returns>Black player object</returns>
        public Player GetPlayer2()
        {
            return this.player2;
        }
    }
}
