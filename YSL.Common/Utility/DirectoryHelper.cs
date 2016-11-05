using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using YSL.Common.Log;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 常用目录操作类
    /// </summary>
    public class DirectoryHelper
    {
        public struct CopyParameter
        {
            /// <summary>
            /// 目标目录
            /// </summary>
            public string Destination;
            /// <summary>
            /// 源目录
            /// </summary>
            public string Source;
            /// <summary>
            /// 存在同名文件是否覆盖该文件
            /// </summary>
            public bool IsOverwrite;
            /// <summary>
            /// 忽略的文件夹名称
            /// </summary>
            public string[] IgnoreFolders;
        }
        /// <summary>
        /// 异步复制目录
        /// </summary>
        /// <param name="cp"></param>
        public static void AsyncCopy(CopyParameter cp)
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadCopy));
            t.Start(cp);
        }

        private static void ThreadCopy(object cp)
        {
            Copy((CopyParameter)cp);
        }

        public static void Copy(string sourcePath, string destPath, bool isOverwrite, bool isAsync, params string[] ignoreFolders)
        {
            CopyParameter cp = new CopyParameter();
            cp.Source = sourcePath;
            cp.Destination = destPath;
            cp.IsOverwrite = isOverwrite;
            cp.IgnoreFolders = ignoreFolders;
            if (isAsync)
                AsyncCopy(cp);
            else
                Copy(cp);
        }

        public static void Copy(CopyParameter cp)
        {
            CopyParameter Info = cp;
            if (!Directory.Exists(Info.Source))
            {
                throw new FileNotFoundException();
            }
            if (!Directory.Exists(Info.Destination))
            {
                Directory.CreateDirectory(Info.Destination);
            }
            string[] DirFiles;
            string[] DirDirs;
            try
            {
                DirFiles = Directory.GetFiles(Info.Source);
                DirDirs = Directory.GetDirectories(Info.Source);
            }
            catch { throw new FileNotFoundException(); }
            foreach (string SingleDir in DirDirs)
            {
                bool isIgnore = false;
                if (cp.IgnoreFolders != null)
                {
                    foreach (string iFolder in cp.IgnoreFolders)
                    {
                        if (SingleDir.ToLower().IndexOf(string.Concat("\\", iFolder.ToLower())) > 0) { isIgnore = true; break; }
                    }
                }
                if (isIgnore) continue;
                string DirName = "\\";
                DirName = string.Concat(DirName, SingleDir.Split('\\')[SingleDir.Split('\\').Length - 1]);
                CopyParameter NextInfo = new CopyParameter();
                NextInfo.Destination = string.Concat(Info.Destination, DirName);
                NextInfo.Source = SingleDir;
                NextInfo.IgnoreFolders = cp.IgnoreFolders;
                NextInfo.IsOverwrite = cp.IsOverwrite;
                Copy(NextInfo);
            }
            foreach (string SingleFile in DirFiles)
            {
                try
                {
                    string FileName = SingleFile.Split('\\')[SingleFile.Split('\\').Length - 1];
                    string destFileName = string.Concat(Info.Destination, "\\", FileName);
                    if (!Info.IsOverwrite && File.Exists(destFileName)) continue;
                    File.Copy(SingleFile, destFileName, Info.IsOverwrite);
                }
                catch (Exception ex)
                {
                    LogBuilder.NLogger.Error(ex);
                    //throw ex;
                }
            }
        }

        public static void Copy(string sourcePath, string dstPath, bool isCopySubDir)
        {
            DirectoryInfo dSource = new DirectoryInfo(sourcePath);
            DirectoryInfo dDst = Directory.CreateDirectory(dstPath);
            foreach (FileInfo f in dSource.GetFiles())
            {
                f.CopyTo(dDst.FullName + "\\" + f.Name);
            }
            if (!isCopySubDir)
                return;
            else
            {
                foreach (DirectoryInfo d in dSource.GetDirectories())
                {
                    Copy(d.FullName, dstPath + d.FullName.Replace(sourcePath, ""), isCopySubDir);
                }
            }
        }

        /// <summary>
        /// 目录的最后修改时间
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="ignoreFolders"></param>
        /// <returns></returns>
        public static DateTime GetLastUpdatedTime(string sourcePath, params string[] ignoreFolders)
        {
            DateTime latelyUpdateTime = DateTime.MinValue;
            if (!Directory.Exists(sourcePath))
            {
                throw new FileNotFoundException();
            }
            string[] DirFiles;
            string[] DirDirs;
            try
            {
                DirFiles = Directory.GetFiles(sourcePath);
                DirDirs = Directory.GetDirectories(sourcePath);
            }
            catch { throw new FileNotFoundException(); }
            foreach (string SingleDir in DirDirs)
            {
                bool isIgnore = false;
                if (ignoreFolders != null)
                {
                    foreach (string iFolder in ignoreFolders)
                    {
                        if (SingleDir.ToLower().IndexOf(string.Concat("\\", iFolder.ToLower())) > 0) { isIgnore = true; break; }
                    }
                }
                if (isIgnore) continue;
                DateTime latelyUpdateTimeCurrent = GetLastUpdatedTime(SingleDir, ignoreFolders);
                if (latelyUpdateTimeCurrent > latelyUpdateTime) latelyUpdateTime = latelyUpdateTimeCurrent;
            }
            foreach (string SingleFile in DirFiles)
            {
                try
                {
                    string FileName = SingleFile.Split('\\')[SingleFile.Split('\\').Length - 1];
                    System.IO.FileInfo fi = new FileInfo(SingleFile);
                    if (fi.LastWriteTime > latelyUpdateTime) latelyUpdateTime = fi.LastWriteTime;
                }
                catch (Exception ex)
                {
                    LogBuilder.NLogger.Error(ex);
                }
            }
            return latelyUpdateTime;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string d in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(d))
                    {
                        File.Delete(d);
                    }
                    else
                    {
                        DeleteFolder(d);
                    }
                }
                Directory.Delete(path, true);
            }
        }
    }
}
