using EBIMa.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EBIMa.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceSupplyController : ControllerBase
	{
		private readonly DataContext _context;

        public ServiceSupplyController(DataContext context)
        {
            _context = context;
        }

		[HttpPost]
		public async Task<IActionResult> PostServiceSupplyAsync([FromBody] ServiceSupplyDTO dto)
		{

			if (string.IsNullOrWhiteSpace(dto.Name) 
				|| string.IsNullOrEmpty(dto.Surname) 
				|| string.IsNullOrEmpty(dto.Profession) 
				|| string.IsNullOrEmpty(dto.PhoneNumber))
			{
				return BadRequest("Səhv məlumat daxil edilib.");
			}

			var serviceSupply = new ServiceSupply
			{
				Name = dto.Name,
				Surname = dto.Surname,
				Profession = dto.Profession,
				PhoneNumber = dto.PhoneNumber
			};

			await _context.ServiceSupplys.AddAsync(serviceSupply);
			await _context.SaveChangesAsync();

			return Ok("İşçi əlavə olundu.");
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ServiceSupplyDTO>>> GetServiceSupplyAsync()
		{
			var serviceSupplys = await _context.ServiceSupplys
				.Select(s => new ServiceSupplyDTO
				{
					Name = s.Name,
					Surname = s.Surname,
					Profession = s.Profession,
					PhoneNumber = s.PhoneNumber
				}).ToListAsync();

			return Ok(serviceSupplys);
		}

	}
}
