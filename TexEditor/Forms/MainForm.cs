using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using ATABBI.TexE.Customization;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Search;
using System.Windows.Threading;

namespace ATABBI.TexE
{
    public partial class MainForm : Form
    {
        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;
        private DocumentExplorer m_solutionExplorer;
        private PropertyWindow m_propertyWindow;
        private FindReplaceWindow m_findreplaceWindow;
        private Toolbox m_toolbox;
        private OutputWindow m_outputWindow;
        private BibliographyList m_BibliographyList;
        private GoToWindow m_gotoline;
        private SearchPanel sp = new SearchPanel();
        
        public TextDocument ActiveDocument
        {
            get
            {
                return dockPanel.ActiveDocument as TextDocument;
            }
        }
        
        Queue<string> recentDocuments = new Queue<string>();
        
        public MainForm()
        {
            InitializeComponent();
            CreateStandardControls();
            showRightToLeft.Checked = (RightToLeft == RightToLeft.Yes);
            RightToLeftLayout = showRightToLeft.Checked;
            m_solutionExplorer.RightToLeftLayout = RightToLeftLayout;
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            this.dockPanel.ActiveDocumentChanged += new EventHandler(dockPanel_ActiveDocumentChanged);
        }

        void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            this.m_solutionExplorer.UpdateContent(this.dockPanel.ActiveDocument);
            this.m_propertyWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.m_findreplaceWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.m_BibliographyList.UpdateContent(this.dockPanel.ActiveDocument);
            this.m_toolbox.UpdateContent(this.dockPanel.ActiveDocument);
            this.m_gotoline.UpdateContent(this.dockPanel.ActiveDocument);
            sp = null;
            //sp.Dispatcher.BeginInvoke(DispatcherPriority.Inactive, (Action)sp.Reactivate);
            //if (this.sp != null && !this.sp.IsClosed)
            //    sp.Close();
        }

        #region Methods

        private void Run(string cmd = "pdflatex")
        {
            if (!this.m_outputWindow.Visible)
            {
                this.m_outputWindow.Visible = true;
                this.m_outputWindow.VisibleState = DockState.DockBottom;
                this.m_outputWindow.DockState = DockState.DockBottom; 
            }
            if (this.m_outputWindow.Visible)
            {
                this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
                this.SaveFile();
                this.m_outputWindow.Compile(cmd + ".exe");
            }
        }

        SaveFileDialogWithEncoding saveFileDialog1 = new SaveFileDialogWithEncoding();
        
        private void SaveFile()
        {
            this.SaveFile(this.ActiveDocument);
        }
        
        private void SaveFileAs()
        {
            this.SaveFileAs(this.ActiveDocument);
        }
        
