using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siscom.Agua.Api.Services.Extension
{
    public class StreamActionResult : IActionResult
    {
        private readonly Func<HttpResponse, Task> _writeToResponseFunc;

        public StreamActionResult(Func<HttpResponse, Task> writeToResponseFunc)
        {
            if (writeToResponseFunc == null)
            {
                throw new ArgumentNullException(nameof(writeToResponseFunc));
            }

            _writeToResponseFunc = writeToResponseFunc;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            return _writeToResponseFunc(context.HttpContext.Response);
        }
    }
}
