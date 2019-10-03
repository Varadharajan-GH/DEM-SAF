using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    class ItemInfo
    {
        private string Namefield;
        private string Accessionfield;
        private string ItemNumberfield;
        private string PGfield;
        private string DocTypefield;
        private string PageSpanfield;
        private string FullPathfield;
        private string FullParentPathfield;
        private string XMLPathfield;
        private string CurrentImagefield;
        private bool IsContentPagefield;
        private List<string> MainPageTifsfield;
        private List<string> ContentPageTifsfield;

        public ItemInfo()
        {
            MainPageTifsfield = new List<string>();
            ContentPageTifsfield = new List<string>();
        }
        public ItemInfo(string FullPath)
        {
            FullPathfield = FullPath;
            Namefield = Helper.GetFolder(FullPath);
            MainPageTifsfield = new List<string>();
            ContentPageTifsfield = new List<string>();
        }

        public string Name 
        {
            get => Namefield;
            set
            {
                Namefield = value;
                if ((!string.IsNullOrWhiteSpace(FullParentPathfield)) && (!string.IsNullOrWhiteSpace(Namefield)))
                {
                    FullPath = Path.Combine(FullParentPathfield, Namefield);
                }
            }
        }
        public string Accession { get => Accessionfield; set => Accessionfield = value; }
        public string ItemNumber { get => ItemNumberfield; set => ItemNumberfield = value; }
        public string PG { get => PGfield; set => PGfield = value; }
        public string FullParentPath 
        {
            get => FullParentPathfield;
            set
            {
                FullParentPathfield = value;
                if ((!string.IsNullOrWhiteSpace(FullParentPathfield)) && (!string.IsNullOrWhiteSpace(Namefield)))
                {
                    FullPath = Path.Combine(FullParentPathfield,Namefield);
                }
            }
        }
        public string XMLPath { get => XMLPathfield; set => XMLPathfield = value; }
        public List<string> MainPageTifs { get => MainPageTifsfield; set => MainPageTifsfield = value; }
        public List<string> ContentPageTifs { get => ContentPageTifsfield; set => ContentPageTifsfield = value; }
        public string FullPath
        {
            get => FullPathfield;
            set
            {
                FullPathfield = value;
                if (!string.IsNullOrWhiteSpace(FullPathfield))
                {
                    XMLPathfield = Path.Combine(FullPathfield, Namefield + ".XML");
                }
            }        
        }

        public string DocType { get => DocTypefield; set => DocTypefield = value; }
        public string PageSpan { get => PageSpanfield; set => PageSpanfield = value; }
        public string CurrentImage { get => CurrentImagefield; set => CurrentImagefield = value; }
        public bool IsContentPage { get => IsContentPagefield; set => IsContentPagefield = value; }
    }
}