        private void SaveFile(TextDocument ad)
        {
            if (ad == null)
                return;
            if (string.IsNullOrEmpty(ad.FileName))
            {
                // Default file extension
                //this.saveFileDialog1.DefaultExt = "txt";
                // Available file extensions
                saveFileDialog1.Filter = "tex files (*.tex)|*.tex|bibtex files (*.bib)|*.bib|txt files (*.txt)|*.txt|All files (*.*)|*.*";
                 
                // show dialog
                if (saveFileDialog1.ShowDialog()  == DialogResult.OK)
                {
                    string format = ".tex";
                    // resolve file format
                    switch (Path.GetExtension(saveFileDialog1.FileName).ToLower())
                    {
                        case ".txt":
                            format = ".txt";
                            break;
                        case ".tex":
                            format = ".tex";
                            break;
                        case ".bib":
                            format = ".bib";
                            break;
                        default:
                            MessageBox.Show(this, "Unsupported file format was specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }
                }

                ad.txtEditor.Encoding = System.Text.Encoding.Default;
                if(saveFileDialog1.EncodingType == EncodingType.Ansi )
                    ad.txtEditor.Encoding = System.Text.Encoding.Default;
                if (saveFileDialog1.EncodingType == EncodingType.Unicode)
                    ad.txtEditor.Encoding = System.Text.Encoding.Unicode;
                if (saveFileDialog1.EncodingType == EncodingType.UTF8)
                    ad.txtEditor.Encoding = new  System.Text.UTF8Encoding(false);
                ad.FileName = saveFileDialog1.FileName;
            }

            this.UpdateRecent(ad.FileName);
            ActiveDocument.SaveFile();
        }

        private void SaveFileAs(TextDocument ad)
        {
            if (ad == null)
            return;
            // Default file extension
            //this.saveFileDialog1.DefaultExt = "txt";
            // Available file extensions
            saveFileDialog1.Filter = "tex files (*.tex)|*.tex|bibtex files (*.bib)|*.bib|txt files (*.txt)|*.txt|All files (*.*)|*.*";

            // show dialog
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string format = ".tex";
                // resolve file format
                switch (Path.GetExtension(saveFileDialog1.FileName).ToLower())
                {
                    case ".txt":
                        format = ".txt";
                        break;
                    case ".tex":
                        format = ".tex";
                        break;
                    case ".bib":
                        format = ".bib";
                        break;
                    default:
                        MessageBox.Show(this, "Unsupported image format was specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                ad.FileName = saveFileDialog1.FileName; 
                ad.txtEditor.Encoding = System.Text.Encoding.Default;
                if (saveFileDialog1.EncodingType == EncodingType.Ansi)
                    ad.txtEditor.Encoding = System.Text.Encoding.Default;
                if (saveFileDialog1.EncodingType == EncodingType.Unicode)
                    ad.txtEditor.Encoding = System.Text.Encoding.Unicode;
                if (saveFileDialog1.EncodingType == EncodingType.UTF8)
                    ad.txtEditor.Encoding = new System.Text.UTF8Encoding(false);
            }

            this.UpdateRecent(ad.FileName);
            ad.SaveFile();
        }

        private void SaveAll()
        {
            foreach (IDockContent document in dockPanel.Documents)
            {
                if (string.IsNullOrEmpty((document as TextDocument).FileName))
                {
                    this.SaveFileAs((document as TextDocument));
                }
                else
                {
                    this.SaveFile((document as TextDocument));
                }
            }
        }

        public void Open(string defaultFileName = "")
        {
            string fullName = "";
            string fileName = "";

            if (!string.IsNullOrEmpty(defaultFileName))
            {
                fullName = defaultFileName;
                fileName = Path.GetFileName(fullName);

                if (FindDocument(fileName) != null)
                {
                    //MessageBox.Show("The document: " + fileName + " has already opened!");
                    return;
                }
            }
            else
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.InitialDirectory = Application.ExecutablePath;
                openFile.Filter = "tex files (*.tex)|*.tex|bibtex files (*.bib)|*.bib|txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFile.FilterIndex = 1;
                openFile.RestoreDirectory = true;
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    fullName = openFile.FileName;
                    fileName = Path.GetFileName(fullName);

                    if (FindDocument(fileName) != null)
                    {
                        MessageBox.Show("The document: " + fileName + " has already opened!");
                        return;
                    }
                }
            }
            if (string.IsNullOrEmpty(fullName))
                return;
            TextDocument dummyDoc = new TextDocument();
            dummyDoc.Text = fileName;
            try
            {
                dummyDoc.FileName = fullName;
                dummyDoc.LoadFile();
                this.UpdateRecent(fullName);
            }
            catch (Exception exception)
            {
                dummyDoc.Close();
                MessageBox.Show(exception.Message);
            }

            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                dummyDoc.MdiParent = this;
                dummyDoc.Show();
            }
            else
            {
                dummyDoc.Show(dockPanel);
            }

        }

        private void UpdateRecent(string fileName="")
        {
            recentDocuments.Clear();
            this.recentDocumentsToolStripMenuItem.DropDownItems.Clear();
            foreach (string s in ATABBI.TexE.Properties.Settings.Default.RecentDocuments.ToString().Split(new string[] { "|,|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.recentDocumentsToolStripMenuItem.DropDownItems.Add(s, null, recent_Item_Click);
                if (!recentDocuments.Contains(s))
                    recentDocuments.Enqueue(s);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                if(!recentDocuments.Contains(fileName))
                    recentDocuments.Enqueue(fileName);
                if (recentDocuments.Count > 15)
                    recentDocuments.Dequeue();

                string ss = string.Join("|,|", recentDocuments.ToArray());
                ATABBI.TexE.Properties.Settings.Default.RecentDocuments = ss;
                ATABBI.TexE.Properties.Settings.Default.Save();
            }
             
            

             

        }
        
        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

        private TextDocument CreateNewDocument()
        {
            TextDocument dummyDoc = new TextDocument();

            int count = 1;
            //string text = "C:\\MADFDKAJ\\ADAKFJASD\\ADFKDSAKFJASD\\ASDFKASDFJASDF\\ASDFIJADSFJ\\ASDFKDFDA" + count.ToString();
            string text = "Document" + count.ToString();
            while (FindDocument(text) != null)
            {
                count++;
                //text = "C:\\MADFDKAJ\\ADAKFJASD\\ADFKDSAKFJASD\\ASDFKASDFJASDF\\ASDFIJADSFJ\\ASDFKDFDA" + count.ToString();
                text = "Document" + count.ToString();
            }
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private TextDocument CreateNewDocument(string text)
        {
            TextDocument dummyDoc = new TextDocument();
            dummyDoc.Text = text;
            return dummyDoc;
        }

        private void CloseAllDocuments()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    this.CloseDocument(document);
                 //   document.DockHandler.Close();
                }
            }
        }

