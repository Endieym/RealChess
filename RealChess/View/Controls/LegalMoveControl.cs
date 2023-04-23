using RealChess.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealChess.View.Controls
{
    /// <summary>
    /// Control for a legal move shown on the board
    /// </summary>
    public partial class LegalMoveControl : UserControl
    {
        public event EventHandler<TransferEventArgs> Transfer;
        public bool IsCapture { get; set; }
        public Move CurrentMove { get; set; }

        public LegalMoveControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets this control as a capture move
        /// </summary>
        public void SetCapture()
        {
            IsCapture = true;
            Image image = Properties.Resources.ResourceManager.GetObject("CaptureMove") as Image;
            this.piecePic.Image = image;
            
        }

        /// <summary>
        /// On transferring from square to square
        /// </summary>
        /// <param name="e">TransferEventArgs which contains the move made</param>
        private void OnTransfer(TransferEventArgs e)
        {
            Transfer?.Invoke(this, e);
        }

        // Event handler for when the legal move control was clicked
        private void piecePic_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseArgs = e as MouseEventArgs;
            piecePic.Click -= piecePic_Click;

            // Check if the control is clicked with the left mouse button
            if (mouseArgs.Button == MouseButtons.Left)
            {
                // Raise the transfer event with the clicked legal move control's coordinates
                OnTransfer(new TransferEventArgs(this.CurrentMove));
            }
        }

    }
}
