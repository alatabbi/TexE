using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Classes;
namespace SmartOCR.Code
{
    public delegate void DelegateAddItem(ListResult s);
    public delegate void DelegateShowList(Boolean val);
    public class WorkingSetParameters
    {
        public WorkingSetParameters()
        { 
        }
        public string  WorkingSetPath ;
        public bool ConsultStopWords ;
        public bool ProcessSubFolders ;
        public string FilesType ;
        public bool CurrentFileOnly = true; 
        public static WorkingSetParameters Default
        {
            get
            {
                WorkingSetParameters para = new WorkingSetParameters();

                return para;
            }
        }
    }

}
