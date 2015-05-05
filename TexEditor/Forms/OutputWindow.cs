using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Classes;

namespace ATABBI.TexE
{
    public partial class OutputWindow : ToolWindow
    {
        public OutputWindow()
        {

            InitializeComponent();
            this.listView1.Columns[0].Width = -1;
        }

        private void OutputWindow_Load(object sender, EventArgs e)
        {
            this.listView1.SmallImageList = this.imageList1;
        }
        
        public override void CallUpdateContent()
        {
            if (this.Visible && this.DockPanel.ActiveDocument != null)
                this.UpdateContent(this.DockPanel.ActiveDocument);
        }

        public override void UpdateContent(IDockContent doc)
        {
            try
            {

                if (!this.Visible)
                    return;
                this.Textdoc = doc as TextDocument;
                if (this.p != null)
                {
                    //this.p.Close();
                    this.p.Kill();
                }
                this.p = null;
            }
            catch (System.Exception ex)
            {
            }
        }
        
        Process p;

        public void Compile(string cmd = "pdflatex.exe")
        { 
            if (this.Textdoc == null)
                return;
            this.DockState = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            
            p = new Process();
            if (!String.IsNullOrEmpty(this.Textdoc.FileName))
            {
                try
                {
                    string command = @"C:\Program Files (x86)\MiKTeX 2.9\miktex\bin\" + cmd; 
                    if (!File.Exists(command))
                        MessageBox.Show("can't find tex program, make sure it's installed!!");
                    this.listView1.Items.Clear();
                    this.textBoxOutput.Clear();

                    if (!File.Exists(command))
                        MessageBox.Show("can't find tex");
                    ProcessStartInfo psI = new ProcessStartInfo("cmd");
                    psI.UseShellExecute = false;
                    psI.RedirectStandardInput = true;
                    psI.RedirectStandardOutput = true;
                    psI.RedirectStandardError = true;
                    psI.CreateNoWindow = true;
                    psI.FileName = command;
                    psI.Arguments = this.Textdoc.CurrentFileInfo.Name;// +@" -output-directory=c:\texout";//-aux-directory=" + this.Textdoc.CurrentFileInfo.DirectoryName;
                    psI.WorkingDirectory = this.Textdoc .CurrentFileInfo.DirectoryName;
                    p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                    p.Exited += new EventHandler(p_Exited);
                    p.StartInfo = psI;
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    this.textBoxOutput.Focus();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void ViewPDF()
        {
            try
            {
                System.Diagnostics.Process.Start(this.Textdoc.CurrentFileInfo.FullName.Replace(this.Textdoc.CurrentFileInfo.Extension, ".pdf"));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void p_Exited(object sender, EventArgs e)
        {
            try
            {
                this.p = null;
                ViewPDF();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //MessageBox.Show("Done");
        }
        
        StringBuilder sb = new StringBuilder();
        
        Regex ErrorRe = new Regex(@"l\.\s*(\d+)", RegexOptions.Compiled);
        
        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (p == null)
                return;
            string data= e.Data;
            CheckForIllegalCrossThreadCalls = false;
            if (!string.IsNullOrEmpty(data))
            {
                lock(this)
                {
                    this.textBoxOutput.AppendText(data);//+ Environment.NewLine);
                    this.textBoxOutput.AppendText(Environment.NewLine + Environment.NewLine);
                }
            }
            this.textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
            //this.textBoxOutput.SelectionLength= 1;
            this.textBoxOutput.ScrollToCaret();

            CheckForIllegalCrossThreadCalls = false;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Trim()) && (data.Trim().StartsWith("!")))
                {   
                    foreach (string s in sb.ToString().Trim().Split(new string[]{"\r\n", "..."},  StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (ErrorRe.Match(s).Success)
                        {
                            ListViewItem item = new ListViewItem(new string[] {"", s.Trim(), "" }, -1);
                            //item.BackColor = Color.Bisque;
                            item.ToolTipText = s.Trim();
                            item.ImageIndex = 0;
                            this.listView1.Items.Add(item);
                        }
                        else
                        {
                            ListViewItem item = new ListViewItem(new string[] {"", s.Trim(), "" }, -1);
                            item.ToolTipText = s.Trim();
                            item.ImageIndex = 1;// s.ToLower().Contains("error") ? 1 : 2;
                            //item.ImageIndex = s.ToLower().Contains("warning") ? 1 : 2;
                            this.listView1.Items.Add(item);
                        
                        }
                    }
                    sb.Clear();
                }
                sb.Append(data.Trim());
                //this.textBoxOutput.AppendText(Environment.NewLine + Environment.NewLine);
            }
            //this.textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
            //this.textBoxOutput.ScrollToCaret();

            //this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            if (p.HasExited)
            {
                //this.lbStatus.Text = "Done";
            }
        }

        private void textBoxOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (p == null)
                return;
            if (e.KeyCode == Keys.Enter)
            {
                TextBox textBox = sender as TextBox;
                string cmd = this.textBoxOutput.Lines[this.textBoxOutput.Lines.Length - 1];
                p.StandardInput.AutoFlush = true;
                p.StandardInput.Write(cmd + p.StandardInput.NewLine);
            }
        }
  
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Regex re = new Regex(@"l\.\s*(\d+)", RegexOptions.Compiled);
            Match m = re.Match(this.listView1.SelectedItems[0].ToolTipText.ToString());
            if (m.Success)
            {
                this.Textdoc.txtEditor.TextArea.Caret.Line = Helper.getInt(m.Groups[1].Captures[0].Value.ToString());
                this.Textdoc.txtEditor.TextArea.Caret.BringCaretToView();
            }
            //MessageBox.Show(this.listView1.SelectedItems[0].Text);    
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxOutput.ScrollToCaret();
        }
        
    }
}