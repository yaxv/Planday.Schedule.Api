using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Queries;
using Planday.Schedule.UseCases.Interfaces;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly IGetAllShiftsQuery _getAllShiftsQuery;
        private readonly IGetShiftService _getShiftService;
        private readonly ICreateShiftService _createShiftService;
        private readonly IAssignEmployeeToShiftService _assignEmployeeToShiftService;

        public ShiftController(IGetAllShiftsQuery getAllShiftsQuery, 
            IGetShiftService getShiftService,
            ICreateShiftService createShiftService,
            IAssignEmployeeToShiftService assignEmployeeToShiftService)
        {
            _getAllShiftsQuery = getAllShiftsQuery;
            _getShiftService = getShiftService;
            _createShiftService = createShiftService;
            _assignEmployeeToShiftService = assignEmployeeToShiftService;
        }

        [HttpGet]
        public async Task<IEnumerable<Shift>> Get()
        {
            var shifts = await _getAllShiftsQuery.QueryAsync();
            return shifts;
        }
        
        [HttpGet, Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]long id)
        {
            var shiftEmployee = await _getShiftService.GetShiftByIdAsync(id);
            if (shiftEmployee != null)
                return Ok(shiftEmployee);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShift shift)
        {
            var newShift = await _createShiftService.HandleAsync(shift);
            return Ok(newShift);
        }

        [HttpPatch, Route("{id}/employee/{employeeId}/assign")]
        public async Task<IActionResult> Patch([FromRoute] long id, [FromRoute] long employeeId)
        {
            await _assignEmployeeToShiftService.HandleAsync(id, employeeId);
            return Ok();
        }
    }
}