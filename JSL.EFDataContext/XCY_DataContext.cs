using JSL.EFDataContext.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSL.EFDataContext
{
    /// <summary>
    /// EF数据上下文 格调装修网
    /// </summary>
    public partial class XCY_DataContext
    {

       
        public DbSet<SaleShop_Member> SaleShop_Member { get; set; }
      
    }
}
