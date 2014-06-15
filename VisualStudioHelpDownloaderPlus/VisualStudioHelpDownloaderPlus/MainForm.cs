namespace VisualStudioHelpDownloaderPlus
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Threading.Tasks;
	using System.Windows.Forms;

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
			cacheDirectory.Text = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ), "Downloads", "HelpLibrary" );
            //cacheDirectory.Text = Path.Combine( @"F:\Visual Studio 2012\HelpLibrary" );
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
            browseDirectory.Enabled = false;
			downloadProgress.Style = ProgressBarStyle.Marquee;
            Task.Factory.StartNew(
                () =>
                {
                    using ( Downloader downloader = new Downloader() )
                    {
                        return downloader.LoadAvailableCatalogs();
                    }
                })
            .ContinueWith(
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
                            }
                            else
                            {
                                VisualStudioSelection.DisplayMember = "DisplayName";
                                t.Result.ForEach( x => VisualStudioSelection.Items.Add( x ) );
                                ClearVisualStudioBusyState();
                                startupTip.Visible = true;
                                if (0 != VisualStudioSelection.Items.Count )
                                {
                                    //VisualStudioSelection.SelectedIndex = 1; // DEBUG
                                    VisualStudioSelection.SelectedIndex = 0; // DEBUG
                                }
                            }
                            browseDirectory.Enabled = true;
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
            //string nameCatalog = ( ( Catalog )VisualStudioSelection.SelectedItem).Name;
            //string codeLocale = ( ( Locale )languageSelection.SelectedItem ).Code;
            Catalog nameCatalog = ( Catalog )VisualStudioSelection.SelectedItem;
            Locale codeLocale = ( Locale )languageSelection.SelectedItem;

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
                            downloader.DownloadBooks(products, cacheDirectory.Text, nameCatalog, codeLocale, this);
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
            Catalog nameCatalog = ( Catalog )VisualStudioSelection.SelectedItem;

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
                    using ( Downloader downloader = new Downloader( ) )
                    {
                        return downloader.LoadAvailableLocales( nameCatalog );
                    }
                }).ContinueWith(
                        t =>
                        {
                            if ( t.Status == TaskStatus.Faulted )
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
                            }
                            else
                            {
                                languageSelection.DisplayMember = "Name";
                                t.Result.ForEach( x => languageSelection.Items.Add( x ) );
                                startupTip.Visible = true;
                                if( 0 != languageSelection.Items.Count )
                                {
                                    languageSelection.SelectedIndex = 2; // DEBUG
                                    //languageSelection.SelectedIndex = 12; // DEBUG
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
            Locale codeLocale = ( Locale )languageSelection.SelectedItem;

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
						using ( Downloader downloader = new Downloader( ) )
						{
                            return downloader.LoadBooksInformation( codeLocale );
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
            string nameCatalog = (( Catalog )VisualStudioSelection.SelectedItem ).Name;
            string codeLocale = ( ( Locale )languageSelection.SelectedItem ).Code;

            booksList.Items.Clear();
            // unused
			if ( !string.IsNullOrEmpty( cacheDirectory.Text ) )
			{
                Downloader.CheckPackagesStates( products, cacheDirectory.Text, nameCatalog, codeLocale );
			}

			Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
			foreach ( BookGroup product in products )
			{
				foreach ( Book book in product.Books )
				{
					// Calculate some details about any prospective download
					long totalSize = 0;
					long downloadSize = 0;
					int packagesOutOfDate = 0;
					int packagesCached = 0;
					foreach ( Package package in book.Packages )
					{
						totalSize += package.Size;
						
                        //if ( package.State == PackageState.NotDownloaded )
                        if ( package.State != PackageState.Ready )
                        {
                            downloadSize += package.Size;
                            packagesOutOfDate++;
                        }

                        if ( package.State != PackageState.NotDownloaded )
                        {
                            packagesCached++;
                        }
					}

					// Make sure the groups aren't duplicated
					ListViewGroup itemGroup;
					if ( groups.ContainsKey( book.Category ) )
					{
						itemGroup = groups[book.Category];
					}
					else
					{
						itemGroup = booksList.Groups.Add( book.Category, book.Category );
						groups.Add( book.Category, itemGroup );
					}

					ListViewItem item = booksList.Items.Add( book.Name );
					item.SubItems.Add( ( totalSize / ( 1024*1024.0 ) ).ToString( "F1", CultureInfo.CurrentCulture ) );
					item.SubItems.Add( book.Packages.Count.ToString( CultureInfo.CurrentCulture ) );
					item.SubItems.Add( ( downloadSize / ( 1024*1024.0 ) ).ToString( "F1", CultureInfo.CurrentCulture ) );
					item.SubItems.Add( packagesOutOfDate.ToString( CultureInfo.CurrentCulture ) );
					item.ToolTipText = book.Description;
					item.Checked = packagesCached > 1;
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
	}
}
