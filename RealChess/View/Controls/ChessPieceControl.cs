using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealChess.View
{
    public partial class ChessPieceControl : UserControl
    {
        public ChessPiece Piece { get; set; }

        public ChessPieceControl()
        {
            InitializeComponent();
        }
        public void SetPiece(ChessPiece piece)
        {
            Piece = piece;
            Image image = LoadPieceImage(piece);
            piecePic.Image = image;
            piecePic.Refresh();
           
        }

        private Image LoadPieceImage(ChessPiece piece)
        {
            string imageName = $"{piece.Color.ToString().ToLower()}_{piece.Type.ToString().ToLower()}";
            
            return Properties.Resources.ResourceManager.GetObject(imageName) as Image;
            
        }

       
    }
}
