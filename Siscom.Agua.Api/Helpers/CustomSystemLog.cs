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
        
        private void ResetContextState() => _context.ChangeTracker.Entries()
                                .Where(e => e.Entity != null && e.State == EntityState.Added)
                                .ToList().ForEach(e => e.State = EntityState.Detached);
    }
}
