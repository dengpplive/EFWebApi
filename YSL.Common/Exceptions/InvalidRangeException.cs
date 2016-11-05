using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 无效值异常
    /// </summary>
    [System.Serializable]
    public class InvalidRangeException : CustomException
    {
        public InvalidRangeException() : this("区间无效") { }
        public InvalidRangeException(string message) : base(message) { }
    }
}
