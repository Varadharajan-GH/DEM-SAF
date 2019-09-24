using System;
using System.IO;

namespace MainApp
{
    public class Paths
    {
        internal Folders Folders { get; } = new Folders();
        internal Files Files { get; } = new Files();
    }
    class Folders
    {
        public  Folders()
        {
            //Application folders
            Root_Dir = "D:\\data\\A_Process";
            Admin_Dir = $"{Root_Dir}\\Admin";
            Current_Dir = $"{Root_Dir}\\Current";
            Input_Dir = $"{Root_Dir}\\Input";
            Priority_Dir = $"{Root_Dir}\\Priority";
            Output_Dir = $"{Root_Dir}\\Output";
            Completed_Dir = $"{Root_Dir}\\Completed";
            Log_Dir = $"{Root_Dir}\\Log";

            //Data folders
            Dict_en_us_DIR = $"{Admin_Dir}\\Dict\\en_us";
            Ocrdata_Dir = $"{Admin_Dir}\\ocrdata";

            MakeFolders();
        }

        private void MakeFolders()
        {
            Directory.CreateDirectory(Root_Dir);
            Directory.CreateDirectory(Admin_Dir);
            Directory.CreateDirectory(Current_Dir);
            Directory.CreateDirectory(Input_Dir);
            Directory.CreateDirectory(Priority_Dir);
            Directory.CreateDirectory(Output_Dir);
            Directory.CreateDirectory(Log_Dir);
            Directory.CreateDirectory(Completed_Dir);
            //Directory.CreateDirectory(Ocrdata_Dir);
        }

        public string Root_Dir { get; }
        public string Admin_Dir { get;  }
        public string Current_Dir { get; }
        public string Input_Dir { get;  }
        public string Priority_Dir { get; }
        public string Output_Dir { get;  }
        public string Completed_Dir { get; }
        public string Log_Dir { get; }
        public string Dict_en_us_DIR { get; }
        public string Ocrdata_Dir { get; }
    }
    class Files
    {
        public Files()
        {
            En_aff_file = $"{new Folders().Dict_en_us_DIR}\\en_us.aff";
            En_dic_file = $"{new Folders().Dict_en_us_DIR}\\en_us.dic";
            CustomWordsPath = $"{new Folders().Dict_en_us_DIR}\\CustomWords-en_us.txt";
        }
        public string En_aff_file { get; }
        public string En_dic_file { get; }
        public string CustomWordsPath { get; }
    }

}
