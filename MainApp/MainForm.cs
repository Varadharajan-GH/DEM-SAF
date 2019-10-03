using CustomSerilization;
using NHunspell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using NetSpell.SpellChecker;
//using Tesseract;
using tessnet2;

namespace MainApp
{
    public partial class MainForm : Form
    {
        #region Declaration

        private char HyphenChar = (char)173;
        private readonly StringBuilder sbLog;
        private Point mouseDown;
        private Point mouseUp;
        private readonly BackgroundWorker bw_OCR_Selection; 
        //private readonly BackgroundWorker bw_OCR_AllPages;
        private Rectangle rect;
        //private Control ctlTextBox;
        private string strText;
        delegate void SetTextCallback(string text);
        delegate void EndUpdateCallback(Control ctl);
        delegate void SpellCheckCallback(RichTextBox rtb);
        private Control _lastFocusedControl;
        //private readonly string currentDir;
        //private readonly string currentAccession;
        private string wordToCheck;
        private string oldText;
        //private string currentImage;
        private RectangleBox rBox = new RectangleBox();
        private readonly Paths paths = new Paths();
        SerializeDeserialize<ISSUE> serializeIssue;
        ISSUE deserializedIssues;
        //private string currentItem;
        private const int WM_SETREDRAW = 11;
        private ACCESSION objAccession;
        private AccessionInfo accessionInfo;
        public string UserName;
        private Dictionary<int, List<Word>> ResultText;
        private readonly string LockID;
        ItemInfo currentItem;

        #endregion Declaration

        public MainForm()
        {
            sbLog = new StringBuilder();

            InitializeComponent();

            bw_OCR_Selection = new BackgroundWorker();
            bw_OCR_Selection.DoWork += ConvertImageToText;
            bw_OCR_Selection.RunWorkerCompleted += BwOSCompleted;

            //ConvertTifToPDF("BL7CX160A");

            //bw_OCR_AllPages = new BackgroundWorker();
            //bw_OCR_AllPages.DoWork += ConvertTifToText;
            //bw_OCR_AllPages.RunWorkerCompleted += BwOAPCompleted;                       

            AdjustWindow();

            SetTextBoxEnterEvents(this);

            LoadLanguageInCombo();

            SetIDType();

            rtbTitleTitle.Focus();

            _lastFocusedControl = rtbTitleTitle;

            using (LoginForm loginForm = new LoginForm())
            {
                loginForm.Show();
                while (loginForm.UserName == null) Application.DoEvents();
                UserName = loginForm.UserName;
            }

            LockID = UserName;

            RunApp();

            Helper.WriteLog(sbLog.ToString());

            bw_OCR_Selection.Dispose();
        }

        private void LoadLanguageInCombo()
        {
            cmbTitleLang.Items.Clear();
            foreach (object lang in Enum.GetValues(typeof(LANGUAGE)))
            {
                cmbTitleLang.Items.Add(lang);
            }
        }

        private void RunApp()
        {
            AddLog("Started");

            string lockedDir = "";

            lockedDir = FindInput(LockID);

            if (string.IsNullOrWhiteSpace(lockedDir))
            {
                AddLog("No input files found.");
                MessageBox.Show("No input files found.");
                return;
            }
            AddLog("Found " + lockedDir);

            //objAccession = new ACCESSION();
            accessionInfo = new AccessionInfo();

            if (lockedDir.Contains(paths.Folders.Priority_Dir))
            {
                //objAccession.ParentDirectory = paths.Folders.Priority_Dir;
                //objAccession.IsPriority = true;
                accessionInfo.FullSourceParentPath = paths.Folders.Priority_Dir;
                accessionInfo.IsPriority = true;
            }
            else
            {
                //objAccession.ParentDirectory = paths.Folders.Input_Dir;
                //objAccession.IsPriority = false;
                accessionInfo.FullSourceParentPath = paths.Folders.Input_Dir;
                accessionInfo.IsPriority = false;
            }
            //objAccession.FolderName = Helper.GetFolder(lockedDir);
            //objAccession.LockID = LockID;
            accessionInfo.SourceName = Helper.GetFolder(lockedDir).Split('.').FirstOrDefault();
            accessionInfo.LockID = LockID;

            //UserShortName=GetShortText(UserName,10);

            if (!Directory.Exists(accessionInfo.FullSourceAliasPath))
            {
                AddLog("Input folder does not exists");
                return;
            }

            string[] itemsArray = Directory.GetDirectories(accessionInfo.FullSourceAliasPath);

            if (itemsArray.Length == 0)
            {
                AddLog($"No items in {accessionInfo.FullSourcePath}");
                return;
            }

            //objAccession.CurrentDirectory = paths.Folders.Current_Dir;
            accessionInfo.CurrentName = accessionInfo.SourceName;
            accessionInfo.FullCurrentParentPath = Path.Combine(paths.Folders.Current_Dir, LockID);

            accessionInfo.UnprocessedItems = new Queue<ItemInfo>();
            if (itemsArray.Length <= 10)
            {
                MoveNItems(itemsArray, itemsArray.Length);
                //foreach (string item in itemsArray)
                //{
                //    try
                //    {
                //        string destPath = Path.Combine(Directory.CreateDirectory(accessionInfo.FullCurrentPath).FullName, Helper.GetFolder(item));
                //        Directory.Move(item, destPath);
                //        accessionInfo.UnprocessedItems.Enqueue(Helper.GetFolder(destPath));
                //    }
                //    catch (Exception e)
                //    {
                //        AddLog("Unable to move " + item);
                //        AddLog(e.Message);
                //    }
                //}
            }
            else if (itemsArray.Length <= 20)
            {
                MoveNItems(itemsArray, (int)Math.Ceiling((double)itemsArray.Length / 2));
            }
            else
            {
                MoveNItems(itemsArray, 10);
            }

            //foreach (string dir in Directory.GetDirectories(objAccession.FullSourcePath()))
            //{

            //}

            //DirectoryInfo di = new DirectoryInfo(objAccession.FullSourcePath());
            //di.MoveTo(di.FullName + "." + UserShortName);
            //objAccession.FolderName = di.Name;

            //ProcessAccession(currentDir + Path.DirectorySeparatorChar + currentAccession);    

            AddLog("ProcessAccession");
            ProcessAccession();

            //bw_OCR_AllPages.Dispose();
        }

        private void MoveNItems(string[] itemsArray, int n)
        {
            int i = 0;
            foreach (string item in itemsArray)
            {
                if (i >= n) break;
               
                    try
                    {
                        string destPath = Path.Combine(Directory.CreateDirectory( accessionInfo.FullCurrentPath).FullName, Helper.GetFolder(itemsArray[i]));
                        Directory.Move(itemsArray[i], destPath);
                        accessionInfo.UnprocessedItems.Enqueue(new ItemInfo(destPath));
                        i++;
                    }
                    catch(Exception e)
                    {
                        AddLog("Unable to move " + itemsArray[i]);
                        AddLog(e.Message);
                    }
            }
            AddLog(i + " items moved out of " + itemsArray.Length + " items.");
        }

