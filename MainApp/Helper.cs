using System;
using System.IO;
using System.Linq;

namespace MainApp
{
    public class Helper
    {
        public string GetFolder(string dirPath)
        {
            string folder = "";

            dirPath = dirPath.Trim();
            if (Directory.Exists(dirPath))
            {
                if (dirPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    dirPath = dirPath.TrimEnd(Path.DirectorySeparatorChar);
                    folder = dirPath.Split(Path.DirectorySeparatorChar).LastOrDefault();
                    return folder;
                }
                else
                {
                    folder = dirPath.Split(Path.DirectorySeparatorChar).LastOrDefault();
                    return folder;
                }
            }
            else if (File.Exists(dirPath))
            {
                folder = Directory.GetParent(dirPath).Name;
                return folder;
            }
            return folder;
        }
        #region TimeStamp
        public string GetDateTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
        }
        public string GetTimeStamp()
        {
            return DateTime.Now.ToString("HHmmssfff");
        }
        public void WriteLog(string log)
        {
            Paths paths = new Paths();
            File.WriteAllText($"{paths.Folders.Log_Dir}\\SAF_{GetDateTimeStamp()}.log", log);
        }
        #endregion TimeStamp
    }


}
