using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
namespace Siscom.Agua.Api.Controllers
{
    [Route("api/BreachArticle/")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class BreanchArticleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public BreanchArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BreanchArticle
        [HttpGet]
        public IEnumerable<BreachArticle> GetBreachArticles()
        {
            return _context.BreachArticles;
        }
    }
}
