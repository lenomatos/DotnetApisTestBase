using Microsoft.AspNetCore.Mvc;
using PrimeWebApi.Services.Interfaces;

namespace PrimeWebApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PrimeController : ControllerBase
    {
        private readonly IPrimeService _primeService;

        public PrimeController(IPrimeService primeService)
        {
            _primeService = primeService;
        }

        [HttpGet("random/{min}/{max}")]
        public async Task<IActionResult> GetRandomNumber(int min, int max)
        {
            try
            {
                var number = await _primeService.GenerateRandomNumberAsync(min, max);
                var type = _primeService.CheckNumberType(number);
                return Ok(new { Number = number, Type = type });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("day/{dayNumber}")]
        public IActionResult GetDayName(int dayNumber)
        {
            try
            {
                var dayName = _primeService.GetDayName(dayNumber);
                return Ok(dayName);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("checknumber/{number}")]
        public IActionResult Checknumber(int number)
        {
            try
            {
                var numberType = _primeService.CheckNumberType(number);
                return Ok(numberType);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("todo/{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            try
            {
                var title = await _primeService.GetTodoTitleAsync(id);
                return Ok(new { Title = title });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
        }
    }
}
