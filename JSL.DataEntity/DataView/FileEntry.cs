using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSL.DataEntity.DataView
{
    public class FileEntry
    {
        /// <summary>
        /// 文件数据
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 文件路径或者文件名
        /// </summary>
        public string FileName { get; set; }
    }
}
