﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.ChessPieces
{
    internal class Bishop : ChessPiece
    {
        public Bishop()
        {
            this.Type = PieceType.BISHOP;

        }

        public Bishop(int row, int col) : base(row, col)
        {
            this.Type = PieceType.BISHOP;

        }

        public override ulong GetMoves()
        {
            throw new NotImplementedException();
        }
    }
}
