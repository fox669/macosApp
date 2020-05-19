using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MacosApp.Web.Data;
using MacosApp.Web.Data.Entities;
using MacosApp.web.Data.Entities;

namespace MacosApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LabourTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public LabourTypesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<LabourType> GetLabourTypes()
        {
            return _context.LabourTypes.OrderBy(pt => pt.Name);
        }
    }
}