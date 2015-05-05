using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Text.RegularExpressions;
using Classes;

namespace ATABBI.TexE
{
    public partial class BibliographyList : ToolWindow
    {
        public BibliographyList()
        {
            InitializeComponent();
        }
        public override void UpdateContent(IDockContent doc)
        {
            if (!this.Visible)
                return;
            this.Textdoc = doc as TextDocument;
            if (this.Textdoc!= null) 
                this.populate();
        }
        public override void CallUpdateContent()
        {
            if (this.Visible && this.DockPanel.ActiveDocument != null)
                 this.UpdateContent(this.DockPanel.ActiveDocument);
        }
        string path = "";
        private void populate(string s="")
        {
            Regex reBiblio = new Regex(@"(?<!%)(?<=\\bibliography\{)([^\{]+)(?=\})", RegexOptions.IgnoreCase);
            Match m = reBiblio.Match(Textdoc.txtEditor.Text);
            if (m.Success)
            {
                this.path =this.Textdoc.CurrentFileInfo.Directory + "\\" + m.Groups[1].Captures[0].Value.ToString() + ".bib";
            }

            this.listView1.Items.Clear();
            foreach (BibTex entry in BibTex.Parse(this.path))
            {
                if (entry.Text.Contains(s) || string.IsNullOrEmpty(s))
                {
                    ListViewItem item = new ListViewItem(new string[] { "", "", entry.UID, entry["author"], entry["title"], entry["journal"], entry.Text });
                    item.ToolTipText = entry.Text;
                    item.Tag = entry.UID;
                    this.listView1.Items.Add(item);
                }
            }
        }
        private void InsertRef()
        {
            try
            {
                string s = this.listView1.SelectedItems[0].Tag.ToString();
                this.Textdoc.txtEditor.Document.Insert(this.Textdoc.txtEditor.TextArea.Caret.Offset, s);
                this.Textdoc.txtEditor.TextArea.Caret.BringCaretToView();
            }
            catch (System.Exception ex)
            {
            }
        }
        private void textBoxRef_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length>1)
            {
                this.populate(this.textBox1.Text);
            }
        }
        
        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter))
            {
                InsertRef();
            }
        }

        
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            InsertRef();
        }

        private void BibliographyList_Load(object sender, EventArgs e)
        {
             
        }

       
    }
}