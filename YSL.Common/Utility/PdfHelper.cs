using System;
using System.Collections.Generic;
using System.Text;

namespace YSL.Common.Utility
{
    /// <summary>
    /// Pdfπ§æﬂ¿‡
    /// </summary>
    public class PdfHelper
    {
        public static string GeneratePdfByHtml(string exeFile, string outPath, string fileName, string html, string title, bool isCallOnly)
        {
            //string dirBase = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Files\\");
            //string exeFile = "wkhtmltopdf-0.8.3.exe";
            string invFlag = string.Concat(fileName);
            string filePdf = string.Concat(invFlag, ".pdf");
            string pathPdf = string.Concat(outPath, filePdf);
            //if (System.IO.File.Exists(pathPdf)) return pathPdf;

            string fileHtml = string.Concat(invFlag, ".html");
            string pathHtml = string.Concat(outPath, fileHtml);
            html = string.Concat("<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"><title>", title, "</title></header><body>", html, "</body></html>");
            //if (!System.IO.File.Exists(pathHtml))
            System.IO.File.WriteAllText(pathHtml, html, System.Text.Encoding.UTF8);

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = exeFile;
            p.StartInfo.Arguments = string.Concat(pathHtml, " \"", pathPdf, "\"");
            p.StartInfo.WorkingDirectory = outPath;
            p.Start();
            p.Close();
            if (isCallOnly) return pathPdf;
            int timeSlip = 0;
            while (true)
            {
                if (System.IO.File.Exists(pathPdf) && !FileHelper.IsInUsing(pathPdf))
                {
                    break;
                }

                timeSlip++;
                System.Threading.Thread.Sleep(1000);
                if (timeSlip > 3600000) break;
            }
            //Log(string.Concat("Html To PDF,Time:", timeSlip / 2, ",File:", filePdf));
            return pathPdf;
        }
    }
}
