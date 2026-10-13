using Siemens.Engineering;
using System;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseTagTableInfo : ResponseAttributes
    {
        /// <summary>Root-relative path, e.g. "TagGroup1/Table1". Feed back into the tag tools.</summary>
        public string? Path { get; set; }
        public string? Name { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? ModifiedTimeStamp { get; set; }
        public int? TagCount { get; set; }
        public int? UserConstantCount { get; set; }
        public int? SystemConstantCount { get; set; }
    }
}
