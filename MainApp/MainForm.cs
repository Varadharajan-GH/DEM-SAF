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
using System.Xml.Linq;
using System.Xml.Serialization;
using tessnet2;

namespace MainApp
{
    public partial class MainForm : Form
    {
        #region Declaration

        private readonly StringBuilder sbLog;
        private Point mouseDown;
        private Point mouseUp;
        private readonly BackgroundWorker backgroundWorker;
        private Rectangle rect;
        private Control ctlTextBox;
        private string strText;
        delegate void SetTextCallback(string text);
        delegate void SpellCheckCallback(RichTextBox rtb);
        private Control _lastFocusedControl;
        private string wordToCheck;
        private string oldText;
        private string currentImage;
        private RectangleBox rBox = new RectangleBox();
        private readonly Paths paths = new Paths();
        SerializeDeserialize<ISSUE> serializeIssue;
        ISSUE deserializedIssues;
        private string currentItem;

        #endregion Declaration

        public MainForm()
        {
            sbLog = new StringBuilder();

            backgroundWorker = new BackgroundWorker();

            InitializeComponent();

            AdjustWindow();

            SetTextBoxEnterEvents(this);

            SetIDType();
            
            _lastFocusedControl = txtTitleTitle;

            currentItem = "BL7CX160A";

            ProcessXML(currentItem);

            txtTitleTitle.Focus();

            backgroundWorker.DoWork += ConvertImageToText;
            backgroundWorker.RunWorkerCompleted += BwRunWorkerCompleted;

            backgroundWorker.Dispose();
        }

        #region OCR

        public void ConvertImageToText(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            if (_lastFocusedControl != null)
            {
                ctlTextBox = _lastFocusedControl;
            }
            else
            {
                MessageBox.Show("Select a field to copy text");
                return;
            }
            
            Bitmap image = (Bitmap)pbeImage.Image; 
                        
            StringBuilder sbOcrText = new StringBuilder();
            using (Tesseract ocr = new Tesseract())
            {
                _ = ocr.Init(paths.Folders.Ocrdata_Dir,"eng", false);
                List<Word> result;
                if (rect.X < 0)
                {
                    rect.Width = rect.Width + rect.X;
                    rect.X = 0;
                }
                if (rect.Y < 0)
                {
                    rect.Height = rect.Height + rect.Y;
                    rect.Y = 0;
                }
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
                    tsStatus.Text = "Unable to Read. Try to Draw inside bounds";
                    return;
                }

                for (int i = 0; i < Tesseract.LineCount(result); i++)
                {
                    _ = sbOcrText.Append(Tesseract.GetLineText(result, i) + " ");
                }

                strText = sbOcrText.ToString();
                SetText(strText);
                SpellCheck(rtbAbstract);
            }
            
        }

        #endregion OCR

        #region SpellChecker

