﻿using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.Bitboard.BoardOperations;
using System.Collections.Specialized;
using RealChess.Model.Bitboard;

namespace RealChess.Model
{

    public class Board
    {
        public const int SIZE = 8;

        internal Player WhitePlayer {
            set
            {
                player1 = value;
            }
            get
            {
                return this.player1;
            }
        }
        internal Player BlackPlayer {
            set
            {
                player2 = value;
            }
            get
            {
                return this.player2;
            }
        }

        internal ulong BlackBoard {
            set
            {
                blackBoard = value;
            }
            get
            {
                return blackBoard;
            }
        }
        internal ulong WhiteBoard {
            set
            {
                whiteBoard = value;
            }
            get
            {
                return whiteBoard;
            }
        }
        internal ulong BitBoard {
            set
            {
                bitBoard = value;
            }
            get
            {
                return bitBoard;
            }
        }

        Player player1;
        Player player2;
        UInt64 bitBoard;
        UInt64 whiteBoard;
        UInt64 blackBoard;

        List<Move> movesList;
        

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

        public int GetMorale(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.Morale : player2.Morale;
        }

        

        public bool IsKingUnderAttack(PieceColor color)
        {
            ulong attackingSquares = GetPlayerAttack(color);

            return color == PieceColor.WHITE ? player1.InCheck(attackingSquares) :
                player2.InCheck(attackingSquares);
           
        }
        internal King GetKing(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.GetKing() :
                player2.GetKing();
        }
        
        public int GetKingPos(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.GetKingPos() :
                player2.GetKingPos();
        }
        
        //public bool IsInCheck(PieceColor color)
        //{
        //    return color == PieceColor.WHITE ? player1.InCheck(attackingSquares) :
        //        player2.InCheck(attackingSquares);
        //}

        public ulong GetPlayerAttack(PieceColor color)
        {
            return color == PieceColor.WHITE ? player2.GetAttacks(whiteBoard, bitBoard) :
               player1.GetAttacks(blackBoard, bitBoard);
            
        }
        
        public ulong GetPlayerMove(PieceColor color)
        {
            return color == PieceColor.WHITE ? player1.GetMoves(whiteBoard, bitBoard) :
               player2.GetMoves(blackBoard, bitBoard);

        }


        public ulong GetPlayerAttacksAndOcuppied(PieceColor color)
        {
            ulong mask = GetPlayerMove(color);
            mask |= color == PieceColor.WHITE ? whiteBoard : blackBoard;
            return mask;
        }

        //public List<ulong> GetMovesMaskList(PieceColor color)
        //{
        //    var pieces = color == PieceColor.WHITE ? player1.Pieces : player2.Pieces;
        //    List<ulong> movesMasks = new List<ulong>();

            
        //    foreach (var piece in pieces.Values)
        //    {
        //        movesMasks.Add(piece.GenerateLegalMoves(bitBoard));
        //    }

        //    return movesMasks;
        //}


        // Updates the board according to a move made      
        public void UpdateBoard(Move move)
        {

            BoardUpdate.UpdateBoard(move);
                
            this.bitBoard = blackBoard | whiteBoard;
            

        }
        public Move MakePromotionMove(Move move, ChessPiece chessPiece)
        {
            var endKey = move.EndSquare;
            var beforeKey = move.StartSquare;
            if (chessPiece.Color == PieceColor.WHITE)
            {
                player1.SwitchPiece(beforeKey, chessPiece);

            }
            else
            {
                player2.SwitchPiece(beforeKey, chessPiece);
            }

            Move promotion = new Move(endKey, chessPiece)
            {
                IsCapture = move.IsCapture,
                DefendsCheck = move.DefendsCheck
            };

            if (IsMoveCheck(promotion)) 
            {
                promotion.IsCheck = true;
                promotion.Type = Move.MoveType.Check;

            }

            return promotion;

        }

        public virtual List<Move> GetMovesPiece(ChessPiece piece)
        {                        
            return GetPsuedoLegalMoves(piece);
        }
        
        public List<Move> GetPsuedoLegalMoves(ChessPiece piece)
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
                if (!IsMoveLegal(newMove))
                {
                    finalMoves &= finalMoves - 1; // reset LS1B
                    continue;

                }
                if (IsKingUnderAttack(piece.Color))
                    newMove.DefendsCheck = true;
                if (IsMoveCheck(newMove))
                {
                    newMove.IsCheck = true;
                    newMove.Type = Move.MoveType.Check;
                }
                if (piece.Type == PieceType.PAWN && IsMovePromotion(piece.Color, finalMoves))
                    newMove.IsPromotion = true;

