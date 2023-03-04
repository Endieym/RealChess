﻿using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model
{

    public class Board
    {
        public static int SIZE = 8;
        Player player1;
        Player player2;
        UInt64 bitBoard;
        UInt64 whiteBoard;
        UInt64 blackBoard;
        public Board()
        {
            this.player1 = new Player(true);
            this.player2 = new Player(false);
            this.bitBoard = 0;
            this.whiteBoard = 0;
            this.blackBoard = 0;

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

        // Updates the board according to a piece moving
        public void UpdateBoard(ChessPiece piece, int oldKey, int newKey)
        {
            var playerColor = piece.Color;
            if (playerColor == PieceColor.WHITE)
            {
                this.GetPlayer1().UpdatePiece(oldKey, newKey);
                this.whiteBoard ^= (ulong)1 << oldKey; // Removes previous position
                this.whiteBoard |= (ulong)1 << newKey; // Adds new position

            }
            else
            {
                this.GetPlayer2().UpdatePiece(oldKey, newKey);
                this.blackBoard ^= (ulong)1 << oldKey; // Removes previous position
                this.blackBoard |= (ulong)1 << newKey; // Adds new position
            }

            //this.bitBoard ^= (ulong)1 << oldKey; 
            //this.bitBoard |= (ulong)1 << newKey; 
        }

        public List<int> GetCapturesPiece(ChessPiece piece)
        {
            List<int> list = new List<int>();
            // Gets the masks for the captures, according to piece type
            ulong movesMask = piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures():
                piece.GetMoves();

            // Checks colliding squares with enemy
            movesMask &= piece.Color == PieceColor.WHITE ? blackBoard : whiteBoard;
            // checks for legal moves using the moves bitmask
            for (int i = 0; i < 64; i++)
            {
                if ((movesMask & 1) > 0)
                    list.Add(i);
                // shift by one bit, to check the next bit
                movesMask >>= 1;

                // If mask is 0, there are no more legal moves
                if (movesMask == 0)
                    break;
            }
            return list;



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
