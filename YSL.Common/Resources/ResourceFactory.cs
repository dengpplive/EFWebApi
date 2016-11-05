using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace YSL.Common.Resources
{
    public class ResourceFactory
    {
        public static ResourceAccess GetResource(string keyResource, ResourceManager resMgr)
        {
            ResourceManager rm = new ResourceManager(keyResource, typeof(ResourceFactory).Assembly);
            return new ResourceAccess(resMgr, rm);
        }
    }
}