        public string FindInput(string LockID)
        {
            string curFolder = paths.Folders.Priority_Dir;

            AddLog("Searching for Input");
            while (true)
            {
                foreach (string dir in Directory.GetDirectories(curFolder))
                {
                    if (string.IsNullOrEmpty(dir)) continue;

                    string tempName = dir.Split(Path.DirectorySeparatorChar).LastOrDefault();

                    if (tempName.Contains("."))
                    {
                        if (tempName.Split('.').LastOrDefault() == LockID)
                        {
                            return dir;
                        }
                    }
                    else
                    {
                        try
                        {
                            AddLog("Locking Folder");
                            Directory.Move(dir, dir + "." + LockID);
                            AddLog("Locked");
                            return dir + "." + LockID;
                        }
                        catch { }
                    }
                }

                if (curFolder == paths.Folders.Priority_Dir)
                {
                    curFolder = paths.Folders.Input_Dir;
                }
                else
                {
                    return "";
                }
            }
        }

        private void ProcessAccession()
        {
            AddLog("Processing Accession " + accessionInfo.SourceName);
            int num = 0;
            //Helper helper = new Helper();

            //if (objAccession.FolderName.Contains("."))
            //{
            //    if (!UserName.Contains(objAccession.FolderName.Split('.').LastOrDefault()))
            //    {
            //        AddLog($"The file {objAccession.FolderName.Split('.').FirstOrDefault()} locked by another user");
            //        return;
            //    }
            //    else
            //    {
            //        if (Directory.Exists(paths.Folders.Current_Dir + "\\" + objAccession.Name))
            //        {
            //            AddLog($"{paths.Folders.Current_Dir + "\\" + objAccession.Name} already exist and will be renamed");
            //            Directory.Move(paths.Folders.Current_Dir + "\\" + objAccession.Name,
            //                paths.Folders.Current_Dir + "\\" + objAccession.Name + "." + Helper.GetTimeStamp());
            //        }
            //        try
            //        {
            //            Directory.Move(objAccession.FullSourcePath(), paths.Folders.Current_Dir + "\\" + objAccession.Name);
            //            objAccession.FolderName = objAccession.Name;
            //        }
            //        catch (Exception)
            //        {
            //            AddLog("Could move to Current directory");
            //        }
            //        objAccession.CurrentDirectory = paths.Folders.Current_Dir;
            //    }
            //}
            //else
            //{
            //    AddLog($"The file {objAccession.FolderName.Split('.').FirstOrDefault()} not locked");
            //    return;
            //}
            //foreach (string dir in Directory.GetDirectories(objAccession.CurrentDirectory + "\\" + objAccession.Name, objAccession.Name + "*"))
            
            foreach(ItemInfo item in accessionInfo.UnprocessedItems)
            {

                accessionInfo.CurrentItem = item.Name;                

                item.FullParentPath = accessionInfo.FullCurrentPath;

                AddLog($"Processing item {item.Name}");

                //string xmlFile = item.XMLPath; // Path.Combine(item, accessionInfo.CurrentItem + ".XML");

                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    AddLog("Loading xml " + item.XMLPath);
                    xmlDoc.Load(item.XMLPath);
                }
                catch (Exception e)
                {
                    AddLog(e.Message);
                    continue;
                }

                num++;
                item.Accession = xmlDoc.SelectSingleNode("//ID_ACCESSION").InnerText;
                item.ItemNumber = xmlDoc.SelectSingleNode("//ITEM").Attributes["ITEMNO"].Value;
                item.PG = accessionInfo.CurrentItem.Substring(accessionInfo.CurrentItem.Length - 1, 1);
                item.DocType = xmlDoc.SelectSingleNode("//DT_DOCUMENTTYPE").InnerText;
                item.PageSpan = xmlDoc.SelectSingleNode("//PG_PAGESPAN").InnerText;

                ListViewItem listViewItem = new ListViewItem(num.ToString());
                listViewItem.SubItems.AddRange(new string[]
                {
                    item.Accession, item.ItemNumber, item.PG, item.DocType, "Queued", item.PageSpan
                });
                _ = lvItems.Items.Add(listViewItem);
            }
            lvItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            //ProcessXML(accnDir, objAccession.Items[0]);
            //while (accessionInfo.UnprocessedItems.Count > 0)
            //{                
                ProcessItem();
            //}
            AddLog($"{accessionInfo.ProcessedItems.Count} Items of Accession {accessionInfo.SourceName} completed");
        }

        private void ProcessItem()
        {
            if (accessionInfo.UnprocessedItems.Count == 0) return;
            currentItem = accessionInfo.UnprocessedItems.Dequeue();
            accessionInfo.CurrentItemPath = currentItem.FullPath;

            AddLog("Processing item " + currentItem.Name);

            //bw_OCR_AllPages.RunWorkerAsync();

            serializeIssue = new SerializeDeserialize<ISSUE>();

            //string currentXML = Path.Combine(accessionInfo.CurrentItemPath, accessionInfo.CurrentItem + ".XML");

            XmlDocument xmldoc = new XmlDocument();
            
            try
            {
                AddLog("Loading xml " + currentItem.XMLPath);
                xmldoc.Load(currentItem.XMLPath);
            }
            catch (Exception e)
            {
                AddLog(e.Message);
                accessionInfo.ErrorItems.Add(currentItem);
                return;
            }

            AddLog("Deserializing");
            deserializedIssues = serializeIssue.DeserializeData(xmldoc.OuterXml);

            currentItem.ContentPageTifs = new List<string>();
            currentItem.MainPageTifs = new List<string>();

            foreach (string tif in Directory.GetFiles(currentItem.FullPath, $"{currentItem.Name}*.TIF"))
            {
                if (tif.Split(Path.PathSeparator).LastOrDefault().Contains("_CP"))
                {
                    currentItem.ContentPageTifs.Add(tif);
                    AddLog($"{tif} added to ContentPageTifs");
                }
                else
                {
                    currentItem.MainPageTifs.Add(tif);
                    AddLog($"{tif} added to MainPageTifs");
                }
            }

            LoadValues(deserializedIssues);

            //LoadPDF($"{objAccn.CurrentDirectory}\\{objAccn.FolderName}\\OCR_PDF", objAccn.CurrentItem);
            currentItem.CurrentImage = currentItem.MainPageTifs.First();
            currentItem.IsContentPage = false;
            LoadImage(currentItem.CurrentImage);
            lvItems.Items[accessionInfo.ProcessedItems.Count].SubItems[5].Text = "In Progress";
            lvItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            tcTagArea.SelectedTab = tpTitle;
            _ = rtbTitleTitle.Focus();
        }

