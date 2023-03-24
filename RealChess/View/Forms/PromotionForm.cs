using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.View.Forms
{
    public partial class PromotionForm : Form
    {
        public PieceType PieceClicked { get; set; }
        public PromotionForm()
        {
            InitializeComponent();
            this.ControlBox = false;

            queenPic.Click += PictureBox_Click;
            bishopPic.Click += PictureBox_Click;
            knightPic.Click += PictureBox_Click;
            rookPic.Click += PictureBox_Click;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            switch (pictureBox.Name)
            {
                case "queenPic":

                    PieceClicked = PieceType.QUEEN;
                    this.Close();
                    break;

                case "knightPic":
                    PieceClicked = PieceType.KNIGHT;
                    this.Close();

                    break;

                case "rookPic":
                    PieceClicked = PieceType.ROOK;
                    this.Close();

                    break;

                case "bishopPic":
                    PieceClicked = PieceType.BISHOP;
                    this.Close();

                    break;
                
            }
        }
    }
}
