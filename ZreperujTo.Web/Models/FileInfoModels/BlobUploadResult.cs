using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.FileInfoModels
{
    public class BlobUploadResult
    {
        public string FileName { get; set; }
        public string Uri { get; set; }
        public long? Size { get; set; }
    }
}
