using System;
using System.Collections.Generic;

namespace YSL.Common.Utility.Mapping
{
    public static class MappingInfoCache
    {
        public static Dictionary<string, List<PropertyMappingInfo>> Caches = new Dictionary<string, List<PropertyMappingInfo>>();
        public static List<PropertyMappingInfo> GetCache(string typeName)
        {
            List<PropertyMappingInfo> info = null;
            try
            {
                info = (List<PropertyMappingInfo>)Caches[typeName];

            }
            catch (KeyNotFoundException) { }

            return info;
        }

        public static void SetCache(string typeName, List<PropertyMappingInfo> mappingInfoList)
        {
            try
            {
                Caches[typeName] = mappingInfoList;
            }
            catch
            {
                Caches = new Dictionary<string, List<PropertyMappingInfo>>();
            }
        }
    }
}
