namespace ScreenShotLite
{
    partial class Main
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
            Title = new Label();
            SaveLocationLabel = new Label();
            Panel = new Panel();
            checkBox7 = new CheckBox();
            button8 = new Button();
            button7 = new Button();
            checkBox6 = new CheckBox();
            numericUpDown2 = new NumericUpDown();
            checkBox5 = new CheckBox();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            comboBox2 = new ComboBox();
            comboBox1 = new ComboBox();
            checkBox4 = new CheckBox();
            checkBox3 = new CheckBox();
            button3 = new Button();
            textBox2 = new TextBox();
            numericUpDown1 = new NumericUpDown();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            button2 = new Button();
            button1 = new Button();
            textBox1 = new TextBox();
            richTextBox1 = new RichTextBox();
            label1 = new Label();
            panel1 = new Panel();
            button4 = new Button();
            label2 = new Label();
            label3 = new Label();
            Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Title
            // 
            Title.AutoSize = true;
            Title.Font = new Font("Noto Sans SC", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title.Location = new Point(12, 9);
            Title.Name = "Title";
            Title.Size = new Size(149, 27);
            Title.TabIndex = 0;
            Title.Text = "ScreenShotLite";
            // 
            // SaveLocationLabel
            // 
            SaveLocationLabel.AutoSize = true;
            SaveLocationLabel.Location = new Point(3, 6);
            SaveLocationLabel.Name = "SaveLocationLabel";
            SaveLocationLabel.Size = new Size(59, 17);
            SaveLocationLabel.TabIndex = 1;
            SaveLocationLabel.Text = "保存位置:";
            // 
            // Panel
            // 
            Panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Panel.Controls.Add(label3);
            Panel.Controls.Add(label2);
            Panel.Controls.Add(checkBox7);
            Panel.Controls.Add(button8);
            Panel.Controls.Add(button7);
            Panel.Controls.Add(checkBox6);
            Panel.Controls.Add(numericUpDown2);
            Panel.Controls.Add(checkBox5);
            Panel.Controls.Add(radioButton3);
            Panel.Controls.Add(radioButton2);
            Panel.Controls.Add(radioButton1);
            Panel.Controls.Add(comboBox2);
            Panel.Controls.Add(comboBox1);
            Panel.Controls.Add(checkBox4);
            Panel.Controls.Add(checkBox3);
            Panel.Controls.Add(button3);
            Panel.Controls.Add(textBox2);
            Panel.Controls.Add(numericUpDown1);
            Panel.Controls.Add(checkBox2);
            Panel.Controls.Add(checkBox1);
            Panel.Controls.Add(button2);
            Panel.Controls.Add(button1);
            Panel.Controls.Add(textBox1);
            Panel.Controls.Add(SaveLocationLabel);
            Panel.Location = new Point(12, 39);
            Panel.Name = "Panel";
            Panel.Size = new Size(698, 269);
            Panel.TabIndex = 2;
            // 
            // checkBox7
            // 
            checkBox7.AutoSize = true;
            checkBox7.Location = new Point(86, 148);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(128, 21);
            checkBox7.TabIndex = 31;
            checkBox7.Text = "使用Alt+PrtSc截图";
            checkBox7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button8.Location = new Point(623, 179);
            button8.Name = "button8";
            button8.Size = new Size(75, 23);
            button8.TabIndex = 30;
            button8.Text = "刷新";
            button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button7.Location = new Point(623, 147);
            button7.Name = "button7";
            button7.Size = new Size(75, 23);
            button7.TabIndex = 29;
            button7.Text = "刷新";
            button7.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Location = new Point(3, 90);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(102, 21);
            checkBox6.TabIndex = 26;
            checkBox6.Text = "自动截图间隔:";
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown2.Location = new Point(111, 88);
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(587, 23);
            numericUpDown2.TabIndex = 25;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new Point(261, 32);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(111, 21);
            checkBox5.TabIndex = 24;
            checkBox5.Text = "截图后打开图片";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Checked = true;
            radioButton3.Location = new Point(3, 207);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(74, 21);
            radioButton3.TabIndex = 23;
            radioButton3.TabStop = true;
            radioButton3.Text = "框选截图";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(3, 178);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(77, 21);
            radioButton2.TabIndex = 22;
            radioButton2.Text = "屏幕截图:";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(3, 147);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(77, 21);
            radioButton1.TabIndex = 21;
            radioButton1.Text = "窗口截图:";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(87, 177);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(530, 25);
            comboBox2.TabIndex = 20;
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(220, 146);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(397, 25);
            comboBox1.TabIndex = 17;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(3, 119);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(78, 21);
            checkBox4.TabIndex = 14;
            checkBox4.Text = "热键捕捉:";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(3, 61);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(90, 21);
            checkBox3.TabIndex = 13;
            checkBox3.Text = "截图倒计时:";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(3, 234);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 12;
            button3.Text = "截图";
            button3.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Location = new Point(87, 117);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(611, 23);
            textBox2.TabIndex = 11;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown1.Location = new Point(99, 59);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(599, 23);
            numericUpDown1.TabIndex = 9;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(120, 32);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(135, 21);
            checkBox2.TabIndex = 6;
            checkBox2.Text = "截图后复制到剪切板";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(3, 32);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(111, 21);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "截图时隐藏窗口";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(623, 3);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 4;
            button2.Text = "打开";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(542, 3);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "选择";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(68, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(462, 23);
            textBox1.TabIndex = 2;
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Location = new Point(3, 26);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(692, 204);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 6);
            label1.Name = "label1";
            label1.Size = new Size(35, 17);
            label1.TabIndex = 4;
            label1.Text = "日志:";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(button4);
            panel1.Controls.Add(richTextBox1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(12, 314);
            panel1.Name = "panel1";
            panel1.Size = new Size(698, 236);
            panel1.TabIndex = 5;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.Location = new Point(620, 3);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 5;
            button4.Text = "保存日志";
            button4.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(91, 237);
            label2.Name = "label2";
            label2.Size = new Size(164, 17);
            label2.TabIndex = 32;
            label2.Text = "自动截图时再按一次截图停止";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(613, 237);
            label3.Name = "label3";
            label3.Size = new Size(82, 17);
            label3.TabIndex = 33;
            label3.Text = "©CEllOMiKA";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(723, 556);
            Controls.Add(panel1);
            Controls.Add(Panel);
            Controls.Add(Title);
            Name = "Main";
            Text = "ScreenShotLite";
            Panel.ResumeLayout(false);
            Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title;
        private Label SaveLocationLabel;
        private Panel Panel;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Button button2;
        private Button button1;
        private TextBox textBox1;
        private TextBox textBox2;
        private NumericUpDown numericUpDown1;
        private ComboBox comboBox1;
        private CheckBox checkBox4;
        private CheckBox checkBox3;
        private Button button3;
        private CheckBox checkBox5;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private ComboBox comboBox2;
        private CheckBox checkBox6;
        private NumericUpDown numericUpDown2;
        private RichTextBox richTextBox1;
        private Label label1;
        private Panel panel1;
        private Button button4;
        private Button button8;
        private Button button7;
        private CheckBox checkBox7;
        private Label label3;
        private Label label2;
    }
}
