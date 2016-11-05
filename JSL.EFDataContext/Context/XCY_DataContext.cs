using JSL.EFDataContext.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Extender;
using YSL.Common.Log;
using YSL.Common.Resources;
namespace JSL.EFDataContext
{
    public partial class XCY_DataContext : DbContext
    {
        #region 构造函数
        public XCY_DataContext()
            : this("name=" + Constant.ConnectionName)
        {
            //禁用延迟加载
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

            //this.Configuration.AutoDetectChangesEnabled = false;
            //this.Configuration.ValidateOnSaveEnabled = false;

            //Database.SetInitializer<XCY_DataContext>(null);
        }
        public XCY_DataContext(string connectionString)
            : base(connectionString)
        {
            //禁用延迟加载
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();                      
           // modelBuilder.Configurations.Add(new SaleShop_MemberMap());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            #region DbEntityValidationException
            catch (DbEntityValidationException ex)
            {
                StringBuilder sbLog = new StringBuilder();
                foreach (var error in ex.EntityValidationErrors)
                {
                    var entry = error.Entry;
                    foreach (var err in error.ValidationErrors)
                    {
                        sbLog.AppendFormat("{0} {1}", err.PropertyName, err.ErrorMessage);
                    }
                }
                LogBuilder.NLogger.Error("DbEntityValidationException:" + sbLog.ToString() + " " + ex.MostInnerException());
                throw new Exception("DbEntityValidationException", ex.MostInnerException());
            }
            #endregion
            #region DbUpdateConcurrencyException
            catch (DbUpdateConcurrencyException ex)
            {
                var dbEntityEntry = ex.Entries.Single();
                //store wins
                dbEntityEntry.Reload();
                //OR client wins
                //var dbPropertyValues = dbEntityEntry.GetDatabaseValues();
                // dbEntityEntry.OriginalValues.SetValues(dbPropertyValues);
                LogBuilder.NLogger.Error("DbUpdateConcurrencyException: " + ex.MostInnerException());
                throw new Exception("DbUpdateConcurrencyException", ex.MostInnerException());
            }
            #endregion
            #region DbUpdateException
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    Debug.WriteLine(ex.InnerException.Message);
                }
                //which exceptions does it relate to
                foreach (var entry in ex.Entries)
                {
                    Debug.WriteLine(entry.Entity);
                }
                LogBuilder.NLogger.Error(ex);
                throw new Exception("DbUpdateException", ex.MostInnerException());
            }
            #endregion
        }
    }
}
