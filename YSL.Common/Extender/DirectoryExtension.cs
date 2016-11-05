namespace YSL.Common.Extender
{
    using System.IO;
    using System.Threading.Tasks;
    /// <summary>
    /// 目录 扩展
    /// </summary>
    public static class DirectoryExtension
    {
        public static void CopyTo(this DirectoryInfo originalDirectory, string targetDirectoryFullName)
        {
            if (!Directory.Exists(targetDirectoryFullName))
            {
                Directory.CreateDirectory(targetDirectoryFullName);
            }
            Parallel.Invoke(
                () => copyFileOnCurrentFold(originalDirectory, targetDirectoryFullName),
                () => copySubFold(originalDirectory, targetDirectoryFullName));
        }
        static void copyFileOnCurrentFold(DirectoryInfo originalDirectory, string targetDirectoryFullName)
        {
            Parallel.ForEach(originalDirectory.GetFiles(), file =>
            {
                var targetFileFullName = Path.Combine(targetDirectoryFullName, file.Name);
                file.MoveTo(targetFileFullName);
            });
        }
        static void copySubFold(DirectoryInfo originalDirectory, string targetDirectoryFullName)
        {
            Parallel.ForEach(originalDirectory.GetDirectories(), directory =>
            {
                var targetSubFoldFullName = Path.Combine(targetDirectoryFullName, directory.Name);
                directory.CopyTo(targetSubFoldFullName);
            });
        }
    }
}
