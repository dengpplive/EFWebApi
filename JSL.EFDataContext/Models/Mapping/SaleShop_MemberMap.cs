using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JSL.EFDataContext.Models.Mapping
{
    public class SaleShop_MemberMap : EntityTypeConfiguration<SaleShop_Member>
    {
        public SaleShop_MemberMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
        }
    }
}
