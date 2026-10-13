using Siemens.Engineering;
using System;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseTypeInfo : ResponseAttributes
    {
        /// <summary>Root-relative path, e.g. "Common/BtnTyp_X". Feed back into the type tools.</summary>
        public string? Path { get; set; }
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public string? Namespace { get; set; }
        public bool? IsConsistent { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsKnowHowProtected { get; set; }
        public string? Description { get; set; }
    }
}
