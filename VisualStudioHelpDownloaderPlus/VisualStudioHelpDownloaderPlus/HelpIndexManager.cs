namespace VisualStudioHelpDownloaderPlus
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;

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
        public static ICollection<Catalog> LoadCatalogs( byte[] data )
		{
			XDocument document;
            List<Catalog> result = new List<Catalog>();

			if ( data == null )
			{
				throw new ArgumentNullException( "data" );
			}

			using ( MemoryStream stream = new MemoryStream( data ) )
			{
				document = XDocument.Load( stream );
			}

			if ( document.Root != null )
			{
				IEnumerable<XElement> query =
					document.Root.Elements()
                            .Where( x => x.GetClassName() == "catalogs" )
							.Take( 1 )
							.Single()
							.Elements()
                            .Where( x => x.GetClassName() == "catalog-list" )
							.Take( 1 )
							.Single()
							.Elements()
                            .Where( x => x.GetClassName() == "catalog" );

				result.AddRange(
					query.Select(
						x =>
                        new Catalog
							{
                                Name = x.GetChildClassValue( "name" ),
                                Description = x.GetChildClassValue( "description" ),
                                Catalog_Link = x.GetChildClassAttributeValue( "catalog-link", "href" )
							} ) );
			}
			else
			{
				throw new XmlException( "Missing document root" );
			}

			return result;
		}

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
        public static ICollection<Locale> LoadLocales(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            List<Locale> result = new List<Locale>();
            XDocument document;

            using (MemoryStream stream = new MemoryStream(data))
            {
                document = XDocument.Load(stream);
            }

            if (document.Root != null)
            {
                IEnumerable<XElement> query =
                    document.Root.Elements()
                            .Where(x => x.GetClassName() == "catalogLocales")
                            .Take(1)
                            .Single()
                            .Elements()
                            .Where(x => x.GetClassName() == "catalog-locale-list")
                            .Take(1)
                            .Single()
                            .Elements()
                            .Where(x => x.GetClassName() == "catalog-locale");

                result.AddRange(
                    query.Select(
                        x =>
                        new Locale
                        {
                            Code = x.GetChildClassValue("locale"),
                            CatalogName = x.GetChildClassValue("catalog-name"),
                            Description = x.GetChildClassValue("description"),
                            Locale_Link = x.GetChildClassAttributeValue("locale-link", "href")
                        }));
            }
            else
            {
                throw new XmlException("Missing document root");
            }

            return result;
        }

		/// <summary>
		/// Find the available book groups, books, and packages for the selected language
		/// </summary>
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
		public static ICollection<BookGroup> LoadBooks( byte[] data )
		{
			if ( data == null )
			{
				throw new ArgumentNullException( "data" );
			}

            XDocument document;
            List<BookGroup> result = new List<BookGroup>();

			using ( MemoryStream stream = new MemoryStream( data ) )
			{
				document = XDocument.Load( stream );
			}
            

			if ( document.Root != null )
			{
				IEnumerable<XElement> groups =
					document.Root.Elements()
							.Where( x => x.GetClassName() == "book-list" )
							.Take( 1 )
							.Single()
							.Elements()
							.Where( x => x.GetClassName() == "book-groups" )
							.Take( 1 )
							.Single()
							.Elements()
							.Where( x => x.GetClassName() == "book-group" );
				foreach ( XElement group in groups )
				{
					BookGroup bookGroup = new BookGroup
										{
											Code = group.GetChildClassValue( "id" ),
											Name = group.GetChildClassValue( "name" ),
                                            Vendor = group.GetChildClassValue( "vendor" ),
											Books = new List<Book>()
										};

					result.Add( bookGroup );

					IEnumerable<XElement> books = group.Elements().Where( x => x.GetClassName() == "book" );
					foreach ( XElement book in books )
					{
                        Book b = new Book
                        {
                            Code = book.GetChildClassValue( "id" ),
                            Locale = new Locale { Code = book.GetChildClassValue( "locale" ) },
                            //Local = book.GetChildClassValue( "locale" ),
                            Name = book.GetChildClassValue( "name" ),
                            Description = book.GetChildClassValue( "description" ),
                            BrandingPackageName = book.GetChildClassValue( "BrandingPackageName" ),
                            Paths = new List<MSDNPath>(),
                            Packages = new List<Package>()
                        };

                        IEnumerable<XElement> paths =
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

                        foreach (XElement path in paths)
                        {
                            MSDNPath p = new MSDNPath
                                            {
                                                Languages = path.GetChildClassValue( "languages" ),
                                                Membership = path.GetChildClassValue( "membership" ),
                                                Name = path.GetChildClassValue( "name" ),
                                                //Priority = path.GetChildClassValue( "priority" ),
                                                Priority = long.Parse( path.GetChildClassValue( "priority" ), CultureInfo.InvariantCulture ),
                                                //SkuId = path.GetChildClassValue( "skuId" ),
                                                SkuId = long.Parse( path.GetChildClassValue( "skuId" ), CultureInfo.InvariantCulture ),
                                                SkuName = path.GetChildClassValue( "skuName" )
                                            };
											
											if ( p.SkuName == "Enterprise" )
												p.SkuId = 3000;
											else if ( p.SkuName == "Professional" )
												p.SkuId = 2000;
											else if ( p.SkuName == "Standard" )
												p.SkuId = 1000;
											else if ( p.SkuName == "Express" )
												p.SkuId = 500;
											
                            b.Paths.Add(p);

                            string bookPath = p.Name.TrimStart(new[] { '\\' });
                            b.Category = bookPath;
                        }

						IEnumerable<XElement> packages =
							book.Elements()
								.Where( x => x.GetClassName() == "packages" )
								.Take( 1 )
								.Single()
								.Elements()
								.Where( x => x.GetClassName() == "package" );
						foreach ( XElement package in packages )
						{
							Package pa = new Package
											{
                                                PackageType = package.GetChildClassValue( "packageType" ),
                                                PackageFormat = package.GetChildClassValue( "packageFormat" ),
												Name = package.GetChildClassValue( "name" ),
                                                Deployed = package.GetChildClassValue( "deployed" ),
												LastModified = DateTime.Parse( package.GetChildClassValue( "last-modified" ), CultureInfo.InvariantCulture ),
												Tag = package.GetChildClassValue( "package-etag" ),
                                                Link = package.GetChildClassAttributeValue( "current-link", "href" ),
                                                Size = long.Parse( package.GetChildClassValue( "package-size-bytes" ), CultureInfo.InvariantCulture ),// unused
                                                UncompressedSize = long.Parse( package.GetChildClassValue( "package-size-bytes-uncompressed" ), CultureInfo.InvariantCulture ),// unused
                                                ConstituentLink = package.GetChildClassAttributeValue( "package-constituent-link", "href" )
											};

							b.Packages.Add( pa );
						}

                        bookGroup.Books.Add(b);
					}
				}


			}
			else
			{
				throw new XmlException( "Missing document root" );
			}

			return result;
		}

		/// <summary>
		/// Creates main help setup index.
		/// </summary>
		/// <param name="bookGroups">
		/// A collection of book groups to add to the index
		/// </param>
		/// <returns>
		/// The xml document text
		/// </returns>
        public static string CreateSetupIndex( IEnumerable<BookGroup> bookGroups, string nameCatalog, string codeLocale )
		{
			XDocument document = new XDocument( new XDeclaration( "1.0", "utf-8", null ), CreateElement( "html", null, null ) );

			XElement headElement = CreateElement( "head", null, null );
			XElement metaDateElemet1 = CreateElement( "meta", null, null );
            metaDateElemet1.SetAttributeValue( XName.Get( "name", string.Empty ), "ROBOTS" );
            metaDateElemet1.SetAttributeValue( XName.Get( "content", string.Empty ), "NOINDEX, NOFOLLOW" );

            XElement metaDateElemet2 = CreateElement( "meta", null, null );
            metaDateElemet2.SetAttributeValue( XName.Get( "http-equiv", string.Empty ), "Content-Location" );
            metaDateElemet2.SetAttributeValue(
                    XName.Get( "content", string.Empty ),
                    string.Format( CultureInfo.InvariantCulture, @"http://services.mtps.microsoft.com/serviceapi/catalogs/{0}/{1}",
                    nameCatalog.ToLowerInvariant( ), codeLocale));

            XElement linkElement = CreateElement( "link", null, null );
            linkElement.SetAttributeValue( XName.Get( "type", string.Empty ), "text/css" );
            linkElement.SetAttributeValue( XName.Get( "rel", string.Empty ), "stylesheet" );
            linkElement.SetAttributeValue( XName.Get( "href", string.Empty), "../../styles/global.css" );

            XElement titleElement = CreateElement( "title", null, "All Book Listings" );

			headElement.Add( metaDateElemet1 );
            headElement.Add( metaDateElemet2 );
            headElement.Add( linkElement );
            headElement.Add( titleElement );

			XElement bodyElement = CreateElement( "body", "book-list", null );
			XElement detailsElement = CreateElement( "div", "details", null );

            XElement catalogLocaleLinkElement = CreateElement( "a", "catalog-locale-link", "Catalog locales" );
            catalogLocaleLinkElement.SetAttributeValue( XName.Get( "href", string.Empty ),
                string.Format(CultureInfo.InvariantCulture, @"../../catalogs/{0}", nameCatalog.ToLowerInvariant()));

            detailsElement.Add( catalogLocaleLinkElement );
			
			XElement bookgroupsElement = CreateBookGroupBooksIndex ( bookGroups, codeLocale );
			
			bodyElement.Add(detailsElement, bookgroupsElement);
			if ( document.Root != null )
			{
				document.Root.Add( headElement, bodyElement );
			}

			return document.ToStringWithDeclaration();
		}
        //public static string CreateSetupIndex( IEnumerable<BookGroup> bookGroups )
        //{
        //    XDocument document = new XDocument( new XDeclaration( "1.0", "utf-8", null ), CreateElement( "html", null, null ) );

        //    //XElement bodyElement = CreateElement( "body", "product-list", null );
        //    XElement bodyElement = CreateElement( "body", "book-list", null );

        //    foreach ( BookGroup bookGroup in bookGroups )
        //    {
        //        XElement productElement = CreateElement( "div", "product", null );

        //        XElement linkElement = CreateElement( "a", "product-link", null );
        //        linkElement.SetAttributeValue( XName.Get( "href", string.Empty ), bookGroup.CreateFileName() );

        //        productElement.Add(
        //            CreateElement( "span", "name", bookGroup.Name ), 
        //            CreateElement( "span", "locale", bookGroup.Locale.Code ), 
        //            CreateElement( "span", "description", bookGroup.Description ), 
        //            linkElement );

        //        bodyElement.Add( productElement );
        //    }

        //    if ( document.Root != null )
        //    {
        //        document.Root.Add( bodyElement );
        //    }

        //    return document.ToStringWithDeclaration();
        //}

		/// <summary>
		/// Create book group books index.
		/// </summary>
		/// <param name="bookGroup">
		/// The book group to create the index for.
		/// </param>
		/// <returns>
		/// The xml document text
		/// </returns>
		public static XElement CreateBookGroupBooksIndex( IEnumerable<BookGroup> bookGroups, string codeLocale )
		{
			XElement bookgroupsElement = CreateElement( "div", "book-groups", null );

            foreach (BookGroup bookGroup in bookGroups)
            {
                XElement bookgroupElement = CreateElement( "div", "book-group", null );
                bookgroupElement.Add(
                    CreateElement( "span", "id", bookGroup.Code ),
                    CreateElement( "span", "name", bookGroup.Name ),
                    CreateElement( "span", "vendor", bookGroup.Vendor )
                    );

                foreach (Book book in bookGroup.Books)
                {
                    if ( !book.Wanted )
                    {
                        continue;
                    }
						
                    bookgroupElement.Add( CreateBookPackagesIndex( book, codeLocale ) );
                }

                bookgroupsElement.Add(bookgroupElement);
            }
			
			return bookgroupsElement;
		}
		
        //public static string CreateBookGroupBooksIndex( BookGroup bookGroup )
        //{
        //    XDocument document = new XDocument( new XDeclaration( "1.0", "utf-8", null ), CreateElement( "html", null, null ) );

        //    XElement headElement = CreateElement( "head", null, null );
        //    XElement metaDateElemet = CreateElement( "meta", null, null );
        //    metaDateElemet.SetAttributeValue( XName.Get( "http-equiv", string.Empty ), "Date" );
        //    metaDateElemet.SetAttributeValue( XName.Get( "content", string.Empty ), DateTime.Now.ToString( "R", CultureInfo.InvariantCulture ) );
        //    headElement.Add( metaDateElemet );

        //    XElement bodyElement = CreateElement( "body", "product", null );
        //    XElement detailsElement = CreateElement( "div", "details", null );
        //    detailsElement.Add(
        //        CreateElement( "span", "name", bookGroup.Name ), 
        //        CreateElement( "span", "locale", bookGroup.Locale.Code ), 
        //        CreateElement( "span", "description", bookGroup.Description ) );
        //    XElement bookListElement = CreateElement( "div", "book-list", null );

        //    foreach ( Book book in bookGroup.Books )
        //    {
        //        if ( book.Wanted )
        //        {
        //            XElement bookElement = CreateElement( "div", "book", null );

        //            XElement linkElement = CreateElement( "a", "book-link", null );
        //            linkElement.SetAttributeValue( XName.Get( "href", string.Empty ), book.  CreateFileName() );

        //            bookElement.Add(
        //                CreateElement( "span", "name", book.Name ),
        //                CreateElement( "span", "locale", book.Locale.Code ),
        //                CreateElement( "span", "description", book.Description ),
        //                linkElement );

        //            bookListElement.Add( bookElement );					
        //        }
        //    }

        //    bodyElement.Add( detailsElement, bookListElement );
        //    if ( document.Root != null )
        //    {
        //        document.Root.Add( headElement, bodyElement );
        //    }

        //    return document.ToStringWithDeclaration();
        //}

		/// <summary>
		/// Create book packages index.
		/// </summary>
		/// <param name="bookGroup">
		/// The book Group associated with the book.
		/// </param>
		/// <param name="book">
		/// The book associated with the packages
		/// </param>
		/// <returns>
		/// The xml document text
		/// </returns>
		public static XElement CreateBookPackagesIndex( Book book, string codeLocale )
		{
			XElement bookElement = CreateElement( "div", "book", null );
								
			XElement propertiesElement = CreateElement( "div", "properties", null );
			XElement pathsElement = CreateElement( "div", "paths", null );
			foreach (MSDNPath path in book.Paths)
			{
				XElement pathElement = CreateElement("div", "path", null);

				pathElement.Add(
					CreateElement( "span", "languages", path.Languages ),
					CreateElement( "span", "membership", path.Membership ), 
					CreateElement( "span", "name", path.Name ), 
					CreateElement( "span", "priority", path.Priority.ToString() ),
					CreateElement( "span", "skuId", path.SkuId.ToString() ),
					CreateElement( "span", "skuName", path.SkuName )
					);

				pathsElement.Add( pathElement );
			}
			propertiesElement.Add( pathsElement );

			XElement packageListElement = CreateElement( "div", "packages", null );
			string packageListTick = @"The following packages are available: in this book:";
			//packageListElement.Add( new XText( packageListTick ) );
			//packageListElement.Value = packageListTick;
			//XElement child1 = packageListElement.Element( "div" );
			//child1.( packageListTick );
			//child1.AddAfterSelf( new XElement( packageListTick ) );
			
			string deployedTick = @"Deployed: ";
			string leftRoundBracketsTick = @"(Package Constituents: ";
			string rightRoundBracketsTick = @")";
					
			foreach ( Package package in book.Packages )
			{
				XElement packageElement = CreateElement( "div", "package", null );

				XElement currentLinkElement = CreateElement( "a", "current-link", package.CreateFileName() );

				string curlink = string.Format( CultureInfo.InvariantCulture, "packages/{0}", ( package.CreateFileName() ).ToLowerInvariant() );
				if ( package.Name.ToLowerInvariant().Contains( @"en-us" ) )
					curlink = string.Format( CultureInfo.InvariantCulture, "packages/en-us/{0}", ( package.CreateFileName() ).ToLowerInvariant() );
				else if ( package.Name.ToLowerInvariant().Contains( codeLocale.ToLowerInvariant() ) )
					curlink = string.Format( CultureInfo.InvariantCulture, "packages/{0}/{1}", codeLocale.ToLowerInvariant() , ( package.CreateFileName() ).ToLowerInvariant());
				else
					curlink = string.Format( CultureInfo.InvariantCulture, "packages/{0}", ( package.CreateFileName() ).ToLowerInvariant() );
			
				currentLinkElement.SetAttributeValue(
					XName.Get( "href", string.Empty ), 
					curlink
					);

				XElement constituentLinkElement = CreateElement("a", "package-constituent-link", package.Name);
				constituentLinkElement.SetAttributeValue( XName.Get( "href", string.Empty), package.ConstituentLink );
				
				XElement lastModified;
				if ( ( package.LastModified.Millisecond % 10 ) == 0 )
					lastModified = CreateElement( "span", "last-modified", package.LastModified.ToUniversalTime().ToString( "yyyy-MM-ddThh:mm:ss.ffZ", CultureInfo.InvariantCulture ) );
				else
					lastModified = CreateElement( "span", "last-modified", package.LastModified.ToUniversalTime().ToString( "yyyy-MM-ddThh:mm:ss.fffZ", CultureInfo.InvariantCulture ) ); 
				
				packageElement.Add(
					CreateElement( "span", "packageType", package.PackageType ),							
					CreateElement( "span", "packageFormat", package.PackageFormat), 
					CreateElement( "span", "name", package.Name ),
					//new XText( "Deployed: " ),
					CreateElement( "span", "deployed", package.Deployed ),
					lastModified,
					//CreateElement( "span", "last-modified", package.LastModified.ToUniversalTime().ToString( "{O:G}", CultureInfo.InvariantCulture ) ), 
					//CreateElement( "span", "last-modified", package.LastModified.ToUniversalTime().ToString( "yyyy-MM-ddThh:mm:ss.fffZ", CultureInfo.InvariantCulture ) ), 
					CreateElement( "span", "package-etag", package.Tag ),
					currentLinkElement,
					CreateElement( "span", "package-size-bytes", package.Size.ToString() ),
					CreateElement( "span", "package-size-bytes-uncompressed", package.UncompressedSize.ToString() ),
					//new XText( @"(Package Constituents: " ),
					constituentLinkElement
					//new XText( @")" )						
					);
					
				packageListElement.Add( packageElement );
			}
			
			bookElement.Add(
				CreateElement( "span", "id", book.Code ),
				CreateElement( "span", "locale", book.Locale.Code ),
				CreateElement( "span", "name", book.Name ),
				CreateElement( "span", "description", book.Description ),
				CreateElement( "span", "BrandingPackageName", book.BrandingPackageName ),
				propertiesElement,
				packageListElement
				);
				
			return bookElement;
		}

        public static string CreateBookPackagesIndex( IEnumerable<BookGroup> bookGroups, string nameCatalog, string codeLocale/*bookGroups BookGroup bookGroup, Book book*/ )
		{
			XDocument document = new XDocument( new XDeclaration( "1.0", "utf-8", null ), CreateElement( "html", null, null ) );

			XElement headElement = CreateElement( "head", null, null );
			XElement metaDateElemet1 = CreateElement( "meta", null, null );
            metaDateElemet1.SetAttributeValue( XName.Get( "name", string.Empty ), "ROBOTS" );
            metaDateElemet1.SetAttributeValue( XName.Get( "content", string.Empty ), "NOINDEX, NOFOLLOW" );

            XElement metaDateElemet2 = CreateElement( "meta", null, null );
            metaDateElemet2.SetAttributeValue( XName.Get( "http-equiv", string.Empty ), "Content-Location" );
            metaDateElemet2.SetAttributeValue(
                    XName.Get( "content", string.Empty ),
                    string.Format( CultureInfo.InvariantCulture, "http://services.mtps.microsoft.com/serviceapi/catalogs/{0}/{1}",
                    nameCatalog.ToLowerInvariant( ), codeLocale ) );

            XElement linkElement = CreateElement( "link", null, null );
            linkElement.SetAttributeValue( XName.Get( "type", string.Empty ), "text/css" );
            linkElement.SetAttributeValue( XName.Get( "rel", string.Empty ), "stylesheet" );
            linkElement.SetAttributeValue( XName.Get( "href", string.Empty ), "../../styles/global.css" );

            XElement titleElement = CreateElement( "title", null, "All Book Listings" );

			headElement.Add( metaDateElemet1 );
            headElement.Add( metaDateElemet2 );
            headElement.Add( linkElement );
            headElement.Add( titleElement );

			XElement bodyElement = CreateElement( "body", "book-list", null );
			XElement detailsElement = CreateElement( "div", "details", null );

            XElement catalogLocaleLinkElement = CreateElement( "a", "catalog-locale-link", "Catalog locales" );
            catalogLocaleLinkElement.SetAttributeValue( XName.Get( "href", string.Empty ),
                string.Format( CultureInfo.InvariantCulture, @"../../../catalogs/{0}", nameCatalog.ToLowerInvariant() ) );

            detailsElement.Add( catalogLocaleLinkElement );

            XElement bookgroupsElement = CreateElement( "div", "book-groups", null );

            //foreach (Book book in bookGroup.Books)
            foreach (BookGroup bookGroup in bookGroups)
            {
                XElement bookgroupElement = CreateElement( "div", "book-group", null );
                bookgroupElement.Add(
                    CreateElement( "span", "id", bookGroup.Code ),
                    CreateElement( "span", "name", bookGroup.Name ),
                    CreateElement( "span", "vendor", bookGroup.Vendor )
                    );

                foreach (Book book in bookGroup.Books)
                {
                    if ( !book.Wanted )
                    {
                        continue;
                    }
                    XElement bookElement = CreateElement( "div", "book", null );
                                        
                    XElement propertiesElement = CreateElement( "div", "properties", null );
                    XElement pathsElement = CreateElement( "div", "paths", null );
                    foreach (MSDNPath path in book.Paths)
                    {
                        XElement pathElement = CreateElement("div", "path", null);

                        pathElement.Add(
            		        CreateElement( "span", "languages", path.Languages ),
            		        CreateElement( "span", "membership", path.Membership ), 
            		        CreateElement( "span", "name", path.Name ), 
            		        CreateElement( "span", "priority", path.Priority.ToString() ),
            		        CreateElement( "span", "skuId", path.SkuId.ToString() ),
            		        CreateElement( "span", "skuName", path.SkuName )
                            );

                        pathsElement.Add( pathElement );
                    }
                    propertiesElement.Add( pathsElement );


                    XElement packageListElement = CreateElement( "div", "packages", null );
					string packageListTick = @"The following packages are available: in this book:";
					//packageListElement.Add( new XText( packageListTick ) );
					//packageListElement.Value = packageListTick;
					XElement child1 = packageListElement.Element( "div" );
					//child1.( packageListTick );
					//child1.AddAfterSelf( new XElement( packageListTick ) );
	
					string deployedTick = @"Deployed: ";
					string leftRoundBracketsTick = @"(Package Constituents: ";
					string rightRoundBracketsTick = @")";
						
			        foreach ( Package package in book.Packages )
			        {
				        XElement packageElement = CreateElement( "div", "package", null );

                        XElement currentLinkElement = CreateElement( "a", "current-link", package.CreateFileName() );

						string curlink = string.Format( CultureInfo.InvariantCulture, "packages/{0}", ( package.CreateFileName() ).ToLowerInvariant() );
						if ( package.Name.ToLowerInvariant().Contains( @"en-us" ) )
							curlink = string.Format( CultureInfo.InvariantCulture, "packages/en-us/{0}", ( package.CreateFileName() ).ToLowerInvariant() );
						else if ( package.Name.ToLowerInvariant().Contains( codeLocale.ToLowerInvariant() ) )
							curlink = string.Format( CultureInfo.InvariantCulture, "packages/{0}/{1}", codeLocale.ToLowerInvariant() , ( package.CreateFileName() ).ToLowerInvariant());
						else
							curlink = string.Format( CultureInfo.InvariantCulture, "packages/{0}", ( package.CreateFileName() ).ToLowerInvariant() );
					
                        currentLinkElement.SetAttributeValue(
					        XName.Get( "href", string.Empty ), 
					        curlink
							);

                        XElement constituentLinkElement = CreateElement("a", "package-constituent-link", package.Name);
                        constituentLinkElement.SetAttributeValue( XName.Get( "href", string.Empty), package.ConstituentLink );
						
						XElement lastModified;
						if ( ( package.LastModified.Millisecond % 10 ) == 0 )
						    lastModified = CreateElement( "span", "last-modified", package.LastModified.ToUniversalTime().ToString( "yyyy-MM-ddThh:mm:ss.ffZ", CultureInfo.InvariantCulture ) );
						else
						    lastModified = CreateElement( "span", "last-modified", package.LastModified.ToUniversalTime().ToString( "yyyy-MM-ddThh:mm:ss.fffZ", CultureInfo.InvariantCulture ) ); 
						
				        packageElement.Add(
                            CreateElement( "span", "packageType", package.PackageType ),							
                            CreateElement( "span", "packageFormat", package.PackageFormat), 
					        CreateElement( "span", "name", package.Name ),
							//new XText( @"Deployed: " ),
                            CreateElement( "span", "deployed", package.Deployed ),
							lastModified,
					        CreateElement( "span", "package-etag", package.Tag ),
                            currentLinkElement,
                            CreateElement( "span", "package-size-bytes", package.Size.ToString() ),
                            CreateElement( "span", "package-size-bytes-uncompressed", package.UncompressedSize.ToString() ),
							//new XText( @"(Package Constituents: " ),
                            constituentLinkElement
							//new XText( @")"	)					
                            );
							
				        packageListElement.Add( packageElement );
			        }
					
                    bookElement.Add(
                        CreateElement( "span", "id", book.Code ),
                        CreateElement( "span", "locale", book.Locale.Code ),
                        CreateElement( "span", "name", book.Name ),
                        CreateElement( "span", "description", book.Description ),
                        CreateElement( "span", "BrandingPackageName", book.BrandingPackageName ),
                        propertiesElement,
                        packageListElement
                        );
						
                    bookgroupElement.Add(bookElement);
                }

                bookgroupsElement.Add(bookgroupElement);
            }

            bodyElement.Add(detailsElement, bookgroupsElement);
			if ( document.Root != null )
			{
				document.Root.Add( headElement, bodyElement );
			}

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
		private static XElement CreateElement( string name, string className, string value )
		{
			XElement element = new XElement( XName.Get( name, "http://www.w3.org/1999/xhtml" ) );

			if ( className != null )
			{
				element.SetAttributeValue( XName.Get( "class", string.Empty ), className );
			}

			if ( value != null )
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
		private static string GetClassName( this XElement element )
		{
			return GetAttributeValue( element, "class" );
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
		private static string GetAttributeValue( this XElement element, string name )
		{
			XAttribute attribute = element.Attribute( XName.Get( name, string.Empty ) );

			return attribute == null ? null : attribute.Value;
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
		private static string GetChildClassValue( this XElement element, string name )
		{
			XElement result = element.Elements().Where( x => x.GetClassName() == name ).Take( 1 ).Single();

			return null != result ? result.Value : null;
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
		private static string GetChildClassAttributeValue( this XElement element, string name, string attribute )
		{
			XElement result = element.Elements().Where( x => x.GetClassName() == name ).Take( 1 ).Single();

			return null != result ? result.GetAttributeValue( attribute ) : null;
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
		private static string ToStringWithDeclaration( this XDocument document )
		{
			return document.Declaration == null ?
				document.ToString() :
                string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", document.Declaration.ToString(), Environment.NewLine, document.ToString());
		}

        private static string CreateCodeFromUrl(string url)// unused
        {
            if (null == url)
                return null;

            string[] parts = url.Split('/');

            return parts.Length > 0 ? parts[parts.Length - 1] : null;
        }


	}
}