        private void SpellCheck(RichTextBox rtb)
        {
            AddLog("Enter");

            if (rtb.InvokeRequired)
            {
                AddLog("Invoke required");
                SpellCheckCallback spellCheckCallback = new SpellCheckCallback(SpellCheck);
                this.Invoke(spellCheckCallback, new object[] { rtb });
                AddLog("Invoked");
            }
            else
            {
                AddLog("Started");
                rtb.SuspendLayout();

                Font fontRTB = new Font(rtbAbstract.Font.FontFamily, rtbAbstract.Font.Size, FontStyle.Regular);

                using (Hunspell hunspell = new Hunspell(paths.Files.En_aff_file , paths.Files.En_dic_file))
                {
                    if (File.Exists(paths.Files.CustomWordsPath))
                    {
                        string[] lines = ReadAllLines(paths.Files.CustomWordsPath);
                        AddLog("Adding custom words");
                        foreach (string line in lines)
                        {
                            hunspell.Add(line);
                        }
                    }

                    AddLog("Checking spelling");
                    foreach (string word in rtb.Text.Split(' '))
                    {
                        string newWords = Regex.Replace(word, @"[^a-zA-Z]+", " ");
                        foreach(string newWord in newWords.Split(' '))
                        {
                            bool isCorrect = hunspell.Spell(newWord);
                            if (!isCorrect)
                            {
                                rtb.SelectionStart = rtb.Text.IndexOf(word) + word.IndexOf(newWord);
                                rtb.SelectionLength = newWord.Length;
                                rtb.SelectionFont = new Font(fontRTB.FontFamily, fontRTB.Size + 1, FontStyle.Underline);
                            }
                            else
                            {
                                rtb.SelectionStart = rtb.Text.IndexOf(word) + word.IndexOf(newWord);
                                rtb.SelectionLength = newWord.Length;
                                rtb.SelectionFont = new Font(fontRTB.FontFamily, fontRTB.Size, FontStyle.Regular);
                            }
                        }
                    }
                }
                fontRTB.Dispose();
            }

            rtb.ResumeLayout();
            AddLog("Exit");
        }
        private void SuggestionClicked(object sender, EventArgs e)
        {
            AddLog("SuggestionClicked: Enter");
            MenuItem miMenuItem = (MenuItem)sender;
            if (miMenuItem.Text == "More...")
            {
                AddLog("Calling ShowSpellChecker");
                ShowSpellChecker(wordToCheck);
            }
            else if (miMenuItem.Text == "Add to Dictionary")
            {
                AddLog("Calling AddWordToDictionary");
                AddWordToDictionary(wordToCheck);
            }
            else
            {
                oldText = rtbAbstract.Text;
                rtbAbstract.Text = rtbAbstract.Text.Replace(wordToCheck, miMenuItem.Text);
                AddLog("Calling SpellCheck");
                SpellCheck(rtbAbstract);
            }
            AddLog("SuggestionClicked: Exit");
        }
        private void AddWordToDictionary(string wordToCheck)
        {
            using (Hunspell hunspell = new Hunspell(paths.Files.En_aff_file, paths.Files.En_dic_file))
            {
                if (File.Exists(paths.Files.CustomWordsPath))
                {
                    string[] lines = ReadAllLines(paths.Files.CustomWordsPath);
                    foreach (string line in lines)
                    {
                        hunspell.Add(line);
                    }
                }
                bool isCorrect = hunspell.Spell(wordToCheck);
                if (!isCorrect)
                {
                    using (StreamWriter streamWriter = new StreamWriter(paths.Files.CustomWordsPath, true))
                    {
                        streamWriter.WriteLine(wordToCheck);
                    }
                }
            }            
            SpellCheck(rtbAbstract);
        }
        private void ShowSpellChecker(string word)
        {
            //ToDo
            MessageBox.Show("Yet to implement");
        }
        private string WordUnderMouse(RichTextBox rch, int x, int y)
        {
            AddLog("Enter");
            // Get the character's position.
            int pos = rch.GetCharIndexFromPosition(new Point(x, y));
            if (pos <= 0) return "";

            // Find the start of the word.
            string txt = rch.Text;

            int start_pos;
            for (start_pos = pos; start_pos >= 0; start_pos--)
            {
                // Allow digits, letters, and underscores
                // as part of the word.
                char ch = txt[start_pos];
                if (!char.IsLetterOrDigit(ch) && !(ch == '_')) break;
            }
            start_pos++;

            // Find the end of the word.
            int end_pos;
            for (end_pos = pos; end_pos < txt.Length; end_pos++)
            {
                char ch = txt[end_pos];
                if (!char.IsLetterOrDigit(ch) && !(ch == '_')) break;
            }
            end_pos--;

            // Return the result.
            AddLog("Returning");
            if (start_pos > end_pos) return "";
            return txt.Substring(start_pos, end_pos - start_pos + 1);
        }

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
                //rect = new Rectangle(mouseDown.X, mouseDown.Y, mouseUp.X - mouseDown.X, mouseUp.Y - mouseDown.Y);
                rBox.EndPoint = e.Location;
                
                //tsStatus.Text = $"MouseUp ({mouseDown.X} , {mouseDown.Y}) -> ({mouseUp.X} , {mouseUp.Y})";

                rBox = new RectangleBox();
                
