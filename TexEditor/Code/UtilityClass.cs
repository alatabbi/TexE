using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartOCR.Code
{
    public  static class UtilityClass
    {
        public static string connnectionString;
        public static string ConnnectionString 
        {
            get
            {
                return connnectionString;
            }
            set
            {
                connnectionString = value;
            }
        }
        public static string Stats = "";
    }
}
