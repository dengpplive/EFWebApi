namespace YSL.Api.Document
{
    /// <summary>
    /// 定API提供文档的方法
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// 获取文档样例对象
        /// </summary>
        /// <returns></returns>
        object GetSampleObject();
    }
}
