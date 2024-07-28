using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T>(IGenericRepositoryInterface<T> genericRepo) : 
        ControllerBase where T : class
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await genericRepo.GetAll());
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <=0) return BadRequest("Invalid request sent");
            return Ok(await genericRepo.DeleteById(id));
        }
        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if(id <=0) return BadRequest("Invalid request sent");
            return Ok(await genericRepo.GetById(id));
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await genericRepo.Insert(model));
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await genericRepo.Update(model));
        }
    }
}
