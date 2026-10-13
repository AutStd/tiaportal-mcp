using Siemens.Engineering;
using System;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseBlockInfo : ResponseAttributes
    {
        /// <summary>Root-relative path, e.g. "1_Tests/FC_Block_1". Feed back into the block tools.</summary>
        public string? Path { get; set; }
        public string? TypeName { get; set; }
        public string? Name { get; set; }
        public string? Namespace { get; set; }
        public string? ProgrammingLanguage { get; set; }
        public string? MemoryLayout { get; set; }
        public bool? IsConsistent { get; set; }
        public string? HeaderName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsKnowHowProtected { get; set; }
        public string? Description { get; set; }
    }
}
