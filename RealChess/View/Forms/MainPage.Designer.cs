namespace RealChess
{
    partial class MainPage
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_play = new System.Windows.Forms.Button();
            this.button_play_ai = new System.Windows.Forms.Button();
            this.button_rules = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // button_play
            // 
            this.button_play.Image = global::RealChess.Properties.Resources.black_bishop;
            this.button_play.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_play.Location = new System.Drawing.Point(301, 220);
            this.button_play.Name = "button_play";
            this.button_play.Size = new System.Drawing.Size(186, 28);
            this.button_play.TabIndex = 0;
            this.button_play.Text = "PASS & PLAY";
            this.button_play.UseVisualStyleBackColor = true;
            this.button_play.Click += new System.EventHandler(this.button_play_Click);
            // 
            // button_play_ai
            // 
            this.button_play_ai.Enabled = false;
            this.button_play_ai.Location = new System.Drawing.Point(301, 274);
            this.button_play_ai.Name = "button_play_ai";
            this.button_play_ai.Size = new System.Drawing.Size(186, 25);
            this.button_play_ai.TabIndex = 2;
            this.button_play_ai.Text = "PLAY AGAINST COMPUTER";
            this.button_play_ai.UseVisualStyleBackColor = true;
            // 
            // button_rules
            // 
            this.button_rules.Location = new System.Drawing.Point(301, 325);
            this.button_rules.Name = "button_rules";
            this.button_rules.Size = new System.Drawing.Size(186, 25);
            this.button_rules.TabIndex = 3;
            this.button_rules.Text = "RULES";
            this.button_rules.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = global::RealChess.Properties.Resources.realchessIcon;
            this.pictureBox1.InitialImage = global::RealChess.Properties.Resources.realchessIcon;
            this.pictureBox1.Location = new System.Drawing.Point(509, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(191, 189);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RealChess.Properties.Resources.REALCHESS2;
            this.pictureBox2.Location = new System.Drawing.Point(88, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(612, 189);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(171)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(837, 492);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_rules);
            this.Controls.Add(this.button_play_ai);
            this.Controls.Add(this.button_play);
            this.Controls.Add(this.pictureBox2);
            this.Name = "MainPage";
            this.Text = "   ";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_play;
        private System.Windows.Forms.Button button_play_ai;
        private System.Windows.Forms.Button button_rules;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

