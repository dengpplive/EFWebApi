namespace YSL.Api.Document
{
    /// <summary>
    /// Api文档参数
    /// </summary>
    public class ApiDocumentParameter
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public bool IsOptional { get; set; }

        public string Description { get; set; }
    }
}
