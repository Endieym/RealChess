using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.BoardOperations;

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
            King king = color == PieceColor.WHITE ? player1.GetKing() :
                player2.GetKing();

            ulong attackingSquares = color == PieceColor.WHITE ? player1.GetAttacks() :
                player2.GetAttacks();

            return true;
        }

        // Updates the board according to a piece moving
        // ChessPiece piece, int oldKey, int newKey, bool isCapture
        public void UpdateBoard(Move move)
        {
            // Checks if the piece has moved before, if not, change the attribute
            if (!move.PieceMoved.HasMoved)
                move.PieceMoved.HasMoved = true;

            var playerColor = move.PieceMoved.Color;
            
            var oldKey = move.StartSquare;
            var newKey = move.EndSquare;
            bool isCapture = move.IsCapture;

            if (playerColor == PieceColor.WHITE)
            {
                this.player1.UpdatePiece(oldKey, newKey);
                this.whiteBoard ^= (ulong)1 << oldKey; // Removes previous position
                this.whiteBoard |= (ulong)1 << newKey; // Adds new position

                // If move is a capture, delete the piece from the other player
                // and add the captured piece to player's captured pieces list
                if (isCapture)
                {
                    this.player1.AddCapture(player2.Pieces[newKey]);
                    if(!move.IsEnPassantCapture)
                        this.player2.DeletePiece(newKey);
                    else
                        this.player2.DeletePiece(newKey + 8);
                    this.blackBoard ^= (ulong)1 << newKey;
                    
                }
            }
            else
            {
                this.player2.UpdatePiece(oldKey, newKey);
                this.blackBoard ^= (ulong)1 << oldKey; // Removes previous position
                this.blackBoard |= (ulong)1 << newKey; // Adds new position

                // If move is a capture, delete the piece from the other player
                // and add the captured piece to player's captured pieces list
                if (isCapture)
                {
                    this.player2.AddCapture(player2.Pieces[newKey]);
                    if (!move.IsEnPassantCapture)
                        this.player1.DeletePiece(newKey);
                    else
                        this.player1.DeletePiece(newKey - 8);
                    this.whiteBoard ^= (ulong)1 << newKey;

                }

            }
            movesList.Add(move);
            this.bitBoard = blackBoard | whiteBoard;
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

            // checks for legal moves using the moves bitmask
            while (finalMoves != 0)
            {
                // determine bit index, also referred as BitScan
                int bitIndex = (int)Math.Log(finalMoves & ~(finalMoves - 1), 2);
                Move newMove = new Move(bitIndex, piece);
                list.Add(newMove);
                finalMoves &= finalMoves - 1; // reset LS1B
            }
            return list;
        }

        public List<Move> GetCapturesPiece(ChessPiece piece)
        {
            List<Move> captureList = new List<Move>();
            // Gets the masks for the captures, according to piece type
            ulong movesMask = piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures():
                piece.GenerateLegalMoves(bitBoard);

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
                    captureList.Add(enPassant);

                }
            }

            while (movesMask != 0)
            {
                // determine bit index, also referred as BitScan
                int bitIndex = (int)Math.Log(movesMask & ~(movesMask - 1),2);
                Move newMove = new Move(bitIndex, piece)
                {
                    IsCapture = true
                };
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
