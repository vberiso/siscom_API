using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siscom.Agua.Api.Enums;
using Siscom.Agua.Api.Model;
using Siscom.Agua.DAL;

namespace Siscom.Agua.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOfProdcutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SaleOfProdcutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost()]
        public async Task<IActionResult> AddProduct([FromBody] BillableProduct billableProduct)
        {
            string error = string.Empty;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "billing_product";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id_agreement", billableProduct.AgreementId));
                    command.Parameters.Add(new SqlParameter("@id_product", billableProduct.ProductId));
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@error",
                        DbType = DbType.String,
                        Size = 200,
                        Direction = ParameterDirection.Output
                    });
                    this._context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (!result.HasRows)
                        {
                            error = command.Parameters["@error"].Value.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        return StatusCode((int)TypeError.Code.Ok, new { Success = "Se Facturo el Producto" });
                    }
                    else
                    {
                        return StatusCode((int)TypeError.Code.Conflict, new { Error = error });
                    }

                }

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}