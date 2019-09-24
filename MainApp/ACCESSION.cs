using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    public class ACCESSION
    {
        private string Namefield;
        private string FolderNamefield;
        private string ParentDirectoryfield;
        private string CurrentDirectoryfield;
        private List<string> Itemsfield;
        private List<string> UnprocessedItemsfield;
        private List<string> ProcessedItemsfield;
        private string CurrentItemfield;
        private bool IsPriorityfield;
        private bool IsAllCompletedfield;

        public ACCESSION()
        {
            Namefield = "";
            ParentDirectoryfield = "";
            CurrentDirectoryfield = "";
            FolderName = "";

            Itemsfield = new List<string>();
            UnprocessedItemsfield = new List<string>();
            ProcessedItemsfield = new List<string>();
            CurrentItemfield = "";

            IsPriorityfield = false;
            IsAllCompletedfield = false;
        }

        public string Name { get => Namefield; set => Namefield = value; }
        public string ParentDirectory
        {
            get => ParentDirectoryfield;
            set {
                ParentDirectoryfield = value;
                Paths paths = new Paths();
                if (value.Contains(paths.Folders.Priority_Dir))
                {
                    IsPriorityfield = true;
                }
                else
                {
                    IsPriorityfield = false;
                }
            }
        }
        public List<string> Items { get => Itemsfield; set => Itemsfield = value; }
        public List<string> UnprocessedItems { get => UnprocessedItemsfield; set => UnprocessedItemsfield = value; }
        public List<string> ProcessedItems { get => ProcessedItemsfield; set => ProcessedItemsfield = value; }
        public string CurrentItem { get => CurrentItemfield; set => CurrentItemfield = value; }
        public bool IsPriority { get => IsPriorityfield; set => IsPriorityfield = value; }
        public bool IsAllCompleted { get => IsAllCompletedfield; set => IsAllCompletedfield = value; }
        public string CurrentDirectory { get => CurrentDirectoryfield; set => CurrentDirectoryfield = value; }
        public string FolderName { get => FolderNamefield; set => FolderNamefield = value; }

        internal string FullPath()
        {
            if (FolderName == "") throw new Exception();
            return ParentDirectory + System.IO.Path.DirectorySeparatorChar + FolderName;
        }
    }
}
