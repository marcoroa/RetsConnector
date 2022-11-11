namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.IO;
    using System.Net.Mime;

    public class FileObject : IDisposable
    {
        private bool isDisposed;

        public string ContentId { get; set; }
        public int ObjectId { get; set; }
        public ContentType ContentType { get; set; }
        public string ContentDescription { get; set; }
        public string ContentSubDescription { get; set; }
        public Uri ContentLocation { get; set; }
        public string MemeVersion { get; set; }
        public bool IsPreferred { get; set; }
        public string Extension { get; set; }
        public Stream Content { get; set; }

        public bool IsImage => this.ContentType?.MediaType.StartsWith("image", StringComparison.CurrentCultureIgnoreCase) ?? false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetStream(Stream stream)
        {
            this.Content = stream;

            if (stream != null)
            {
                this.Content.Position = 0;
            }
        }

        public Stream GetStream()
        {
            return this.Content;
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing && this.Content != null)
            {
                this.Content.Close();
                this.Content.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
