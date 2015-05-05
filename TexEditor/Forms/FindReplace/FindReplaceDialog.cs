#region Using Directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#endregion Using Directives


namespace SmartOCR
{
    public partial class FindReplaceDialog : Form
    {
        #region Fields

        private BindingSource _bindingSourceFind = new BindingSource();
        private List<string> _mruFind;
        private BindingSource _bindingSourceReplace = new BindingSource();
        private List<string> _mruReplace;
        private int _mruMaxCount = 10;
        //private Scintilla _scintilla;
        //private Range _searchRange = null;

        #endregion Fields


        #region Methods

        private void AddFindMru()
        {
            string find = cboFindF.Text;
            _mruFind.Remove(find);

            _mruFind.Insert(0, find);

            if (_mruFind.Count > _mruMaxCount)
                _mruFind.RemoveAt(_mruFind.Count - 1);

            _bindingSourceFind.ResetBindings(false);
            cboFindR.SelectedIndex = 0;
            cboFindF.SelectedIndex = 0;
        }


        private void AddReplacMru()
        {
            string find = cboFindR.Text;
            _mruFind.Remove(find);

            _mruFind.Insert(0, find);

            if (_mruFind.Count > _mruMaxCount)
                _mruFind.RemoveAt(_mruFind.Count - 1);

            string replace = cboReplace.Text;
            if (replace != string.Empty)
            {
                _mruReplace.Remove(replace);

                _mruReplace.Insert(0, replace);

                if (_mruReplace.Count > _mruMaxCount)
                    _mruReplace.RemoveAt(_mruReplace.Count - 1);
            }

            _bindingSourceFind.ResetBindings(false);
            _bindingSourceReplace.ResetBindings(false);
            cboFindR.SelectedIndex = 0;
            cboFindF.SelectedIndex = 0;
            cboReplace.SelectedIndex = 0;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            
        }


        private void btnFindAll_Click(object sender, EventArgs e)
        {
            
        }


        private void btnFindNext_Click(object sender, EventArgs e)
        {
           
        }


        private void btnFindPrevious_Click(object sender, EventArgs e)
        {
          
        }


        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
             
        }


        private void btnReplaceNext_Click(object sender, EventArgs e)
        {
            
        }


        private void btnReplacePrevious_Click(object sender, EventArgs e)
        {
             
        }


        private void chkEcmaScript_CheckedChanged(object sender, EventArgs e)
        {
           
        }


        public void FindNext()
        {
            
        }


        

        public void FindPrevious()
        {
             
        }


        private void FindReplaceDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


      


       


      
     
         


        


      


     


      

        #endregion Methods


        #region Properties

        public List<string> MruFind
        {
            get
            {
                return _mruFind;
            }
            set
            {
                _mruFind = value;
                _bindingSourceFind.DataSource = _mruFind;
            }
        }


        public int MruMaxCount
        {
            get { return _mruMaxCount; }
            set { _mruMaxCount = value; }
        }


        public List<string> MruReplace
        {
            get
            {
                return _mruReplace;
            }
            set
            {
                _mruReplace = value;
                _bindingSourceReplace.DataSource = _mruReplace;
            }
        }


       

        #endregion Properties


        #region Constructors

        public FindReplaceDialog()
        {
            InitializeComponent();

            _mruFind = new List<string>();
            _mruReplace = new List<string>();
            _bindingSourceFind.DataSource = _mruFind;
            _bindingSourceReplace.DataSource = _mruReplace;
            cboFindF.DataSource = _bindingSourceFind;
            cboFindR.DataSource = _bindingSourceFind;
            cboReplace.DataSource = _bindingSourceReplace;
        }

        #endregion Constructors

        private void FindReplaceDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
