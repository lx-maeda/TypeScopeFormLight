namespace TypeScopeFormLight
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
            Buttonフォルダ選択Read = new Button();
            TextBoxフォルダ選択Read = new TextBox();
            CheckBoxPublicのみ = new CheckBox();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            Buttonフォルダ選択Save = new Button();
            TextBoxフォルダ選択Save = new TextBox();
            ButtonMD出力 = new Button();
            DataGridView1 = new DataGridView();
            label1 = new Label();
            DataGridView2 = new DataGridView();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DataGridView2).BeginInit();
            SuspendLayout();
            // 
            // Buttonフォルダ選択Read
            // 
            Buttonフォルダ選択Read.Location = new Point(518, 13);
            Buttonフォルダ選択Read.Margin = new Padding(3, 4, 3, 4);
            Buttonフォルダ選択Read.Name = "Buttonフォルダ選択Read";
            Buttonフォルダ選択Read.Size = new Size(100, 25);
            Buttonフォルダ選択Read.TabIndex = 0;
            Buttonフォルダ選択Read.Text = "フォルダ選択";
            Buttonフォルダ選択Read.UseVisualStyleBackColor = true;
            Buttonフォルダ選択Read.Click += Buttonフォルダ選択Read_Click;
            // 
            // TextBoxフォルダ選択Read
            // 
            TextBoxフォルダ選択Read.Location = new Point(12, 13);
            TextBoxフォルダ選択Read.Margin = new Padding(3, 4, 3, 4);
            TextBoxフォルダ選択Read.Name = "TextBoxフォルダ選択Read";
            TextBoxフォルダ選択Read.ReadOnly = true;
            TextBoxフォルダ選択Read.Size = new Size(500, 25);
            TextBoxフォルダ選択Read.TabIndex = 1;
            // 
            // CheckBoxPublicのみ
            // 
            CheckBoxPublicのみ.AutoSize = true;
            CheckBoxPublicのみ.Checked = true;
            CheckBoxPublicのみ.CheckState = CheckState.Checked;
            CheckBoxPublicのみ.Location = new Point(624, 16);
            CheckBoxPublicのみ.Name = "CheckBoxPublicのみ";
            CheckBoxPublicのみ.Size = new Size(88, 22);
            CheckBoxPublicのみ.TabIndex = 2;
            CheckBoxPublicのみ.Text = "public のみ";
            CheckBoxPublicのみ.UseVisualStyleBackColor = true;
            CheckBoxPublicのみ.CheckedChanged += CheckBoxPublicのみ_CheckedChanged;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1 });
            toolStrip1.Location = new Point(0, 536);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(984, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(16, 22);
            toolStripLabel1.Text = "...";
            // 
            // Buttonフォルダ選択Save
            // 
            Buttonフォルダ選択Save.Location = new Point(518, 507);
            Buttonフォルダ選択Save.Margin = new Padding(3, 4, 3, 4);
            Buttonフォルダ選択Save.Name = "Buttonフォルダ選択Save";
            Buttonフォルダ選択Save.Size = new Size(100, 25);
            Buttonフォルダ選択Save.TabIndex = 0;
            Buttonフォルダ選択Save.Text = "フォルダ選択";
            Buttonフォルダ選択Save.UseVisualStyleBackColor = true;
            Buttonフォルダ選択Save.Click += Buttonフォルダ選択Save_Click;
            // 
            // TextBoxフォルダ選択Save
            // 
            TextBoxフォルダ選択Save.Location = new Point(12, 507);
            TextBoxフォルダ選択Save.Margin = new Padding(3, 4, 3, 4);
            TextBoxフォルダ選択Save.Name = "TextBoxフォルダ選択Save";
            TextBoxフォルダ選択Save.ReadOnly = true;
            TextBoxフォルダ選択Save.Size = new Size(500, 25);
            TextBoxフォルダ選択Save.TabIndex = 1;
            // 
            // ButtonMD出力
            // 
            ButtonMD出力.Location = new Point(624, 507);
            ButtonMD出力.Margin = new Padding(3, 4, 3, 4);
            ButtonMD出力.Name = "ButtonMD出力";
            ButtonMD出力.Size = new Size(100, 25);
            ButtonMD出力.TabIndex = 0;
            ButtonMD出力.Text = "MD出力";
            ButtonMD出力.UseVisualStyleBackColor = true;
            ButtonMD出力.Click += ButtonMD出力_Click;
            // 
            // DataGridView1
            // 
            DataGridView1.AllowUserToAddRows = false;
            DataGridView1.AllowUserToDeleteRows = false;
            DataGridView1.AllowUserToResizeColumns = false;
            DataGridView1.AllowUserToResizeRows = false;
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.Location = new Point(12, 100);
            DataGridView1.MultiSelect = false;
            DataGridView1.Name = "DataGridView1";
            DataGridView1.ReadOnly = true;
            DataGridView1.RowHeadersVisible = false;
            DataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DataGridView1.ScrollBars = ScrollBars.Vertical;
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridView1.ShowCellErrors = false;
            DataGridView1.ShowCellToolTips = false;
            DataGridView1.ShowEditingIcon = false;
            DataGridView1.ShowRowErrors = false;
            DataGridView1.Size = new Size(240, 400);
            DataGridView1.TabIndex = 4;
            DataGridView1.CellClick += DataGridView1_CellClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 73);
            label1.Name = "label1";
            label1.Size = new Size(20, 18);
            label1.TabIndex = 5;
            label1.Text = "...";
            // 
            // DataGridView2
            // 
            DataGridView2.AllowUserToAddRows = false;
            DataGridView2.AllowUserToDeleteRows = false;
            DataGridView2.AllowUserToResizeColumns = false;
            DataGridView2.AllowUserToResizeRows = false;
            DataGridView2.CausesValidation = false;
            DataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView2.Location = new Point(258, 100);
            DataGridView2.MultiSelect = false;
            DataGridView2.Name = "DataGridView2";
            DataGridView2.RowHeadersVisible = false;
            DataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridView2.ShowCellErrors = false;
            DataGridView2.ShowCellToolTips = false;
            DataGridView2.ShowEditingIcon = false;
            DataGridView2.ShowRowErrors = false;
            DataGridView2.Size = new Size(714, 400);
            DataGridView2.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            ClientSize = new Size(984, 561);
            Controls.Add(label1);
            Controls.Add(DataGridView2);
            Controls.Add(DataGridView1);
            Controls.Add(toolStrip1);
            Controls.Add(CheckBoxPublicのみ);
            Controls.Add(TextBoxフォルダ選択Save);
            Controls.Add(ButtonMD出力);
            Controls.Add(Buttonフォルダ選択Save);
            Controls.Add(TextBoxフォルダ選択Read);
            Controls.Add(Buttonフォルダ選択Read);
            Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            Text = "TypeScopeFormLight";
            Load += MainForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)DataGridView2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Buttonフォルダ選択Read;
        private TextBox TextBoxフォルダ選択Read;
        private CheckBox CheckBoxPublicのみ;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private Button Buttonフォルダ選択Save;
        private TextBox TextBoxフォルダ選択Save;
        private Button ButtonMD出力;
        private DataGridView DataGridView1;
        private Label label1;
        private DataGridView DataGridView2;
    }
}
