using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealChess.View.Controls
{
    public class TransferEventArgs : EventArgs
    {

        public Panel TargetSquare { get; set; }

        public TransferEventArgs(Panel targetSquare)
        {
            TargetSquare = targetSquare;
        }

    }
}
