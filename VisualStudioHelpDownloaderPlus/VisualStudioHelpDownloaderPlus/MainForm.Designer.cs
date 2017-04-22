using System.ComponentModel;
using System.Windows.Forms;

namespace VisualStudioHelpDownloaderPlus
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label _labelDirectory;
            System.Windows.Forms.Label _labelVS;
            System.Windows.Forms.Label _labelFilter;
            System.Windows.Forms.ColumnHeader bookName;
            System.Windows.Forms.ColumnHeader totalSize;
            System.Windows.Forms.ColumnHeader totalPackages;
            System.Windows.Forms.ColumnHeader downloadSize;
            System.Windows.Forms.ColumnHeader packagesToDownload;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.loadLanguages = new System.Windows.Forms.Button();
            this.loadBooks = new System.Windows.Forms.Button();
            this.cacheDirectory = new System.Windows.Forms.TextBox();
            this.browseDirectory = new System.Windows.Forms.Button();
            this.downloadProgress = new System.Windows.Forms.ProgressBar();
            this.startupTip = new System.Windows.Forms.Label();
            this.downloadBooks = new System.Windows.Forms.Button();
            this.loadingBooksTip = new System.Windows.Forms.Label();
            this.booksList = new System.Windows.Forms.ListView();
            this.VisualStudioSelection = new System.Windows.Forms.ComboBox();
            this.languageSelection = new System.Windows.Forms.ComboBox();
            this.loadLanguagesTip = new System.Windows.Forms.Label();
            _labelDirectory = new System.Windows.Forms.Label();
            _labelVS = new System.Windows.Forms.Label();
            _labelFilter = new System.Windows.Forms.Label();
            bookName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            totalSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            totalPackages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            downloadSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            packagesToDownload = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // _labelDirectory
            // 
            _labelDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _labelDirectory.AutoSize = true;
            _labelDirectory.Location = new System.Drawing.Point(12, 463);
            _labelDirectory.Name = "_labelDirectory";
            _labelDirectory.Size = new System.Drawing.Size(71, 15);
            _labelDirectory.TabIndex = 10;
            _labelDirectory.Text = "Store files in";
            // 
            // _labelVS
            // 
            _labelVS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _labelVS.AutoSize = true;
            _labelVS.Location = new System.Drawing.Point(12, 501);
            _labelVS.Name = "_labelVS";
            _labelVS.Size = new System.Drawing.Size(109, 15);
            _labelVS.TabIndex = 12;
            _labelVS.Text = "Select Visual Studio";
            // 
            // _labelFilter
            // 
            _labelFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _labelFilter.AutoSize = true;
            _labelFilter.Location = new System.Drawing.Point(378, 501);
            _labelFilter.Name = "_labelFilter";
            _labelFilter.Size = new System.Drawing.Size(93, 15);
            _labelFilter.TabIndex = 13;
            _labelFilter.Text = "Select Language";
            // 
            // bookName
            // 
            bookName.Text = "Book";
            bookName.Width = 400;
            // 
            // totalSize
            // 
            totalSize.Text = "Total PackageSizeBytes (MB)";
            totalSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            totalSize.Width = 93;
            // 
            // totalPackages
            // 
            totalPackages.Text = "# Packages";
            totalPackages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            totalPackages.Width = 73;
            // 
            // downloadSize
            // 
            downloadSize.Text = "Download PackageSizeBytes (MB)";
            downloadSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            downloadSize.Width = 119;
            // 
            // packagesToDownload
            // 
            packagesToDownload.Text = "Num Downloads";
            packagesToDownload.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            packagesToDownload.Width = 105;
            // 
            // loadLanguages
            // 
            this.loadLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadLanguages.Enabled = false;
            this.loadLanguages.Location = new System.Drawing.Point(253, 495);
            this.loadLanguages.Name = "loadLanguages";
            this.loadLanguages.Size = new System.Drawing.Size(108, 27);
            this.loadLanguages.TabIndex = 2;
            this.loadLanguages.Text = "Load Languages";
            this.loadLanguages.UseVisualStyleBackColor = true;
            this.loadLanguages.Click += new System.EventHandler(this.LoadLanguagesClick);
            // 
            // loadBooks
            // 
            this.loadBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadBooks.Enabled = false;
            this.loadBooks.Location = new System.Drawing.Point(592, 495);
            this.loadBooks.Name = "loadBooks";
            this.loadBooks.Size = new System.Drawing.Size(108, 27);
            this.loadBooks.TabIndex = 4;
            this.loadBooks.Text = "Load Books";
            this.loadBooks.UseVisualStyleBackColor = true;
            this.loadBooks.Click += new System.EventHandler(this.LoadBooksClick);
            // 
            // cacheDirectory
            // 
            this.cacheDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cacheDirectory.Location = new System.Drawing.Point(122, 459);
            this.cacheDirectory.Name = "cacheDirectory";
            this.cacheDirectory.ReadOnly = true;
            this.cacheDirectory.Size = new System.Drawing.Size(666, 23);
            this.cacheDirectory.TabIndex = 11;
            // 
            // browseDirectory
            // 
            this.browseDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.browseDirectory.Enabled = false;
            this.browseDirectory.Image = ((System.Drawing.Image)(resources.GetObject("browseDirectory.Image")));
            this.browseDirectory.Location = new System.Drawing.Point(794, 457);
            this.browseDirectory.Name = "browseDirectory";
            this.browseDirectory.Size = new System.Drawing.Size(31, 27);
            this.browseDirectory.TabIndex = 0;
            this.browseDirectory.UseVisualStyleBackColor = true;
            this.browseDirectory.Click += new System.EventHandler(this.BrowseDirectoryClick);
            // 
            // downloadProgress
            // 
            this.downloadProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadProgress.Location = new System.Drawing.Point(12, 530);
            this.downloadProgress.MarqueeAnimationSpeed = 25;
            this.downloadProgress.Name = "downloadProgress";
            this.downloadProgress.Size = new System.Drawing.Size(813, 11);
            this.downloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.downloadProgress.TabIndex = 14;
            // 
            // startupTip
            // 
            this.startupTip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.startupTip.AutoSize = true;
            this.startupTip.BackColor = System.Drawing.SystemColors.Window;
            this.startupTip.Location = new System.Drawing.Point(138, 214);
            this.startupTip.Name = "startupTip";
            this.startupTip.Size = new System.Drawing.Size(521, 15);
            this.startupTip.TabIndex = 7;
            this.startupTip.Text = "Select your Visual Studio version, then press \"Load Languages\" to  retrieve the a" +
    "vailable languages";
            this.startupTip.Visible = false;
            // 
            // downloadBooks
            // 
            this.downloadBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBooks.Enabled = false;
            this.downloadBooks.Location = new System.Drawing.Point(717, 495);
            this.downloadBooks.Name = "downloadBooks";
            this.downloadBooks.Size = new System.Drawing.Size(108, 27);
            this.downloadBooks.TabIndex = 6;
            this.downloadBooks.Text = "Download";
            this.downloadBooks.UseVisualStyleBackColor = true;
            this.downloadBooks.Click += new System.EventHandler(this.DownloadBooksClick);
            // 
            // loadingBooksTip
            // 
            this.loadingBooksTip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingBooksTip.AutoSize = true;
            this.loadingBooksTip.BackColor = System.Drawing.SystemColors.Window;
            this.loadingBooksTip.Location = new System.Drawing.Point(324, 213);
            this.loadingBooksTip.Name = "loadingBooksTip";
            this.loadingBooksTip.Size = new System.Drawing.Size(148, 15);
            this.loadingBooksTip.TabIndex = 9;
            this.loadingBooksTip.Text = "Downloading please wait...";
            // 
            // booksList
            // 
            this.booksList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.booksList.CheckBoxes = true;
            this.booksList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            bookName,
            totalSize,
            totalPackages,
            downloadSize,
            packagesToDownload});
            this.booksList.FullRowSelect = true;
            this.booksList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.booksList.HoverSelection = true;
            this.booksList.Location = new System.Drawing.Point(12, 13);
            this.booksList.Name = "booksList";
            this.booksList.ShowItemToolTips = true;
            this.booksList.Size = new System.Drawing.Size(813, 432);
            this.booksList.TabIndex = 5;
            this.booksList.UseCompatibleStateImageBehavior = false;
            this.booksList.View = System.Windows.Forms.View.Details;
            this.booksList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.BooksListItemChecked);
            // 
            // VisualStudioSelection
            // 
            this.VisualStudioSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.VisualStudioSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VisualStudioSelection.Enabled = false;
            this.VisualStudioSelection.FormattingEnabled = true;
            this.VisualStudioSelection.Location = new System.Drawing.Point(122, 497);
            this.VisualStudioSelection.Name = "VisualStudioSelection";
            this.VisualStudioSelection.Size = new System.Drawing.Size(125, 23);
            this.VisualStudioSelection.Sorted = true;
            this.VisualStudioSelection.TabIndex = 1;
            // 
            // languageSelection
            // 
            this.languageSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.languageSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageSelection.Enabled = false;
            this.languageSelection.FormattingEnabled = true;
            this.languageSelection.Location = new System.Drawing.Point(477, 497);
            this.languageSelection.Name = "languageSelection";
            this.languageSelection.Size = new System.Drawing.Size(108, 23);
            this.languageSelection.Sorted = true;
            this.languageSelection.TabIndex = 3;
            // 
            // loadLanguagesTip
            // 
            this.loadLanguagesTip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadLanguagesTip.AutoSize = true;
            this.loadLanguagesTip.BackColor = System.Drawing.SystemColors.Window;
            this.loadLanguagesTip.Location = new System.Drawing.Point(193, 214);
            this.loadLanguagesTip.Name = "loadLanguagesTip";
            this.loadLanguagesTip.Size = new System.Drawing.Size(411, 15);
            this.loadLanguagesTip.TabIndex = 8;
            this.loadLanguagesTip.Text = "Select your language then press \"Load Books\" to  retrieve the available books";
            this.loadLanguagesTip.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 553);
            this.Controls.Add(this.VisualStudioSelection);
            this.Controls.Add(_labelVS);
            this.Controls.Add(this.languageSelection);
            this.Controls.Add(_labelFilter);
            this.Controls.Add(_labelDirectory);
            this.Controls.Add(this.loadLanguagesTip);
            this.Controls.Add(this.loadingBooksTip);
            this.Controls.Add(this.startupTip);
            this.Controls.Add(this.downloadProgress);
            this.Controls.Add(this.browseDirectory);
            this.Controls.Add(this.cacheDirectory);
            this.Controls.Add(this.downloadBooks);
            this.Controls.Add(this.loadBooks);
            this.Controls.Add(this.loadLanguages);
            this.Controls.Add(this.booksList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(861, 490);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "%TITLE%";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button loadBooks;
        private Button loadLanguages;
        private TextBox cacheDirectory;
        private Button browseDirectory;
        private ProgressBar downloadProgress;
        private Label startupTip;
        private Button downloadBooks;
        private Label loadLanguagesTip;
        private Label loadingBooksTip;
        private ListView booksList;
        private ComboBox VisualStudioSelection;
        private ComboBox languageSelection;
    }
}

