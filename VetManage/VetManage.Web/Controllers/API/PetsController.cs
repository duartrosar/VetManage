using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VetManage.Web.Data.Repositories;

namespace VetManage.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PetsController : Controller
    {
        private readonly ITreatmentRepository _treatmentRepository;

        public PetsController(
            ITreatmentRepository treatmentRepository)
        {
            _treatmentRepository = treatmentRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTreatments(int id)
        {
            var treatment = await _treatmentRepository.GetByIdAsync(id);

            if (treatment == null)
            {
                return Ok(_treatmentRepository.GetAllByPetId(id));
            }

            return Ok(treatment);

        }
    }
}
