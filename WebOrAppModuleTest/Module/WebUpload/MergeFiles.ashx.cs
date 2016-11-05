﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;

namespace WebUploadTest
{
    /// <summary>
    /// Summary description for MergeFiles
    /// </summary>
    public class MergeFiles : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //上传文件的根目录
            string rootDir = "UploadFolder";
            string guid = context.Request["guid"];
            string fileExt = context.Request["fileExt"];
            string root = context.Server.MapPath(string.Format("~/{0}/", rootDir));
            //源数据文件夹
            string sourcePath = Path.Combine(context.Server.MapPath(string.Format("~/{0}/{1}/", rootDir, guid)));
            //合并后的文件
            string fileName = Guid.NewGuid() + fileExt;
            string targetPath = Path.Combine(root, fileName);
            string relativePath = string.Format("/{0}/{1}", rootDir, fileName);
            DirectoryInfo dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                FileInfo[] files = dicInfo.GetFiles();
                foreach (FileInfo file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    FileStream addFile = new FileStream(targetPath, FileMode.Append, FileAccess.Write);
                    BinaryWriter AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流
                    Stream stream = file.Open(FileMode.Open);
                    BinaryReader TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(sourcePath);
                context.Response.Write("{\"chunked\" : true, \"hasError\" : false, \"savePath\" :\"" + System.Web.HttpUtility.UrlEncode(relativePath) + "\"}");

                //context.Response.Write("{\"hasError\" : false}");
            }
            else
                context.Response.Write("{\"hasError\" : true}");
        }



        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteFolder(string strPath)
        {
            new Thread(p =>
            {
                try
                {
                    //删除这个目录下的所有子目录
                    if (Directory.GetDirectories(strPath).Length > 0)
                    {
                        foreach (string fl in Directory.GetDirectories(strPath))
                            Directory.Delete(fl, true);
                    }
                    //删除这个目录下的所有文件
                    if (Directory.GetFiles(strPath).Length > 0)
                        foreach (string f in Directory.GetFiles(strPath))
                            System.IO.File.Delete(f);
                    Directory.Delete(strPath, true);
                }
                catch (Exception ex)
                {
                }
            }).Start();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}