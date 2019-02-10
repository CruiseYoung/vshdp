using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace VisualStudioHelpDownloaderPlus
{
    /// <summary>
    ///     Class to perfom the downloading of the MSDN book information and the books themselves
    /// </summary>
    internal sealed class Downloader : IDisposable
    {
        /// <summary>
        /// The http client used for downloading
        /// </summary>
        private WebClient _client = new WebClient();
        private readonly WebProxy _proxy;
        private const string BrandingPackageUrl = @"http://packages.mtps.microsoft.com/brands/";
        public const string BrandingPackageName1 = "dev10";
        public const string BrandingPackageName2 = "dev10-ie6";

        /// <summary>
        /// Initializes a new instance of the <see cref="Downloader"/> class.
        /// </summary>
        /// <exception cref="XmlException">
        /// If the settings cannot be loaded
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the data cannot be processed
        /// </exception>
        public Downloader()
        {
            _client.Encoding = Encoding.UTF8;
            _client.BaseAddress = @"http://services.mtps.microsoft.com/ServiceAPI/catalogs/";

            string directory = Path.GetDirectoryName(Application.ExecutablePath);
            if (directory != null)
            {
                string settingsFile = Path.Combine(
                    directory,
                    string.Format(CultureInfo.InvariantCulture, "{0}.xml", Assembly.GetEntryAssembly().GetName().Name));

                if (File.Exists(settingsFile))
                {
                    XElement element = XDocument.Load(settingsFile).Root;
                    if (element != null)
                    {
                        element = element.Elements().Single(x => x.Name.LocalName?.Equals("proxy", StringComparison.OrdinalIgnoreCase) ?? false);
                        //WebProxy
                        _proxy = new WebProxy(element.Attributes().Single(x => x.Name.LocalName?.Equals("address", StringComparison.OrdinalIgnoreCase) ?? false).Value)
                        {
                            Credentials =
                                new NetworkCredential(
                                element.Attributes().Single(x => x.Name.LocalName?.Equals("login", StringComparison.OrdinalIgnoreCase) ?? false).Value,
                                element.Attributes().Single(x => x.Name.LocalName?.Equals("password", StringComparison.OrdinalIgnoreCase) ?? false).Value,
                                element.Attributes().Single(x => x.Name.LocalName?.Equals("domain", StringComparison.OrdinalIgnoreCase) ?? false).Value)
                        };

                        /*
                        element = element.Elements().Single(x => x.Name.LocalName == "proxy");
                        //WebProxy
                        _proxy = new WebProxy(element.Attributes().Single(x => x.Name.LocalName == "address").Value);
                        if (element.Attributes().Any(x => x.Name.LocalName == "default" && (x.Value == "1" || x.Value == "true")))
                        {
                            _proxy.UseDefaultCredentials = true;
                            _proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                        }
                        else
                        {
                            _proxy.Credentials =
                                new NetworkCredential(
                                    element.Attributes().Single(x => x.Name.LocalName == "login").Value,
                                    element.Attributes().Single(x => x.Name.LocalName == "password").Value,
                                    element.Attributes().Single(x => x.Name.LocalName == "domain").Value);
                        }
                        */

                        _client.Proxy = _proxy;
                    }
                    else
                    {
                        throw new XmlException("Missing root element");
                    }
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Downloader"/> class. 
        /// </summary>
        ~Downloader()
        {
            Dispose(false);
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
        /// <param name="catalog"></param>
        /// <param name="catalogLocale"></param>
        /// <exception cref="ArgumentNullException">
        /// If bookGroups or cachePath are null
        /// </exception>
        //public static void CheckPackagesStates(ICollection<BookGroup> bookGroups, string cachePath, string nameCatalog, string codeLocale)// unused
        public static void CheckPackagesStates(ICollection<BookGroup> bookGroups, string cachePath, Catalog catalog, CatalogLocale catalogLocale)// unused
        {
            if (bookGroups == null)
            {
                throw new ArgumentNullException(nameof(bookGroups));
            }

            if (cachePath == null)
            {
                throw new ArgumentNullException(nameof(cachePath));
            }

            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (catalogLocale == null)
            {
                throw new ArgumentNullException(nameof(catalogLocale));
            }

            //if ( !Directory.Exists( cachePath ) )
            //    return;

            string nameCatalog = catalog.DisplayName;
            string codeLocale = catalogLocale.Locale;

            string vsPath = Path.Combine(cachePath, nameCatalog);
            string packagePath = Path.Combine(vsPath, @"packages");
            string targetDirectoryLocale = Path.Combine(packagePath, codeLocale.ToLowerInvariant());
            string targetDirectoryEnUs = Path.Combine(packagePath, "en-us");

            List<string> cheakCabDirectoryPath = new List<string>
            {
                cachePath,
                vsPath,
                packagePath,
                targetDirectoryLocale
            };
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us"))
                cheakCabDirectoryPath.Add(targetDirectoryEnUs);

            string oldVsDirName = catalog.Name;
            if (oldVsDirName == "dev10")
                oldVsDirName = "VisualStudio10";
            string oldvsPath = Path.Combine(cachePath, oldVsDirName);
            string oldpackagePath = Path.Combine(oldvsPath, @"packages");
            string oldtargetDirectoryLocale = Path.Combine(oldpackagePath, codeLocale.ToLowerInvariant());
            string oldtargetDirectoryEnUs = Path.Combine(oldpackagePath, "en-us");
            cheakCabDirectoryPath.Add(cachePath);
            cheakCabDirectoryPath.Add(oldvsPath);
            cheakCabDirectoryPath.Add(oldpackagePath);
            cheakCabDirectoryPath.Add(oldtargetDirectoryLocale);
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us"))
                cheakCabDirectoryPath.Add(oldtargetDirectoryEnUs);

            cheakCabDirectoryPath.Add(Path.Combine(cachePath, @"packages"));
            cheakCabDirectoryPath.Add(Path.Combine(cachePath, @"packages", codeLocale.ToLowerInvariant()));
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us"))
                cheakCabDirectoryPath.Add(Path.Combine(cachePath, @"packages", @"en-us"));


            List<string> cabDirectoryPath = new List<string>();
            foreach (string directoryPath in cheakCabDirectoryPath)
            {
                if (!Directory.Exists(directoryPath))
                    continue;

                if (Directory.GetFiles(directoryPath, "*.cab").Length > 0)
                    cabDirectoryPath.Add(directoryPath);
            }

            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            if (!Directory.Exists(vsPath))
                Directory.CreateDirectory(vsPath);

            if (!Directory.Exists(packagePath))
                Directory.CreateDirectory(packagePath);

            if (!Directory.Exists(targetDirectoryLocale))
                Directory.CreateDirectory(targetDirectoryLocale);

            if (!codeLocale.ToLowerInvariant().Contains(@"en-us"))
                if (!Directory.Exists(targetDirectoryEnUs))
                    Directory.CreateDirectory(targetDirectoryEnUs);

            if (0 == cabDirectoryPath.Count)
                return;

            foreach (BookGroup bookGroup in bookGroups)
            {
                foreach (Book book in bookGroup.Books)
                {
                    foreach (Package package in book.Packages)
                    {
                        string packagePathDest = packagePath;
                        if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                            packagePathDest = targetDirectoryEnUs;
                        else if (package.Name.ToLowerInvariant().Contains(codeLocale.ToLowerInvariant()))
                            packagePathDest = targetDirectoryLocale;

                        packagePathDest = Path.Combine(packagePathDest, package.CreateFileName());
                        FileInfo packageFileDest = new FileInfo(packagePathDest);

                        foreach (string directoryPath in cheakCabDirectoryPath)
                        {
                            string packagePathSrc = Path.Combine(directoryPath, package.CreateFileName());

                            FileInfo packageFileSrc = new FileInfo(packagePathSrc);
                            //packageFileDest = new FileInfo(packagePathDest);

                            //if (packageFileSrc.Exists)
                            //    packageFileSrc.MoveTo(Path.Combine(packagePathDest));

                            if (packagePathSrc != packagePathDest)
                                if (packageFileSrc.Exists)
                                    if (!packageFileDest.Exists)
                                        packageFileSrc.MoveTo(Path.Combine(packagePathDest));
                                    else
                                        File.Delete(packagePathSrc);

                        }

                        packageFileDest = new FileInfo(packagePathDest);
                        if (packageFileDest.Exists)
                        {
                            package.State = PackageState.Ready;
                            //if ( packageFileDest.Length == new Downloader().FetchContentLength( package.CurrentLink ) )
                            //{
                            //var downloader = new Downloader();
                            //if (packageFileDest.LastWriteTime != package.LastModified)
                            //{
                            //    package.LastModified = downloader.FetchLastModified(package.CurrentLink);
                            //}
                            //package.State = packageFileDest.LastWriteTime == package.LastModified ? PackageState.Ready : PackageState.OutOfDate;
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
            Dispose(true);
            GC.SuppressFinalize(this);
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
            ICollection<Catalog> result = HelpIndexManager.LoadCatalogs(_client.DownloadData(""));

            Catalog catalogVisualStudio10 = new Catalog
            {
                Name = "VisualStudio10",
                Description = "VisualStudio10",
                CatalogLink = string.Empty//"../catalogs/VisualStudio10"
            };
            Catalog catalogDev10 = new Catalog
            {
                Name = "dev10",
                Description = "dev10",
                CatalogLink = string.Empty//"../catalogs/dev10"
            };
            if (!result.Contains(catalogVisualStudio10) && !result.Contains(catalogDev10))
                result.Add(catalogDev10);

            var catalogs = result as List<Catalog>;
            catalogs?.Sort();
            return result;
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
        public ICollection<CatalogLocale> LoadAvailableLocales(Catalog nameCatalog)
        {

            if (nameCatalog == null)
            {
                throw new ArgumentNullException(nameof(nameCatalog));
            }

            var result = string.IsNullOrEmpty(nameCatalog.CatalogLink) ? new List<CatalogLocale>() : HelpIndexManager.LoadLocales(_client.DownloadData(nameCatalog.CatalogLink));

            //List<CatalogLocale> result = new List<CatalogLocale>();

            string[] names = {
                                "cs-cz", "de-de", "en-us", "es-es", "fr-fr",
                                "it-it", "ja-jp", "ko-kr", "pl-pl", "pt-br",
                                "ru-ru", "tr-tr", "zh-cn", "zh-tw"
                            };

            //if (result.Count < names.Length)
            {
                //result.Clear();
                foreach (string name in names)
                {
                    CatalogLocale cataloglocale = new CatalogLocale
                    {
                        Locale = name,
                        CatalogName = nameCatalog.Name,
                        Description = nameCatalog.Name,
                        LocaleLink = (!string.IsNullOrEmpty(nameCatalog.CatalogLink)
                                ? (nameCatalog.CatalogLink + @"/" + name)
                                : (@"../catalogs/" + nameCatalog.Name + @"/" + name))
                    };
                    if (!result.Contains(cataloglocale))
                        result.Add(cataloglocale);
                }
            }
            var catalogLocales = result as List<CatalogLocale>;
            catalogLocales?.Sort();
            return result;
        }

        /// <summary>
        /// Download information about the available books for the selected locale
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="catalogLocale"></param>
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
        public ICollection<BookGroup> LoadBooksInformation(Catalog catalog, CatalogLocale catalogLocale)
        {
            if (catalogLocale == null)
            {
                throw new ArgumentNullException(nameof(catalogLocale));
            }

            // 由于返回的Locale_Link的相对地址有问题，所以
            // client.BaseAddress = @"http://services.mtps.microsoft.com/ServiceAPI/catalogs/";
            // 不能改为
            // client.BaseAddress = @"http://services.mtps.microsoft.com/ServiceAPI/catalogs";
            return HelpIndexManager.LoadBooks(catalog, _client.DownloadData(catalogLocale.LocaleLink));
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
        /// <param name="objLocale"></param>
        /// <param name="progress">
        /// Interface used to report the percentage progress back to the GUI
        /// </param>
        /// <param name="objCatalog"></param>
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
        public void DownloadBooks(ICollection<BookGroup> bookGroups, string cachePath, Catalog objCatalog, CatalogLocale objLocale, IProgress<int> progress)
        {
            if (bookGroups == null)
            {
                throw new ArgumentNullException(nameof(bookGroups));
            }

            if (cachePath == null)
            {
                throw new ArgumentNullException(nameof(cachePath));
            }

            if (objCatalog == null)
            {
                throw new ArgumentNullException(nameof(objCatalog));
            }

            if (objLocale == null)
            {
                throw new ArgumentNullException(nameof(objLocale));
            }

            string nameCatalog = objCatalog.DisplayName;
            string codeLocale = objLocale.Locale;

            //if ( progress == null )
            //{
            //    throw new ArgumentNullException( "progress" );
            //}

            DateTime lastModifiedTimeCatalogLocale = new DateTime(2000, 1, 1, 0, 0, 0);
            List<string> strLocales = new List<string>();
            // Create list of unique packages for possible download and write the book group and
            // book index files
            Dictionary<string, Package> packages = new Dictionary<string, Package>();
            if (objCatalog.Name == "dev10")
            {
                var lastModifiedTime = new DateTime(2000, 1, 1, 0, 0, 0);

                // Add branding packages
                var brandingPackgeName1 = new Package
                {
                    Name = BrandingPackageName1,
                    Deployed = @"true",
                    LastModified = DateTime.Now,
                    PackageEtag = null,
                    CurrentLink = string.Format(CultureInfo.InvariantCulture, "{0}{1}.cab", BrandingPackageUrl, BrandingPackageName1),
                    CurrentLinkContext = string.Format(CultureInfo.InvariantCulture, "{0}.cab", BrandingPackageName1),
                    PackageSizeBytes = 0,
                    PackageSizeBytesUncompressed = 0,
                    PackageConstituentLink = string.Format(CultureInfo.InvariantCulture, "{0}{1}", @"../../serviceapi/packages/brands/", BrandingPackageName1)
                };
                brandingPackgeName1.LastModified = FetchLastModified(brandingPackgeName1.CurrentLink);
                brandingPackgeName1.PackageSizeBytes = FetchContentLength(brandingPackgeName1.CurrentLink);
                brandingPackgeName1.PackageSizeBytesUncompressed = brandingPackgeName1.PackageSizeBytes;
                //packages.Add(brandingPackgeName1.Name.ToLowerInvariant(), brandingPackgeName1);
                packages.Add(brandingPackgeName1.CurrentLinkContext.ToLowerInvariant(), brandingPackgeName1);

                if (brandingPackgeName1.LastModified > lastModifiedTime)
                    lastModifiedTime = brandingPackgeName1.LastModified;

                var brandingPackgeName2 = new Package
                {
                    Name = BrandingPackageName2,
                    Deployed = @"true",
                    LastModified = DateTime.Now,
                    PackageEtag = null,
                    CurrentLink = string.Format(CultureInfo.InvariantCulture, "{0}{1}.cab", BrandingPackageUrl, BrandingPackageName2),
                    CurrentLinkContext = string.Format(CultureInfo.InvariantCulture, "{0}.cab", BrandingPackageName2),
                    PackageSizeBytes = 0,
                    PackageSizeBytesUncompressed = 0,
                    PackageConstituentLink = string.Format(CultureInfo.InvariantCulture, "{0}{1}", @"../../serviceapi/packages/brands/", BrandingPackageName2)
                };
                brandingPackgeName2.LastModified = FetchLastModified(brandingPackgeName2.CurrentLink);
                brandingPackgeName2.PackageSizeBytes = FetchContentLength(brandingPackgeName2.CurrentLink);
                brandingPackgeName2.PackageSizeBytesUncompressed = brandingPackgeName2.PackageSizeBytes;
                //packages.Add(brandingPackgeName2.Name.ToLowerInvariant(), brandingPackgeName2);
                packages.Add(brandingPackgeName2.CurrentLinkContext.ToLowerInvariant(), brandingPackgeName2);
                if (brandingPackgeName2.LastModified > lastModifiedTime)
                    // ReSharper disable once RedundantAssignment
                    lastModifiedTime = brandingPackgeName2.LastModified;
            }

            foreach (BookGroup bookGroup in bookGroups)
            {
                //File.WriteAllText(
                //    Path.Combine( cachePath, bookGroup.CreateFileName() ), 
                //    HelpIndexManager.CreateBookGroupBooksIndex( bookGroup ), 
                //    Encoding.UTF8 );
                //Debug.Print( "BookGroup: {0}", bookGroup.Name );
                var lastModifiedTimeBookGroup = new DateTime(2000, 1, 1, 0, 0, 0);
                foreach (Book book in bookGroup.Books)
                {
                    var lastModifiedTimeBook = new DateTime(2000, 1, 1, 0, 0, 0);
                    foreach (Package package in book.Packages)
                    {
                        if (book.Wanted)
                        {
                            //string name = package.Name.ToLowerInvariant();
                            string name = package.CurrentLinkContext.ToLowerInvariant();
                            //Debug.Print( "      Package: {0}", name );

                            if (!packages.ContainsKey(name))
                            {
                                //package.LastModified = FetchLastModified(package.CurrentLink);
                                //package.PackageSizeBytes = FetchContentLength(package.CurrentLink);

                                packages.Add(name, package);
                            }
                            //package.LastModified = packages[name].LastModified;
                            //package.PackageSizeBytes = packages[name].PackageSizeBytes;
                        }

                        if (package.LastModified > lastModifiedTimeBook)
                            lastModifiedTimeBook = package.LastModified;
                    }

                    if (book.Wanted)
                    {
                        //Debug.Print( "   Book: {0}", book.Name );
                        //File.WriteAllText(
                        //    Path.Combine( cachePath, book.CreateFileName() ), 
                        //    HelpIndexManager.CreateBookPackagesIndex( bookGroup, book ), 
                        //    Encoding.UTF8 );
                        if (!strLocales.Contains(book.Locale))
                            strLocales.Add(book.Locale);
                    }

                    book.LastModified = lastModifiedTimeBook;
                    if (book.LastModified > lastModifiedTimeBookGroup)
                        lastModifiedTimeBookGroup = book.LastModified;
                }

                bookGroup.LastModified = lastModifiedTimeBookGroup;
                if (bookGroup.LastModified > lastModifiedTimeCatalogLocale)
                    lastModifiedTimeCatalogLocale = bookGroup.LastModified;
            }

            bool includeEnUs = false;
            if (0 == strLocales.Count)
                return;
            if (1 == strLocales.Count)
                codeLocale = strLocales[0];
            else if (2 == strLocales.Count)
            {
                includeEnUs = true;
                foreach (string strLoc in strLocales)
                    if (strLoc.ToLowerInvariant() != "en-us")
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
                string listFileName = objLocale.CreatePackageListFileName();
                string listFilePath = Path.Combine(cachePath, listFileName);

                if (File.Exists(listFilePath))
                    File.Delete(listFilePath);

                listFilePath = Path.Combine(vsDirectory, listFileName);
                if (File.Exists(listFilePath))
                    File.Delete(listFilePath);

                StreamWriter writer = new StreamWriter(listFilePath);
                //IEnumerable<Package> query = packages.Values.OrderBy(package => package.CurrentLink);
                var query = packages.Values.ToList();
                query.Sort();
                foreach (Package package in query)
                    writer.WriteLine(package.CurrentLink);
                writer.Close();

                FileLastModifiedTime(listFilePath, lastModifiedTimeCatalogLocale);
                //File.SetCreationTime( listFilePath, lastModifiedTimeCatalogLocale );
                //File.SetLastAccessTime( listFilePath, lastModifiedTimeCatalogLocale );
                //File.SetLastWriteTime( listFilePath, lastModifiedTimeCatalogLocale );
            }
            catch (Exception e)
            {
                Program.LogException(e);
            }

            Directory.GetFiles(cachePath, "*.xml").ForEach(File.Delete);

            foreach (string file in Directory.GetFiles(cachePath, "*.msha"))
            {
                string fileName = Path.GetFileName(file);
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (!fileName.Contains(@"HelpContentSetup"))
                    {
                        File.Delete(file);
                    }

                    if (fileName == @"(" + codeLocale + @")HelpContentSetup")
                    //if ( fileName == @"HelpContentSetup(" + codeLocale + @")" )
                    {
                        File.Delete(file);
                        //break;
                    }
                }
            }

            foreach (string file in Directory.GetFiles(vsDirectory, "*.msha"))
            {
                string fileName = Path.GetFileName(file);
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (!fileName.Contains(@"HelpContentSetup"))
                    {
                        File.Delete(file);
                    }

                    if (fileName == @"(" + codeLocale + @")HelpContentSetup")
                    {
                        File.Delete(file);
                        //break;
                    }
                }
            }

            // Creating setup indexes
            //File.WriteAllText(
            //Path.Combine( cachePath, "HelpContentSetup.msha" ), HelpIndexManager.CreateSetupIndex( bookGroups, nameCatalog, catalogLocale ), Encoding.UTF8 );

            string xmlname = Path.Combine(vsDirectory, objLocale.CreateFileName());
            File.WriteAllText(xmlname,
                objCatalog.Name == "dev10"
                    ? HelpIndexManager.CreateSetupIndex10(bookGroups, objCatalog, objLocale, vsDirectory)
                    : HelpIndexManager.CreateSetupIndex(bookGroups, objCatalog, objLocale), Encoding.UTF8);

            FileLastModifiedTime(xmlname, lastModifiedTimeCatalogLocale);
            //File.SetCreationTime( xmlname, lastModifiedTimeCatalogLocale );
            //File.SetLastAccessTime( xmlname, lastModifiedTimeCatalogLocale );
            //File.SetLastWriteTime( xmlname, lastModifiedTimeCatalogLocale );

            string packagesDirectory = Path.Combine(vsDirectory, "packages");
            string packagesDirectoryLocale = Path.Combine(packagesDirectory, codeLocale.ToLowerInvariant());
            string packagesDirectoryEnUs = Path.Combine(packagesDirectory, "en-us");

            // Cleanup old files
            //Directory.GetFiles( packagesDirectory, "*.cab" ).ForEach( File.Delete );
            CleanupOldPackages(packages, null, cachePath, false);

            string oldVsDirName = objCatalog.Name;
            if (oldVsDirName == "dev10")
                oldVsDirName = "VisualStudio10";
            string oldvsDirectory = Path.Combine(cachePath, oldVsDirName);
            string oldpackagesDirectory = Path.Combine(oldvsDirectory, @"packages");
            string oldpackagesDirectoryLocale = Path.Combine(oldpackagesDirectory, codeLocale.ToLowerInvariant());
            string oldpackagesDirectoryEnUs = Path.Combine(oldpackagesDirectory, "en-us");
            CleanupOldPackages(packages, null, oldvsDirectory, false);
            CleanupOldPackages(packages, null, oldpackagesDirectory, false);
            CleanupOldPackages(packages, codeLocale, oldpackagesDirectoryLocale, false);
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us") && includeEnUs)
                CleanupOldPackages(packages, @"en-us", oldpackagesDirectoryEnUs, false);

            CleanupOldPackages(packages, null, Path.Combine(cachePath, @"packages"), false);
            CleanupOldPackages(packages, null, Path.Combine(cachePath, @"packages", codeLocale.ToLowerInvariant()), false);
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us") && includeEnUs)
                CleanupOldPackages(packages, null, Path.Combine(cachePath, @"packages", @"en-us"), false);

            CleanupOldPackages(packages, null, vsDirectory, false);
            CleanupOldPackages(packages, null, packagesDirectory, false);
            CleanupOldPackages(packages, codeLocale, packagesDirectoryLocale, true);
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us") && includeEnUs)
                CleanupOldPackages(packages, @"en-us", packagesDirectoryEnUs, true);

            // Create cachePath
            if (!Directory.Exists(packagesDirectory))
                Directory.CreateDirectory(packagesDirectory);

            if (!Directory.Exists(packagesDirectoryLocale))
                Directory.CreateDirectory(packagesDirectoryLocale);

            if (includeEnUs && !Directory.Exists(packagesDirectoryEnUs))
                Directory.CreateDirectory(packagesDirectoryEnUs);


            // Download the packages
            DateTime lastModifiedDirLoc = new DateTime(2000, 1, 1, 0, 0, 0);
            DateTime lastModifiedDirEnUs = new DateTime(2000, 1, 1, 0, 0, 0);
            int packagesCountCurrent = 0;
            foreach (Package package in packages.Values)
            {
                string destDirectory;
                if (package.Name.ToLowerInvariant().Contains(codeLocale.ToLowerInvariant()))
                {
                    destDirectory = packagesDirectoryLocale;
                    if (package.LastModified > lastModifiedDirLoc)
                        lastModifiedDirLoc = package.LastModified;
                }
                else if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                {
                    destDirectory = packagesDirectoryEnUs;
                    if (package.LastModified > lastModifiedDirEnUs)
                        lastModifiedDirEnUs = package.LastModified;
                }
                else
                    destDirectory = packagesDirectory;

                string targetFileName = Path.Combine(destDirectory, package.CreateFileName());

                // If file exist and file length is the same, skip it
                if (package.State != PackageState.NotDownloaded)
                //if ( package.State == PackageState.OutOfDate )
                {
                    if (File.Exists(targetFileName))
                    {
                        FileInfo curFileInfo = new FileInfo(targetFileName);

                        //if (package.State == PackageState.OutOfDate)
                        if (package.LastModified != curFileInfo.LastWriteTime)
                        {
                            package.LastModified = FetchLastModified(package.CurrentLink);
                        }
                        if (package.LastModified != curFileInfo.LastWriteTime)
                        {
                            //File.Delete(targetFileName);
                            package.State = PackageState.OutOfDate;
                        }

                        if (package.PackageSizeBytes != curFileInfo.Length)
                        {
                            package.PackageSizeBytes = FetchContentLength(package.CurrentLink);
                        }
                        if (package.PackageSizeBytes != curFileInfo.Length)
                        {
                            File.Delete(targetFileName);
                            package.State = PackageState.NotDownloaded;
                        }
                    }
                    else
                    {
                        package.LastModified = FetchLastModified(package.CurrentLink);
                        package.State = PackageState.NotDownloaded;
                    }
                }
                else if (package.State == PackageState.NotDownloaded)
                {
                    package.LastModified = FetchLastModified(package.CurrentLink);
                }

                if (package.State == PackageState.NotDownloaded /*|| package.State == PackageState.OutOfDate*/)
                {
                    //Debug.Print("         Downloading : '{0}' to '{1}'", package.CurrentLink, targetFileName);
                    _client.DownloadFile(package.CurrentLink, targetFileName);
                }

                FileLastModifiedTime(targetFileName, package.LastModified);
                //File.SetCreationTime( targetFileName, package.LastModified );
                //File.SetLastAccessTime( targetFileName, package.LastModified );
                //File.SetLastWriteTime( targetFileName, package.LastModified );

                package.State = PackageState.Ready;
                progress.Report(100 * ++packagesCountCurrent / packages.Count);
            }

            FileLastModifiedTime(packagesDirectoryLocale, lastModifiedDirLoc, true);
            if (!codeLocale.ToLowerInvariant().Contains(@"en-us") && includeEnUs)
                FileLastModifiedTime(packagesDirectoryEnUs, lastModifiedDirEnUs, true);
            FileLastModifiedTime(packagesDirectory, lastModifiedTimeCatalogLocale, true);
            FileLastModifiedTime(vsDirectory, lastModifiedTimeCatalogLocale, true);
            FileLastModifiedTime(cachePath, lastModifiedTimeCatalogLocale, true);
        }

        private void CleanupOldPackages(Dictionary<string, Package> packages, string codeLocale, string cabDirectory, bool bTargetDirectoryLocale)
        {
            if (!Directory.Exists(cabDirectory))
                return;

            foreach (string file in Directory.GetFiles(cabDirectory, "*.cab"))
            {
                var fileName = Path.GetFileName(file);
                fileName = fileName.ToLowerInvariant();

                //var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                //if (fileNameWithoutExtension != null)
                {
                    //string fileName = fileNameWithoutExtension.ToLowerInvariant();
                    //var pos = fileName.LastIndexOf('(');
                    //if (pos >= 0)
                    //    fileName = fileName.Substring(0, pos);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (bTargetDirectoryLocale)
                        {
                            if (!packages.ContainsKey(fileName)
                                 || !fileName.Contains(codeLocale.ToLowerInvariant()))
                            {
                                File.Delete(file);
                            }
                            else
                            {
                                string packagePathDest = Path.Combine(cabDirectory, packages[fileName].CreateFileName());
                                if (file != packagePathDest)
                                {
                                    if (File.Exists(packagePathDest))
                                    {
                                        File.Delete(file);
                                    }
                                    else
                                    {
                                        FileInfo oldFile = new FileInfo(file);
                                        oldFile.MoveTo(packagePathDest);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (packages.ContainsKey(fileName))
                                File.Delete(file);

                            //foreach (Package package in packages.Values)
                            //{
                            //    if (string.Compare(fileName, 0, package.Name.ToLowerInvariant() + @"(", 0, package.Name.Length + 1, true) == 0
                            //         || string.Compare(fileName, 0, package.Name.ToLowerInvariant() + @".", 0, package.Name.Length + 1, true) == 0)

                            ////    if (fileName.ToLowerInvariant().Contains(package.Name.ToLowerInvariant()/* + @"("*/)
                            ////        /*|| string.Compare(fileName, package.Name, true) == 0*/)
                            //    {
                            //        File.Delete(file);
                            //        break;
                            //    }
                            //}
                        }
                    }
                }
            }
        }

        private long FetchContentLength(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return 0;

            long result = 0;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                if (null != _proxy)
                    request.Proxy = _proxy;

                var response = (HttpWebResponse)request.GetResponse();

                result = response.ContentLength;
                response.Close();
            }
            catch (Exception e)
            {
                Program.LogException(e);
            }

            return result;
        }

        private DateTime FetchLastModified(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return DateTime.Now;

            DateTime result = DateTime.Now;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                if (null != _proxy)
                    request.Proxy = _proxy;

                var response = (HttpWebResponse)request.GetResponse();

                result = response.LastModified/*.ToUniversalTime()*/;
                response.Close();
            }
            catch (Exception e)
            {
                Program.LogException(e);
            }

            return result;
        }

        public static void FileLastModifiedTime(string filePath, DateTime lastModifiedTime, bool bDirectory = false)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            //DateTime lastModifiedTimeUtc = lastModifiedTime/*.ToUniversalTime()*/;
            try
            {
                if (bDirectory)
                {
                    Directory.SetCreationTime/*Utc*/(filePath, lastModifiedTime);
                    Directory.SetLastWriteTime/*Utc*/(filePath, lastModifiedTime);
                    Directory.SetLastAccessTime/*Utc*/(filePath, lastModifiedTime);
                }
                else
                {
                    File.SetCreationTime/*Utc*/(filePath, lastModifiedTime);
                    File.SetLastWriteTime/*Utc*/(filePath, lastModifiedTime);
                    File.SetLastAccessTime/*Utc*/(filePath, lastModifiedTime);
                }
            }
            catch (Exception e)
            {
                Program.LogException(e);
            }

        }

        /// <summary>
        /// Standard IDispose pattern
        /// </summary>
        /// <param name="disposing">
        /// true if called by Dispose, false if called from destructor
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }
    }
}
