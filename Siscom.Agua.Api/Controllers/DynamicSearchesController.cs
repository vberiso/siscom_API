using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.Api.Services.Extension;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;


namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class DynamicSearchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DynamicSearchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DynamicSearches/5
        [HttpPost("Padron")]
        public IEnumerable<Agreement> Gett(PadronFilter filter)
        {
            DateTime star;
            DateTime end;
            DateTime.TryParse(filter.StratDate, out star);
            DateTime.TryParse(filter.EndDate, out end);

            if(filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//1
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(c => c.Clients)
                                            .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Include(d => d.Debts)
                                            .Where(x => x.TypeConsumeId == filter.TypeConsume && 
                                            (from d in x.Debts
                                            where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                            select d).Sum(z => z.Amount) > filter.Amount    &&
                                            x.TypeIntakeId == filter.TypeIntake     &&
                                            x.TypeServiceId == filter.TypeService   &&
                                            x.StratDate >= star &&
                                            x.StratDate <= end);

                var sql = a.ToSql();
               // var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//2
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(c => c.Clients)
                                             .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Include(d => d.Debts)
                                            .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                            (from d in x.Debts
                                             where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                             select d).Sum(z => z.Amount) > filter.Amount &&
                                            x.TypeIntakeId == filter.TypeIntake &&
                                            x.StratDate >= star &&
                                            x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//3
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(c => c.Clients)
                                            .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Include(d => d.Debts)
                                            .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                            (from d in x.Debts
                                             where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                             select d).Sum(z => z.Amount) > filter.Amount &&
                                            x.TypeServiceId == filter.TypeService &&
                                            x.StratDate >= star &&
                                            x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//4
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(c => c.Clients)
                                             .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Include(d => d.Debts)
                                            .Where(x => x.TypeConsumeId == filter.TypeConsume  &&
                                            x.TypeIntakeId == filter.TypeIntake &&
                                            x.TypeServiceId == filter.TypeService &&
                                            x.StratDate >= star &&
                                            x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//5
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(c => c.Clients)
                                             .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Include(d => d.Debts)
                                            .Where(x => (from d in x.Debts
                                             where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                             select d).Sum(z => z.Amount) > filter.Amount &&
                                            x.TypeIntakeId == filter.TypeIntake &&
                                            x.TypeServiceId == filter.TypeService &&
                                            x.StratDate >= star &&
                                            x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//6
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                            .Include(ti => ti.TypeIntake)
                                            .Include(ts => ts.TypeService)
                                            .Include(c => c.Clients)
                                             .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                            .Include(d => d.Debts)
                                            .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                            (from d in x.Debts
                                             where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                             select d).Sum(z => z.Amount) > filter.Amount &&
                                            x.TypeIntakeId == filter.TypeIntake &&
                                            x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//7
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//8
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//9
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//10
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeServiceId == filter.TypeService &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//11
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake && 
                                          x.TypeServiceId == filter.TypeService &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//12
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//13
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//14
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//15
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeServiceId == filter.TypeService &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//16
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//17
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//18
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume && 
                                          (from d in x.Debts
                                            where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                            select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//19
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//20
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//21
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//22
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//23
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//24
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//25
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake &&
                                          x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star == DateTime.MinValue && end == DateTime.MinValue)//26
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeServiceId == filter.TypeService);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//27
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//28
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//29
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//30
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//31
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//32
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//33
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeIntakeId == filter.TypeIntake);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//34
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//35
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star == DateTime.MinValue && end == DateTime.MinValue)//36
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                           where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                           select d).Sum(z => z.Amount) > filter.Amount);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume == 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//37
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount > 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//38
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                                      (from d in x.Debts
                                                       where _context.Statuses.Where(y => y.GroupStatusId == 4).Select(y => y.CodeName).Contains(d.Status)
                                                       select d).Sum(z => z.Amount) > filter.Amount &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService > 0 && star != DateTime.MinValue && end != DateTime.MinValue)//39
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          x.TypeServiceId == filter.TypeService &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake > 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//40
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          x.TypeIntakeId == filter.TypeIntake &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (filter.TypeConsume > 0 && filter.Amount == 0 && filter.TypeIntake == 0 && filter.TypeService == 0 && star != DateTime.MinValue && end != DateTime.MinValue)//41
            {
                var a = _context.Agreements.Include(x => x.TypeConsume)
                                          .Include(ti => ti.TypeIntake)
                                          .Include(ts => ts.TypeService)
                                          .Include(c => c.Clients)
                                           .Include(ad => ad.Addresses)
                                                .ThenInclude(s => s.Suburbs)
                                          .Include(d => d.Debts)
                                          .Where(x => x.TypeConsumeId == filter.TypeConsume &&
                                          x.StratDate >= star &&
                                          x.StratDate <= end);

                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            return null;
        }

        [HttpPost("Recaudacion")]
        public IEnumerable<Payment> Get(FundraisingFilter filter)
        {
            DateTime star;
            DateTime end;
            DateTime.TryParse(filter.StratDate, out star);
            DateTime.TryParse(filter.EndDate, out end);
            if(star != DateTime.MinValue && end != DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.BranchOffice == filter.BranchOffice &&
                                                     x.Type == filter.Type &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.BranchOffice == filter.BranchOffice &&
                                                     x.Type == filter.Type);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.BranchOffice == filter.BranchOffice &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.Type == filter.Type &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.Type == filter.Type);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date &&
                                                     x.BranchOffice == filter.BranchOffice);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star != DateTime.MinValue && end != DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.PaymentDate.Date >= star.Date &&
                                                     x.PaymentDate.Date <= end.Date);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if(star == DateTime.MinValue && end == DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.BranchOffice == filter.BranchOffice &&
                                                     x.Type == filter.Type &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if(star == DateTime.MinValue && end == DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.BranchOffice == filter.BranchOffice &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star == DateTime.MinValue && end == DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.Type == filter.Type &&
                                                     x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star == DateTime.MinValue && end == DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.Type == filter.Type);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star == DateTime.MinValue && end == DateTime.MinValue && string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && !string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.Status == filter.Status);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if(star == DateTime.MinValue && end == DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.BranchOffice == filter.BranchOffice);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            else if (star == DateTime.MinValue && end == DateTime.MinValue && !string.IsNullOrEmpty(filter.BranchOffice) && !string.IsNullOrEmpty(filter.Type) && string.IsNullOrEmpty(filter.Status))
            {
                var a = _context.Payments.Where(x => x.BranchOffice == filter.BranchOffice &&
                                                     x.Type == filter.Type);
                var sql = a.ToSql();
                var f = a.Count();
                return a;
            }
            return null;
        }
      
    }
}
