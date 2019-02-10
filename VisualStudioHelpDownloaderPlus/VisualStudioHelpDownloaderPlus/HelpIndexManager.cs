using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace VisualStudioHelpDownloaderPlus
{
    /// <summary>
    ///     Helper class for parsing and creating documents with help indexes.
    /// </summary>
    internal static class HelpIndexManager
    {
        /// <summary>
        /// Find the available locales for the help collections
        /// </summary>
        /// <param name="data">
        /// The page data downloaded containing the locale catalog data
        /// </param>
        /// <returns>
        /// Collection of locales in which the help collection may be downloaded
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If data is null
        /// </exception>
        /// <exception cref="XmlException">
        /// If there was a error processing the xml data
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If there was a error processing the xml data
        /// </exception>
        public static ICollection<Catalog> LoadCatalogs(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            List<Catalog> result = new List<Catalog>();

            XDocument document = XDocument.Parse(Encoding.UTF8.GetString(data).Trim());
            
            /*
            XDocument document;

            string str_tmp = Encoding.UTF8.GetString(data).Trim();
            data = Encoding.UTF8.GetBytes(str_tmp);

            using (MemoryStream stream = new MemoryStream(data))
            {
                document = XDocument.Load(stream);
            }
            */

            if (document.Root != null)
            {
                IEnumerable<XElement> query =
                    document.Root.Elements()
                    /*
                    .Where(x => x.GetClassName() == "catalogs")
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName() == "catalog-list")
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName() == "catalog");
                    */
                    .Where(x => x.GetClassName()?.Equals("catalogs", StringComparison.OrdinalIgnoreCase) ?? false)
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName()?.Equals("catalog-list", StringComparison.OrdinalIgnoreCase) ?? false)
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName()?.Equals("catalog", StringComparison.OrdinalIgnoreCase) ?? false);

                result.AddRange(query.Select(x =>
                    new Catalog
                    {
                        Name = x.GetChildClassValue("name"),
                        Description = x.GetChildClassValue("description"),
                        CatalogLink = x.GetChildClassAttributeValue("catalog-link", "href")
                    }));
            }
            else
            {
                throw new XmlException("Catalog Listing");
            }

            return result;
        }

        public static ICollection<CatalogLocale> LoadLocales(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            List<CatalogLocale> result = new List<CatalogLocale>();
            // This would normally be loaded directly from the bytes but the MS server
            // seems to be returning junk whitespace at the start that cant be loaded
            // directly as XML
            XDocument document = XDocument.Parse(Encoding.UTF8.GetString(data).Trim());
/*            
            XDocument document;

            string str_tmp = Encoding.UTF8.GetString(data).Trim();
            data = Encoding.UTF8.GetBytes(str_tmp);
            using (MemoryStream stream = new MemoryStream(data))
            {
                document = XDocument.Load(stream);
            }
*/
            if (document.Root != null)
            {
                IEnumerable<XElement> query =
                    document.Root.Elements()
                    /*
                    .Where(x => x.GetClassName() == "catalogLocales")
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName() == "catalog-locale-list")
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName() == "catalog-locale");
                    */
                    .Where(x => x.GetClassName()?.Equals("catalogLocales", StringComparison.OrdinalIgnoreCase) ?? false)
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName()?.Equals("catalog-locale-list", StringComparison.OrdinalIgnoreCase) ?? false)
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName()?.Equals("catalog-locale", StringComparison.OrdinalIgnoreCase) ?? false);

                result.AddRange(query.Select(x =>
                    new CatalogLocale
                    {
                        Locale = x.GetChildClassValue("locale"),
                        CatalogName = x.GetChildClassValue("catalog-name"),
                        Description = x.GetChildClassValue("description"),
                        LocaleLink = x.GetChildClassAttributeValue("locale-link", "href")
                    }));
            }
            else
            {
                throw new XmlException( "Missing document root" );
            }
            return result;
        }

        /// <summary>
        /// Find the available book groups, books, and packages for the selected language
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="data">
        /// The page data downloaded containing the book data
        /// </param>
        /// <returns>
        /// Collection of book groups for the help collection
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If data is null
        /// </exception>
        /// <exception cref="XmlException">
        /// If there was a error processing the xml data
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If there was a error processing the xml data
        /// </exception>
        public static ICollection<BookGroup> LoadBooks(Catalog catalog, byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            List<BookGroup> result = new List<BookGroup>();

            //XDocument document = XDocument.Parse(Encoding.UTF8.GetString(data).Trim());

            XDocument document;
            //string str_tmp = Encoding.UTF8.GetString(data).Trim();
            //data = Encoding.UTF8.GetBytes(str_tmp);
            using (MemoryStream stream = new MemoryStream(data))
            {
                document = XDocument.Load(stream);
            }

            if (document.Root != null)
            {
                IEnumerable<XElement> groups =
                    document.Root.Elements()
                    /*
                    .Where(x => x.GetClassName() == "book-list")
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName() == "book-groups")
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName() == "book-group");
                    */
                    .Where(x => x.GetClassName()?.Equals("book-list", StringComparison.OrdinalIgnoreCase) ?? false)
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName()?.Equals("book-groups", StringComparison.OrdinalIgnoreCase) ?? false)
                    .Take(1)
                    .Single()
                    .Elements()
                    .Where(x => x.GetClassName()?.Equals("book-group", StringComparison.OrdinalIgnoreCase) ?? false);

                foreach (XElement group in groups)
                {
                    BookGroup bookGroup = new BookGroup
                    {
                        Id = group.GetChildClassValue("id"),
                        Name = group.GetChildClassValue("name"),
                        Vendor = group.GetChildClassValue("vendor"),
                        Books = new List<Book>()
                    };

                    if (catalog.Name == "dev10")
                    {
                        var parts = bookGroup.Name.Split('_');
                        string name = parts[0];
                        if (name == "Microsoft BizTalk Server 2010"
                            || name == "Kinect for Windows"
                            || name == "MS Office 2010"
                            || name == "SharePoint Products and Technologies")
                            name = ".NET Development";
                        else if (name == "SQL Server \"Denali\"")
                            name = "SQL Server 2012";
                        else if (name == "SQL Server 2014 Books Online")
                            name = "SQL Server 2014";

                        bookGroup.Name = name;
                    }

                    if (catalog.Name == "dev10")
                    {
                        BookGroup bookgroup = null;
                        foreach (var bg in result)
                        {
                            if (bg.Name == bookGroup.Name)
                            {
                                bookgroup = bg;
                            }
                        }

                        if (bookgroup != null)
                            bookGroup = bookgroup;
                        else
                            result.Add(bookGroup);
                    }
                    else
                        result.Add(bookGroup);

                    //IEnumerable<XElement> books = group.Elements().Where(x => x.GetClassName() == "book");
                    IEnumerable<XElement> books = group.Elements().Where(x => x.GetClassName()?.Equals("book", StringComparison.OrdinalIgnoreCase) ?? false);
                    foreach (XElement book in books)
                    {
                        Book b = new Book
                        {
                            Id = book.GetChildClassValue("id"),
                            Locale = book.GetChildClassValue("locale"),
                            Name = book.GetChildClassValue("name"),
                            Description = book.GetChildClassValue("description"),
                            BrandingPackageName = book.GetChildClassValue("BrandingPackageName"),
                            Paths = new List<MsdnPath>(),
                            PackagesBeforeContext = book.GetChildClassFirstNodeContext("packages"),
                            Packages = new List<Package>()
                        };

                        if (catalog.Name == "dev10")
                        {
                            var localeDes = new Dictionary<string, string>
                            {
                                {"fr-fr", @"français (France)"},
                                {"pt-br", @"Português (Brasil)"},
                                {"es-es", @"Español (España, alfabetización internacional)"},
                                //{"en-us", @""},
                                {"pl-pl", @"polski (Polska)"},
                                {"de-de", @"Deutsch (Deutschland)"},
                                {"cs-cz", @"čeština (Česká republika)"},
                                {"it-it", @"italiano (Italia)"},
                                {"tr-tr", @"Türkçe (Türkiye)"},
                                {"ru-ru", @"русский (Россия)"},
                                {"ja-jp", @"日本語 (日本)"},
                                {"ko-kr", @"한국어(대한민국)"},
                                {"zh-tw", @"中文(台灣)"},
                                {"zh-cn", @"中文(中华人民共和国)"}
                            };
                            if (localeDes.ContainsKey(b.Locale))
                                b.Name = b.Name + @" - " + localeDes[b.Locale];
                        }

                        if (catalog.Name != "dev10")
                        {
                            IEnumerable<XElement> paths =
                                /*
                                book.Elements()
                                .Where(x => x.GetClassName() == "properties")
                                .Take(1)
                                .Single()
                                .Elements()
                                .Where(x => x.GetClassName() == "paths")
                                .Take(1)
                                .Single()
                                .Elements()
                                .Where(x => x.GetClassName() == "path");
                                //.Take( 1 )
                                //.Single();
                                */
                                //XElement path =
                                book.Elements()
                                .Where(x => x.GetClassName()?.Equals("properties", StringComparison.OrdinalIgnoreCase) ?? false)
                                .Take(1)
                                .Single()
                                .Elements()
                                .Where(x => x.GetClassName()?.Equals("paths", StringComparison.OrdinalIgnoreCase) ?? false)
                                .Take(1)
                                .Single()
                                .Elements()
                                .Where(x => x.GetClassName()?.Equals("path", StringComparison.OrdinalIgnoreCase) ?? false);
                                //.Take(1)
                                //.Single();

                            foreach (XElement path in paths)
                            {
                                MsdnPath p = new MsdnPath
                                {
                                    Languages = path.GetChildClassValue("languages"),
                                    Membership = path.GetChildClassValue("membership"),
                                    Name = path.GetChildClassValue("name"),
                                    //Priority = path.GetChildClassValue( "priority" ),
                                    Priority = long.Parse(path.GetChildClassValue("priority"), CultureInfo.InvariantCulture),
                                    //SkuId = path.GetChildClassValue( "skuId" ),
                                    SkuId = long.Parse(path.GetChildClassValue("skuId"), CultureInfo.InvariantCulture),
                                    SkuName = path.GetChildClassValue("skuName")
                                };

                                if (p.SkuName == "Enterprise")
                                    p.SkuId = 3000;
                                else if (p.SkuName == "Professional")
                                    p.SkuId = 2000;
                                else if (p.SkuName == "Standard")
                                    p.SkuId = 1000;
                                else if (p.SkuName == "Express")
                                    p.SkuId = 500;

                                b.Paths.Add(p);

                                string bookPath = p.Name.TrimStart('\\');
                                b.Category = bookPath;
                            }
                        }

                        //XElement packs =
                        //    book.Elements()
                        //    .Where(x => x.GetClassName() == "packages")
                        //    .Take(1).Single();
                        //if (packs != null)
                        //{
                        //    b.PackagesBeforeContext = ((XText)(packs.FirstNode)).Value;
                        //}
                        IEnumerable<XElement> packages;
                        //                if (catalog.Name == "dev10")
                        //                {
                        //                    //var parts = bookGroup.Name.Split('_');
                        //                    //b.Category = parts[0];
                        //                    b.Category = bookGroup.Name;

                        ////                    packages =
                        ////                        book.Elements()
                        ////                        //.Where(x => x.GetClassName() == "properties")
                        ////                        //.Take(1)
                        ////                        //.Single()
                        ////                        //.Elements()
                        ////                        .Where(x => x.GetClassName() == "packages")
                        ////                        .Take( 1 )
                        ////                        .Single()
                        ////                        .Elements()
                        ////                        .Where( x => x.GetClassName() == "package");
                        //                }
                        //                //else
                        {
                            packages =
                                book.Elements()
                                /*
                                .Where(x => x.GetClassName() == "packages")
                                .Take(1)
                                .Single()
                                .Elements()
                                .Where(x => x.GetClassName() == "package");
                                */
                                .Where(x => x.GetClassName()?.Equals("packages", StringComparison.OrdinalIgnoreCase) ?? false)
                                .Take(1)
                                .Single()
                                .Elements()
                                .Where(x => x.GetClassName()?.Equals("package", StringComparison.OrdinalIgnoreCase) ?? false);
                        }

                        foreach (XElement package in packages)
                        {
                            Package pa = new Package
                            {
                                PackageType = package.GetChildClassValue("packageType"),
                                PackageFormat = package.GetChildClassValue("packageFormat"),
                                Name = package.GetChildClassValue("name"),
                                DeployedBeforeContext = package.GetChildClassBeforeContext("deployed"),
                                Deployed = package.GetChildClassValue("deployed"),
                                LastModified = DateTime.Parse(package.GetChildClassValue("last-modified"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)/*.ToUniversalTime()*/,
                                PackageEtag = package.GetChildClassValue("package-etag"),
                                CurrentLink = package.GetChildClassAttributeValue("current-link", "href"),
                                CurrentLinkContext = package.GetChildClassValue("current-link"),
                                PackageSizeBytes = long.Parse(package.GetChildClassValue("package-size-bytes"), CultureInfo.InvariantCulture),// unused
                                PackageSizeBytesUncompressed = long.Parse(package.GetChildClassValue("package-size-bytes-uncompressed"), CultureInfo.InvariantCulture),// unused
                                PackageConstituentLink = package.GetChildClassAttributeValue("package-constituent-link", "href"),
                                ConstituentLinkBeforeContext = package.GetChildClassBeforeContext("package-constituent-link"),
                                PackageConstituentLinkContext = package.GetChildClassValue("package-constituent-link"),
                                ConstituentLinkAfterContext = package.GetChildClassAfterContext("package-constituent-link")
                            };

                            b.Packages.Add(pa);
                        }

                        bookGroup.Books.Add(b);
                    }
                }
            }
            else
            {
                throw new XmlException("Missing document root");
            }

            return result;
        }

        /// <summary>
        /// Creates main help setup index.
        /// </summary>
        /// <param name="bookGroups">
        /// A collection of book groups to add to the index
        /// </param>
        /// <param name="objCatalog"></param>
        /// <param name="objLocale"></param>
        /// <returns>
        /// The xml document text
        /// </returns>
        public static string CreateSetupIndex(IEnumerable<BookGroup> bookGroups, Catalog objCatalog, CatalogLocale objLocale)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null), CreateElement("html", null, null));

            XElement headElement = CreateElement("head", null, null);
            XElement metaDateElemet1 = CreateElement("meta", null, null);
            metaDateElemet1.SetAttributeValue(XName.Get("name", string.Empty), "ROBOTS");
            metaDateElemet1.SetAttributeValue(XName.Get("content", string.Empty), "NOINDEX, NOFOLLOW");

            XElement metaDateElemet2 = CreateElement("meta", null, null);
            metaDateElemet2.SetAttributeValue(XName.Get("http-equiv", string.Empty), "Content-Location");
            metaDateElemet2.SetAttributeValue(
                XName.Get("content", string.Empty),
                string.Format(CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/serviceapi/catalogs/{0}/{1}",
                objCatalog.Name.ToLowerInvariant(), objLocale.Locale));

            XElement linkElement = CreateElement("link", null, null);
            linkElement.SetAttributeValue(XName.Get("type", string.Empty), "text/css");
            linkElement.SetAttributeValue(XName.Get("rel", string.Empty), "stylesheet");
            linkElement.SetAttributeValue(XName.Get("href", string.Empty), "../../styles/global.css");
            //linkElement.SetAttributeValue(XName.Get("href", string.Empty), "http://services.mtps.microsoft.com/serviceapi/styles/global.css");

            XElement titleElement = CreateElement("title", null, "All Book Listings");

            headElement.Add(metaDateElemet1);
            headElement.Add(metaDateElemet2);
            headElement.Add(linkElement);
            headElement.Add(titleElement);

            XElement bodyElement = CreateElement("body", "book-list", null);
            XElement detailsElement = CreateElement("div", "details", null);

            XElement catalogLocaleLinkElement = CreateElement("a", "catalog-locale-link", "Catalog locales");
            catalogLocaleLinkElement.SetAttributeValue(XName.Get("href", string.Empty),
                string.Format(CultureInfo.InvariantCulture, @"../../catalogs/{0}/{1}", objCatalog.Name.ToLowerInvariant(), objLocale.Locale));
            //catalogLocaleLinkElement.SetAttributeValue(XName.Get("href", string.Empty),
            //    string.Format(CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/ServiceAPI/catalogs/{0}", objCatalog.Name.ToLowerInvariant()));


            detailsElement.Add(catalogLocaleLinkElement);

            XElement bookgroupsElement = CreateBookGroupBooksIndex(bookGroups, objCatalog, objLocale);

            bodyElement.Add(detailsElement, bookgroupsElement);

            var divElement = CreateElement("div", null, null);
            divElement.SetAttributeValue(XName.Get("id", string.Empty), "GWDANG_HAS_BUILT");
            bodyElement.Add(divElement);

            document.Root?.Add(headElement, bodyElement);

            return document.ToStringWithDeclaration();
        }

        /// <summary>
        /// Create book group books index.
        /// </summary>
        /// <returns>
        /// The xml document text
        /// </returns>
        public static XElement CreateBookGroupBooksIndex(IEnumerable<BookGroup> bookGroups, Catalog objCatalog, CatalogLocale objLocale)
        {
            XElement bookgroupsElement = CreateElement("div", "book-groups", null);

            var groups = bookGroups as List<BookGroup>;
            groups?.Sort();
            foreach (BookGroup bookGroup in bookGroups)
            {
                XElement bookgroupElement = CreateElement("div", "book-group", null);
                bookgroupElement.Add(
                    CreateElement("span", "id", bookGroup.Id),
                    CreateElement("span", "name", bookGroup.Name),
                    CreateElement("span", "vendor", bookGroup.Vendor)
                    );

                var books = bookGroup.Books as List<Book>;
                books?.Sort();
                foreach (Book book in bookGroup.Books)
                {
                    if (!book.Wanted)
                    {
                        continue;
                    }

                    bookgroupElement.Add(CreateBookPackagesIndex(book, objCatalog, objLocale));
                }

                bookgroupsElement.Add(bookgroupElement);
            }

            return bookgroupsElement;
        }

        /// <summary>
        /// Create book packages index.
        /// </summary>
        /// <param name="book">
        /// The book associated with the packages
        /// </param>
        /// <param name="objCatalog"></param>
        /// <param name="objLocale"></param>
        /// <returns>
        /// The xml document text
        /// </returns>
        public static XElement CreateBookPackagesIndex(Book book, Catalog objCatalog, CatalogLocale objLocale)
        {
            XElement bookElement = CreateElement("div", "book", null);

            XElement propertiesElement = CreateElement("div", "properties", null);
            if (objCatalog.Name != "dev10")
            {
                var msdnPaths = book.Paths as List<MsdnPath>;
                msdnPaths?.Sort();
                XElement pathsElement = CreateElement("div", "paths", null);
                foreach (MsdnPath path in book.Paths)
                {
                    XElement pathElement = CreateElement("div", "path", null);

                    pathElement.Add(
                        CreateElement("span", "languages", path.Languages),
                        CreateElement("span", "membership", path.Membership),
                        CreateElement("span", "name", path.Name),
                        CreateElement("span", "priority", path.Priority.ToString()),
                        CreateElement("span", "skuId", path.SkuId.ToString()),
                        CreateElement("span", "skuName", path.SkuName)
                        );

                    pathsElement.Add(pathElement);
                }
                propertiesElement.Add(pathsElement);
            }

            XElement packageListElement = CreateElement("div", "packages", null);
            //packageListElement.Add(new XText(book.PackagesBeforeContext));

            var packages = book.Packages as List<Package>;
            packages?.Sort();
            foreach (Package package in book.Packages)
            {
                XElement packageElement = CreateElement("div", "package", null);
                //var lastModifiedFmt = (package.LastModified.Millisecond % 10) == 0 ? "yyyy-MM-ddThh:mm:ss.ffZ" : "yyyy-MM-ddThh:mm:ss.fffZ";
                var lastModifiedFmt = "s";
                XElement lastModified = CreateElement("span", "last-modified", package.LastModified.ToUniversalTime().ToString(lastModifiedFmt, CultureInfo.InvariantCulture));
                //XElement lastModified = new XElement("span", package.LastModified);

                string curlink;
                if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                    curlink = string.Format(CultureInfo.InvariantCulture, "packages/en-us/{0}", package.CreateFileNameUri());
                else if (package.Name.ToLowerInvariant().Contains(objLocale.Locale.ToLowerInvariant()))
                    curlink = string.Format(CultureInfo.InvariantCulture, "packages/{0}/{1}", objLocale.Locale.ToLowerInvariant(), package.CreateFileNameUri());
                else
                    curlink = string.Format(CultureInfo.InvariantCulture, "packages/{0}", package.CreateFileNameUri());

                string constituentLink;
                if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                    constituentLink = string.Format(CultureInfo.InvariantCulture, "packages/en-us/{0}", package.Name);
                else if (package.Name.ToLowerInvariant().Contains(objLocale.Locale.ToLowerInvariant()))
                    constituentLink = string.Format(CultureInfo.InvariantCulture, "packages/{0}/{1}", objLocale.Locale.ToLowerInvariant(), package.Name);
                else
                    constituentLink = string.Format(CultureInfo.InvariantCulture, "packages/{0}", package.Name);

                XElement currentLinkElement = CreateElement("a", "current-link", package.CreateFileName());
                currentLinkElement.SetAttributeValue(XName.Get("href", string.Empty), curlink);

                XElement constituentLinkElement = CreateElement("a", "package-constituent-link", package.Name);
                constituentLinkElement.SetAttributeValue(XName.Get("href", string.Empty), constituentLink);

                packageElement.Add(
                    CreateElement("span", "packageType", package.PackageType),
                    CreateElement("span", "packageFormat", package.PackageFormat),
                    CreateElement("span", "name", package.Name),
                    //new XText(package.DeployedBeforeContext),
                    CreateElement("span", "deployed", package.Deployed),
                    lastModified,
                    CreateElement("span", "package-etag", package.PackageEtag),
                    currentLinkElement,
                    CreateElement("span", "package-size-bytes", package.PackageSizeBytes.ToString()),
                    CreateElement("span", "package-size-bytes-uncompressed", package.PackageSizeBytesUncompressed.ToString()),
                    //new XText(package.ConstituentLinkBeforeContext),
                    constituentLinkElement
                //,
                //new XText(package.ConstituentLinkAfterContext)
                );

                packageListElement.Add(packageElement);
            }

            bookElement.Add(
                CreateElement("span", "id", book.Id),
                CreateElement("span", "locale", book.Locale),
                CreateElement("span", "name", book.Name),
                CreateElement("span", "description", book.Description),
                CreateElement("span", "BrandingPackageName", book.BrandingPackageName),
                propertiesElement,
                packageListElement
                );

            return bookElement;
        }

        /// <summary>
        /// Creates main help setup index.
        /// </summary>
        /// <param name="bookGroups">
        /// A collection of book groups to add to the index
        /// </param>
        /// <param name="objCatalog"></param>
        /// <param name="objLocale"></param>
        /// <param name="vsDirectory"></param>
        /// <returns>
        /// The xml document text
        /// </returns>
        public static string CreateSetupIndex10(IEnumerable<BookGroup> bookGroups, Catalog objCatalog, CatalogLocale objLocale, string vsDirectory)
        {
            Contract.Requires(null != bookGroups);

            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null), CreateElement("html", null, null));

            XElement headElement = CreateElement("head", null, null);
            XElement metaDateElemet1 = CreateElement("meta", null, null);
            metaDateElemet1.SetAttributeValue(XName.Get("name", string.Empty), "ROBOTS");
            metaDateElemet1.SetAttributeValue(XName.Get("content", string.Empty), "NOINDEX, NOFOLLOW");

            XElement metaDateElemet2 = CreateElement("meta", null, null);
            metaDateElemet2.SetAttributeValue(XName.Get("http-equiv", string.Empty), "Content-Location");
            metaDateElemet2.SetAttributeValue(
                    XName.Get("content", string.Empty),
                    string.Format(CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/serviceapi/catalogs/{0}/{1}",
                    objCatalog.Name.ToLowerInvariant(), objLocale.Locale));

            XElement linkElement = CreateElement("link", null, null);
            linkElement.SetAttributeValue(XName.Get("type", string.Empty), "text/css");
            linkElement.SetAttributeValue(XName.Get("rel", string.Empty), "stylesheet");
            linkElement.SetAttributeValue(XName.Get("href", string.Empty), "../../styles/global.css");
            //linkElement.SetAttributeValue(XName.Get("href", string.Empty), "http://services.mtps.microsoft.com/serviceapi/styles/global.css");

            XElement titleElement = CreateElement("title", null, "All Book Listings");

            headElement.Add(metaDateElemet1);
            headElement.Add(metaDateElemet2);
            headElement.Add(linkElement);
            headElement.Add(titleElement);

            XElement bodyElement = CreateElement("body", "product-list", null);
            XElement detailsElement = CreateElement("div", "details", null);

            var iconElement = CreateElement("img", "icon", null);
            iconElement.SetAttributeValue(XName.Get("src", string.Empty), "../../content/image/ic298417");
            //iconElement.SetAttributeValue(XName.Get("src", string.Empty), @"http://services.mtps.microsoft.com/ServiceAPI/content/image/ic298417");
            iconElement.SetAttributeValue(XName.Get("alt", string.Empty), "VS 100 Icon");
            detailsElement.Add(iconElement);

            XElement catalogLocaleLinkElement = CreateElement("a", "catalog-locale-link", "Catalog locales");
            catalogLocaleLinkElement.SetAttributeValue(XName.Get("href", string.Empty),
                string.Format(CultureInfo.InvariantCulture, @"../../catalogs/{0}/{1}", objCatalog.Name.ToLowerInvariant(), objLocale.Locale));
            //catalogLocaleLinkElement.SetAttributeValue(XName.Get("href", string.Empty),
            //    string.Format(CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/ServiceAPI/catalogs/{0}", objCatalog.Name.ToLowerInvariant()));
            detailsElement.Add(catalogLocaleLinkElement);

            var groups = bookGroups as List<BookGroup>;
            groups?.Sort();
            foreach (var product in bookGroups)
            {
                string xmlname = Path.Combine(vsDirectory, product.CreateFileName());
                File.WriteAllText(xmlname, CreateBookGroupBooksIndex10(product, objCatalog, objLocale, vsDirectory), Encoding.UTF8);
                Downloader.FileLastModifiedTime(xmlname, product.LastModified);

                //var productElement = CreateElement("div", "product", "Product Details:");
                var productElement = CreateElement("div", "product", null);

                var descElement = CreateElement("span", "description", string.Empty);
                var iconProductElement = CreateElement("img", "icon", null);
                iconProductElement.SetAttributeValue(XName.Get("src", string.Empty), "../../content/image/ic298417");
                //iconProductElement.SetAttributeValue(XName.Get("src", string.Empty), @"http://services.mtps.microsoft.com/ServiceAPI/content/image/ic298417");
                iconProductElement.SetAttributeValue(XName.Get("alt", string.Empty), "VS 100 Icon");

                string productLickStr = product.CreateFileName();
                var productLick = CreateElement("a", "product-link", product.Name);
                productLick.SetAttributeValue(XName.Get("href", string.Empty), productLickStr);

                //descElement.Add(
                //    iconProductElement,
                //    productLick
                //    );

                productElement.Add(
                    CreateElement("span", "locale", "en-us"),
                    CreateElement("span", "name", product.Name),
                    descElement
                    ,
                    iconProductElement,
                    productLick
                );

                bodyElement.Add(productElement);
            }

            var divElement = CreateElement("div", null, null);
            divElement.SetAttributeValue(XName.Get("id", string.Empty), "GWDANG_HAS_BUILT");
            bodyElement.Add(divElement);

            document.Root?.Add(headElement, bodyElement);

            return document.ToStringWithDeclaration();
        }

        /// <summary>
        /// Create book group books index.
        /// </summary>
        /// <param name="bookGroup">
        /// The book group to create the index for.
        /// </param>
        /// <param name="objCatalog"></param>
        /// <param name="objLocale"></param>
        /// <param name="vsDirectory"></param>
        /// <returns>
        /// The xml document text
        /// </returns>
        public static string CreateBookGroupBooksIndex10(BookGroup bookGroup, Catalog objCatalog, CatalogLocale objLocale, string vsDirectory)
        {
            Contract.Requires(null != bookGroup);
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null), CreateElement("html", null, null));

            var headElement = CreateElement("head", null, null);

            var metaDateElemet1 = CreateElement("meta", null, null);
            metaDateElemet1.SetAttributeValue(XName.Get("name", string.Empty), "ROBOTS");
            metaDateElemet1.SetAttributeValue(XName.Get("content", string.Empty), "NOINDEX, NOFOLLOW");

            //XElement metaDateElemet2 = CreateElement("meta", null, null);
            //metaDateElemet2.SetAttributeValue(XName.Get("http-equiv", string.Empty), "Content-Location");
            //metaDateElemet2.SetAttributeValue(
            //    XName.Get("content", string.Empty),
            //    string.Format(CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/serviceapi/catalogs/{0}/{1}",
            //    objCatalog.Name.ToLowerInvariant(), objLocale.Locale));

            var linkElement1 = CreateElement("link", null, null);
            linkElement1.SetAttributeValue(XName.Get("type", string.Empty), "text/css");
            linkElement1.SetAttributeValue(XName.Get("rel", string.Empty), "stylesheet");
            linkElement1.SetAttributeValue(XName.Get("href", string.Empty), "../../styles/global.css");
            //linkElement1.SetAttributeValue(XName.Get("href", string.Empty), @"http://services.mtps.microsoft.com/serviceapi/styles/global.css");

            var strproduct = string.Format(CultureInfo.InvariantCulture, @"Constituents of Product {0}", bookGroup.Id);
            var titleElement = CreateElement("title", null, strproduct);

            headElement.Add(metaDateElemet1);
            //headElement.Add(metaDateElemet2);
            headElement.Add(linkElement1);
            headElement.Add(titleElement);

            var bodyElement = CreateElement("body", "product", null);
            var detailsElement = CreateElement("div", "details", null);

            var descElement = CreateElement("span", "description", string.Empty);

            var iconElement = CreateElement("img", "icon", null);
            iconElement.SetAttributeValue(XName.Get("src", string.Empty), "../../content/image/ic298417");
            //iconElement.SetAttributeValue(XName.Get("src", string.Empty), @"http://services.mtps.microsoft.com/ServiceAPI/content/image/ic298417");
            iconElement.SetAttributeValue(XName.Get("alt", string.Empty), "VS 100 Icon");

            //string productGroupsLink = string.Empty;//objLocale.CreateFileName();
            //var productGroupsLinkElement = CreateElement("a", "product-groups-link", bookGroup.Name);
            //productGroupsLinkElement.SetAttributeValue(XName.Get("href", string.Empty), productGroupsLink);

            //descElement.Add(
            //    iconElement
            //    //,
            //    //productGroupsLinkElement
            //);

            detailsElement.Add(
                //new XText("\"Product Details:\r\n\""),
                CreateElement("span", "locale", "en-us"),
                CreateElement("span", "name", bookGroup.Name),
                descElement
                ,
                iconElement
                //,
                //productGroupsLinkElement
            );

            var bookListElement = CreateElement("div", "book-list", null);
            //bookListElement.Add(new XText("\"This Product contains the following\r\n\""));

            string productLickStr = bookGroup.CreateFileName();
            var bookListLinkElement = CreateElement("a", "book-list-link", "Books:");
            bookListLinkElement.SetAttributeValue(
                XName.Get("href", string.Empty),
                productLickStr
            );
            bookListElement.Add(bookListLinkElement);

            var books = bookGroup.Books as List<Book>;
            books?.Sort();
            foreach (Book book in bookGroup.Books)
            {
                string xmlname = Path.Combine(vsDirectory, book.CreateFileName());
                File.WriteAllText(xmlname, CreateBookPackagesIndex10(bookGroup, book, objLocale), Encoding.UTF8);
                Downloader.FileLastModifiedTime(xmlname, book.LastModified);

                var bookElement = CreateElement("div", "book", null);

                var descBookElement = CreateElement("span", "description", string.Empty);

                var iconBookElement = CreateElement("img", "icon", null);
                iconBookElement.SetAttributeValue(XName.Get("src", string.Empty), "../../content/image/ic298417");
                //iconBookElement.SetAttributeValue(XName.Get("src", string.Empty), @"http://services.mtps.microsoft.com/ServiceAPI/content/image/ic298417");
                iconBookElement.SetAttributeValue(XName.Get("alt", string.Empty), "VS 100 Icon");

                string bookStr = book.CreateFileName();
                var linkElement = CreateElement("a", "book-link", book.Name);
                linkElement.SetAttributeValue(
                    XName.Get("href", string.Empty),
                    bookStr);

                //descBookElement.Add(
                //    iconBookElement,
                //    linkElement
                //    );
                bookElement.Add(
                    CreateElement("span", "name", book.Name),
                    CreateElement("span", "locale", book.Locale),
                    descBookElement
                    ,
                    //iconBookElement,
                    linkElement
                );

                bookListElement.Add(bookElement);
            }

            var divElement = CreateElement("div", null, null);
            divElement.SetAttributeValue(XName.Get("id", string.Empty), "GWDANG_HAS_BUILT");
            bookListElement.Add(divElement);

            bodyElement.Add(
                detailsElement,
                bookListElement);

            document.Root?.Add(headElement, bodyElement);

            return document.ToStringWithDeclaration();
        }

        /// <summary>
        /// Create book packages index.
        /// </summary>
        /// <param name="bookGroup">
        /// The book Group associated with the book.
        /// </param>
        /// <param name="book">
        /// The book associated with the packages
        /// </param>
        /// <param name="objLocale"></param>
        /// <returns>
        /// The xml document text
        /// </returns>
        public static string CreateBookPackagesIndex10(BookGroup bookGroup, Book book, CatalogLocale objLocale)
        {
            Contract.Requires(null != bookGroup);
            Contract.Requires(null != book);

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                CreateElement("html", null, null));

            var headElement = CreateElement("head", null, null);
            var metaDateElemet1 = CreateElement("meta", null, null);
            metaDateElemet1.SetAttributeValue(XName.Get("name", string.Empty), "ROBOTS");
            metaDateElemet1.SetAttributeValue(XName.Get("content", string.Empty), "NOINDEX, NOFOLLOW");

            //string bookStr = book.CreateFileName();
            //XElement metaDateElemet2 = CreateElement("meta", null, null);
            //metaDateElemet2.SetAttributeValue(XName.Get("http-equiv", string.Empty), "Content-Location");
            //metaDateElemet2.SetAttributeValue(
            //    XName.Get("content", string.Empty),
            //    string.Format(CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/serviceapi/catalogs/{0}/{1}",
            //    objCatalog.Name.ToLowerInvariant(), objLocale.Locale));

            var linkElement1 = CreateElement("link", null, null);
            linkElement1.SetAttributeValue(XName.Get("type", string.Empty), "text/css");
            linkElement1.SetAttributeValue(XName.Get("rel", string.Empty), "stylesheet");
            linkElement1.SetAttributeValue(XName.Get("href", string.Empty), "../../styles/global.css");
            //linkElement1.SetAttributeValue(XName.Get("href", string.Empty), @"http://services.mtps.microsoft.com/serviceapi/styles/global.css");

            var strproduct = string.Format(CultureInfo.InvariantCulture, @"Package Listing for book {0}", book.Id);
            var titleElement = CreateElement("title", null, strproduct);

            headElement.Add(metaDateElemet1);
            //headElement.Add(metaDateElemet2);
            headElement.Add(linkElement1);
            headElement.Add(titleElement);

            var bodyElement = CreateElement("body", "book", null);
            var detailsElement = CreateElement("div", "details", null);

            var descElement = CreateElement("span", "description", string.Empty);

            var iconElement = CreateElement("img", "icon", null);
            iconElement.SetAttributeValue(XName.Get("src", string.Empty), "../../content/image/ic298417");
            //iconElement.SetAttributeValue(XName.Get("src", string.Empty), @"http://services.mtps.microsoft.com/ServiceAPI/content/image/ic298417");
            iconElement.SetAttributeValue(XName.Get("alt", string.Empty), "VS 100 Icon");

            //string productLink = bookGroup.CreateFileName();
            //var productLinkElement = CreateElement("a", "product-link", bookGroup.Name);
            //productLinkElement.SetAttributeValue(XName.Get("href", string.Empty), productLink);

            //string productGroupsLink = string.Empty; //objLocale.CreateFileName();
            //var productGroupsLinkElement = CreateElement("a", "product-group-link", bookGroup.Name);
            //productGroupsLinkElement.SetAttributeValue(XName.Get("href", string.Empty), productGroupsLink);

            var brandingPackageElement1 = CreateElement("a", "branding-package-link", Downloader.BrandingPackageName1);
            brandingPackageElement1.SetAttributeValue(
                XName.Get("href", string.Empty),
                string.Format(CultureInfo.InvariantCulture, @"packages\{0}.cab", Downloader.BrandingPackageName1));

            var brandingPackageElement2 = CreateElement("a", "branding-package-link", Downloader.BrandingPackageName2);
            brandingPackageElement2.SetAttributeValue(
                XName.Get("href", string.Empty),
                string.Format(CultureInfo.InvariantCulture, @"packages\{0}.cab", Downloader.BrandingPackageName2));

            //var lastModifiedFmtBook = (book.LastModified.Millisecond % 10) == 0 ? "yyyy-MM-ddThh:mm:ss.ffZ" : "yyyy-MM-ddThh:mm:ss.fffZ";
            var lastModifiedFmtBook = "s";
            XElement bookLastModified = CreateElement("span", "last-modified", book.LastModified.ToUniversalTime().ToString(lastModifiedFmtBook, CultureInfo.InvariantCulture));

            //descElement.Add(
            //    bookLastModified,
            //    iconElement
            //    //,
            //    //productLinkElement,
            //    //productGroupsLinkElement
            //);

            detailsElement.Add(
                CreateElement("span", "vendor", "Microsoft"),
                CreateElement("span", "locale", book.Locale),
                CreateElement("span", "name", book.Name),
                descElement,
                bookLastModified,
                iconElement,
                //productLinkElement,
                //productGroupsLinkElement,
                brandingPackageElement1,
                brandingPackageElement2
                );

            var packageListElement = CreateElement("div", "package-list", null);
            //packageListElement.Add(new XText(book.PackagesBeforeContext));

            var packages = book.Packages as List<Package>;
            packages?.Sort();
            foreach (Package package in book.Packages)
            {
                XElement packageElement = CreateElement("div", "package", null);
                //var lastModifiedFmt = (package.LastModified.Millisecond % 10) == 0 ? "yyyy-MM-ddThh:mm:ss.ffZ" : "yyyy-MM-ddThh:mm:ss.fffZ";
                var lastModifiedFmt = "s";
                XElement lastModified = CreateElement("span", "last-modified", package.LastModified.ToUniversalTime().ToString(lastModifiedFmt, CultureInfo.InvariantCulture));
                //XElement lastModified = new XElement("span", package.LastModified);

                string curlink;
                if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                    curlink = string.Format(CultureInfo.InvariantCulture, "packages/en-us/{0}", package.CreateFileNameUri());
                else if (package.Name.ToLowerInvariant().Contains(objLocale.Locale.ToLowerInvariant()))
                    curlink = string.Format(CultureInfo.InvariantCulture, "packages/{0}/{1}", objLocale.Locale.ToLowerInvariant(), package.CreateFileNameUri());
                else
                    curlink = string.Format(CultureInfo.InvariantCulture, "packages/{0}", package.CreateFileNameUri());

                string constituentLink;
                if (package.Name.ToLowerInvariant().Contains(@"en-us"))
                    constituentLink = string.Format(CultureInfo.InvariantCulture, "packages/en-us/{0}", package.Name);
                else if (package.Name.ToLowerInvariant().Contains(objLocale.Locale.ToLowerInvariant()))
                    constituentLink = string.Format(CultureInfo.InvariantCulture, "packages/{0}/{1}", objLocale.Locale.ToLowerInvariant(), package.Name);
                else
                    constituentLink = string.Format(CultureInfo.InvariantCulture, "packages/{0}", package.Name);

                XElement currentLinkElement = CreateElement("a", "current-link", package.CreateFileName());
                currentLinkElement.SetAttributeValue(XName.Get("href", string.Empty), curlink);

                XElement constituentLinkElement = CreateElement("a", "package-constituent-link", package.Name);
                constituentLinkElement.SetAttributeValue(XName.Get("href", string.Empty), constituentLink);

                packageElement.Add(
                    CreateElement("span", "name", package.Name),
                    //new XText(package.DeployedBeforeContext),
                    CreateElement("span", "deployed", package.Deployed),
                    lastModified,
                    CreateElement("span", "package-etag", package.PackageEtag),
                    currentLinkElement,
                    CreateElement("span", "package-size-bytes", package.PackageSizeBytes.ToString()),
                    CreateElement("span", "package-size-bytes-uncompressed", package.PackageSizeBytesUncompressed.ToString()),
                    //new XText(package.ConstituentLinkBeforeContext),
                    constituentLinkElement
                    //,
                    //new XText(package.ConstituentLinkAfterContext)
                );

                packageListElement.Add(packageElement);
            }

            var divElement = CreateElement("div", null, null);
            divElement.SetAttributeValue(XName.Get("id", string.Empty), "GWDANG_HAS_BUILT");

            bodyElement.Add(
                detailsElement,
                packageListElement
                ,
                divElement
            );

            document.Root?.Add(headElement, bodyElement);

            return document.ToStringWithDeclaration();
        }

        /// <summary>
        /// Create a new xml element
        /// </summary>
        /// <param name="name">
        /// The name of the element
        /// </param>
        /// <param name="className">
        /// The class attribute value (may be null)
        /// </param>
        /// <param name="value">
        /// The element content (may be null)
        /// </param>
        /// <returns>
        /// The created element
        /// </returns>
        private static XElement CreateElement(string name, string className, string value)
        {
            XElement element = new XElement(XName.Get(name, "http://www.w3.org/1999/xhtml"));

            if (className != null)
            {
                element.SetAttributeValue(XName.Get("class", string.Empty), className);
            }

            if (value != null)
            {
                element.Value = value;
            }

            return element;
        }

        /// <summary>
        /// Get the name of the class of an xml element
        /// </summary>
        /// <param name="element">
        /// The element to get the class name of
        /// </param>
        /// <returns>
        /// The class name or null if there is no class name
        /// </returns>
        private static string GetClassName(this XElement element)
        {
            return GetAttributeValue(element, "class");
        }

        /// <summary>
        /// The the value of an attribute of an xml element
        /// </summary>
        /// <param name="element">
        /// The element to get the attribute value from
        /// </param>
        /// <param name="name">
        /// The name of the attrbute to query
        /// </param>
        /// <returns>
        /// The attribute value or null if the attribute was not found
        /// </returns>
        private static string GetAttributeValue(this XElement element, string name)
        {
            XAttribute attribute = element.Attribute(XName.Get(name, string.Empty));

            return attribute?.Value;
        }

        /// <summary>
        /// Get the value of the first child element of the specified element with the class attribute that matched
        /// the specified name
        /// </summary>
        /// <param name="element">
        /// The element to get the child class value from
        /// </param>
        /// <param name="name">
        /// The class name to find
        /// </param>
        /// <returns>
        /// The value of the child class element
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If there was no child element with the class attribute
        /// </exception>
        private static string GetChildClassValue(this XElement element, string name)
        {
            string value = string.Empty;
            try
            {
                //XElement result = element.Elements().Where(x => x.GetClassName() == name).Take(1).Single();
                XElement result = element.Elements().Where(x => x.GetClassName()?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false).Take(1).Single();
                value = result?.Value;
            }
            catch (Exception)
            {
                // ignored
            }

            return value;
        }

        /// <summary>
        /// Get the value of the first child element of the specified element with the class attribute that matched
        /// the specified name
        /// </summary>
        /// <param name="element">
        /// The element to get the child class value from
        /// </param>
        /// <param name="name">
        /// The class name to find
        /// </param>
        /// <returns>
        /// The value of the child class element
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If there was no child element with the class attribute
        /// </exception>
        private static string GetChildClassFirstNodeContext(this XElement element, string name)
        {
            string value = string.Empty;
            try
            {
                //XText result = element.Elements().Where(x => x.GetClassName() == name).Take(1).Single().FirstNode as XText;
                XText result = element.Elements().Where(x => x.GetClassName()?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false).Take(1).Single().FirstNode as XText;
                value = result?.Value;
            }
            catch (Exception)
            {
                // ignored
            }

            return value;
        }

        /// <summary>
        /// Get the value of the first child element of the specified element with the class attribute that matched
        /// the specified name
        /// </summary>
        /// <param name="element">
        /// The element to get the child class value from
        /// </param>
        /// <param name="name">
        /// The class name to find
        /// </param>
        /// <returns>
        /// The value of the child class element
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If there was no child element with the class attribute
        /// </exception>
        private static string GetChildClassBeforeContext(this XElement element, string name)
        {
            string value = string.Empty;
            try
            {
                //XText result = element.Elements().Where(x => x.GetClassName() == name).Take(1).Single().PreviousNode as XText;
                XText result = element.Elements().Where(x => x.GetClassName()?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false).Take(1).Single().PreviousNode as XText;
                value = result?.Value;
            }
            catch (Exception)
            {
                // ignored
            }

            return value;
        }

        /// <summary>
        /// Get the value of the first child element of the specified element with the class attribute that matched
        /// the specified name
        /// </summary>
        /// <param name="element">
        /// The element to get the child class value from
        /// </param>
        /// <param name="name">
        /// The class name to find
        /// </param>
        /// <returns>
        /// The value of the child class element
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If there was no child element with the class attribute
        /// </exception>
        private static string GetChildClassAfterContext(this XElement element, string name)
        {
            string value = string.Empty;
            try
            {
                //XText result = element.Elements().Where(x => x.GetClassName() == name).Take(1).Single().NextNode as XText;
                XText result = element.Elements().Where(x => x.GetClassName()?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false).Take(1).Single().NextNode as XText;
                value = result?.Value;
            }
            catch (Exception)
            {
                // ignored
            }

            return value;
        }

        /// <summary>
        /// Get the value of the specified attribute of the first child element of the specified element with the 
        /// class attribute that matched the specified name
        /// </summary>
        /// <param name="element">
        /// The element to get the child attribute value from
        /// </param>
        /// <param name="name">
        /// The class name to find
        /// </param>
        /// <param name="attribute">
        /// The attribute name to find
        /// </param>
        /// <returns>
        /// The value of the attribute
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If there was no child element with the class attribute
        /// </exception>
        private static string GetChildClassAttributeValue(this XElement element, string name, string attribute)
        {
            string value = string.Empty;
            try
            {
                //XElement result = element.Elements().Where(x => x.GetClassName() == name).Take(1).Single();
                XElement result = element.Elements().Where(x => x.GetClassName()?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false).Take(1).Single();
                value = result?.GetAttributeValue(attribute);
            }
            catch (Exception)
            {
                // ignored
            }

            return value;
        }

        /// <summary>
        /// XDocument extension method to get the XML text including the declaration from an XDocument
        /// </summary>
        /// <param name="document">
        /// The document to get the xml text from
        /// </param>
        /// <returns>
        /// The xml text for the document
        /// </returns>
        private static string ToStringWithDeclaration(this XDocument document)
        {
            return document.Declaration == null ?
                document.ToString() :
                string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", document.Declaration.ToString(), Environment.NewLine, document.ToString(SaveOptions.None));
        }

        /*
        private static string CreateCodeFromUrl(string url)// unused
        {
            if (string.IsNullOrEmpty(url))
                return null;

            string[] parts = url.Split('/');

            return parts.Length > 0 ? parts[parts.Length - 1] : null;
        }
        */
    }
}
