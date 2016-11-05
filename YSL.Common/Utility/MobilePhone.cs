using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 手机号码类
    /// </summary>
    public class MobilePhone {
        private readonly IEnumerable<string> MobelZones = new[] { "139", "138", "137", "136", "135", "134", "147", "150", "151", "152", "157", "158", "159", "182", "183", "184", "187", "188" };
        private readonly IEnumerable<string> UnicomZones = new[] { "130", "131", "132", "155", "156", "185", "186", "145" };
        private readonly IEnumerable<string> TelecomZones = new[] { "133", "153", "180", "181", "189" };
        private readonly Regex PatternReg = new Regex("^(\\+86)?(?<zone>1([38][0-9]|4[57]|5[0-35-9]))[0-9]{8}$", RegexOptions.Compiled);

        public MobilePhone(string no) {
            No = no;
            Zone = string.Empty;
        }

        public string No { get; private set; }

        public bool Execute() {
            if(!string.IsNullOrWhiteSpace(No)) {
                var match = PatternReg.Match(No.Trim());
                if(match.Success) {
                    var zone = match.Groups["zone"].Value;
                    if(MobelZones.Contains(zone)) {
                        Zone = "mobel";
                    } else if(UnicomZones.Contains(zone)) {
                        Zone = "unicom";
                    } else if(TelecomZones.Contains(zone)) {
                        Zone = "telecom";
                    }
                    return true;
                }
            }
            return false;
        }

        public string Zone { get; private set; }
    }
}