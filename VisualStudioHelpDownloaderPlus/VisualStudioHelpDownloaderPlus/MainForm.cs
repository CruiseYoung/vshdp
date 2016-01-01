namespace VisualStudioHelpDownloaderPlus
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Threading.Tasks;
	using System.Windows.Forms;
    using System.Diagnostics;
	/// <summary>
	///     Main application form.
	/// </summary>
	internal sealed partial class MainForm : Form, IProgress<int>
	{
		/// <summary>
		/// The products.
		/// </summary>
		private ICollection<BookGroup> products;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();

			Text = Application.ProductName;
			products = new List<BookGroup>();
            //startupTip.Visible = false;
            cacheDirectory.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "HelpLibrary");
            //cacheDirectory.Text = Path.Combine(@"F:\Visual Studio\HelpLibrary"); // DEBUG
        }

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">
        /// The value of the updated progress. (percentage complete)
        /// </param>
        public void Report( int value )
		{
			Invoke(
				new MethodInvoker(
					delegate
						{
							downloadProgress.Value = value;
						} ) );
		}

		/// <summary>
		/// Called when the form is loaded. Start retrieving the list of available
		/// languages in the background.
		/// </summary>
		/// <param name="e">
		/// The parameter is not used.
		/// </param>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
            //loadingBooksTip.Visible = false;
            //startupTip.Visible = true;
			UpdateVisualStudioSelection();
		}
		
		/// <summary>
        /// Called to update the available locales for the selected version of visual studio
        /// </summary>

        private void UpdateVisualStudioSelection()
        {
            //loadingBooksTip.Visible = true;
            //startupTip.Visible = false;
            //languageSelection.Items.Clear();
            VisualStudioSelection.Items.Clear();

            browseDirectory.Enabled = false;
			downloadProgress.Style = ProgressBarStyle.Marquee;
            Task.Factory.StartNew(
                () =>
                {
                    using ( var downloader = new Downloader() )
                    {
                        return downloader.LoadAvailableCatalogs();
                    }
                }).ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.Faulted)
                    {
                        string message = string.Format(
                            CultureInfo.CurrentCulture,
                            "Load Catalogs - {0}",
                            t.Exception == null ? "Unknown error" : t.Exception.GetBaseException().Message );
                        MessageBox.Show(
                            message,
                            Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1,
                            0);
                        downloadProgress.Style = ProgressBarStyle.Continuous;
                    }
                    else
                    {
                        //VisualStudioSelection.DisplayMember = "DisplayName";
                        t.Result.ForEach( x => VisualStudioSelection.Items.Add( x ) );

                        ClearVisualStudioBusyState();
                        startupTip.Visible = true;
                        if (0 != VisualStudioSelection.Items.Count )
                        {
                            //VisualStudioSelection.SelectedIndex = 1; // DEBUG
                            VisualStudioSelection.SelectedIndex = VisualStudioSelection.Items.Count -1;
                        }
                    }
                    browseDirectory.Enabled = true;
                },
                TaskScheduler.FromCurrentSynchronizationContext() );
		}

        /// <summary>
        /// Called when the load books button is clicked. Load the list of available books for the selected
        /// language
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The parameter is not used.
        /// </param>
        private void LoadLanguagesClick(object sender, EventArgs e)
        {
            var nameCatalog = VisualStudioSelection.SelectedItem as Catalog;
            if (nameCatalog == null)
                return;

            SetVisualStudioBusyState();
            downloadProgress.Style = ProgressBarStyle.Marquee;
            startupTip.Visible = false;
            loadLanguagesTip.Visible = false;
            loadingBooksTip.Visible = true;
            browseDirectory.Enabled = false;
            languageSelection.Enabled = false;
            loadBooks.Enabled = false;

            booksList.Items.Clear();
            languageSelection.Items.Clear();

            Task.Factory.StartNew(
                () =>
                {
                    using (var downloader = new Downloader())
                    {
                        return downloader.LoadAvailableLocales(nameCatalog);
                    }
                }).ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.Faulted)
                    {
                        string message = string.Format(
                            CultureInfo.CurrentCulture,
                            "Load Catalogs - {0}",
                            t.Exception == null ? "Unknown error" : t.Exception.GetBaseException().Message);
                        MessageBox.Show(
                            message,
                            Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1,
                            0);
                    }
                    else
                    {
                        languageSelection.DisplayMember = "Locale";
                        t.Result.ForEach(x => languageSelection.Items.Add(x));

                        startupTip.Visible = true;
                        if( 0 != languageSelection.Items.Count )
                        {
                            int i = 0;
                            languageSelection.SelectedIndex = 0;
                            foreach (CatalogLocale loc in languageSelection.Items)
                            {
                                if ("en-us" == loc.Locale)
                                {
                                    languageSelection.SelectedIndex = i;
                                    //break; // DEBUG
                                }

                                //if ("zh-cn" == loc.Locale) // DEBUG
                                //{
                                //    languageSelection.SelectedIndex = i;
                                //    //break;
                                //}
                                ++i;
                            }
                        }
                        ClearBusyState();
                        loadLanguagesTip.Visible = true;
                    }
                    ClearVisualStudioBusyState();  
                    languageSelection.Enabled = true;
                    loadBooks.Enabled = true;
                },
                TaskScheduler.FromCurrentSynchronizationContext() );
        }

		/// <summary>
		/// Called when the load books button is clicked. Load the list of available books for the selected
		/// language
		/// </summary>
		/// <param name="sender">
		/// The parameter is not used.
		/// </param>
		/// <param name="e">
		/// The parameter is not used.
		/// </param>
		private void LoadBooksClick( object sender, EventArgs e )
		{
            var catalog = VisualStudioSelection.SelectedItem as Catalog;
            var catalogLocale = languageSelection.SelectedItem as CatalogLocale;
            if (catalog == null || catalogLocale == null)
                return;

            SetVisualStudioBusyState();
			SetBusyState();
			downloadProgress.Style = ProgressBarStyle.Marquee;
            startupTip.Visible = false;
            loadLanguagesTip.Visible = false;
            loadingBooksTip.Visible = true;

            booksList.Items.Clear();

			Task.Factory.StartNew(
				() =>
					{
						using ( var downloader = new Downloader( ) )
						{
                            return downloader.LoadBooksInformation(catalog, catalogLocale);
						}
					} ).ContinueWith(
						t =>
							{
								if ( t.Status == TaskStatus.Faulted )
								{
									string message = string.Format(
										CultureInfo.CurrentCulture,
										"Failed to retrieve book information - {0}",
										t.Exception == null ? "Unknown error" : t.Exception.GetBaseException().Message );
									MessageBox.Show(
										message,
										Application.ProductName,
										MessageBoxButtons.OK,
										MessageBoxIcon.Error,
										MessageBoxDefaultButton.Button1,
										0 );
								}
								else
								{
                                    products = t.Result;
									DisplayBooks();
								}

                                ClearVisualStudioBusyState();
								ClearBusyState();
							}, 
						TaskScheduler.FromCurrentSynchronizationContext() );
		}

		/// <summary>
		/// Called when the download books button is clicked. Start downloading in a background thread
		/// </summary>
		/// <param name="sender">
		/// The parameter is not used.
		/// </param>
		/// <param name="e">
		/// The parameter is not used.
		/// </param>
		private void DownloadBooksClick( object sender, EventArgs e )
		{
            var catalog = VisualStudioSelection.SelectedItem as Catalog;
            var catalogLocale = languageSelection.SelectedItem as CatalogLocale;
            if (catalog == null || catalogLocale == null)
                return;

            SetVisualStudioBusyState();
			SetBusyState();

            loadLanguagesTip.Visible = false;
            loadingBooksTip.Visible = false;
            startupTip.Visible = false;

            downloadProgress.Style = ProgressBarStyle.Continuous;
            downloadProgress.Value = 0;
			Task.Factory.StartNew(
				() =>
					{
						using ( Downloader downloader = new Downloader( ) )
						{
                            downloader.DownloadBooks(products, cacheDirectory.Text, catalog, catalogLocale, this);
						}
					} )
			.ContinueWith(
						t =>
							{
								if ( t.Status == TaskStatus.Faulted )
								{
									string message = string.Format(
										CultureInfo.CurrentCulture,
										"Download failed - {0}",
										t.Exception == null ? "Unknown error" : t.Exception.GetBaseException().Message );
									MessageBox.Show(
										message,
										Application.ProductName,
										MessageBoxButtons.OK,
										MessageBoxIcon.Error,
										MessageBoxDefaultButton.Button1,
										0 );
								}
								else
								{
									MessageBox.Show(
										"Download completed successfully",
										Application.ProductName,
										MessageBoxButtons.OK,
										MessageBoxIcon.Information,
										MessageBoxDefaultButton.Button1,
										0 );
								}

                                ClearVisualStudioBusyState();
								ClearBusyState();
								DisplayBooks();
                                downloadProgress.Value = 0;
							}, 
						TaskScheduler.FromCurrentSynchronizationContext() );
		}

		/// <summary>
		/// Enable/disable, hide/show controls for when the program is not busy 
		/// </summary>
		private void ClearBusyState()
		{
			languageSelection.Enabled = true;
			loadBooks.Enabled = true;
			downloadBooks.Enabled = (booksList.Items.Count > 0) && !string.IsNullOrEmpty( cacheDirectory.Text );
            //browseDirectory.Enabled = true;
			downloadProgress.Style = ProgressBarStyle.Continuous;
            loadLanguagesTip.Visible = false;
			loadingBooksTip.Visible = false;
		}

		/// <summary>
		/// Enable/disable, hide/show controls for when the program is busy 
		/// </summary>
		private void SetBusyState()
		{
			languageSelection.Enabled = false;
			loadBooks.Enabled = false;
			downloadBooks.Enabled = false;
            //browseDirectory.Enabled = false;
		}

        /// <summary>
        /// Enable/disable, hide/show controls for when the program is not busy 
        /// </summary>
        private void ClearVisualStudioBusyState()
        {
            VisualStudioSelection.Enabled = true;
            loadLanguages.Enabled = true;
            browseDirectory.Enabled = true;
            startupTip.Visible = false;
            loadingBooksTip.Visible = false;
            downloadProgress.Style = ProgressBarStyle.Continuous;
        }

        /// <summary>
        /// Enable/disable, hide/show controls for when the program is busy 
        /// </summary>
        private void SetVisualStudioBusyState()
        {
            VisualStudioSelection.Enabled = false;
            loadLanguages.Enabled = false;
            browseDirectory.Enabled = false;
        }

		/// <summary>
		/// Populate the list view control with the books available for download
		/// </summary>
		private void DisplayBooks()
		{
            var catalog = VisualStudioSelection.SelectedItem as Catalog;
            var catalogLocale = languageSelection.SelectedItem as CatalogLocale;
            if (catalog == null || catalogLocale == null)
                return;

            booksList.Items.Clear();
            // unused
			if ( !string.IsNullOrEmpty( cacheDirectory.Text ) )
			{
                Downloader.CheckPackagesStates( products, cacheDirectory.Text, catalog, catalogLocale);
			}

			Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
			foreach ( var product in products )
			{
				foreach ( var book in product.Books )
				{
					// Calculate some details about any prospective download
					long totalSize = 0;
					long downloadSize = 0;
					int packagesOutOfDate = 0;
					int packagesCached = 0;
					foreach ( var package in book.Packages )
					{
						totalSize += package.PackageSizeBytes;
						
                        //if ( package.State == PackageState.NotDownloaded )
                        if ( package.State != PackageState.Ready )
                        {
                            downloadSize += package.PackageSizeBytes;
                            ++packagesOutOfDate;
                        }

                        if (package.State != PackageState.NotDownloaded)
                        {
                            ++packagesCached;
                        }
                    }

                    // Make sure the groups aren't duplicated
                    string category;
                    if (catalog.Name == "dev10")
                        category = product.Name;
                    else
                        category = book.Category;

                    ListViewGroup itemGroup;
                    if (groups.ContainsKey(category))
                    {
                        itemGroup = groups[category];
                    }
                    else
                    {
                        itemGroup = booksList.Groups.Add(category, category);
                        groups.Add(category, itemGroup);
                    }

                    ListViewItem item = booksList.Items.Add( book.Name );
					item.SubItems.Add( ( totalSize / ( 1024*1024.0 ) ).ToString( "F2", CultureInfo.CurrentCulture ) );
					item.SubItems.Add( book.Packages.Count.ToString( CultureInfo.CurrentCulture ) );
					item.SubItems.Add( ( downloadSize / ( 1024*1024.0 ) ).ToString( "F2", CultureInfo.CurrentCulture ) );
					item.SubItems.Add( packagesOutOfDate.ToString( CultureInfo.CurrentCulture ) );
					item.ToolTipText = book.Description;
					item.Checked = packagesCached > 0;
					book.Wanted = item.Checked;
					item.Tag = book;
					item.Group = itemGroup;
				}
			}
		}

		/// <summary>
		/// Called when the browse for directory button is clicked. Show an folder browser to allow the
		/// user to select a directory to store the cached file in
		/// </summary>
		/// <param name="sender">
		/// The parameter is not used.
		/// </param>
		/// <param name="e">
		/// The parameter is not used.
		/// </param>
		private void BrowseDirectoryClick( object sender, EventArgs e )
		{
			using ( FolderBrowserDialog dialog = new FolderBrowserDialog() )
			{
				dialog.RootFolder = Environment.SpecialFolder.MyComputer;
				dialog.SelectedPath = cacheDirectory.Text;
				dialog.ShowNewFolderButton = true;
				dialog.Description = "Select local cache folder to store selected HelpLibrary books";

				if ( DialogResult.OK == dialog.ShowDialog( this ) )
				{
					cacheDirectory.Text = dialog.SelectedPath;
					downloadBooks.Enabled = ( booksList.Items.Count > 0 ) && !string.IsNullOrEmpty( cacheDirectory.Text );
                    if ( booksList.Items.Count > 0 )
					    DisplayBooks();
				}
			}
		}

		/// <summary>
		/// Called when the checkbox of one of the listview items is checked or unchecked. Mark the associated book state
		/// </summary>
		/// <param name="sender">
		/// The parameter is not used.
		/// </param>
		/// <param name="e">
		/// Details about the item checked/unchecked
		/// </param>
		private void BooksListItemChecked( object sender, ItemCheckedEventArgs e )
		{
			Book book = e.Item.Tag as Book;
			if ( book != null )
			{
				book.Wanted = e.Item.Checked;
			}
		}

		/// <summary>
		/// Called when the language combobox selection is changed. Clear the
		/// currently list of available books and reshow the instruction.
		/// </summary>
		/// <param name="sender">
		/// The parameter is not used.
		/// </param>
		/// <param name="e">
		/// The parameter is not used.
		/// </param>
		private void BookOptionsChanged( object sender, EventArgs e )
		{
			booksList.Items.Clear();
			downloadBooks.Enabled = false;
			startupTip.Visible = true;
        }

        /// <summary>
        /// Called when the visual studio language combobox selection is changed. Clear the
        /// currently list of available books and reshow the instruction.
        /// </summary>
        /// <param name="sender">
        /// The parameter is not used.
        /// </param>
        /// <param name="e">
        /// The parameter is not used.
        /// </param>
        private void VsVersionChanged(object sender, EventArgs e)
        {
            booksList.Items.Clear();
            languageSelection.Items.Clear();
            languageSelection.SelectedItem = -1;
            //UpdateVisualStudioSelection();
        }
    }
}
