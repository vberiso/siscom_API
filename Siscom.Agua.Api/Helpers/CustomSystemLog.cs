using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Services.Security;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Siscom.Agua.Api.Helpers
{
    public class CustomSystemLog
    {

        private readonly ApplicationDbContext _context;
        //private readonly ConnectionString appSettings;

        //public CustomSystemLog(ApplicationDbContext context, ConnectionString appSettings)
        //{
        //    _context = context;
        //    this.appSettings = appSettings;
        //}
        //public CustomSystemLog(ConnectionString appSettings)
        //{
        //    this.appSettings = appSettings;
        //}
        public CustomSystemLog(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddLog(SystemLog systemLog)
        {
            ResetContextState();
            _context.SystemLogs.Add(systemLog);
            int returnValue = _context.SaveChanges();
            return returnValue > 0 ? true : false;
        }

        #region Methods Test
        //public async System.Threading.Tasks.Task<bool> AddLogAsync(SystemLog systemLog)
        //{
        //    string connectionString = appSettings.SiscmomConnection;
        //    int returnValue = 0;
        //    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //                .UseSqlServer(new SqlConnection(connectionString))
        //                .Options;

        //    //ResetContextState();
        //    using(var dbcontext = new ApplicationDbContext(options))
        //    {
        //        using(var transaction = new TransactionScope())
        //        {
        //            try
        //            {
        //                dbcontext.SystemLogs.Add(systemLog);
        //                returnValue = dbcontext.SaveChanges();
        //                transaction.Complete();
        //            }
        //            catch (System.Exception)
        //            {

        //                throw;
        //            }
        //        }

        //        return returnValue > 0 ? true : false;
        //    }

        //}

        #endregion
        private void ResetContextState() => _context.ChangeTracker.Entries()
                                .Where(e => e.Entity != null && e.State == EntityState.Added)
                                .ToList().ForEach(e => e.State = EntityState.Detached);
    }
}
