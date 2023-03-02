using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealChess.View.Controls
{
    public partial class LegalMoveControl : UserControl
    {
        public event EventHandler<TransferEventArgs> Transfer;

        public LegalMoveControl()
        {
            InitializeComponent();
        }


        private void OnTransfer(TransferEventArgs e)
        {
            Transfer?.Invoke(this, e);
        }

        private void LegalMoveControl_MouseClick(object sender, MouseEventArgs e)
        {

            // Check if the control is clicked with the left mouse button
            if (e.Button == MouseButtons.Left)
            {
                // Raise the transfer event with the clicked legal move control's coordinates
                OnTransfer(new TransferEventArgs((Panel)this.Parent));
            }
        }

        private void piecePic_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseArgs = e as MouseEventArgs;

            // Check if the control is clicked with the left mouse button
            if (mouseArgs.Button == MouseButtons.Left)
            {
                // Raise the transfer event with the clicked legal move control's coordinates
                OnTransfer(new TransferEventArgs((Panel)this.Parent));
            }
        }
    }
}
