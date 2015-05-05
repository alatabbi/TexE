namespace ATABBI.TexE
{
    partial class OutputWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputWindow));
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Tabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox
            // 
            this.comboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.Items.AddRange(new object[] {
            "Build"});
            this.comboBox.Location = new System.Drawing.Point(10, 10);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(630, 21);
            this.comboBox.TabIndex = 1;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.Location = new System.Drawing.Point(3, 3);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(616, 276);
            this.textBoxOutput.TabIndex = 2;
            this.textBoxOutput.WordWrap = false;
            this.textBoxOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxOutput_KeyDown);
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.tabPage1);
            this.Tabs.Controls.Add(this.tabPage2);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(10, 31);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(630, 308);
            this.Tabs.TabIndex = 4;
            this.Tabs.SelectedIndexChanged += new System.EventHandler(this.Tabs_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(622, 282);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Build";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Controls.Add(this.splitter1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(622, 282);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Errors";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader1});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(6, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(613, 276);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "!";
            this.columnHeader2.Width = 18;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Double click the row to view the error";
            this.columnHeader1.Width = 600;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(3, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 276);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Error_red_16x16_cyan.ico");
            this.imageList1.Images.SetKeyName(1, "Warning_yellow_7231_16x16_cyan.ico");
            this.imageList1.Images.SetKeyName(2, "FeatureNotAvailable_5734_12x12_16x.ico");
            this.imageList1.Images.SetKeyName(3, "Flagthread_7317.ico");
            this.imageList1.Images.SetKeyName(4, "Information_blue_6227_16x16_cyan.ico");
            this.imageList1.Images.SetKeyName(5, "Warning_grey_7315_16x16.ico");
            this.imageList1.Images.SetKeyName(6, "Error_grey_677_16x16.ico");
            // 
            // OutputWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(650, 349);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.comboBox);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputWindow";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
            this.TabText = "Output";
            this.Text = "Output";
            this.Load += new System.EventHandler(this.OutputWindow_Load);
            this.Tabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ImageList imageList1;
    }
}