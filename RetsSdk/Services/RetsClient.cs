namespace CrestApps.RetsSdk.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml.Linq;
    using CrestApps.RetsSdk.Contracts;
    using CrestApps.RetsSdk.Exceptions;
    using CrestApps.RetsSdk.Helpers.Extensions;
    using CrestApps.RetsSdk.Models;
    using CrestApps.RetsSdk.Models.Enums;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using MimeTypes.Core;

    public class RetsClient : RetsResponseBase<RetsClient>, IRetsClient
    {
        private readonly IRetsRequester requester;
        private readonly IRetsSession session;

        public RetsClient(IRetsSession session, IRetsRequester requester, ILogger<RetsClient> logger)
            : base(logger)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.requester = requester ?? throw new ArgumentNullException(nameof(requester));
        }

        protected Uri GetObjectUri => this.session.Resource.GetCapability(Capability.GetObject);
        protected Uri SearchUri => this.session.Resource.GetCapability(Capability.Search);
        protected Uri GetMetadataUri => this.session.Resource.GetCapability(Capability.GetMetadata);

        public async Task Connect()
        {
            await this.session.Start();
        }

        public async Task Disconnect()
        {
            await this.session.End();
        }

        public Task<SearchResult> Search(SearchRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.GetSearchResult(request);
        }

        public async Task<RetsSystem> GetSystemMetadata()
        {
            var uriBuilder = new UriBuilder(this.GetMetadataUri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("Type", "METADATA-SYSTEM");
            query.Add("ID", "*");
            query.Add("Format", "STANDARD-XML");

            uriBuilder.Query = query.ToString();

            return await this.requester.Get(uriBuilder.Uri, async (response) => await ProcessResponse(response), this.session.Resource);

            async Task<RetsSystem> ProcessResponse(HttpResponseMessage response)
            {
                using (Stream stream = await this.GetStream(response))
                {
                    XDocument doc = XDocument.Load(stream);

                    this.AssertValidReplay(doc.Root);

                    XNamespace ns = doc.Root.GetDefaultNamespace();

                    XElement metaData = doc.Descendants(ns + "METADATA").FirstOrDefault();

                    XElement metadataSystem = metaData.Elements().FirstOrDefault();
                    XElement systemMeta = metadataSystem.Elements().FirstOrDefault();

                    XElement metaDataResource = metadataSystem.Descendants("METADATA-RESOURCE").FirstOrDefault();

                    RetsResourceCollection resources = new RetsResourceCollection();
                    resources.Load(metaDataResource);

                    var system = new RetsSystem()
                    {
                        SystemId = systemMeta.Attribute("SystemID")?.Value,
                        SystemDescription = systemMeta.Attribute("SystemDescription")?.Value,

                        Version = metadataSystem.Attribute("Version")?.Value,
                        Date = DateTime.Parse(metadataSystem.Attribute("Date")?.Value),
                        Resources = resources,
                    };

                    return system;
                }
            }
        }

        public async Task<RetsResourceCollection> GetResourcesMetadata()
        {
            RetsResourceCollection capsule = await this.MakeMetadataRequest<RetsResourceCollection>("METADATA-RESOURCE", "0");

            return capsule;
        }

        public Task<RetsResource> GetResourceMetadata(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                throw new ArgumentNullException($"{resourceId} cannot be null.");
            }

            return GetRetsResource(resourceId);

            async Task<RetsResource> GetRetsResource(string resourceId)
            {
                RetsResourceCollection capsule = await this.GetResourcesMetadata();
                var resource = capsule.Get().FirstOrDefault(x => x.ResourceId.Equals(resourceId, StringComparison.CurrentCultureIgnoreCase)) ?? throw new ResourceDoesNotExistsException();
                return resource;
            }
        }

        public Task<RetsClassCollection> GetClassesMetadata(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                throw new ArgumentNullException($"{resourceId} cannot be null.");
            }

            return this.MakeMetadataRequest<RetsClassCollection>("METADATA-CLASS", resourceId);
        }

        public async Task<RetsObjectCollection> GetObjectMetadata(string resourceId)
        {
            return await this.MakeMetadataRequest<RetsObjectCollection>("METADATA-OBJECT", resourceId);
        }

        public Task<RetsLookupTypeCollection> GetLookupValues(string resourceId, string lookupName)
        {
            return this.MakeMetadataRequest<RetsLookupTypeCollection>("METADATA-LOOKUP_TYPE", string.Format("{0}:{1}", resourceId, lookupName));
        }

        public Task<IEnumerable<RetsLookupTypeCollection>> GetLookupValues(string resourceId)
        {
            return this.MakeMetadataCollectionRequest<RetsLookupTypeCollection>("METADATA-LOOKUP_TYPE", resourceId);
        }

        public Task<RetsFieldCollection> GetTableMetadata(string resourceId, string className)
        {
            return this.MakeMetadataRequest<RetsFieldCollection>("METADATA-TABLE", string.Format("{0}:{1}", resourceId, className));
        }

        public Task<IEnumerable<FileObject>> GetObject(string resource, string type, PhotoId id, bool useLocation = false)
        {
            return this.GetObject(resource, type, new List<PhotoId> { id }, useLocation);
        }

        public Task<IEnumerable<FileObject>> GetObject(string resource, string type, IEnumerable<PhotoId> ids, bool useLocation = false)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource), $"'{nameof(resource)}' cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(nameof(type), $"'{nameof(type)}' cannot be null or empty.");
            }

            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return this.GetFileObjects(resource, type, ids, useLocation);
        }

        public Task<IEnumerable<FileObject>> GetObject(string resource, string type, IEnumerable<PhotoId> ids, int batchSize, bool useLocation = false)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource), $"'{nameof(resource)}' cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException(nameof(type), $"'{nameof(type)}' cannot be null or empty.");
            }

            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return ProcessResultsPages(resource, type, ids, batchSize, useLocation);

            async Task<IEnumerable<FileObject>> ProcessResultsPages(string resource, string type, IEnumerable<PhotoId> ids, int batchSize, bool useLocation)
            {
                List<FileObject> fileObjects = new List<FileObject>();
                IEnumerable<IEnumerable<PhotoId>> pages = ids.Partition(batchSize);
                foreach (var page in pages)
                {
                    // To prevent having to many outstanding requests
                    // we should connect, force round trip on every page
                    IEnumerable<FileObject> files = await this.RoundTrip(async () =>
                    {
                        return await this.GetObject(resource, type, page, useLocation);
                    });

                    fileObjects.AddRange(files);
                }

                return fileObjects;
            }
        }

        public async Task RoundTrip(Func<Task> action)
        {
            try
            {
                if (!this.session.IsStarted())
                {
                    await this.Connect();
                }

                action?.Invoke();
            }
            finally
            {
                await this.Disconnect();
            }
        }

        public async Task<TResult> RoundTrip<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                if (!this.session.IsStarted())
                {
                    await this.Connect();
                }

                TResult result = await action.Invoke();
                return result;
            }
            finally
            {
                await this.Disconnect();
            }
        }

        protected async Task<T> MakeMetadataRequest<T>(string type, string id, string format = "STANDARD-XML")
            where T : class, IRetsCollectionXElementLoader
        {
            var uriBuilder = new UriBuilder(this.GetMetadataUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("Type", type);
            query.Add("ID", id);
            query.Add("Format", format);

            uriBuilder.Query = query.ToString();

            return await this.requester.Get(uriBuilder.Uri, async (response) => await this.ParseMetadata<T>(response), this.session.Resource);
        }

        protected async Task<IEnumerable<T>> MakeMetadataCollectionRequest<T>(string type, string resourceId, string format = "STANDARD-XML")
            where T : class, IRetsCollectionXElementLoader
        {
            var uriBuilder = new UriBuilder(this.GetMetadataUri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("Type", type);
            query.Add("ID", $"{resourceId}:*");
            query.Add("Format", format);

            uriBuilder.Query = query.ToString();

            return await this.requester.Get(uriBuilder.Uri, async (response) => await this.ParseMetadataCollection<T>(response), this.session.Resource);
        }

        protected async Task<T> ParseMetadata<T>(HttpResponseMessage response)
            where T : class, IRetsCollectionXElementLoader
        {
            using (Stream stream = await this.GetStream(response))
            {
                XDocument doc = XDocument.Load(stream);

                this.AssertValidReplay(doc.Root);

                XNamespace ns = doc.Root.GetDefaultNamespace();

                T collection = (T)Activator.CreateInstance(typeof(T));

                XElement metaData = doc.Descendants(ns + "METADATA").FirstOrDefault();

                if (metaData != null)
                {
                    // INSTEAD OF FirstOrDefault
                    // loop over all the elements
                    XElement metaDataNode = metaData.Elements().FirstOrDefault();
                    if (metaDataNode != null)
                    {
                        collection.Load(metaDataNode);
                    }
                }

                return collection;
            }
        }

        protected async Task<IEnumerable<T>> ParseMetadataCollection<T>(HttpResponseMessage response)
            where T : class, IRetsCollectionXElementLoader
        {
            using (Stream stream = await this.GetStream(response))
            {
                XDocument doc = XDocument.Load(stream);

                this.AssertValidReplay(doc.Root);

                XNamespace ns = doc.Root.GetDefaultNamespace();

                var list = new List<T>();

                XElement metaData = doc.Descendants(ns + "METADATA").FirstOrDefault();

                if (metaData != null)
                {
                    // INSTEAD OF FirstOrDefault
                    // loop over all the elements
                    foreach (XElement metaDataNode in metaData.Elements())
                    {
                        T collection = (T)Activator.CreateInstance(typeof(T));

                        collection.Load(metaDataNode);

                        list.Add(collection);
                    }
                }

                return list;
            }
        }

        protected char GetCompactDelimiter(XDocument doc)
        {
            XNamespace ns = doc.Root.GetDefaultNamespace();
            XElement delimiter = doc.Descendants(ns + "DELIMITER").FirstOrDefault();

            if (delimiter == null)
            {
                throw new RetsParsingException("Unable to find the delimiter! Only 'COMPACT' or 'COMPACT-DECODED' are supported when querying the data");
            }

            var delimiterAttribute = delimiter.Attribute("value");

            if (delimiterAttribute != null && int.TryParse(delimiterAttribute.Value, out int value))
            {
                return Convert.ToChar(value);
            }

            return Convert.ToChar(9);
        }

        protected FileObject ProcessMessage(MimePart message)
        {
            var file = new FileObject()
            {
                ContentId = message.ContentId,
                ContentType = new System.Net.Mime.ContentType(message.ContentType.MimeType),
                ContentDescription = message.Headers["Content-Description"],
                ContentSubDescription = message.Headers["Content-Sub-Description"],
                ContentLocation = message.ContentLocation,
                MemeVersion = message.Headers["MIME-Version"],
                Extension = MimeTypeMap.GetExtension(message.ContentType.MimeType),
            };

            if (int.TryParse(message.Headers["Object-ID"], out int objectId))
            {
                file.ObjectId = objectId;
            }
            else
            {
                // This should never happen
                throw new RetsParsingException("For some reason Object-ID does not exists in the response or it is not an integer value as expected");
            }

            if (bool.TryParse(message.Headers["Preferred"], out bool isPreferred))
            {
                file.IsPreferred = isPreferred;
            }

            if (message.ContentLocation == null)
            {
                file.Content = new MemoryStream();
                message.Content.DecodeTo(file.Content);
                file.Content.Position = 0; // This is important otherwise the next seek with start at the end
            }

            return file;
        }

        private async Task<SearchResult> GetSearchResult(SearchRequest request)
        {
            RetsResource resource = await this.GetResourceMetadata(request.SearchType);
            if (resource == null)
            {
                string message = string.Format("The provided '{0}' is not valid. You can get a list of all valid value by calling '{1}' method on the Session object.", nameof(SearchRequest.SearchType), nameof(this.GetResourcesMetadata));
                throw new InvalidOperationException(message);
            }

            var uriBuilder = new UriBuilder(this.SearchUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("SearchType", request.SearchType);
            query.Add("Class", request.Class);
            query.Add("QueryType", request.QueryType);
            query.Add("Count", request.Count.ToString());
            query.Add("Format", request.Format);
            query.Add("Limit", request.Limit.ToString());
            query.Add("StandardNames", request.StandardNames.ToString());
            query.Add("RestrictedIndicator", request.RestrictedIndicator);
            query.Add("Query", request.ParameterGroup.ToString());

            if (request.HasColumns())
            {
                var columns = request.GetColumns().ToList();

                if (!request.HasColumn(resource.KeyField))
                {
                    columns.Add(resource.KeyField);
                }

                query.Add("Select", string.Join(",", columns));
            }

            uriBuilder.Query = query.ToString();

            return await this.requester.Get(uriBuilder.Uri, async (response) => await ProcessResponse(response), this.session.Resource);

            async Task<SearchResult> ProcessResponse(HttpResponseMessage response)
            {
                using (Stream stream = await this.GetStream(response))
                {
                    XDocument doc = XDocument.Load(stream);

                    int code = this.GetReplayCode(doc.Root);

                    this.AssertValidReplay(doc.Root, code);

                    var result = new SearchResult(resource, request.Class, request.RestrictedIndicator);

                    if (code == 0)
                    {
                        char delimiterValue = this.GetCompactDelimiter(doc);

                        XNamespace ns = doc.Root.GetDefaultNamespace();
                        XElement columns = doc.Descendants(ns + "COLUMNS").FirstOrDefault();

                        IEnumerable<XElement> records = doc.Descendants(ns + "DATA");

                        string[] tableColumns = columns.Value.Split(delimiterValue);
                        result.SetColumns(tableColumns);

                        if (resource.ResourceId == "Media")
                        {
                            resource.KeyField = "PHOTOKEY";
                        }

                        foreach (var record in records)
                        {
                            string[] fields = record.Value.Split(delimiterValue);

                            SearchResultRow row = new SearchResultRow(tableColumns, fields, resource.KeyField, request.RestrictedIndicator);

                            result.AddRow(row);
                        }
                    }

                    return result;
                }
            }
        }

        private async Task<IEnumerable<FileObject>> GetFileObjects(string resource, string type, IEnumerable<PhotoId> ids, bool useLocation)
        {
            var uriBuilder = new UriBuilder(this.GetObjectUri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add("Resource", resource);
            query.Add("Type", type);
            query.Add("ID", string.Join(',', ids.Select(x => x.ToString())));
            query.Add("Location", useLocation ? "1" : "0");
            query.Add("ObjectData", "*");

            uriBuilder.Query = query.ToString();

            return await this.requester.Get(uriBuilder.Uri, async (response) => await ProcessResponse(response), this.session.Resource);

            async Task<List<FileObject>> ProcessResponse(HttpResponseMessage response)
            {
                string responseContentType = response.Content.Headers.ContentType.ToString();
                var files = new List<FileObject>();
                if (!ContentType.TryParse(responseContentType, out ContentType documentContentType))
                {
                    return files;
                }

                using (Stream memoryStream = await this.GetStream(response))
                {
                    if (documentContentType.MediaSubtype.Equals("xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // At this point we know there is a problem because Mime response is expected not XML.
                        XDocument doc = XDocument.Load(memoryStream);

                        this.AssertValidReplay(doc.Root);

                        return files;
                    }

                    MimeEntity entity = MimeEntity.Load(documentContentType, memoryStream);
                    if (entity is Multipart multipart)
                    {
                        // At this point we know this is a multi-image response
                        foreach (MimePart part in multipart.OfType<MimePart>())
                        {
                            files.Add(this.ProcessMessage(part));
                        }

                        return files;
                    }

                    if (entity is MimePart message)
                    {
                        // At this point we know this is a single image response
                        files.Add(this.ProcessMessage(message));
                    }
                }

                return files;
            }
        }
    }
}
