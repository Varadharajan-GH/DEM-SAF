﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    class AccessionInfo
    {
        private string LockIDfield;
        private string SourceNamefield;
        private string SourceAliasNamefield;
        private string FullSourcePathfield;
        private string FullSourceAliasPathfield;
        private string SourceParentNamefield;
        private string FullSourceParentPathfield;
        private string CurrentNamefield;
        private string FullCurrentPathfield;
        private string CurrentParentNamefield;
        private string FullCurrentParentPathfield;
        private bool IsPriorityfield;
        private Queue<ItemInfo> UnprocessedItemsfield;
        private List<ItemInfo> ProcessedItemsfield;
        private List<ItemInfo> ErrorItemsfield;
        private string CurrentItemfield;
        private string CurrentItemPathfield;

        public AccessionInfo()
        {
            UnprocessedItemsfield = new Queue<ItemInfo>();
            ProcessedItemsfield = new List<ItemInfo>();
            ErrorItemsfield = new List<ItemInfo>();
        }
        public string SourceName 
        {
            get => SourceNamefield;
            set
            {
                SourceNamefield = value;
                if (!string.IsNullOrWhiteSpace(SourceNamefield) && !string.IsNullOrWhiteSpace(LockIDfield))
                {
                    SourceAliasNamefield = SourceNamefield + "." + LockIDfield;
                }
                if (!string.IsNullOrWhiteSpace(SourceNamefield) && !string.IsNullOrWhiteSpace(FullSourceParentPathfield))
                {
                    FullSourcePathfield= Path.Combine(FullSourceParentPathfield, SourceNamefield);
                }
            }
        }
        public string SourceAliasName 
        { 
            get => SourceAliasNamefield;
            set
            {
                SourceAliasNamefield = value;
                if (!string.IsNullOrWhiteSpace(FullSourceParentPathfield) && !string.IsNullOrWhiteSpace(SourceAliasNamefield))
                {
                    FullSourceAliasPathfield = Path.Combine(FullSourceParentPathfield, SourceAliasNamefield);
                }
            }
        }
        public string LockID
        {
            get => LockIDfield;
            set
            {
                LockIDfield = value;
                if (!string.IsNullOrWhiteSpace(SourceNamefield) && !string.IsNullOrWhiteSpace(LockIDfield))
                {
                    SourceAliasName = SourceNamefield + "." + LockIDfield;
                }
            }
        }
        public string FullSourcePath { get => FullSourcePathfield; set => FullSourcePathfield = value; }
        public string FullSourceAliasPath { get => FullSourceAliasPathfield; set => FullSourceAliasPathfield = value; }
        public string SourceParentName { get => SourceParentNamefield; set => SourceParentNamefield = value; }
        public string FullSourceParentPath
        {
            get => FullSourceParentPathfield;
            set
            {
                FullSourceParentPathfield = value;
                if (!string.IsNullOrWhiteSpace(FullSourceParentPathfield))
                {
                    SourceParentNamefield = Helper.GetFolder(FullSourceParentPathfield);
                    if (!string.IsNullOrWhiteSpace(SourceNamefield))
                    {
                        FullSourcePathfield= Path.Combine(FullSourceParentPathfield, SourceNamefield);
                    }
                }
            }
        }
        public bool IsPriority { get => IsPriorityfield; set => IsPriorityfield = value; }
        public string CurrentName
        {
            get => CurrentNamefield;
            set
            {
                CurrentNamefield = value;
                if (!string.IsNullOrWhiteSpace(CurrentNamefield) && !string.IsNullOrWhiteSpace(FullCurrentParentPathfield))
                {
                    FullCurrentPathfield = Path.Combine(FullCurrentParentPathfield, CurrentNamefield);
                }
            }
        }
        public string FullCurrentPath { get => FullCurrentPathfield; set => FullCurrentPathfield = value; }
        public string CurrentParentName { get => CurrentParentNamefield; set => CurrentParentNamefield = value; }
        public string FullCurrentParentPath
        {
            get => FullCurrentParentPathfield;
            set
            {
                FullCurrentParentPathfield = value;
                if (!string.IsNullOrWhiteSpace(CurrentNamefield) && !string.IsNullOrWhiteSpace(FullCurrentParentPathfield))
                {
                    FullCurrentPathfield = Path.Combine(FullCurrentParentPathfield, CurrentNamefield);
                }
            }
        }
        public Queue<ItemInfo> UnprocessedItems { get => UnprocessedItemsfield; set => UnprocessedItemsfield = value; }
        public List<ItemInfo> ProcessedItems { get => ProcessedItemsfield; set => ProcessedItemsfield = value; }
        public string CurrentItem { get => CurrentItemfield; set => CurrentItemfield = value; }
        public string CurrentItemPath { get => CurrentItemPathfield; set => CurrentItemPathfield = value; }
        public List<ItemInfo> ErrorItems { get => ErrorItemsfield; set => ErrorItemsfield = value; }
        //internal List<ItemInfo> ItemsList { get => ItemsListfield; set => ItemsListfield = value; }
    }
}
