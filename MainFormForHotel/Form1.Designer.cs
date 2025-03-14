namespace MainFormForHotel
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            exit_btn = new Button();
            button1 = new Button();
            exesync_btn = new Button();
            SuspendLayout();
            // 
            // exit_btn
            // 
            exit_btn.Location = new Point(494, 366);
            exit_btn.Name = "exit_btn";
            exit_btn.Size = new Size(206, 55);
            exit_btn.TabIndex = 1;
            exit_btn.Text = "Exit";
            exit_btn.UseVisualStyleBackColor = true;
            exit_btn.Click += exit_btn_Click;
            // 
            // button1
            // 
            button1.Location = new Point(95, 198);
            button1.Name = "button1";
            button1.Size = new Size(155, 86);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // exesync_btn
            // 
            exesync_btn.Location = new Point(494, 253);
            exesync_btn.Name = "exesync_btn";
            exesync_btn.Size = new Size(206, 55);
            exesync_btn.TabIndex = 3;
            exesync_btn.Text = "Execute";
            exesync_btn.UseVisualStyleBackColor = true;
            exesync_btn.Click += exesync_btn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(774, 455);
            Controls.Add(exesync_btn);
            Controls.Add(button1);
            Controls.Add(exit_btn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "HotelBackEnd";
            ResumeLayout(false);
        }

        #endregion
        private Button exit_btn;
        private Button button1;
        private Button exesync_btn;
    }
}
