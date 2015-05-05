using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartOCR.Code
{
    public interface IOcrEngine
    {
        void Start();
        void ShutDown();
        IDocumentManager DocumentManager
        {
            get;
            set;
        }
    }
    public interface IDocumentManager
    {
    }
    public interface IOcrDocument
    {
        List<IOcrPage> Pages
        {
            get;
            set;
        }
    }
    public interface IOcrPage
    {
    }
    public interface IOcrZone
    {
    }
}
