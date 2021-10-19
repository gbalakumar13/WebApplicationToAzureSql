using System;
using System.IO;

namespace DotNetAppSqlDb.Controllers
{
    public class BlobInformation
    {
        public BlobInformation()
        {
        }
        public string BlobName
        {
            get
            {
                return BlobUri.Segments[BlobUri.Segments.Length - 1];
            }
        }
        public string BlobNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(BlobName);
            }
        }

        public int Id { get; set; }
        public Uri BlobUri { get; set; }
    }
}