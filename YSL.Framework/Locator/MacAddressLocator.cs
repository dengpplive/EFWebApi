namespace YSL.Framework.AddressLocator
{
    using System.Collections.Generic;
    using System.Management;

    public class MacAddressLocator {
        /// <summary>
        /// 获取Mac地址(仅启用)
        /// </summary>
        public static IList<string> GetMacAddress() {
            IList<string> result = new List<string>();
            ManagementObjectCollection queryCollection = getManagementCollection();
            foreach (ManagementObject mo in queryCollection) {
                if (mo["IPEnabled"].ToString().ToLower() == "true") {
                    result.Add(mo["MacAddress"].ToString());
                }
            }
            return result;
        }
        /// <summary>
        /// 获取所有Mac地址
        /// </summary>
        public static IList<string> GetAllMacAddress() {
            IList<string> result = new List<string>();
            ManagementObjectCollection queryCollection = getManagementCollection();
            foreach (ManagementObject mo in queryCollection) {
                object macAddress = mo["MacAddress"];
                if (macAddress != null) {
                    result.Add(macAddress.ToString());
                }
            }
            return result;
        }
        private static ManagementObjectCollection getManagementCollection() {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            return searcher.Get();
        }
    }
}
