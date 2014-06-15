namespace VisualStudioHelpDownloaderPlus
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Linq;

	/// <summary>
	///     Class to perfom the downloading of the MSDN book information and the books themselves
	/// </summary>
	internal sealed class Downloader : IDisposable
	{
		/// <summary>
		/// The http client used for downloading
		/// </summary>
		private WebClient client = new WebClient();

		/// <summary>
		/// Initializes a new instance of the <see cref="Downloader"/> class.
		/// </summary>
		/// <exception cref="XmlException">
		/// If the settings cannot be loaded
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// If the data cannot be processed
		/// </exception>
		public Downloader( )
		{
            client.BaseAddress = @"http://services.mtps.microsoft.com/ServiceAPI/catalogs/";

			string directory = Path.GetDirectoryName( Application.ExecutablePath );
			if ( directory != null )
			{
				string settingsFile = Path.Combine(
					directory,
					string.Format( CultureInfo.InvariantCulture, "{0}.xml", Assembly.GetEntryAssembly().GetName().Name ) );

				if ( File.Exists( settingsFile ) )
				{
					XElement element = XDocument.Load( settingsFile ).Root;
					if ( element != null )
					{
						element = element.Elements().Single( x => x.Name.LocalName == "proxy" );
						WebProxy proxy = new WebProxy( element.Attributes().Single( x => x.Name.LocalName == "address" ).Value )
						{
							Credentials =
								new NetworkCredential(
								element.Attributes().Single( x => x.Name.LocalName == "login" ).Value,
								element.Attributes().Single( x => x.Name.LocalName == "password" ).Value,
								element.Attributes().Single( x => x.Name.LocalName == "domain" ).Value )
						};

						client.Proxy = proxy;
					}
					else
					{
						throw new XmlException( "Missing root element" );
					}
				}				
			}
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="Downloader"/> class. 
		/// </summary>
		~Downloader()
		{
			Dispose( false );
		}

		/// <summary>
		/// Check the current caching status of the packages so that the required downloads can be
		/// determined
		/// </summary>
		/// <param name="bookGroups">
		/// The collection of bookGroups to check the packages for
		/// </param>
		/// <param name="cachePath">
		/// The directory where the packages are locally cached
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// If bookGroups or cachePath are null
		/// </exception>
        public static void CheckPackagesStates(ICollection<BookGroup> bookGroups, string cachePath, string nameCatalog, string codeLocale)// unused
		{
			if ( bookGroups == null )
			{
				throw new ArgumentNullException( "bookGroups" );
			}

			if ( cachePath == null )
			{
				throw new ArgumentNullException( "cachePath" );
			}

            if ( nameCatalog == null )
			{
				throw new ArgumentNullException( "nameCatalog" );
			}

            if ( codeLocale == null )
			{
				throw new ArgumentNullException( "codeLocale" );
			}

            //if ( !Directory.Exists( cachePath ) )
            //    return;

            List<string> cheakCabDirectoryPath = new List<string>();
            cheakCabDirectoryPath.Add(cachePath);

            cheakCabDirectoryPath.Add(Path.Combine(cachePath, @"packages"));
            cheakCabDirectoryPath.Add(Path.Combine(cachePath, @"packages", codeLocale.ToLowerInvariant()));
            if ( !codeLocale.ToLowerInvariant().Contains( @"en-us" ) )
                cheakCabDirectoryPath.Add(Path.Combine(cachePath, @"packages", @"en-us"));

            string vsPath = Path.Combine(cachePath, nameCatalog);
            string packagePath = Path.Combine(vsPath, @"packages");
            string targetDirectory_Locale = Path.Combine(packagePath, codeLocale.ToLowerInvariant());
            string targetDirectory_en_us = Path.Combine(packagePath, "en-us");

            cheakCabDirectoryPath.Add(vsPath);
            cheakCabDirectoryPath.Add(packagePath);
            cheakCabDirectoryPath.Add(targetDirectory_Locale);
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us"))
                cheakCabDirectoryPath.Add(targetDirectory_en_us);

            List<string> cabDirectoryPath = new List<string>(); 
            foreach (string directoryPath in cheakCabDirectoryPath)
            {
                if (!Directory.Exists(directoryPath))
                    continue;

                foreach (string file in Directory.GetFiles( directoryPath, "*.cab"))
			    {
                    cabDirectoryPath.Add(directoryPath);
				    break;
			    }
            }

            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            if (!Directory.Exists(vsPath))
                Directory.CreateDirectory(vsPath);

            if (!Directory.Exists(packagePath))
                Directory.CreateDirectory(packagePath);

            if (!Directory.Exists(targetDirectory_Locale))
                Directory.CreateDirectory(targetDirectory_Locale);

            if (!codeLocale.ToLowerInvariant().Contains(@"en-us"))
                if (!Directory.Exists(targetDirectory_en_us))
                    Directory.CreateDirectory(targetDirectory_en_us);

            if (0 == cabDirectoryPath.Count)
                return;

			foreach ( BookGroup bookGroup in bookGroups )
			{
				foreach ( Book book in bookGroup.Books )
				{
					foreach ( Package package in book.Packages )
					{
                        string packagePathDest = packagePath;
                        if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                            packagePathDest = targetDirectory_en_us;
                        else if (package.Name.ToLowerInvariant().Contains(codeLocale.ToLowerInvariant()))
                            packagePathDest = targetDirectory_Locale;

                        packagePathDest = Path.Combine(packagePathDest, package.Name);
                        packagePathDest = string.Format(CultureInfo.InvariantCulture, "{0}({1})", packagePathDest, package.Tag);
                        packagePathDest = Path.ChangeExtension(packagePathDest, ".cab");

                        FileInfo packageFileDest;

                        foreach (string directoryPath in cheakCabDirectoryPath)
                        {
                            string packagePathSrc = Path.Combine(directoryPath, package.Name);
                            packagePathSrc = string.Format(CultureInfo.InvariantCulture, "{0}({1})", packagePathSrc, package.Tag);
                            packagePathSrc = Path.ChangeExtension(packagePathSrc, ".cab");

                            FileInfo packageFileSrc = new FileInfo(packagePathSrc);
                            packageFileDest = new FileInfo(packagePathDest);

                            //if (packageFileSrc.Exists)
                            //    packageFileSrc.MoveTo(Path.Combine(packagePathDest));

                            if (packagePathSrc != packagePathDest)
                                if (packageFileSrc.Exists)
                                    if (!packageFileDest.Exists)
                                        packageFileSrc.MoveTo(Path.Combine(packagePathDest));
                                    else
                                        File.Delete(packagePathSrc);

                        }

                        packageFileDest = new FileInfo( packagePathDest );
                        if ( packageFileDest.Exists )
						{
                            //if ( packageFileDest.Length == new Downloader().FetchContentLength( package.Link ) )
                            //{
                                if ( packageFileDest.LastWriteTime == package.LastModified )
							    {
                                    package.State = PackageState.Ready;
							    }
							    else
							    {
								    package.State = PackageState.OutOfDate;
							    }
                            //}
                            //else
                            //{
                            //    package.State = PackageState.NotDownloaded;
                            //    File.Delete( packagePath );
                            //}
						}
						else
						{
							package.State = PackageState.NotDownloaded;
						}
					}
				}
			}
		}

		/// <summary>
		/// The dispose.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Retrieves a collection of locales available to download the help for
		/// </summary>
		/// <returns>
		/// Collection of Locales available
		/// </returns>
		/// <exception cref="WebException">
		/// If the data cannot be downloaded
		/// </exception>
		/// <exception cref="XmlException">
		/// If the data cannot be processed
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// If the data cannot be processed
		/// </exception>
        public ICollection<Catalog> LoadAvailableCatalogs()
		{
            return HelpIndexManager.LoadCatalogs( client.DownloadData( "" ) );
		}

        /// <summary>
        /// Retrieves a collection of locales available to download the help for
        /// </summary>
        /// <returns>
        /// Collection of Locales available
        /// </returns>
        /// <exception cref="WebException">
        /// If the data cannot be downloaded
        /// </exception>
        /// <exception cref="XmlException">
        /// If the data cannot be processed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the data cannot be processed
        /// </exception>
        public ICollection<Locale> LoadAvailableLocales( Catalog nameCatalog )
        {

            if (nameCatalog == null)
            {
                throw new ArgumentNullException("nameCatalog");
            }

            return HelpIndexManager.LoadLocales( client.DownloadData( nameCatalog.Catalog_Link ) );
        }

		/// <summary>
		/// Download information about the available books for the selected locale
		/// </summary>
		/// <param name="path">
		/// The relative path to the book catalog download location
		/// </param>
		/// <returns>
		/// Collection of available bookGroups
		/// </returns>
		/// <exception cref="NullReferenceException">
		/// If path is null or empty
		/// </exception>
		/// <exception cref="WebException">
		/// If the data cannot be downloaded
		/// </exception>
		/// <exception cref="XmlException">
		/// If the data cannot be processed
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// If the data cannot be processed
		/// </exception>
        public ICollection<BookGroup> LoadBooksInformation( Locale codeLocale )
		{
            if (codeLocale == null)
            {
                throw new ArgumentNullException("codeLocale");
            }

            // 由于返回的Locale_Link的相对地址有问题，所以
            // client.BaseAddress = @"http://services.mtps.microsoft.com/ServiceAPI/catalogs/";
            // 不能改为
            // client.BaseAddress = @"http://services.mtps.microsoft.com/ServiceAPI/catalogs";
            return HelpIndexManager.LoadBooks( client.DownloadData( codeLocale.Locale_Link ) );
		}

		/// <summary>
		/// Download the requested books and create the appropriate index files for MSDN HelpViewer
		/// </summary>
		/// <param name="bookGroups">
		/// The collection of bookGroups to with the books to download indicated by the Book.Wanted
		/// property
		/// </param>
		/// <param name="cachePath">
		/// The path where the downloaded books are cached
		/// </param>
		/// <param name="progress">
		/// Interface used to report the percentage progress back to the GUI
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// If any of the parameters are null
		/// </exception>
		/// <exception cref="WebException">
		/// If the data cannot be downloaded
		/// </exception>
		/// <exception cref="XmlException">
		/// If the data cannot be processed
		/// </exception>
		/// <exception cref="IOException">
		/// If there was a problem reading or writing to the cache directory
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// If the user does not have permission to write to the cache directory
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// If the data cannot be processed
		/// </exception>
        public void DownloadBooks( ICollection<BookGroup> bookGroups, string cachePath, Catalog objCatalog, Locale objLocale, IProgress<int> progress )
		{
			if ( bookGroups == null )
			{
				throw new ArgumentNullException( "bookGroups" );
			}

			if ( cachePath == null )
			{
				throw new ArgumentNullException( "cachePath" );
			}

            if ( objCatalog == null)
            {
                throw new ArgumentNullException( "nameCatalog" );
            }

            if ( objLocale == null)
			{
				throw new ArgumentNullException( "codeLocale" );
			}

            string nameCatalog = objCatalog.Name;
            string codeLocale = objLocale.Code;

			//if ( progress == null )
			//{
			//	throw new ArgumentNullException( "progress" );
			//}

            DateTime lastModifiedTime = new DateTime( 2000, 1, 1, 0, 0, 0 );
            List<string> strLocales = new List<string>();
			bool include_en_us = false;
			// Create list of unique packages for possible download and write the book group and
			// book index files
			Dictionary<string, Package> packages = new Dictionary<string, Package>();
			foreach ( BookGroup bookGroup in bookGroups )
			{
                //File.WriteAllText(
                //    Path.Combine( cachePath, bookGroup.CreateFileName() ), 
                //    HelpIndexManager.CreateBookGroupBooksIndex( bookGroup ), 
                //    Encoding.UTF8 );
				Debug.Print( "BookGroup: {0}", bookGroup.Name );
				foreach ( Book book in bookGroup.Books )
				{
					if ( book.Wanted )
					{				
						Debug.Print( "   Book: {0}", book.Name );
                        //File.WriteAllText(
                        //    Path.Combine( cachePath, book.CreateFileName() ), 
                        //    HelpIndexManager.CreateBookPackagesIndex( bookGroup, book ), 
                        //    Encoding.UTF8 );
                        if ( !strLocales.Contains( book.Locale.Name ) )
                            strLocales.Add( book.Locale.Name );
													
						foreach ( Package package in book.Packages )
						{
                            string name = string.Format( CultureInfo.InvariantCulture, "{0}({1})", package.Name, package.Tag );
							Debug.Print( "      Package: {0}", name );

                            if ( !packages.ContainsKey( name.ToLowerInvariant() ) )
                            {
                                packages.Add( name.ToLowerInvariant(), package );
                                if ( package.LastModified > lastModifiedTime )
                                    lastModifiedTime = package.LastModified;
                            }
						}
					}
				}
			}

            if ( 0 == strLocales.Count )
				return;
            else if ( 1 == strLocales.Count )
                codeLocale = strLocales[0];
            else if ( 2 == strLocales.Count )
			{
                include_en_us = true;
                foreach ( string strLoc in strLocales )
                    if ( strLoc.ToLowerInvariant() != "en-us" )
                    {
                        codeLocale = strLoc;
                        break;
                    }
			}
				
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            string vsDirectory = Path.Combine(cachePath, nameCatalog);
            if (!Directory.Exists(vsDirectory))
                Directory.CreateDirectory(vsDirectory);

            // Generate Download File List
            try
            {
                string listFileName = string.Format( CultureInfo.InvariantCulture, "PackageList({0}).txt", codeLocale );
                string listFilePath = Path.Combine( cachePath, listFileName );

                if ( File.Exists( listFilePath ) )
                    File.Delete( listFilePath );

                listFilePath = Path.Combine( vsDirectory, listFileName );

                StreamWriter writer = new StreamWriter( listFilePath );
                foreach ( Package package in packages.Values )
                    writer.WriteLine( package.Link );
                writer.Close();

                FileLastModifiedTime( listFilePath, lastModifiedTime );
                //File.SetCreationTime( listFilePath, lastModifiedTime );
                //File.SetLastAccessTime( listFilePath, lastModifiedTime );
                //File.SetLastWriteTime( listFilePath, lastModifiedTime );
            }
            catch ( Exception e )
            {
                Program.LogException( e );
            }

            // Cleanup index files
			//Directory.GetFiles( cachePath, "*.msha" ).ForEach( File.Delete );
			Directory.GetFiles( cachePath, "*.xml" ).ForEach( File.Delete );	

			foreach ( string file in Directory.GetFiles( cachePath, "*.msha" ) )
			{
				string fileName = Path.GetFileNameWithoutExtension( file );
				if ( !string.IsNullOrEmpty( fileName ) )
				{
					if ( !fileName.Contains( @"HelpContentSetup" ) )
					{
						File.Delete( file );
					}
					
                    if ( fileName == @"HelpContentSetup(" + codeLocale + @")" )
                    {
                        File.Delete( file );
						//break;
                    }
				}
			}

            foreach ( string file in Directory.GetFiles( vsDirectory, "*.msha" ) )
            {
                string fileName = Path.GetFileNameWithoutExtension( file );
                if ( !string.IsNullOrEmpty( fileName ) )
                {
                    if ( !fileName.Contains( @"HelpContentSetup" ) )
                    {
                        File.Delete( file );
                    }

                    if ( fileName == @"HelpContentSetup( " + codeLocale + @")" )
                    {
                        File.Delete( file );
                        //break;
                    }
                }
            }

			// Creating setup indexes
			//File.WriteAllText(
            //Path.Combine( cachePath, "HelpContentSetup.msha" ), HelpIndexManager.CreateSetupIndex( bookGroups, nameCatalog, codeLocale ), Encoding.UTF8 );
	
			string xmlname = string.Format( CultureInfo.InvariantCulture, "HelpContentSetup({0}).msha", codeLocale );
            xmlname = Path.Combine( vsDirectory, xmlname );
            File.WriteAllText( xmlname, HelpIndexManager.CreateSetupIndex( bookGroups, nameCatalog, codeLocale ), Encoding.UTF8 );

            FileLastModifiedTime( xmlname, lastModifiedTime );
            //File.SetCreationTime( xmlname, lastModifiedTime );
            //File.SetLastAccessTime( xmlname, lastModifiedTime );
            //File.SetLastWriteTime( xmlname, lastModifiedTime );

            string targetDirectory = Path.Combine( vsDirectory, "packages" );
			string targetDirectory_Locale = Path.Combine( targetDirectory, codeLocale.ToLowerInvariant() );
			string targetDirectory_en_us = Path.Combine( targetDirectory, "en-us" );

			// Cleanup old files
            //Directory.GetFiles( targetDirectory, "*.cab" ).ForEach( File.Delete );
            CleanupOldPackages( packages, null, cachePath, false );

            CleanupOldPackages( packages, null, Path.Combine( cachePath, @"packages" ), false);
            CleanupOldPackages( packages, null, Path.Combine( cachePath, @"packages", codeLocale.ToLowerInvariant() ), false) ;
            if ( !codeLocale.ToLowerInvariant().Contains(@"en-us" ) && include_en_us )
                CleanupOldPackages( packages, null, Path.Combine( cachePath, @"packages", @"en-us" ), false );

            CleanupOldPackages( packages, null, vsDirectory, false );
            CleanupOldPackages( packages, null, targetDirectory, false );
            CleanupOldPackages( packages, codeLocale, targetDirectory_Locale, true );
            if ( !codeLocale.ToLowerInvariant().Contains( @"en-us" ) && include_en_us )
                CleanupOldPackages( packages, @"en-us", targetDirectory_en_us, true );

			// Create cachePath
			if ( !Directory.Exists( targetDirectory ) )
				Directory.CreateDirectory( targetDirectory );

			if ( !Directory.Exists( targetDirectory_Locale ) )
				Directory.CreateDirectory( targetDirectory_Locale );

            if ( include_en_us && !Directory.Exists( targetDirectory_en_us ) )
				Directory.CreateDirectory( targetDirectory_en_us );


			// Download the packages
			DateTime lastModifiedDir_Loc = new DateTime( 2000, 1, 1, 0, 0, 0 );
			DateTime lastModifiedDir_en_us = new DateTime( 2000, 1, 1, 0, 0, 0 );
			int packagesCountCurrent = 0;
			foreach ( Package package in packages.Values )
			{
				string destDirectory;
				if ( package.Name.ToLowerInvariant().Contains( codeLocale.ToLowerInvariant() ) )
				{
					destDirectory = targetDirectory_Locale;
					if ( package.LastModified > lastModifiedDir_Loc )
                        lastModifiedDir_Loc = package.LastModified;
				}
				else if ( package.Name.ToLowerInvariant().Contains( @"en-us" ) )
				{
					destDirectory = targetDirectory_en_us;
					if ( package.LastModified > lastModifiedDir_en_us )
                        lastModifiedDir_en_us = package.LastModified;
				}
				else 
					destDirectory = targetDirectory;

                string targetFileName = Path.Combine( destDirectory, package.CreateFileName() );

                // If file exist and file length is the same, skip it
				if ( package.State != PackageState.NotDownloaded)
                //if ( package.State == PackageState.OutOfDate )
				{
					if ( File.Exists( targetFileName ) )
					{
						if ( FetchContentLength( package.Link ) != new FileInfo( targetFileName ).Length )
						{
							package.State = PackageState.NotDownloaded;
							File.Delete( targetFileName );
						}
					}
				}

                if ( package.State == PackageState.NotDownloaded /*|| package.State == PackageState.OutOfDate*/ )
				{
					Debug.Print( "         Downloading : '{0}' to '{1}'", package.Link, targetFileName );
					client.DownloadFile( package.Link, targetFileName );
				}

                FileLastModifiedTime( targetFileName, package.LastModified );
                //File.SetCreationTime( targetFileName, package.LastModified );
                //File.SetLastAccessTime( targetFileName, package.LastModified );
                //File.SetLastWriteTime( targetFileName, package.LastModified );
				
				package.State = PackageState.Ready;
				progress.Report( 100 * ++packagesCountCurrent / packages.Count );
			}

            FileLastModifiedTime( targetDirectory_Locale, lastModifiedDir_Loc, true );
            if ( !codeLocale.ToLowerInvariant().Contains(@"en-us") && include_en_us )
                FileLastModifiedTime( targetDirectory_en_us, lastModifiedDir_en_us, true );
            FileLastModifiedTime( targetDirectory, lastModifiedTime, true );
            FileLastModifiedTime( vsDirectory, lastModifiedTime, true);
            FileLastModifiedTime( cachePath, lastModifiedTime, true );
		}

        private void CleanupOldPackages(Dictionary<string, Package> packages, string codeLocale, string cabDirectory, bool bTargetDirectory_Locale)
        {
            if ( !Directory.Exists( cabDirectory ) )
                return;

            foreach ( string file in Directory.GetFiles( cabDirectory, "*.cab" ) )
            {
                string fileName = Path.GetFileNameWithoutExtension( file );
                if ( !string.IsNullOrEmpty( fileName ) )
                {
                    if ( bTargetDirectory_Locale )
                    {
                        if ( !packages.ContainsKey( fileName.ToLowerInvariant() )
                            || !fileName.ToLowerInvariant().Contains( codeLocale.ToLowerInvariant() ) )
                        {
                            File.Delete( file );
                        }
                        else
                        {
                            string packagePathDest = Path.Combine( cabDirectory, packages[fileName.ToLowerInvariant()].CreateFileName() );
                            if ( file != packagePathDest /*&& !File.Exists( packagePathDest )*/ )
                            {
                                FileInfo oldFile = new FileInfo( file );
                                oldFile.MoveTo( packagePathDest );
                            }
                        }
                    }
                    else
                    {
                        foreach (Package package in packages.Values)
                        {
                            if (fileName.ToLowerInvariant().Contains(package.Name.ToLowerInvariant() + @"(")
                                || string.Compare(fileName.ToLowerInvariant(), package.Name.ToLowerInvariant(), true) == 0)
                            {
                                File.Delete(file);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private long FetchContentLength( string url )
        {
            if ( string.IsNullOrWhiteSpace( url ) )
                return 0;

            long result = 0;

            var request = (HttpWebRequest)HttpWebRequest.Create( url );

            var response = (HttpWebResponse)request.GetResponse();
            //var lastmodified = response.LastModified;

            result = response.ContentLength;
            response.Close();

            return result;
        }

        private void FileLastModifiedTime( string FilePath, DateTime lastModifiedTime, bool bDirectory = false )
        {
            if ( string.IsNullOrWhiteSpace( FilePath ) )
                return;

            DateTime lastModifiedTimeUtc = lastModifiedTime.ToUniversalTime();
            try
            {
                if ( bDirectory )
                {
                    Directory.SetCreationTime/*Utc*/( FilePath, lastModifiedTimeUtc );
                    Directory.SetLastAccessTime/*Utc*/( FilePath, lastModifiedTimeUtc );
                    Directory.SetLastWriteTime/*Utc*/( FilePath, lastModifiedTimeUtc );
                }
                else
                {
                    File.SetCreationTime/*Utc*/( FilePath, lastModifiedTimeUtc );
                    File.SetLastAccessTime/*Utc*/( FilePath, lastModifiedTimeUtc );
                    File.SetLastWriteTime/*Utc*/( FilePath, lastModifiedTimeUtc );
                }
            }
            catch (Exception e)
            {
                Program.LogException( e );
            }

        }

		/// <summary>
		/// Standard IDispose pattern
		/// </summary>
		/// <param name="disposing">
		/// true if called by Dispose, false if called from destructor
		/// </param>
		private void Dispose( bool disposing )
		{
			if ( disposing )
			{
				if ( client != null )
				{
					client.Dispose();
					client = null;
				}
			}
		}
	}
}
