using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacosApp.Common.Helpers;
using MacosApp.Common.Models;
using MacosApp.Web.Data;
using MacosApp.Web.Data.Entities;
using MacosApp.Web.Helpers;

namespace MacosApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LaboursController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;

        public LaboursController(
            DataContext dataContext,
            IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostLabour([FromBody] LabourRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _dataContext.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Not valid employee.");
            }

            var labourType = await _dataContext.LabourTypes.FindAsync(request.LabourTypeId);
            if (labourType == null)
            {
                return BadRequest("Not valid labour type.");
            }

            var imageUrl = string.Empty;
            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(request.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Labours";
                var fullPath = $"~/images/Labours/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            var labour = new Labour
            {
                Start = request.Start.ToUniversalTime(),
                ImageUrl = imageUrl,
                Name = request.Name,
                Employee = employee,
                LabourType = labourType,
                Activity = request.Activity,
                Remarks = request.Remarks
            };

            _dataContext.Labours.Add(labour);
            await _dataContext.SaveChangesAsync();
            return Ok(_converterHelper.ToLabourResponse(labour));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLabour([FromRoute] int id, [FromBody] LabourRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest();
            }

            var oldLabour = await _dataContext.Labours.FindAsync(request.Id);
            if (oldLabour == null)
            {
                return BadRequest("Labour doesn't exists.");
            }

            var labourType = await _dataContext.LabourTypes.FindAsync(request.LabourTypeId);
            if (labourType == null)
            {
                return BadRequest("Not valid labour type.");
            }

            var imageUrl = oldLabour.ImageUrl;
            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(request.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Labours";
                var fullPath = $"~/images/Labours/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            oldLabour.Start = request.Start.ToUniversalTime();
            oldLabour.ImageUrl = imageUrl;
            oldLabour.Name = request.Name;
            oldLabour.LabourType = labourType;
            oldLabour.Activity = request.Activity;
            oldLabour.Remarks = request.Remarks;

            _dataContext.Labours.Update(oldLabour);
            await _dataContext.SaveChangesAsync();
            return Ok(_converterHelper.ToLabourResponse(oldLabour));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabour([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var labour = await _dataContext.Labours
                .Include(p => p.Reports)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (labour == null)
            {
                return this.NotFound();
            }

            if (labour.Reports.Count > 0)
            {
                BadRequest("The labour can't be deleted because it has report.");
            }

            _dataContext.Labours.Remove(labour);
            await _dataContext.SaveChangesAsync();
            return Ok("Labour deleted");
        }
    }
}
