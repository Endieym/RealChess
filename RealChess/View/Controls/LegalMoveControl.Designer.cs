namespace RealChess.View.Controls
{
    partial class LegalMoveControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegalMoveControl));
            this.piecePic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.piecePic)).BeginInit();
            this.SuspendLayout();
            // 
            // piecePic
            // 
            this.piecePic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.piecePic.Cursor = System.Windows.Forms.Cursors.Default;
            this.piecePic.Image = global::RealChess.Properties.Resources.LegalMove;
            this.piecePic.InitialImage = ((System.Drawing.Image)(resources.GetObject("piecePic.InitialImage")));
            this.piecePic.Location = new System.Drawing.Point(0, 0);
            this.piecePic.Margin = new System.Windows.Forms.Padding(0);
            this.piecePic.Name = "piecePic";
            this.piecePic.Size = new System.Drawing.Size(65, 65);
            this.piecePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.piecePic.TabIndex = 1;
            this.piecePic.TabStop = false;
            this.piecePic.Click += new System.EventHandler(this.piecePic_Click);
            // 
            // LegalMoveControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::RealChess.Properties.Resources.LegalMove;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.piecePic);
            this.DoubleBuffered = true;
            this.Name = "LegalMoveControl";
            ((System.ComponentModel.ISupportInitialize)(this.piecePic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox piecePic;
    }
}