        //private void BwOAPCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    AddLog("Called");
        //    tsProgress2.Style = ProgressBarStyle.Continuous;
        //    tsProgress2.MarqueeAnimationSpeed = 0;
        //    tsStatus2.Text = "OCR pages Completed";
        //}
        #region OCR

        //public void ConvertTifToText(object sender, DoWorkEventArgs doWorkEventArgs)
        //{
        //    ResultText = new Dictionary<int, List<Word>>();

        //    tsProgress2.Style = ProgressBarStyle.Marquee;
        //    tsProgress2.MarqueeAnimationSpeed = 30;
        //    tsStatus2.Text = "Convert pages in progress";

        //    using (Bitmap img = new Bitmap (  $"{objAccession.CurrentDirectory}\\{objAccession.FolderName}\\{objAccession.CurrentItem}\\{objAccession.CurrentItem}_11089.TIF"))
        //    {
        //        using (Tesseract tesOCR = new Tesseract())
        //        {
        //            AddLog("Initializing OCR Engine");
        //            _ = tesOCR.Init(paths.Folders.Ocrdata_Dir, "eng", false);
        //            AddLog("OCR Engine Initialized");

        //            try
        //            {
        //                ResultText.Add(11089, tesOCR.DoOCR(img, Rectangle.Empty));
        //                //ResultText = ocr.DoOCR(img, rect);
        //            }
        //            catch (Exception e)
        //            {
        //                tsStatus1.Text = "Unable to Read text";
        //                AddLog(e.Message);
        //                return;
        //            }
        //        }
        //    }                
        //}

        public void ConvertImageToText(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            Bitmap image = (Bitmap)pbeImage.Image;

            StringBuilder sbOcrText = new StringBuilder();
            using (Tesseract ocr = new Tesseract())
            {
                AddLog("Initializing OCR Engine");
                _ = ocr.Init(paths.Folders.Ocrdata_Dir, "eng", false);
                AddLog("OCR Engine Initialized");
                List<Word> result;
                if (rect.X < 0)
                {
                    rect.Width += rect.X;
                    rect.X = 0;
                }
                if (rect.Y < 0)
                {
                    rect.Height += rect.Y;
                    rect.Y = 0;
                }

                //Allow time for resource to be released
                System.Threading.Thread.Sleep(100);

                if (rect.X + rect.Width > image.Width)
                {
                    rect = new Rectangle(rect.X, rect.Y, image.Width - rect.X, rect.Height);
                }
                if (rect.Y + rect.Height > image.Height)
                {
                    rect = new Rectangle(rect.X, rect.Y, rect.Width, image.Height - rect.Y);
                }
                if (rect.Width < 0) return;
                if (rect.Height < 0) return;
                try
                {
                    result = ocr.DoOCR(image, rect);
                }
                catch (Exception)
                {
                    tsStatus1.Text = "Unable to Read. Try to Draw inside bounds";
                    return;
                }

                for (int i = 0; i < Tesseract.LineCount(result); i++)
                {
                    _ = sbOcrText.Append(Tesseract.GetLineText(result, i) + " ");
                }

                strText = sbOcrText.ToString();
                SetText(strText);
                //SpellCheck(rtbAbstract);
                //SpellCheck((RichTextBox)_lastFocusedControl);
            }
        }

        #endregion OCR

        #region SpellChecker
        //private void SpellCheck(RichTextBox rtb)
        //{
        //    AddLog("Enter");

        //    if (rtb.InvokeRequired)
        //    {
        //        AddLog("Invoke required");
        //        SpellCheckCallback spellCheckCallback = new SpellCheckCallback(SpellCheck);
        //        this.Invoke(spellCheckCallback, new object[] { rtb });
        //        AddLog("Invoked");
        //    }
        //    else
        //    {
        //        AddLog("Started");
        //        //rtb.SuspendLayout();
        //        BeginControlUpdate(rtb);
        //        //Font fontrtb = new Font(rtbxAbstract.Font.FontFamily, rtbxAbstract.Font.Size, FontStyle.Regular);

        //        rtb.SelectAll();
        //        rtb.SelectionBackColor = Color.White;

        //        try
        //        {
        //            using (Hunspell hunspell = new Hunspell(paths.Files.En_aff_file, paths.Files.En_dic_file))
        //            {
        //                if (File.Exists(paths.Files.CustomWordsPath))
        //                {
        //                    string[] lines = ReadAllLines(paths.Files.CustomWordsPath);
        //                    AddLog("Adding custom words");
        //                    foreach (string line in lines)
        //                    {
        //                        hunspell.Add(line);
        //                    }
        //                }

        //                AddLog("Checking spelling");
        //                foreach (string word in rtb.Text.Split(' '))
        //                {
        //                    string newWords = Regex.Replace(word, @"[^a-zA-Z0-9]+", " ");
        //                    foreach (string newWord in newWords.Split(' '))
        //                    {
        //                        bool isCorrect = hunspell.Spell(newWord);
        //                        if (!isCorrect)
        //                        {
        //                            rtb.SelectionStart = rtb.Text.IndexOf(word) + word.IndexOf(newWord);
        //                            rtb.SelectionLength = newWord.Length;
        //                            //rtb.SelectionFont = new Font(fontrtb.FontFamily, fontrtb.Size, FontStyle.Underline);
        //                            rtb.SelectionBackColor = Color.LightPink;
        //                        }
        //                        else
        //                        {
        //                            rtb.SelectionStart = rtb.Text.IndexOf(word) + word.IndexOf(newWord);
        //                            rtb.SelectionLength = newWord.Length;
        //                            //rtb.SelectionFont = new Font(fontrtb.FontFamily, fontrtb.Size, FontStyle.Regular);
        //                            rtb.SelectionBackColor = rtb.BackColor;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            AddLog(e.Message);
        //        }
        //        EndControlUpdate(rtb);
        //    }

