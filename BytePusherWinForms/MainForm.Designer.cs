namespace BytePusherWinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ScreenPictureBox = new System.Windows.Forms.PictureBox();
            this.OpenBpFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.LoadBpButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ScreenPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ScreenPictureBox
            // 
            this.ScreenPictureBox.BackColor = System.Drawing.Color.Black;
            this.ScreenPictureBox.Location = new System.Drawing.Point(93, 12);
            this.ScreenPictureBox.Name = "ScreenPictureBox";
            this.ScreenPictureBox.Size = new System.Drawing.Size(256, 256);
            this.ScreenPictureBox.TabIndex = 0;
            this.ScreenPictureBox.TabStop = false;
            // 
            // OpenBpFileDialog
            // 
            this.OpenBpFileDialog.Filter = "*.bp|BytePusher files (*.bp)|*.*|All files";
            // 
            // LoadBpButton
            // 
            this.LoadBpButton.Location = new System.Drawing.Point(12, 12);
            this.LoadBpButton.Name = "LoadBpButton";
            this.LoadBpButton.Size = new System.Drawing.Size(75, 23);
            this.LoadBpButton.TabIndex = 0;
            this.LoadBpButton.Text = "Load BP File";
            this.LoadBpButton.UseVisualStyleBackColor = true;
            this.LoadBpButton.Click += new System.EventHandler(this.LoadBpButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.LoadBpButton);
            this.Controls.Add(this.ScreenPictureBox);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "BytePusher";
            ((System.ComponentModel.ISupportInitialize)(this.ScreenPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox ScreenPictureBox;
        private OpenFileDialog OpenBpFileDialog;
        private Button LoadBpButton;
    }
}