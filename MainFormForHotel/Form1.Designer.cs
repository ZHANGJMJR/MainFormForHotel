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
            datePickerRange1 = new AntdUI.DatePickerRange();
            progress1 = new AntdUI.Progress();
            textBox1 = new TextBox();
            start_btn = new Button();
            stop_btn = new Button();
            SuspendLayout();
            // 
            // exit_btn
            // 
            exit_btn.Image = (Image)resources.GetObject("exit_btn.Image");
            exit_btn.ImageAlign = ContentAlignment.MiddleLeft;
            exit_btn.Location = new Point(541, 365);
            exit_btn.Name = "exit_btn";
            exit_btn.Padding = new Padding(16, 0, 0, 0);
            exit_btn.Size = new Size(206, 66);
            exit_btn.TabIndex = 1;
            exit_btn.Text = "Exit";
            exit_btn.UseVisualStyleBackColor = true;
            exit_btn.Click += exit_btn_Click;
            // 
            // button1
            // 
            button1.Location = new Point(26, 261);
            button1.Name = "button1";
            button1.Size = new Size(99, 43);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Visible = false;
            button1.Click += button1_Click;
            // 
            // exesync_btn
            // 
            exesync_btn.Image = Properties.Resources.execute;
            exesync_btn.ImageAlign = ContentAlignment.MiddleLeft;
            exesync_btn.Location = new Point(541, 287);
            exesync_btn.Name = "exesync_btn";
            exesync_btn.Padding = new Padding(16, 0, 0, 0);
            exesync_btn.Size = new Size(206, 66);
            exesync_btn.TabIndex = 3;
            exesync_btn.Text = "Execute";
            exesync_btn.UseVisualStyleBackColor = true;
            exesync_btn.Click += exesync_btn_Click;
            // 
            // datePickerRange1
            // 
            datePickerRange1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            datePickerRange1.Location = new Point(26, 30);
            datePickerRange1.MinDate = new DateTime(2023, 1, 1, 0, 0, 0, 0);
            datePickerRange1.Name = "datePickerRange1";
            datePickerRange1.Size = new Size(408, 66);
            datePickerRange1.TabIndex = 6;
            datePickerRange1.Text = "2025-03-13\t2025-03-13";
            datePickerRange1.Value = new DateTime[]
    {
    new DateTime(2025, 3, 13, 0, 0, 0, 0),
    new DateTime(2025, 3, 13, 0, 0, 0, 0)
    };
            // 
            // progress1
            // 
            progress1.ContainerControl = this;
            progress1.Location = new Point(37, 353);
            progress1.Name = "progress1";
            progress1.Size = new Size(409, 79);
            progress1.TabIndex = 7;
            progress1.Text = "progress1";
            progress1.Value = 0.3F;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Microsoft YaHei UI", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBox1.Location = new Point(26, 117);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(265, 54);
            textBox1.TabIndex = 8;
            textBox1.Text = "* * * * * * *";
            textBox1.TextAlign = HorizontalAlignment.Center;
            // 
            // start_btn
            // 
            start_btn.Image = (Image)resources.GetObject("start_btn.Image");
            start_btn.ImageAlign = ContentAlignment.MiddleLeft;
            start_btn.Location = new Point(541, 131);
            start_btn.Name = "start_btn";
            start_btn.Padding = new Padding(16, 0, 0, 0);
            start_btn.Size = new Size(206, 66);
            start_btn.TabIndex = 9;
            start_btn.Text = "Start";
            start_btn.UseVisualStyleBackColor = true;
            // 
            // stop_btn
            // 
            stop_btn.Image = (Image)resources.GetObject("stop_btn.Image");
            stop_btn.ImageAlign = ContentAlignment.MiddleLeft;
            stop_btn.Location = new Point(541, 209);
            stop_btn.Name = "stop_btn";
            stop_btn.Padding = new Padding(16, 0, 0, 0);
            stop_btn.Size = new Size(206, 66);
            stop_btn.TabIndex = 10;
            stop_btn.Text = "Stop";
            stop_btn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(774, 455);
            Controls.Add(stop_btn);
            Controls.Add(start_btn);
            Controls.Add(textBox1);
            Controls.Add(progress1);
            Controls.Add(datePickerRange1);
            Controls.Add(exesync_btn);
            Controls.Add(button1);
            Controls.Add(exit_btn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "HotelBackEnd";
            Activated += Form1_Activated;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button exit_btn;
        private Button button1;
        private Button exesync_btn;
        private AntdUI.DatePickerRange datePickerRange1;
        private AntdUI.Progress progress1;
        private TextBox textBox1;
        private Button stop_btn;
        private Button start_btn;
    }
}
