namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseConstantInfo
    {
        public string? Name { get; set; }

        /// <summary>"User" or "System". System constants are read-only in Openness.</summary>
        public string? Kind { get; set; }
        public string? DataTypeName { get; set; }

        /// <summary>
        /// PlcConstant.Value is 'object'; it is stringified so the structured output schema
        /// stays stable across data types.
        /// </summary>
        public string? Value { get; set; }
        public string? Comment { get; set; }
    }
}
