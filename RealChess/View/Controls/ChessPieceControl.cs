using RealChess.Model.ChessPieces;
using System.Drawing;
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

        /// <summary>
        /// Sets the Image of picture box according to piece parameter
        /// </summary>
        /// <param name="piece">The piece</param>
        public void SetPiece(ChessPiece piece)
        {
            Piece = piece;
            Image image = LoadPieceImage(piece);
            piecePic.Image = image;
            piecePic.Refresh();
           
        }

        /// <summary>
        /// Generates an image from resources according to piece type
        /// </summary>
        /// <param name="piece">The piece</param>
        /// <returns>The image of the piece. </returns>
        private Image LoadPieceImage(ChessPiece piece)
        {
            string imageName = $"{piece.Color.ToString().ToLower()}_{piece.Type.ToString().ToLower()}";
            
            return Properties.Resources.ResourceManager.GetObject(imageName) as Image;
            
        }

       
    }
}
