namespace MainApp
{
    public class Paths
    {
        readonly Folders folders = new Folders();
        readonly Files files = new Files();

        internal Folders Folders => folders;
        internal Files Files => files;
    }

    class Folders
    {
        public  Folders()
        {
            AdminDir = "D:\\data\\A_Process\\Admin";
            InputDir = "D:\\data\\A_Process\\Input";
            PriorityDir = "D:\\data\\A_Process\\Input";
            OutputDir = "D:\\data\\A_Process\\Output";
        }
        public string AdminDir { get; set; }
        public string InputDir { get; set; }
        public string PriorityDir { get; set; }
        public string OutputDir { get; set; }
    }
    class Files
    {
        public Files()
        {
            En_aff_file =$"D:\\data\\A_Process\\Admin\\Dict\\en_us\\en_us.aff";
            En_dic_file = $"D:\\data\\A_Process\\Admin\\Dict\\en_us\\en_us.dic";
            CustomWordPath = $"D:\\data\\A_Process\\Admin\\Dict\\en_us\\CustomWords-en_us.txt";
        }
        public string En_aff_file { get; set; }
        public string En_dic_file { get; set; }
        public string CustomWordPath { get; set; }
    }

}
