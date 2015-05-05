using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;
using AvalonEdit.Sample;
using ICSharpCode.AvalonEdit.Folding;
using Classes;
using ATABBI.TexE.Forms;
using ICSharpCode.AvalonEdit.Search;

namespace ATABBI.TexE
{
    public partial class DocumentExplorer : ToolWindow
    {
        public DocumentExplorer()
        {
            InitializeComponent();
        }

        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            treeView1.RightToLeftLayout = RightToLeftLayout;
        }      
      
        #region

        Dictionary<string, string> dict = new Dictionary<string, string>();
            
        public override void CallUpdateContent()
        {
            if (this.Visible && this.DockPanel.ActiveDocument != null)
                this.UpdateContent(this.DockPanel.ActiveDocument);
        }
       
        public override void UpdateContent(IDockContent doc)
        {
            if (!this.Visible)
                return;

            this.Textdoc = doc as TextDocument;
            if (this.Textdoc!= null)
                this.buildTree();
        }

        public void buildTree()
        {
            try
            {
                if (dict.Count == 0)
                {
                    dict.Add(@"Packages", @"usepackage");
                    dict.Add(@"Files", @"include");
                    dict.Add(@"Parts", "part");
                    dict.Add(@"Chapters", "chapter");
                    dict.Add(@"Sections", "section");
                    dict.Add(@"Subsections", "subsection");
                    dict.Add(@"Subsubsections", "subsubsection");
                    dict.Add(@"Paragraphs", "paragraph");
                    dict.Add(@"Subparagraphs", "subparagraph");
                    dict.Add(@"Labels", "label");
                    dict.Add(@"Objects", "begin");
                    dict.Add(@"Cites", "cite");
                    dict.Add(@"Refs", "ref");
                }

                
                this.treeView1.Nodes.Clear();
                //this.treeView1.Visible = false;
                Random rndImage = new Random();
                TreeNode root = new TreeNode("Document");
                this.Textdoc = Textdoc;
                this.treeView1.Nodes.Clear();
                
                foreach (KeyValuePair<string,string> kvp  in dict)
                {
                    TreeNode node = new TreeNode(kvp.Key);
                    Regex re1 = new Regex(@"(?<=\\" + kvp.Value + @"\{)([^\{]+)(?=\})", RegexOptions.IgnoreCase);
                    MatchCollection mc1 = re1.Matches(Textdoc.txtEditor.Document.Text);
                    //doc.txtEditor.Document
                    foreach (Match m in mc1)
                    {
                        TreeNode tn= new TreeNode (m.Groups[1].Value);
                        tn.Tag = m.Index.ToString();
                        tn.ImageIndex = rndImage.Next(imageList1.Images.Count);
                        node.Nodes.Add(tn);
                    }
                    if (node.Nodes.Count>0)
                        root.Nodes.Add(node);
                }
                this.treeView1.Nodes.Add(root);
                root.Expand();
                this.treeView1.Visible = true;
                 
                
            }
            catch (System.Exception ex)
            {
            }
        }

        #endregion

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                this.Textdoc.txtEditor.SelectionStart = (Helper.getInt(e.Node.Tag.ToString()));
                this.Textdoc.txtEditor.SelectionLength = e.Node.Text.Length;
                this.Textdoc.txtEditor.ScrollToLine(this.Textdoc.txtEditor.TextArea.Caret.Line); 
                this.Textdoc.txtEditor.TextArea.Caret.BringCaretToView();

                
            }
            catch (System.Exception ex)
            {
            }
        }

        private void DocumentExplorer_Load(object sender, EventArgs e)
        {
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
             
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.buildTree();
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (((TreeView)sender).SelectedNode == null)
                return;
            if (((TreeView)sender).SelectedNode.Parent.Text == "Files")
            {
                string filePath = System.IO.Path.Combine(this.Textdoc.CurrentFileInfo.DirectoryName, ((TreeView)sender).SelectedNode.Text + this.Textdoc.CurrentFileInfo.Extension);
                if (!System.IO.File.Exists(filePath))
                    return;
                ((MainForm)this.DockPanel.Parent).Open(filePath);
            }
            //if (((TreeView)sender).SelectedNode.Parent.Text == "Cites")
            //{
            //    Regex reBiblio = new Regex(@"(?<!%)(?<=\\bibliography\{)([^\{]+)(?=\})", RegexOptions.IgnoreCase);
            //    Match m = reBiblio.Match(Textdoc.txtEditor.Text);
            //    if (m.Success)
            //    {
            //        if (!System.IO.File.Exists(this.Textdoc.CurrentFileInfo.Directory + "\\" + m.Groups[1].Captures[0].Value.ToString() + ".bib"))
            //            return;
            //        string word = ((TreeView)sender).SelectedNode.Text;
            //        ((MainForm)this.DockPanel.Parent).Open(this.Textdoc.CurrentFileInfo.Directory + "\\" + m.Groups[1].Captures[0].Value.ToString() + ".bib");

                    
            //        this.Textdoc.txtEditor.TextArea.TextView.LineTransformers.Add(new ColorizKeywordEdit(this.Textdoc.txtEditor, word));
            //    }
            //}
            
        }
    }
}