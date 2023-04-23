using RealChess.Model;
using System;

namespace RealChess.View.Controls
{
    /// <summary>
    /// Event args for transferring piece from one square to another
    /// </summary>
    public class TransferEventArgs : EventArgs
    {
        // Move of the control
        public Move CurrentMove { get; set; }

        public TransferEventArgs(Move move)
        {
            CurrentMove = move;
        }
    }
}