                list.Add(newMove);
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
                if (castleQueen > 0)
                {

                    Move castleMove = new Move((int)Math.Log(castleQueen, 2), piece)
                    {
                        IsQueenSideCastle = true
                    };
                    if (IsMoveCheck(castleMove))
                        castleMove.IsCheck = true;
                    list.Add(castleMove);

                }
                if (castleKing > 0)
                {
                    Move castleMove = new Move((int)Math.Log(castleKing, 2), piece)
                    {
                        IsKingSideCastle = true
                    };
                    if (IsMoveCheck(castleMove))
                        castleMove.IsCheck = true;

                    list.Add(castleMove);

                }
            }

            return list;
        }

        public ulong GetBoard()
        {
            return this.bitBoard;
        }

        public List<Move> GetAllMoves()
        {
            return movesList;
        }


        // Returns the attack bitmask for a specific piece
        public ulong GetAttacksMask(ChessPiece piece)
        {

            return piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures():
                piece.GenerateLegalMoves(bitBoard);

        }

        // Checks if a move is legal ie the king is not under attack after the move
        public bool IsMoveLegal(Move newMove)
        {
            MakeTemporaryMove(newMove);

            bool result = IsKingUnderAttack(newMove.PieceMoved.Color);

            UndoMove();

            return !result;
        }

        // Makes a temporary move on the board (only data without graphics)
        public void MakeTemporaryMove(Move move)
        {
            BoardUpdate.UpdateDataStructures(move);
            if (move.IsCapture)
                BoardUpdate.UpdateCaptures(move);
            bitBoard = blackBoard | whiteBoard;
        }

        // Checks if a move results in checking the enemy king
        public bool IsMoveCheck(Move newMove)
        {
            //if (newMove.IsKingSideCastle)
            //{
            //    MakeTemporaryMove(new Move(newMove.EndSquare -1, Rook))
            //}
            MakeTemporaryMove(newMove);

            bool result = IsKingUnderAttack(GetOppositeColor(newMove.PieceMoved.Color));
            if (result)
            {
                if (!CanLegallyMove(GetOppositeColor(newMove.PieceMoved.Color)))
                    newMove.Type = Move.MoveType.Checkmate;
            }
            UndoMove();

            return result;

        }

        // Checks if the given bitboard for the mask results in a promotion
        public bool IsMovePromotion(PieceColor color ,ulong movePosition)
        {
            movePosition &= color == PieceColor.WHITE ? BitboardConstants.RankEight:
                BitboardConstants.RankOne;

            return movePosition > 0;

        }

        // Undos the last move made
        public void UndoMove()
        {

            BoardUpdate.UndoMove();

            bitBoard = blackBoard | whiteBoard;


        }


        // Checks if a player has any legal moves
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
                if (GetPsuedoLegalMoves(piece).Count > 0)
                    return true;
            }

            return false;
        }

        // Returns a list of captures for a single piece
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
                    
                    // Checks if the enpassant is a check
                    if (IsMoveCheck(enPassant))
                  
                    {
                        enPassant.IsCheck = true;
                        enPassant.Type = Move.MoveType.Check;
                        

                    }

                    
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


                if (!IsMoveLegal(newMove))
                {
                    movesMask &= movesMask - 1; // reset LS1B
                    continue;

                }
                
                if (IsKingUnderAttack(piece.Color))
                    newMove.DefendsCheck = true;

                if (IsMoveCheck(newMove))
                {
                    newMove.IsCheck = true;
                    newMove.Type = Move.MoveType.Check;


                }
                if (piece.Type == PieceType.PAWN && IsMovePromotion(piece.Color, movesMask))
                    newMove.IsPromotion = true;

                captureList.Add(newMove);
                movesMask &= movesMask - 1; // reset LS1B

                if (BoardLogic.GetCaptureValue(captureList[captureList.Count - 1].CapturedPiece, piece) > 0)
                    captureList[captureList.Count - 1].IsPositiveCapture = true;

            }


            return captureList;
            
        }

        // Returns a list of every single legal move/ capture a player has ( by color)
        public List<Move> GetAllPlayerMoves(PieceColor color)
        {
            List<Move> allMoves = new List<Move>();
           
            var pieces = color == PieceColor.WHITE ? GetPlayer1().Pieces : GetPlayer2().Pieces;
            
            // Iterates over every piece, and adds all moves to the general list
            foreach(var piece in pieces.Values.ToList())
            {

                GetMovesPiece(piece).ForEach(item => allMoves.Add(item));
                GetCapturesPiece(piece).ForEach(item => allMoves.Add(item));
               

            }
            
            return allMoves;
        }
         
        // Gets the first player (white)
        public Player GetPlayer1()
        {
            return this.player1;
        }

        // Gets the second player (black)
        public Player GetPlayer2()
        {
            return this.player2;
        }


    }
}
