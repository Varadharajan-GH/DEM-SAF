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
            Root_Dir = "D:\\data\\A_Process";
            Admin_Dir = $"{Root_Dir}\\Admin";
            Input_Dir = $"{Root_Dir}\\Input";
            Priority_Dir = $"{Root_Dir}\\Input";
            Output_Dir = $"{Root_Dir}\\Output";
            Dict_en_us_DIR = $"{Admin_Dir}\\Dict\\en_us";
        }
        public string Root_Dir { get; }
        public string Admin_Dir { get;  }
        public string Input_Dir { get;  }
        public string Priority_Dir { get; }
        public string Output_Dir { get;  }
        public string Dict_en_us_DIR { get; }
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