        private void CloseDocument(IDockContent document)
        {
            if ((document as TextDocument).txtEditor.IsModified)
            {
                var window = MessageBox.Show("Save changes to file: " + (document as TextDocument).FileName, "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (window == DialogResult.Yes)
                {
                    this.SaveFile((document as TextDocument));
                    document.DockHandler.Close();
                }
                else if (window == DialogResult.No)
                {
                    document.DockHandler.Close();
                }
                else
                {

                }
            }
            else
            {
                document.DockHandler.Close();
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DocumentExplorer).ToString())
                return m_solutionExplorer;
            else if (persistString == typeof(PropertyWindow).ToString())
                return m_propertyWindow;
            else if (persistString == typeof(Toolbox).ToString())
                return m_toolbox;
            else if (persistString == typeof(OutputWindow).ToString())
                return m_outputWindow;
            else if (persistString == typeof(BibliographyList).ToString())
                return m_BibliographyList;
            else if (persistString == typeof(FindReplaceWindow).ToString())
                return m_findreplaceWindow;
            else if (persistString == typeof(GoToWindow).ToString())
                return m_gotoline;
            else
            {
                // TextDoc overrides GetPersistString to add extra information into persistString.
                // Any DockContent may override this value to add any needed information for deserialization.

                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 3)
                    return null;

                if (parsedStrings[0] != typeof(TextDocument).ToString())
                    return null;

                TextDocument dummyDoc = new TextDocument();
                if (parsedStrings[1] != string.Empty)
                {
                    dummyDoc.FileName = parsedStrings[1];
                    dummyDoc.LoadFile();
                }
                if (parsedStrings[2] != string.Empty)
                    dummyDoc.Text = parsedStrings[2];

                return dummyDoc;
            }
        }

        private void CloseAllContents()
        {
            // we don't want to create another instance of tool window, set DockPanel to null
            m_solutionExplorer.DockPanel = null;
            m_propertyWindow.DockPanel = null;
            m_toolbox.DockPanel = null;
            m_outputWindow.DockPanel = null;
            m_BibliographyList.DockPanel = null;
            m_gotoline.DockPanel = null;
            

            // Close all other document windows
            CloseAllDocuments();
        }

        private void SetSchema(object sender, System.EventArgs e)
        {
            CloseAllContents();

            if (sender == menuItemSchemaVS2005)
                Extender.SetSchema(dockPanel, Extender.Schema.VS2005);
            else if (sender == menuItemSchemaVS2003)
                Extender.SetSchema(dockPanel, Extender.Schema.VS2003);

            menuItemSchemaVS2005.Checked = (sender == menuItemSchemaVS2005);
            menuItemSchemaVS2003.Checked = (sender == menuItemSchemaVS2003);
        }

        private void SetDocumentStyle(object sender, System.EventArgs e)
        {
            DocumentStyle oldStyle = dockPanel.DocumentStyle;
            DocumentStyle newStyle;
            if (sender == menuItemDockingMdi)
                newStyle = DocumentStyle.DockingMdi;
            else if (sender == menuItemDockingWindow)
                newStyle = DocumentStyle.DockingWindow;
            else if (sender == menuItemDockingSdi)
                newStyle = DocumentStyle.DockingSdi;
            else
                newStyle = DocumentStyle.SystemMdi;

            if (oldStyle == newStyle)
                return;

            if (oldStyle == DocumentStyle.SystemMdi || newStyle == DocumentStyle.SystemMdi)
                CloseAllDocuments();

            dockPanel.DocumentStyle = newStyle;
            menuItemDockingMdi.Checked = (newStyle == DocumentStyle.DockingMdi);
            menuItemDockingWindow.Checked = (newStyle == DocumentStyle.DockingWindow);
            menuItemDockingSdi.Checked = (newStyle == DocumentStyle.DockingSdi);
            menuItemSystemMdi.Checked = (newStyle == DocumentStyle.SystemMdi);
            menuItemLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
            menuItemLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);
            toolBarButtonLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
            toolBarButtonLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);
        }

        private void SetDockPanelSkinOptions(bool isChecked)
        {
            if (isChecked)
            {
                // All of these options may be set in the designer.
                // This is not a complete list of possible options available in the skin.

                AutoHideStripSkin autoHideSkin = new AutoHideStripSkin();
                autoHideSkin.DockStripGradient.StartColor = Color.AliceBlue;
                autoHideSkin.DockStripGradient.EndColor = Color.Blue;
                autoHideSkin.DockStripGradient.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
                autoHideSkin.TabGradient.StartColor = SystemColors.Control;
                autoHideSkin.TabGradient.EndColor = SystemColors.ControlDark;
                autoHideSkin.TabGradient.TextColor = SystemColors.ControlText;
                autoHideSkin.TextFont = new Font("Showcard Gothic", 10);

                dockPanel.Skin.AutoHideStripSkin = autoHideSkin;

                DockPaneStripSkin dockPaneSkin = new DockPaneStripSkin();
                dockPaneSkin.DocumentGradient.DockStripGradient.StartColor = Color.Red;
                dockPaneSkin.DocumentGradient.DockStripGradient.EndColor = Color.Pink;

                dockPaneSkin.DocumentGradient.ActiveTabGradient.StartColor = Color.Green;
                dockPaneSkin.DocumentGradient.ActiveTabGradient.EndColor = Color.Green;
                dockPaneSkin.DocumentGradient.ActiveTabGradient.TextColor = Color.White;

                dockPaneSkin.DocumentGradient.InactiveTabGradient.StartColor = Color.Gray;
                dockPaneSkin.DocumentGradient.InactiveTabGradient.EndColor = Color.Gray;
                dockPaneSkin.DocumentGradient.InactiveTabGradient.TextColor = Color.Black;

                dockPaneSkin.TextFont = new Font("SketchFlow Print", 10);

                dockPanel.Skin.DockPaneStripSkin = dockPaneSkin;
            }
            else
            {
                dockPanel.Skin = new DockPanelSkin();
            }

            menuItemLayoutByXml_Click(menuItemLayoutByXml, EventArgs.Empty);
        }
 
        #endregion

        #region Event Handlers
        
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            //dockPanel.ShowDocumentIcon = true;
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
            this.UpdateRecent();
        }
        
        void recent_Item_Click(object sender, EventArgs e)
        {
            this.Open(((ToolStripDropDownItem)sender).Text);
        }
       
        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        private void menuItemExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void menuItemSolutionExplorer_Click(object sender, System.EventArgs e)
        {
            m_solutionExplorer.Show(dockPanel);
        }

        private void menuItemPropertyWindow_Click(object sender, System.EventArgs e)
        {
            m_propertyWindow.Show(dockPanel);
        }

        private void menuItemToolbox_Click(object sender, System.EventArgs e)
        {
            m_toolbox.Show(dockPanel);
        }

        private void menuItemOutputWindow_Click(object sender, System.EventArgs e)
        {
            m_outputWindow.Show(dockPanel);
        }

        private void menuItemBibliographyList_Click(object sender, System.EventArgs e)
        {
            m_BibliographyList.Show(dockPanel);
        }

        private void menuItemAbout_Click(object sender, System.EventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog(this);
        }

        private void menuItemNew_Click(object sender, System.EventArgs e)
        {
            TextDocument dummyDoc = CreateNewDocument();
            if (dockPanel.DocumentStyle == DocumentStyle.DockingWindow)
            {
                dummyDoc.MdiParent = this;
                dummyDoc.Show();
            }
            else
                dummyDoc.Show(dockPanel);
        }

        private void menuItemOpen_Click(object sender, System.EventArgs e)
        {
            this.Open();
        }
      
        private void menuItemFile_Popup(object sender, System.EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                menuItemClose.Enabled = menuItemCloseAll.Enabled = menuItemCloseAllButThisOne.Enabled = (ActiveMdiChild != null);
            }
            else
            {
                menuItemClose.Enabled = (dockPanel.ActiveDocument != null);
                menuItemCloseAll.Enabled = menuItemCloseAllButThisOne.Enabled = (dockPanel.DocumentsCount > 0);
            }
        }

        private void menuItemClose_Click(object sender, System.EventArgs e)
        {
            //if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            //    ActiveMdiChild.Close();
            if (dockPanel.ActiveDocument != null)
                //dockPanel.ActiveDocument.DockHandler.Close();
                this.CloseDocument(ActiveDocument);
        }

        private void menuItemCloseAll_Click(object sender, System.EventArgs e)
        {
            CloseAllDocuments();
        }

        private void menuItemToolBar_Click(object sender, System.EventArgs e)
        {
            toolBar.Visible = menuItemToolBar.Checked = !menuItemToolBar.Checked;
        }

        private void menuItemStatusBar_Click(object sender, System.EventArgs e)
        {
            statusBar.Visible = menuItemStatusBar.Checked = !menuItemStatusBar.Checked;
        }

        private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {

            
            if (e.ClickedItem == toolBarButtonNew)
                menuItemNew_Click(null, null);
            else if (e.ClickedItem == toolBarButtonOpen)
                menuItemOpen_Click(null, null);
            else if (e.ClickedItem == toolBarButtonSolutionExplorer)
                menuItemSolutionExplorer_Click(null, null);
            else if (e.ClickedItem == toolBarButtonPropertyWindow)
                menuItemPropertyWindow_Click(null, null);
            else if (e.ClickedItem == toolBarButtonToolbox)
                menuItemToolbox_Click(null, null);
            else if (e.ClickedItem == toolBarButtonOutputWindow)
                menuItemOutputWindow_Click(null, null);
            else if (e.ClickedItem == toolBarButtonBibliographyList)
                menuItemBibliographyList_Click(null, null);
            else if (e.ClickedItem == toolBarButtonLayoutByCode)
            //    menuItemLayoutByCode_Click(null, null);
                MessageBox.Show("No Implementation");
            else if (e.ClickedItem == toolBarButtonLayoutByXml)
                MessageBox.Show("No Implementation");
            //    menuItemLayoutByXml_Click(null, null);
            else if (e.ClickedItem == toolBarButtonSave)
                saveToolStripMenuItem_Click(null, null);
            else if (e.ClickedItem == toolStripButtonSaveAll)
                saveAllToolStripMenuItem_Click(null, null);
            else if (e.ClickedItem == toolStripButtonQuickFind)
                this.quickSearchToolStripMenuItem_Click(null, null);
            else if (e.ClickedItem == toolStripButtonClose)
                this.menuItemClose_Click(null, null);
            //else if (e.ClickedItem == toolStripButtonRun)
            //{
            //    if (!this.m_outputWindow.Visible)
            //    {
            //        this.m_outputWindow.DockState = DockState.DockBottom;
            //        this.m_outputWindow.Visible = true;
            //    }
            //    this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            //    this.SaveFile();

            //    this.m_outputWindow.Compile();
            //}
            else if (e.ClickedItem == toolStripButtonViewPDF)
            {
                this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
                this.m_outputWindow.ViewPDF();
            }
            else if (e.ClickedItem == toolBarButtonSpellCheck)
            {
                this.ActiveDocument.SpellCheck();
            }

        }

        private void menuItemNewWindow_Click(object sender, System.EventArgs e)
        {
            MainForm newWindow = new MainForm();
            newWindow.Text = newWindow.Text + " - New";
            newWindow.Show();
        }

        private void menuItemTools_Popup(object sender, System.EventArgs e)
        {
            menuItemLockLayout.Checked = !this.dockPanel.AllowEndUserDocking;
        }

        private void menuItemLockLayout_Click(object sender, System.EventArgs e)
        {
            dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
        }

        private void menuItemLayoutByCode_Click(object sender, System.EventArgs e)
        {
            return;
            #region MyRegion
            //dockPanel.SuspendLayout(true);
            //CloseAllDocuments();
            //CreateStandardControls();
            //m_solutionExplorer.Show(dockPanel, DockState.DockRight);
            //m_propertyWindow.Show(m_solutionExplorer.Pane, m_solutionExplorer);
            //m_toolbox.Show(dockPanel, new Rectangle(98, 133, 200, 383));
            //m_outputWindow.Show(m_solutionExplorer.Pane, DockAlignment.Bottom, 0.35);
            //m_BibliographyList.Show(m_toolbox.Pane, DockAlignment.Left, 0.4);
            //TextDocument doc1 = CreateNewDocument("Document1");
            //TextDocument doc2 = CreateNewDocument("Document2");
            //TextDocument doc3 = CreateNewDocument("Document3");
            //TextDocument doc4 = CreateNewDocument("Document4");
            //doc1.Show(dockPanel, DockState.Document);
            //doc2.Show(doc1.Pane, null);
            //doc3.Show(doc1.Pane, DockAlignment.Bottom, 0.5);
            //doc4.Show(doc3.Pane, DockAlignment.Right, 0.5);
            //dockPanel.ResumeLayout(true, true); 
            #endregion
        }

        private void CreateStandardControls()
        {
            m_solutionExplorer = new DocumentExplorer();
            m_propertyWindow = new PropertyWindow();
            m_toolbox = new Toolbox();
            m_outputWindow = new OutputWindow();
            m_BibliographyList = new BibliographyList();
            m_findreplaceWindow = new FindReplaceWindow();
            m_gotoline = new GoToWindow();
        }

        private void menuItemLayoutByXml_Click(object sender, System.EventArgs e)
        {
            return;
            #region MyRegion
            dockPanel.SuspendLayout(true);

            // In order to load layout from XML, we need to close all the DockContents
            CloseAllContents();

            CreateStandardControls();

            Assembly assembly = Assembly.GetAssembly(typeof(MainForm));
            Stream xmlStream = assembly.GetManifestResourceStream("ATABBI.TexE.Resources.DockPanel.xml");
            dockPanel.LoadFromXml(xmlStream, m_deserializeDockContent);
            xmlStream.Close();

            dockPanel.ResumeLayout(true, true); 
            #endregion
        }

        private void menuItemCloseAllButThisOne_Click(object sender, System.EventArgs e)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                Form activeMdi = ActiveMdiChild;
                foreach (Form form in MdiChildren)
                {
                    if (form != activeMdi)
                        form.Close();
                }
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    if (!document.DockHandler.IsActivated)
                        this.CloseDocument(document);
                }
            }
        }

        private void menuItemShowDocumentIcon_Click(object sender, System.EventArgs e)
        {
            dockPanel.ShowDocumentIcon = menuItemShowDocumentIcon.Checked = !menuItemShowDocumentIcon.Checked;
        }

        private void showRightToLeft_Click(object sender, EventArgs e)
        {
            CloseAllContents();
            if (showRightToLeft.Checked)
            {
                this.RightToLeft = RightToLeft.No;
                this.RightToLeftLayout = false;
            }
            else
            {
                this.RightToLeft = RightToLeft.Yes;
                this.RightToLeftLayout = true;
            }
            m_solutionExplorer.RightToLeftLayout = this.RightToLeftLayout;
            showRightToLeft.Checked = !showRightToLeft.Checked;
        }

        private void exitWithoutSavingLayout_Click(object sender, EventArgs e)
        {
            m_bSaveLayout = false;
            Close();
            m_bSaveLayout = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.SaveFile();
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_findreplaceWindow.Show(dockPanel);
            
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.SaveFile();
            this.m_outputWindow.Compile();

        }
       
        #endregion

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.SaveFileAs();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.SaveAll();
        }

        private void goToLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // m_gotoline.DockState = DockState.Float;
            m_gotoline.Show(dockPanel);
        }
       
        private void quickSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveDocument != null)
            {
                sp = new SearchPanel();
                sp.UseRegex = false;
                sp.Attach(this.ActiveDocument.txtEditor.TextArea);
                sp.Open();
                sp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)sp.Reactivate);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }    

        private void toolStripButtonRun_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Run(e.ClickedItem.Text);
            if (!this.m_outputWindow.Visible)
            {
                this.m_outputWindow.DockState = DockState.DockBottom;
                this.m_outputWindow.Visible = true;
            }
            this.m_outputWindow.UpdateContent(this.dockPanel.ActiveDocument);
            this.SaveFile();
            this.m_outputWindow.Compile(e.ClickedItem.Text+ ".exe");
        }

        private void toolStripButtonRun_ButtonClick(object sender, EventArgs e)
        {
            Run();    
        }

        private void toolStripSplitButtonMikTexOption_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == this.packageManagerToolStripMenuItem)
                System.Diagnostics.Process.Start("mpm_mfc_admin.exe");

            else if (e.ClickedItem == this.optionsMikToolStripMenuItem)
                System.Diagnostics.Process.Start("mo.exe");

            else if (e.ClickedItem == this.updateWizardToolStripMenuItem )
                System.Diagnostics.Process.Start("miktex-update_admin.exe");
            
            //mpm_mfc_admin.exe
            //mo.exe
            //miktex-update_admin.exe
        }
            
    }
}