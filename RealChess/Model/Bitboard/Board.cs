using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.BoardOperations;
using System.Collections.Specialized;

namespace RealChess.Model
{

    public class Board
    {
        public const int SIZE = 8;
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

        

        public bool IsKingUnderAttack(PieceColor color)
        {
            ulong attackingSquares = GetPlayerAttack(color);

            return color == PieceColor.WHITE ? player1.InCheck(attackingSquares) :
                player2.InCheck(attackingSquares);
           
        }
        private King GetKing(PieceColor color)
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
        


        // Updates the board according to a piece moving
        // ChessPiece piece, int oldKey, int newKey, bool isCapture
        public void UpdateBoard(Move move)
        {
            // Checks if the piece has moved before, if not, change the attribute
            if (!move.PieceMoved.HasMoved)
                move.PieceMoved.HasMoved = true;

            var oppositeColor = GetOppositeColor(move.PieceMoved.Color);
            if (move.IsCheck)
            {
                GetKing(oppositeColor).InCheck = true;
            }
            bool isCapture = move.IsCapture;
            var newKey = move.EndSquare;
            UpdateDataStructures(move);

            if (isCapture)
            {
                if (move.PieceMoved.Color == PieceColor.WHITE)
                {
                    this.player1.AddCapture(player2.Pieces[newKey]);

                }
                else
                {
                    this.player2.AddCapture(player1.Pieces[newKey]);

                }
                UpdateCaptures(move);
                

            }


            this.bitBoard = blackBoard | whiteBoard;
            if (IsKingUnderAttack(PieceColor.WHITE))
                Console.WriteLine("Test!");
        }

        public void UpdateCaptures(Move move)
        {
            var playerColor = move.PieceMoved.Color;
            var newKey = move.EndSquare;

            if (playerColor == PieceColor.WHITE)
            {
                
                    if (!move.IsEnPassantCapture)
                        this.player2.DeletePiece(newKey);
                    else
                        this.player2.DeletePiece(newKey + 8);
                    this.blackBoard ^= (ulong)1 << newKey;
                
            }
            else
            {                
                    if (!move.IsEnPassantCapture)
                        this.player1.DeletePiece(newKey);
                    else
                        this.player1.DeletePiece(newKey - 8);
                    this.whiteBoard ^= (ulong)1 << newKey;               
            }
        }

        public void UpdateDataStructures(Move move)
        {
            var playerColor = move.PieceMoved.Color;

            var oldKey = move.StartSquare;
            var newKey = move.EndSquare;

            if (playerColor == PieceColor.WHITE)
            {
                this.player1.UpdatePiece(oldKey, newKey);
                this.whiteBoard ^= (ulong)1 << oldKey; // Removes previous position
                this.whiteBoard |= (ulong)1 << newKey; // Adds new position

            }
            else
            {
                this.player2.UpdatePiece(oldKey, newKey);
                this.blackBoard ^= (ulong)1 << oldKey; // Removes previous position
                this.blackBoard |= (ulong)1 << newKey; // Adds new position
            }

            movesList.Add(move);


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

            // Removes moves over friendly squares
            finalMoves &= piece.Color == PieceColor.WHITE ? ~whiteBoard : ~blackBoard;

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
                list.Add(newMove);
                finalMoves &= finalMoves - 1; // reset LS1B
            }
            return list;
        }

        

        public ulong GetAttacksMask(ChessPiece piece)
        {

            return piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures():
                piece.GenerateLegalMoves(bitBoard);

        }

        public bool IsMoveLegal(Move newMove)
        {
            MakeTemporaryMove(newMove);

            bool result = IsKingUnderAttack(newMove.PieceMoved.Color);

            UndoMove();

            return !result;
        }

        public void MakeTemporaryMove(Move move)
        {
            UpdateDataStructures(move);
            if (move.IsCapture)
                UpdateCaptures(move);
            bitBoard = blackBoard | whiteBoard;
        }

        public bool IsMoveCheck(Move newMove)
        {
            MakeTemporaryMove(newMove);

            bool result = IsKingUnderAttack(GetOppositeColor(newMove.PieceMoved.Color));

            UndoMove();

            return result;

        }

        public void UndoMove()
        {
            int index = movesList.Count - 1;
            Move oldMove = movesList[index];
            Move undoMove = new Move(oldMove.StartSquare, oldMove.PieceMoved);
            UpdateDataStructures(undoMove);
            movesList.RemoveAt(index+1);
            movesList.RemoveAt(index);
            if (oldMove.IsCapture)
                UndoCapture(oldMove);
            bitBoard = blackBoard | whiteBoard;

        }

        public void UndoCapture(Move oldCapture)
        {
            var playerColor = oldCapture.PieceMoved.Color;
            var newKey = oldCapture.EndSquare;
            ChessPiece oldPiece = oldCapture.CapturedPiece;

            if (playerColor == PieceColor.WHITE)
            {

                if (!oldCapture.IsEnPassantCapture)
                    this.player2.AddPiece(oldPiece, newKey);
                else
                    this.player2.AddPiece(oldPiece, newKey + 8);
                this.blackBoard |= (ulong)1 << newKey;

            }
            else
            {
                if (!oldCapture.IsEnPassantCapture)
                    this.player1.AddPiece(oldPiece,newKey);
                else
                    this.player1.AddPiece(oldPiece,newKey - 8);
                this.whiteBoard |= (ulong)1 << newKey;
            }
        }
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
                        IsEnPassantCapture = true
                    };
                    if(IsMoveLegal(enPassant))
                        captureList.Add(enPassant);

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
                captureList.Add(newMove);
                movesMask &= movesMask - 1; // reset LS1B

            }

            return captureList;
            

        }

        public Player GetPlayer1()
        {
            return this.player1;
        }
        public Player GetPlayer2()
        {
            return this.player2;
        }


    }
}