        //    AddLog("Exit");
        //}
        //private void SuggestionClicked(object sender, EventArgs e)
        //{
        //    AddLog("SuggestionClicked: Enter");
        //    MenuItem miMenuItem = (MenuItem)sender;
        //    if (miMenuItem.Text == "More...")
        //    {
        //        AddLog("Calling ShowSpellChecker");
        //        ShowSpellChecker(wordToCheck);
        //    }
        //    else if (miMenuItem.Text == "Add to Dictionary")
        //    {
        //        AddLog("Calling AddWordToDictionary");
        //        AddWordToDictionary(wordToCheck);
        //    }
        //    else
        //    {
        //        oldText = rtbAbstract.Text;
        //        rtbAbstract.Text = rtbAbstract.Text.Replace(wordToCheck, miMenuItem.Text);
        //        AddLog("Calling SpellCheck");
        //        SpellCheck(rtbAbstract);
        //    }
        //    AddLog("SuggestionClicked: Exit");
        //}
        //private void AddWordToDictionary(string wordToCheck)
        //{
        //    using (Hunspell hunspell = new Hunspell(paths.Files.En_aff_file, paths.Files.En_dic_file))
        //    {
        //        if (File.Exists(paths.Files.CustomWordsPath))
        //        {
        //            string[] lines = ReadAllLines(paths.Files.CustomWordsPath);
        //            foreach (string line in lines)
        //            {
        //                hunspell.Add(line);
        //            }
        //        }
        //        bool isCorrect = hunspell.Spell(wordToCheck);
        //        if (!isCorrect)
        //        {
        //            using (StreamWriter streamWriter = new StreamWriter(paths.Files.CustomWordsPath, true))
        //            {
        //                streamWriter.WriteLine(wordToCheck);
        //            }
        //        }
        //    }
        //    SpellCheck(rtbAbstract);
        //}
        //private void ShowSpellChecker(string word)
        //{
        //    //ToDo
        //    MessageBox.Show("Yet to implement " + word);
        //}
        //private string WordUnderMouse(RichTextBox rtb, int x, int y)
        //{
        //    AddLog("Enter");
        //    // Get the character's position.
        //    int pos = rtb.GetCharIndexFromPosition(new Point(x, y));
        //    if (pos <= 0) return "";

        //    // Find the start of the word.
        //    string txt = rtb.Text;

        //    int start_pos;
        //    for (start_pos = pos; start_pos >= 0; start_pos--)
        //    {
        //        // Allow digits, letters, and underscores
        //        // as part of the word.
        //        char ch = txt[start_pos];
        //        if (!char.IsLetterOrDigit(ch) && !(ch == '_')) break;
        //    }
        //    start_pos++;

        //    // Find the end of the word.
        //    int end_pos;
        //    for (end_pos = pos; end_pos < txt.Length; end_pos++)
        //    {
        //        char ch = txt[end_pos];
        //        if (!char.IsLetterOrDigit(ch) && !(ch == '_')) break;
        //    }
        //    end_pos--;

        //    // Return the result.
        //    AddLog("Returning");
        //    if (start_pos > end_pos) return "";
        //    return txt.Substring(start_pos, end_pos - start_pos + 1);
        //}

        #endregion SpellChecker

        #region AllEvents