                if (rect.Width > 0 && rect.Height > 0)
                {
                    tsProgress.Style = ProgressBarStyle.Marquee;
                    tsProgress.MarqueeAnimationSpeed = 30;
                    tsStatus.Text = "Capturing text please wait";
                    pbeImage.Invalidate();
                    backgroundWorker.RunWorkerAsync(); 
                }                
            }
        }
        private void PbeImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {                
                rBox.EndPoint=e.Location;
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
        private void RtbAbstract_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                using (Hunspell hunspell = new Hunspell(paths.Files.En_aff_file, paths.Files.En_dic_file))
                {
                    if (File.Exists(paths.Files.CustomWordsPath))
                    {
                        string[] lines = ReadAllLines(paths.Files.CustomWordsPath);
                        foreach (string line in lines)
                        {
                            hunspell.Add(line);
                        }
                    }
                    wordToCheck = WordUnderMouse(rtbAbstract, e.X, e.Y);
                    List<string> suggestions = hunspell.Suggest(wordToCheck);
                    ContextMenu contextMenu = new ContextMenu();
                    int sugCount = 0;
                    MenuItem menuItem;
                    foreach (string suggestion in suggestions)
                    {
                        menuItem = new MenuItem(suggestion);
                        contextMenu.MenuItems.Add(menuItem);
                        menuItem.Click += new EventHandler(SuggestionClicked);
                        sugCount++;
                        if (sugCount >= 10)
                        {
                            menuItem = new MenuItem("More...");
                            contextMenu.MenuItems.Add(menuItem);
                            menuItem.Click += new EventHandler(SuggestionClicked);
                            break;
                        }
                    }

                    menuItem = new MenuItem("Add to Dictionary");
                    contextMenu.MenuItems.Add(menuItem);
                    menuItem.Click += new EventHandler(SuggestionClicked);
                    rtbAbstract.ContextMenu = contextMenu;
                }
            }
        }        
        private void TcTagArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcTagArea.SelectedTab.Name == "tpAbstract")
            {
                rtbAbstract.Focus();
            }
            else if (tcTagArea.SelectedTab.Name == "tpKeywords")
            {
                txtKeywords.Focus();
            }
            else if (tcTagArea.SelectedTab.Name == "tpTitle")
            {
                txtTitleTitle.Focus();
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
            pbeImage.ImageLocation = currentImage;
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
            SpellCheck(rtbAbstract);
            //SpellCheck((RichTextBox)_lastFocusedControl);
        }
        private void TsbUndo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(oldText))
                return;
            if (MessageBox.Show("Do you want to restore abstract to its previous text?", "Confirm Undo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                rtbAbstract.Text = oldText;
                oldText = "";
                SpellCheck(rtbAbstract);
            }
        }

        #endregion ToolStripButtonClicks

        #region TabCOntrolEvents

        private void TpKeywords_Enter(object sender, EventArgs e)
        {
            txtKeywords.Focus();
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
            SaveValues(currentItem);
        }

        #endregion MenuButtonClicks


        #endregion AllEvents

        #region ToolStripButtonMethods
        private void ZoomTopLeft()
        {
            //Bitmap src = pbeImage.Image as Bitmap;
            Bitmap src = Image.FromFile(currentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, 0, (src.Width / 2) + 100, (src.Height / 2) + 100);
            
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
        }
        private void ZoomTopRight()
        {
            Bitmap src = Image.FromFile(currentImage) as Bitmap;
            Rectangle cropRect = new Rectangle((src.Width / 2) - 100, 0,
                                                src.Width / 2 + 100, src.Height / 2 + 100);
            
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
        }
        private void ZoomBottomLeft()
        {
            Bitmap src = Image.FromFile(currentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, (src.Height / 2) - 100,
                                                src.Width / 2 + 100, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
        }
        private void ZoomBottomRight()
        {
            Bitmap src = Image.FromFile(currentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(src.Width / 2 - 100, src.Height / 2 - 100,
                                                src.Width / 2 + 100, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
        }
        private void ZoomTopHalf()
        {
            Bitmap src = Image.FromFile(currentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, 0, src.Width, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
        }
        private void ZoomBottomHalf()
        {
            Bitmap src = Image.FromFile(currentImage) as Bitmap;
            Rectangle cropRect = new Rectangle(0, src.Height / 2 - 100, src.Width, src.Height / 2 + 100);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                pbeImage.Image = target;
            }
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
        private void LoadImage(string itemName,int seq)
        {
            AddLog($"Loading image {itemName}_{seq}.TIF");
            currentImage = $"{paths.Folders.Input_Dir}\\{itemName}\\{itemName}_{seq}.TIF";
            pbeImage.ImageLocation = currentImage;
        }
        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AddLog("Called");
            tsProgress.Style = ProgressBarStyle.Continuous;
            tsProgress.MarqueeAnimationSpeed = 0;
            tsStatus.Text = "Ready";
        }
        //[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        //[System.Runtime.CompilerServices.CallerLineNumber]  int sourceLineNumber = 0 )
        public void AddLog(string log, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {            
            sbLog.AppendLine($"{DateTime.Now.ToString()} : {memberName} : {log} ;");
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
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                List<string> file = new List<string>();
                while (!streamReader.EndOfStream)
                {
                    file.Add(streamReader.ReadLine());
                }
                return file.ToArray();
            }
        }
        private void SetText(string strText)
        {
            AddLog("Called");
            if (ctlTextBox.InvokeRequired)
            {
                SetTextCallback setTextCallback = new SetTextCallback(SetText);
                this.Invoke(setTextCallback, new object[] { strText });
                AddLog("Invoked");
            }
            else
            {
                ctlTextBox.Text = strText;
            }
        }
        private void SetTextBoxEnterEvents(Control control)
        {            
            if ((control is TextBox) || (control is RichTextBox))
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
        private void ProcessXML(string ItemName)
        {
            AddLog("Called");
            
            serializeIssue = new SerializeDeserialize<ISSUE>();

            string currentXML = $"{paths.Folders.Input_Dir}\\{ItemName}\\{ItemName}.XML";

            XmlDocument xmldocument = new XmlDocument();

            xmldocument.Load(currentXML);

            AddLog("Deserializing");
            deserializedIssues = serializeIssue.DeserializeData(xmldocument.OuterXml);
            
            ListViewItem listViewItem = new ListViewItem("1");
            listViewItem.SubItems.AddRange(new string[]
            {
                deserializedIssues.ID_ACCESSION, deserializedIssues.ITEM[0].ITEMNO,
                "A", deserializedIssues.ITEM[0].ITEM_CONTENT.DT_DOCUMENTTYPE.ToString(), "Progress", deserializedIssues.ITEM[0].ITEM_CONTENT.PG_PAGESPAN
            });
            lvItems.Items.Add(listViewItem);

            LoadValues(deserializedIssues);

            LoadImage(ItemName, Convert.ToInt32(deserializedIssues.ITEM[0].ITEM_CONTENT.PG_PAGESPAN.Split('-')[0]));

            
            //AddLog("Serializing");
            //string serializedIssues = serializeIssue.SerializeData(deserializedIssues);

            //Directory.CreateDirectory($"{paths.Folders.Output_Dir}\\{ItemName}\\");

            //xmldocument.LoadXml(serializedIssues);

            //using (StreamWriter streamWriter = new StreamWriter($"{paths.Folders.Output_Dir}\\{ItemName}\\{ItemName}.XML", false, Encoding.UTF8))
            //{
            //    AddLog($"Saving {((FileStream)streamWriter.BaseStream).Name}");
            //    //streamWriter.Write(serializedIssues);             //For UTF-16 encoding
            //    xmldocument.Save(streamWriter);                     //For UTF-8 encoding
            //}

        }
        private void LoadValues(ISSUE issue)
        {
            txtTitleTitle.Text = issue.ITEM[0].ITEM_CONTENT.TITLES.TI_TITLE;

            txtTitleILang.Text = issue.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].Value.ToString();

            txtTitlePRange.Text = issue.ITEM[0].ITEM_CONTENT.PG_PAGESPAN;

            txtKeywords.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            if (issue.ITEM[0].ITEM_CONTENT.KEYWORD != null)
            {
                foreach (KEYWORD keyword in issue.ITEM[0].ITEM_CONTENT.KEYWORD)
                {
                    stringBuilder.AppendLine(keyword.AUTHOR_KEYWORD);
                }
            }                
            txtKeywords.Text = stringBuilder.ToString();

            if(issue.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER != null)
            {
                foreach (AI_ARTICLEIDENTIFIER aid in issue.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER)
                {
                    Controls.Find($"txtTitleID{aid.seq}", true)[0].Text = aid.Value;
                    Controls.Find($"cmbTitleIDType{aid.seq}", true)[0].Text = aid.type.ToString();
                }
            }            

            rtbAbstract.Clear();
            rtbAbstract.Text = issue.ITEM[0].ITEM_CONTENT.ABSTRACT[0].Value;
        }
        private void SaveValues(string item)
        {
            XmlDocument xmldocument = new XmlDocument();

            deserializedIssues.ITEM[0].ITEM_CONTENT.TITLES.TI_TITLE = txtTitleTitle.Text;

            if (Enum.TryParse(txtTitleILang.Text, out LANGUAGE language))
            {
                deserializedIssues.ITEM[0].ITEM_CONTENT.LA_LANGUAGE[0].Value = language;
            }

            deserializedIssues.ITEM[0].ITEM_CONTENT.PG_PAGESPAN = txtTitlePRange.Text;

            var arrKwd = txtKeywords.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            deserializedIssues.ITEM[0].ITEM_CONTENT.KEYWORD = new KEYWORD[arrKwd.Length];
            for (int i = 0; i < arrKwd.Length; i++)
            {
                deserializedIssues.ITEM[0].ITEM_CONTENT.KEYWORD[i] = new KEYWORD
                {
                    seq = $"{i + 1}",
                    AUTHOR_KEYWORD = arrKwd[i]
                };
            }

            List<AI_ARTICLEIDENTIFIER> alAID = new List<AI_ARTICLEIDENTIFIER>();
            for (int i = 1, j=1; i <= 4; i++)
            {
                if (!string.IsNullOrEmpty(Controls.Find($"txtTitleID{i}", true)[0].Text))
                {
                    if (!string.IsNullOrEmpty(Controls.Find($"cmbTitleIDType{i}", true)[0].Text))
                    {
                        if (Enum.TryParse(Controls.Find($"cmbTitleIDType{i}", true)[0].Text, out IDENTIFIER_TYPE idType))
                        {
                            AI_ARTICLEIDENTIFIER aid = new AI_ARTICLEIDENTIFIER();
                            aid.seq = j.ToString();
                            aid.type = idType;
                            aid.Value = Controls.Find($"txtTitleID{i}", true)[0].Text;
                            alAID.Add(aid);
                            j++;
                        }
                    }
                }
            } 
            deserializedIssues.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER = new AI_ARTICLEIDENTIFIER[alAID.Count];
            deserializedIssues.ITEM[0].ITEM_CONTENT.AI_ARTICLEIDENTIFIER = alAID.ToArray();


            deserializedIssues.ITEM[0].ITEM_CONTENT.ABSTRACT[0].Value = rtbAbstract.Text;

            serializeIssue = new SerializeDeserialize<ISSUE>();

            string serializedIssues = serializeIssue.SerializeData(deserializedIssues);

            Directory.CreateDirectory($"{paths.Folders.Output_Dir}\\{item}\\");  

            xmldocument.LoadXml(serializedIssues);   

            using (StreamWriter streamWriter = new StreamWriter($"{paths.Folders.Output_Dir}\\{item}\\{item}.XML", false, Encoding.UTF8))
            {
                AddLog($"Saving {((FileStream)streamWriter.BaseStream).Name}");
                //streamWriter.Write(serializedIssues);             //For UTF-16 encoding
                xmldocument.Save(streamWriter);                     //For UTF-8 encoding
            }
            MessageBox.Show("Saved");
        }

        #endregion CommonMethods           

        
    }
}