        #region Others
        private void PbeImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = e.Location;
                rBox.StartPoint = e.Location;
                mouseDown = pbeImage.TranslatePointToImageCoordinates(mouseDown);
                //tsStatus.Text = $"Mousedown ({mouseDown.X} , {mouseDown.Y})";
            }
        }
        private void PbeImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseUp = e.Location;
                mouseUp = pbeImage.TranslatePointToImageCoordinates(mouseUp);

                rect = new Rectangle(Math.Min(mouseDown.X, mouseUp.X), Math.Min(mouseDown.Y, mouseUp.Y), Math.Abs(mouseDown.X - mouseUp.X), Math.Abs(mouseDown.Y - mouseUp.Y));
                rBox.EndPoint = e.Location;

                rBox = new RectangleBox();

                if (rect.Width > 0 && rect.Height > 0)
                {
                    tsProgress1.Style = ProgressBarStyle.Marquee;
                    tsProgress1.MarqueeAnimationSpeed = 30;
                    tsStatus1.Text = "Capturing text please wait";
                    pbeImage.Invalidate();
                    if (_lastFocusedControl != null)
                    {
                        //ctlTextBox = _lastFocusedControl; 
                        if (!bw_OCR_Selection.IsBusy)
                            bw_OCR_Selection.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Select a field to copy text");
                        return;
                    }
                }
            }
        }
        private void PbeImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                rBox.EndPoint = e.Location;
                //tsStatus.Text = $"MouseMove ({mouseDown.X} , {mouseDown.Y}) -> ({mouseUp.X} , {mouseUp.Y})";
                pbeImage.Invalidate();
            }
        }
        private void PbeImage_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, rBox.rectBox);
            }
        }
        private void LogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogForm lg = new LogForm();
            lg.SetLog(sbLog.ToString());
            lg.Show();
        }
        private void MiQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //private void rtbAbstract_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        using (Hunspell hunspell = new Hunspell(paths.Files.En_aff_file, paths.Files.En_dic_file))
        //        {
        //            if (File.Exists(paths.Files.CustomWordsPath))
        //            {
        //                string[] lines = ReadAllLines(paths.Files.CustomWordsPath);
        //                foreach (string line in lines)
        //                {
        //                    hunspell.Add(line);
        //                }
        //            }
        //            wordToCheck = WordUnderMouse(rtbAbstract, e.X, e.Y);
        //            List<string> suggestions = hunspell.Suggest(wordToCheck);
        //            ContextMenu contextMenu = new ContextMenu();
        //            int sugCount = 0;
        //            MenuItem menuItem;
        //            foreach (string suggestion in suggestions)
        //            {
        //                menuItem = new MenuItem(suggestion);
        //                contextMenu.MenuItems.Add(menuItem);
        //                menuItem.Click += new EventHandler(SuggestionClicked);
        //                sugCount++;
        //                if (sugCount >= 10)
        //                {
        //                    menuItem = new MenuItem("More...");
        //                    contextMenu.MenuItems.Add(menuItem);
        //                    menuItem.Click += new EventHandler(SuggestionClicked);
        //                    break;
        //                }
        //            }

        //            menuItem = new MenuItem("Add to Dictionary");
        //            contextMenu.MenuItems.Add(menuItem);
        //            menuItem.Click += new EventHandler(SuggestionClicked);
        //            rtbAbstract.ContextMenu = contextMenu;
        //        }
        //    }
        //}
        private void TcTagArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcTagArea.SelectedTab.Name == "tpAbstract")
            {
                rtbAbstract.Focus();
            }
            else if (tcTagArea.SelectedTab.Name == "tpKeywords")
            {
                rtbKeywords.Focus();
            }
            else if (tcTagArea.SelectedTab.Name == "tpTitle")
            {
                rtbTitleTitle.Focus();
            }
            else if (tcTagArea.SelectedTab.Name == "tpReference")
            {
                txtRefSurname.Focus();
            }
        }
        #endregion Others

        #region ToolStripButtonClicks      

        private void TsbRestore_Click(object sender, EventArgs e)
        {
            pbeImage.ImageLocation = currentItem.CurrentImage;
            //PDFReader.setZoomScroll(70, 0, 0);
        }
        private void TsbTopLeft_Click(object sender, EventArgs e)
        {
            ZoomTopLeft();
        }
        private void TsbTopRight_Click(object sender, EventArgs e)
        {
            ZoomTopRight();
        }
        private void TsbBottomLeft_Click(object sender, EventArgs e)
        {
            ZoomBottomLeft();
        }
        private void TsbBottomRight_Click(object sender, EventArgs e)
        {
            ZoomBottomRight();
        }
        private void TsbTopHalf_Click(object sender, EventArgs e)
        {
            ZoomTopHalf();
        }
        private void TsbBottomHalf_Click(object sender, EventArgs e)
        {
            ZoomBottomHalf();
        }
        private void TsbSpellCheck_Click(object sender, EventArgs e)
        {
            //SpellCheck(rtbAbstract);
            //SpellCheck((RichTextBox)_lastFocusedControl);            
            bool DoSpellCheck = true;
            //RichTextBox tRTB = null;
            //TextBox tTB = null;
            //try { tRTB = (RichTextBox)_lastFocusedControl; } catch { }
            //try { tTB = (TextBox)_lastFocusedControl; } catch { }

            if (rtbAbstract.Find(HyphenChar.ToString(), 0, RichTextBoxFinds.NoHighlight) > -1)
            {
                DoSpellCheck = MessageBox.Show("Text to be spell-checked contains syllable hyphen(s)." +
                    Environment.NewLine + "This might affect the accuracy of the spell-check.",
                    "Hyphens Present", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.OK;
            }
            // do spell-check if given the OK
            if (DoSpellCheck)
            {
                Spelling spelling = new Spelling();
                if(_lastFocusedControl.GetType().Name == "RichTextBox")
                {
                    if (((RichTextBox)_lastFocusedControl).SelectionLength == 0)
                    {
                        spelling.Text = ((RichTextBox)_lastFocusedControl).Text;         // all text
                    }
                    else
                    {
                        spelling.Text = ((RichTextBox)_lastFocusedControl).SelectedText;// selected text
                    }
                }
                else if (_lastFocusedControl.GetType().Name == "TextBox")
                {
                    if (((TextBox)_lastFocusedControl).SelectionLength == 0)
                    {
                        spelling.Text = ((TextBox)_lastFocusedControl).Text;         // all text
                    }
                    else
                    {
                        spelling.Text = ((TextBox)_lastFocusedControl).SelectedText;// selected text
                    }
                }


                spelling.DeletedWord += Spelling_DeletedWord;
                spelling.ReplacedWord += Spelling_ReplacedWord;
                spelling.SpellCheck();
            }
        }

        private void Spelling_ReplacedWord(object sender, ReplaceWordEventArgs e)
        {
            //RichTextBox rtb = ((RichTextBox)_lastFocusedControl);
            //rtb.Select(0 + e.TextIndex, e.Word.Length);
            //rtb.SelectedText = e.ReplacementWord;
            if (_lastFocusedControl.GetType().Name == "RichTextBox")
            {
                ((RichTextBox)_lastFocusedControl).Select(0 + e.TextIndex, e.Word.Length);
                ((RichTextBox)_lastFocusedControl).SelectedText = e.ReplacementWord;
            }
            else if (_lastFocusedControl.GetType().Name == "TextBox")
            {
                ((TextBox)_lastFocusedControl).Select(0 + e.TextIndex, e.Word.Length);
                ((TextBox)_lastFocusedControl).SelectedText = e.ReplacementWord;
            }

        }

        private void Spelling_DeletedWord(object sender, SpellingEventArgs e)
        {
            //((RichTextBox)_lastFocusedControl).Select(0 + e.TextIndex, e.Word.Length);
            //((RichTextBox)_lastFocusedControl).SelectedText = "";
            if (_lastFocusedControl.GetType().Name == "RichTextBox")
            {
                ((RichTextBox)_lastFocusedControl).Select(0 + e.TextIndex, e.Word.Length);
                ((RichTextBox)_lastFocusedControl).SelectedText = "";
            }
            else if (_lastFocusedControl.GetType().Name == "TextBox")
            {
                ((TextBox)_lastFocusedControl).Select(0 + e.TextIndex, e.Word.Length);
                ((TextBox)_lastFocusedControl).SelectedText = "";
            }
        }

        private void TsbUndo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(oldText))
                return;
            if (MessageBox.Show("Do you want to restore abstract to its previous text?", "Confirm Undo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                rtbAbstract.Text = oldText;
                oldText = "";
                //SpellCheck(rtbAbstract);
            }
        }
        private void TsbSwitchPages_Click(object sender, EventArgs e)
        {
            if (((ToolStripButton)sender).Text == "CP")
            {
                ((ToolStripButton)sender).Text = "NP";
                ((ToolStripButton)sender).ToolTipText = "Switch to Main Pages";
                LoadPDF($"{objAccession.CurrentDirectory}\\{objAccession.FolderName}\\OCR_PDF\\CP", objAccession.Name + "_CP");
            }
            else
            {
                ((ToolStripButton)sender).Text = "CP";
                ((ToolStripButton)sender).ToolTipText = "Switch to Content Pages";
                LoadPDF($"{objAccession.CurrentDirectory}\\{objAccession.FolderName}\\OCR_PDF", objAccession.CurrentItem);
            }
        }
        private void TsbArrangeKW_Click(object sender, EventArgs e)
        {
            string keyWords = rtbKeywords.Text;
            if (!string.IsNullOrWhiteSpace(keyWords))
            {
                keyWords = keyWords.Replace(";", Environment.NewLine);
                rtbKeywords.Text = keyWords;
                StringBuilder newLines = new StringBuilder();
                foreach (string line in rtbKeywords.Lines)
                {
                    if (!string.IsNullOrWhiteSpace(line.Trim()))
                        _ = newLines.AppendLine(line.Trim());
                }
                rtbKeywords.Text = newLines.ToString();
            }
        }

        #endregion ToolStripButtonClicks

        #region TabCOntrolEvents

        private void TpKeywords_Enter(object sender, EventArgs e)
        {
            rtbKeywords.Focus();
        }

        private void TpAbstract_Enter(object sender, EventArgs e)
        {
            rtbAbstract.Focus();
        }

        private void TpReference_Enter(object sender, EventArgs e)
        {
            txtRefSurname.Focus();
        }

        #endregion TabCOntrolEvents

        #region MenuButtonClicks

        private void MiDone_Click(object sender, EventArgs e)
        {
            SaveValues();
            currentItem = null;
            accessionInfo.ProcessedItems.Add(currentItem);
            //objAccession.UnprocessedItems.Remove(objAccession.CurrentItem);
            lvItems.Items[accessionInfo.ProcessedItems.Count - 1].SubItems[5].Text = "Completed";
            lvItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ClearInputFields();

            if (accessionInfo.UnprocessedItems.Count > 0)
            {
                currentItem = accessionInfo.UnprocessedItems.Dequeue();
                ProcessItem();
            }           
        }



        #endregion MenuButtonClicks

        #endregion AllEvents

        #region ToolStripButtonMethods
        private void ZoomTopLeft()
        {
            Bitmap src = Image.FromFile(currentItem.CurrentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, 0, (src.Width / 2) + 100, (src.Height / 2) + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }

            //PDFReader.setViewRect(0, 0, (PDFReader.Width / 2) + 100, (PDFReader.Height / 2) + 100);
            //PDFReader.setZoomScroll(100, 0, 0);

        }
        private void ZoomTopRight()
        {
            Bitmap src = Image.FromFile(currentItem.CurrentImage) as Bitmap;
            Rectangle cropRect = new Rectangle((src.Width / 2) - 100, 0,
                                                src.Width / 2 + 100, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
            //PDFReader.setViewRect((PDFReader.Width / 2) - 100, 0, PDFReader.Width / 2 + 100, PDFReader.Height / 2 + 100);
            //PDFReader.setZoomScroll(100, (PDFReader.Width / 2) - 100, 0);
        }
        private void ZoomBottomLeft()
        {
            Bitmap src = Image.FromFile(currentItem.CurrentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, (src.Height / 2) - 100,
                                                src.Width / 2 + 100, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
            //PDFReader.setViewRect(0, (PDFReader.Height / 2) - 100, PDFReader.Width / 2 + 100, PDFReader.Height / 2 + 100);
            //PDFReader.setZoomScroll(100, 0, (PDFReader.Height / 2 + 200));
        }
        private void ZoomBottomRight()
        {
            Bitmap src = Image.FromFile(currentItem.CurrentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(src.Width / 2 - 100, src.Height / 2 - 100,
                                                src.Width / 2 + 100, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
            //PDFReader.setViewRect(PDFReader.Width / 2 - 100, PDFReader.Height / 2 - 100, PDFReader.Width / 2 + 100, PDFReader.Height / 2 + 100);
            //PDFReader.setZoomScroll(100, PDFReader.Width / 2, PDFReader.Height / 2 + 200);
        }
        private void ZoomTopHalf()
        {
            Bitmap src = Image.FromFile(currentItem.CurrentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, 0, src.Width, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
            //PDFReader.setViewRect(0, 0, PDFReader.Width, PDFReader.Height / 2 + 100);
            //PDFReader.setZoomScroll(70, 0, 0);
        }
        private void ZoomBottomHalf()
        {
            Bitmap src = Image.FromFile(currentItem.CurrentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, src.Height / 2 - 100, src.Width, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
            //PDFReader.setViewRect(0, PDFReader.Height / 2 - 100, PDFReader.Width, PDFReader.Height / 2 + 100);
            //PDFReader.setZoomScroll(70, 0, PDFReader.Height / 2 + 200);
        }

        #endregion ToolStripButtonMethods

        #region CommonMethods       

        private void SetIDType()
        {
            AddLog("Adding IDs");
            for (int i = 1; i <= 4; i++)
            {
                ComboBox cb = (ComboBox)Controls.Find("cmbTitleIDType" + i, true)[0];
                cb.Items.AddRange(new string[] { "ARTN", "DOI", "PII", "PMID", "SICI", "UNSP" });
            }
        }
        private void TextBoxEnter(object sender, EventArgs e)
        {
            _lastFocusedControl = (Control)sender;
        }
        private void LoadImage(string tifPath)
        {
            AddLog($"Loading image {tifPath}");
            //currentImage = $"{objAccession.CurrentDirectory}\\{objAccession.Name}\\{objAccession.CurrentItem}\\{objAccession.CurrentItem}_{seq}.TIF";
            pbeImage.ImageLocation = tifPath;
        }
        private void BwOSCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AddLog("Called");
            tsProgress1.Style = ProgressBarStyle.Continuous;
            tsProgress1.MarqueeAnimationSpeed = 0;
            tsStatus1.Text = "Ready";
            //bw_OCR_Selection.Dispose();
        }
        //[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        //[System.Runtime.CompilerServices.CallerLineNumber]  int sourceLineNumber = 0 )
        public void AddLog(string log, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            sbLog.AppendLine($"{DateTime.Now.ToString()} : {memberName} : {log} ;");
            try
            {
                using (StreamWriter streamWriter = new StreamWriter("D:\\Log.txt", true))
                    streamWriter.WriteLine($"{DateTime.Now.ToString()} : {memberName} : {log} ;");
            }
            catch
            {
                sbLog.AppendLine("Unable to save to file");
            }
        }
        private void AdjustWindow()
        {
            AddLog("Adjusting window");
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = 0;
            this.Top = 0;
        }
        public string[] ReadAllLines(String path)
        {
            AddLog("Called");
            List<string> file = new List<string>();
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        file.Add(streamReader.ReadLine());
                    }
                }
            }
            return file.ToArray();
        }
        private void SetText(string strText)
        {
            AddLog("Called");
            if (_lastFocusedControl.InvokeRequired)
            {
                SetTextCallback setTextCallback = new SetTextCallback(SetText);
                Invoke(setTextCallback, new object[] { strText });
                AddLog("Invoked");
            }
            else
            {
                _lastFocusedControl.Text = strText;
            }
        }
        private void SetTextBoxEnterEvents(Control control)
        {
            if ((control is TextBox) ||  (control is RichTextBox))
            {
                control.Enter += new EventHandler(TextBoxEnter);
                return;
            }
            if (control.HasChildren)
            {
                foreach (Control child in control.Controls)
                {
                    SetTextBoxEnterEvents(child);
                }
            }
        }
        private void ProcessXML(string Accession, string Item)
        {
            AddLog("Called");

            serializeIssue = new SerializeDeserialize<ISSUE>();

            string currentXML = $"{Accession}\\{Item}\\{Item}.XML";

            //currentItem = Item;

            XmlDocument xmldoc = new XmlDocument();

            xmldoc.Load(currentXML);

            AddLog("Deserializing");
            deserializedIssues = serializeIssue.DeserializeData(xmldoc.OuterXml);

            LoadValues(deserializedIssues);

            LoadPDF(Accession + "\\", Item);

        }
        private void LoadValues(ISSUE issue)
        {
            #region TITLE           

            try
            {
                rtbTitleTitle.Text = issue.ITEM[0].ITEM_CONTENT.TITLES.TI_TITLE.Data;
            }
            catch
            {
                rtbTitleTitle.Text = "";
            }

            if (issue.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER != null)
            {
                foreach (AI_ARTICLEIDENTIFIER aid in issue.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER)
                {
                    Controls.Find($"txtTitleID{aid.seq}", true)[0].Text = aid.Value.Data;
                    Controls.Find($"cmbTitleIDType{aid.seq}", true)[0].Text = aid.type.ToString();
                }
            }

            try
            {
                cmbTitleLang.Text = issue.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].Value.ToString();
            }
            catch
            {
                cmbTitleLang.Text = "";
            }

            try
            {
                txtTitlePRange.Text = issue.ITEM[0].ITEM_CONTENT.PG_PAGESPAN;
            }
            catch
            {
                txtTitlePRange.Clear();
            }

            #endregion TITLE

            #region KEYWORDS
            rtbKeywords.Text = "";
            StringBuilder stringBuilder = new StringBuilder();
            if (issue.ITEM[0].ITEM_CONTENT.KEYWORD != null)
            {
                foreach (KEYWORD keyword in issue.ITEM[0].ITEM_CONTENT.KEYWORD)
                {
                    stringBuilder.AppendLine(keyword.AUTHOR_KEYWORD.Data);
                }
            }
            rtbKeywords.Text = stringBuilder.ToString();
            #endregion KEYWORDS

            #region ABSTRACT

            try
            {
                rtbAbstract.Text = issue.ITEM[0].ITEM_CONTENT.ABSTRACT[0].Value; ;//.Data;                
            }
            catch
            {
                rtbAbstract.Text = "";
            }

            #endregion ABSTRACT
        }
        private void SaveValues()
        {
            XmlDocument xmldocument = new XmlDocument();

            #region TITLE       
            if (deserializedIssues.ITEM[0].ITEM_CONTENT.TITLES == null)
            {
                deserializedIssues.ITEM[0].ITEM_CONTENT.TITLES = new TITLES();
            }
            try
            {
                deserializedIssues.ITEM[0].ITEM_CONTENT.TITLES.TI_TITLE = xmldocument.CreateCDataSection(FormatText(rtbTitleTitle.Text));
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            List<AI_ARTICLEIDENTIFIER> alAID = new List<AI_ARTICLEIDENTIFIER>();
            for (int i = 1, j = 1; i <= 4; i++)
            {
                if (!string.IsNullOrEmpty(Controls.Find($"txtTitleID{i}", true)[0].Text))
                {
                    if (!string.IsNullOrEmpty(Controls.Find($"cmbTitleIDType{i}", true)[0].Text))
                    {
                        if (Enum.TryParse(Controls.Find($"cmbTitleIDType{i}", true)[0].Text, out IDENTIFIER_TYPE idType))
                        {
                            AI_ARTICLEIDENTIFIER aid = new AI_ARTICLEIDENTIFIER
                            {
                                seq = j.ToString(),
                                type = idType,
                                Value = xmldocument.CreateCDataSection(Controls.Find($"txtTitleID{i}", true)[0].Text.Trim())
                            };
                            alAID.Add(aid);
                            j++;
                        }
                    }
                }
            }
            deserializedIssues.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER = new AI_ARTICLEIDENTIFIER[alAID.Count];
            deserializedIssues.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER = alAID.ToArray();

            //if (Enum.TryParse(txtTitleILang.Text.Trim(), out LANGUAGE language))
            //{
            //    if (deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE == null)
            //    {
            //        deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE = new LA_LANGUAGE[1];
            //        deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0] = new LA_LANGUAGE();
            //    }
            //    deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].Value = language;
            //    deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].seq = "1";
            //}

            if (Enum.TryParse(cmbTitleLang.Text.Trim(), out LANGUAGE language))
            {
                if (deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE == null)
                {
                    deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE = new LA_LANGUAGE[1];
                    deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0] = new LA_LANGUAGE();
                }
                deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].Value = language;
                deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].seq = "1";
            }

            deserializedIssues.ITEM[0].ITEM_CONTENT.PG_PAGESPAN = txtTitlePRange.Text.Trim();
            #endregion TITLE

            #region KEYWORDS
            var arrKwd = rtbKeywords.Text.Trim().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            deserializedIssues.ITEM[0].ITEM_CONTENT.KEYWORD = new KEYWORD[arrKwd.Length];
            for (int i = 0; i < arrKwd.Length; i++)
            {
                deserializedIssues.ITEM[0].ITEM_CONTENT.KEYWORD[i] = new KEYWORD
                {
                    seq = $"{i + 1}",
                    AUTHOR_KEYWORD = xmldocument.CreateCDataSection(arrKwd[i].Trim())
                };
            }
            #endregion KEYWORDS

            #region ABSTRACT
            if (deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT == null)
            {
                deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT = new ABSTRACT[1];
                deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT[0] = new ABSTRACT();
            }
            //deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT[0].Value = xmldocument.CreateCDataSection(rtbAbstract.Text.Replace("\n", " ").Trim());
            deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT[0].Value = xmldocument.CreateCDataSection(FormatText(rtbAbstract.Text).Trim()).OuterXml;
            deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT[0].seq = "1";
            #endregion ABSTRACT

            #region SAVE
            serializeIssue = new SerializeDeserialize<ISSUE>();

            string serializedIssues = serializeIssue.SerializeData(deserializedIssues);

            try
            {
                string outDir = $"{Directory.CreateDirectory($"{ paths.Folders.Output_Dir}\\{accessionInfo.SourceName}\\{currentItem.Name}").FullName}";
                xmldocument.LoadXml(System.Net.WebUtility.HtmlDecode(serializedIssues));
                using (StreamWriter streamWriter = new StreamWriter($"{outDir}\\{currentItem.Name}.XML", false, Encoding.UTF8))
                {
                    AddLog($"Saving {((FileStream)streamWriter.BaseStream).Name}");
                    xmldocument.Save(streamWriter);                     //For UTF-8 encoding
                }
                //try
                //{
                //    MoveTiffs(objAccession.CurrentDirectory + "\\" + objAccession.Name + "\\" + item,
                //        $"{Directory.CreateDirectory($"{ paths.Folders.Output_Dir}\\{objAccession.Name}\\{item}").FullName}", item);
                //}
                //catch (Exception e)
                //{
                //    MessageBox.Show(e.Message);
                //}

                try
                {
                    MoveCurrentItemDir(accessionInfo.FullCurrentPath + "\\" + currentItem.Name,
                                        Directory.CreateDirectory($"{paths.Folders.Completed_Dir}\\{accessionInfo.SourceName}").FullName + "\\" + currentItem.Name);
                }
                catch (Exception)
                {
                    AddLog($"Unable to move {accessionInfo.FullCurrentPath + "\\" + currentItem.Name }");
                }
                MessageBox.Show("Item completed");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            #endregion SAVE
        }
        private void MoveCurrentItemDir(string srcDir, string destDir)
        {
            Directory.Move(srcDir, destDir);
        }
        private void MoveTiffs(string fromDir, string toDir, string itemName)
        {
            if (!Directory.Exists(fromDir))
            {
                AddLog($"{fromDir} does not Exist. Unable to move images.");
                return;
            }
            if (!Directory.Exists(toDir))
            {
                try
                {
                    Directory.CreateDirectory(toDir);
                }
                catch (Exception)
                {
                    AddLog($"Unable to create {toDir}.");
                    return;
                }
            }
            foreach (string tif in Directory.GetFiles(fromDir, itemName + "*.TIF"))
            {
                try
                {
                    File.Move(tif, toDir + "\\" + Path.GetFileName(tif));
                }
                catch (Exception e)
                {
                    AddLog($"Unable to move file {tif}.");
                    AddLog(e.Message);
                }
            }
        }
        private string FormatText(string text)
        {
            string newText = text;

            if (newText.Contains(Environment.NewLine))
            {
                newText = newText.Replace(Environment.NewLine, " ");
            }
            if (newText.Contains("\n"))
            {
                newText = newText.Replace("\n", " ");
            }
            if (newText.Contains("—"))
            {
                newText = newText.Replace("—", "-");
            }
            return newText.Trim();
        }
        //public void TesseractPDF(string file)
        //{
        //    if (!File.Exists(file))
        //    {
        //        AddLog($"File not found - {file}");
        //        return;
        //    }

        //    try
        //    {
        //        using (var engine = new TesseractEngine(paths.Folders.Ocrdata_Dir, "eng", EngineMode.Default))
        //        {
        //            using (var img = Pix.LoadFromFile(file))
        //            {
        //                using (var page = engine.Process(img))
        //                {
        //                    var text = page.GetText();
        //                    var horc = page.GetHOCRText(1);
        //                    var path = Directory.CreateDirectory(Path.GetDirectoryName(file) + "\\OCR_PDF").FullName + "\\" + Path.GetFileNameWithoutExtension(file);

        //                    using (var renderer = ResultRenderer.CreatePdfRenderer(path, paths.Folders.Ocrdata_Dir))
        //                    {
        //                        using (renderer.BeginDocument(path + ".pdf"))
        //                        {
        //                            renderer.AddPage(page);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        Console.WriteLine("Unexpected Error: " + e.Message);
        //        Console.WriteLine("Details: ");
        //        Console.WriteLine(e.ToString());
        //    }
        //}
        public void BeginControlUpdate(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }
        public void EndControlUpdate(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            //if (control.InvokeRequired)
            //{
            //    EndUpdateCallback d = new EndUpdateCallback(EndControlUpdate);
            //    Invoke(d, new object[] { control });
            //}
            //else
            //{
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                  IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
            control.Invalidate();
            control.Refresh();
            //}            
        }
        private void LoadPDF(string dir, string fName)
        {
            //PDFReader.LoadFile($"{paths.Folders.Input_Dir}\\{itemName}\\OCR_PDF\\{itemName}_{page}.pdf");
            _ = PDFReader.LoadFile($"{dir}\\{fName}.pdf");
        }
        private void ClearInputFields()
        {
            rtbTitleTitle.Text = "";
            txtTitleFTitle.Clear();
            txtTitleID1.Clear();
            txtTitleID2.Clear();
            txtTitleID3.Clear();
            txtTitleID4.Clear();
            cmbTitleIDType1.Text = "";
            cmbTitleIDType2.Text = "";
            cmbTitleIDType3.Text = "";
            cmbTitleIDType4.Text = "";
            //cmbTitleLang.Items.Clear();
            txtTitleMAbs.Clear();
            txtTitlePRange.Clear();
            txtTitleURL.Clear();
            txtTitleURLDate.Clear();

            rtbKeywords.Text = "";
            rtbAbstract.Text = "";

            //PDFReader.src = null;

        }
        private void SaveRtfAsXmlFile(String rtf, String Filename)
        {
            using (XmlTextWriter writer = new XmlTextWriter(Filename, System.Text.Encoding.UTF8))
            {
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("RichTextData");
                writer.WriteElementString("Rtf", rtf);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        #endregion CommonMethods 

        //private void rtbTitleTitle_Enter(object sender, EventArgs e)
        //{
        //    _lastFocusedControl = (RichTextBox)rtbTitleTitle;
        //}

        //private void rtbKeywords_Enter(object sender, EventArgs e)
        //{
        //    _lastFocusedControl = (RichTextBox)rtbKeywords;
        //}

        //private void rtbAbstract_Enter(object sender, EventArgs e)
        //{
        //    _lastFocusedControl = (RichTextBox)rtbAbstract;
        //}
        private Font GetCurrentFont()
        {
            Font currentFont;
            if (_lastFocusedControl.GetType().Name == "RichTextBox")
            {
                currentFont = ((RichTextBox)_lastFocusedControl).SelectionFont;
                if (currentFont == null)
                {
                    int length = ((RichTextBox)_lastFocusedControl).SelectionLength;
                    ((RichTextBox)_lastFocusedControl).SelectionLength = 0;
                    currentFont = ((RichTextBox)_lastFocusedControl).SelectionFont;
                    ((RichTextBox)_lastFocusedControl).SelectionLength = length;  
                }
            }
            else
            {
                currentFont = ((TextBox)_lastFocusedControl).Font;
                if (currentFont == null)
                {
                    int length = ((TextBox)_lastFocusedControl).SelectionLength;
                    ((TextBox)_lastFocusedControl).SelectionLength = 0;
                    currentFont = ((TextBox)_lastFocusedControl).Font;
                    ((TextBox)_lastFocusedControl).SelectionLength = length;  
                }
            }            
            return currentFont;
        }

        private void tsbInsertSymbol_Click(object sender, EventArgs e)
        {
            using(InsertSymbolForm insertSymbolForm=new InsertSymbolForm(GetCurrentFont()))
            {
                if (insertSymbolForm.ShowDialog() == DialogResult.OK)
                {
                    if (_lastFocusedControl.GetType().Name == "RichTextBox")
                    {
                        ((RichTextBox)_lastFocusedControl).SelectionFont = insertSymbolForm.SymbolFont;
                        ((RichTextBox)_lastFocusedControl).SelectedText = insertSymbolForm.SymbolCharacter;
                    }
                    else
                    {
                        ((TextBox)_lastFocusedControl).Font = insertSymbolForm.SymbolFont;
                        ((TextBox)_lastFocusedControl).SelectedText = insertSymbolForm.SymbolCharacter;
                    }
                }
            }
        }

        
    }
}