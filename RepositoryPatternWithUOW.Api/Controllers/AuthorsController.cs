using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core;
using RepositoryPatternWithUOW.Core.Interfaces;
using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IBaseRepository<Author> _authorsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthorsController(IBaseRepository<Author> authorsRepository, IUnitOfWork unitOfWork)
        {
            this._authorsRepository = authorsRepository;
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //return Ok(_authorsRepository.GetById(1));
             return Ok(_unitOfWork.Authors.GetById(2));
        }

        [HttpGet("getByIdAsync")]
        public async Task<IActionResult> GetByIdAsync()
        {
            return Ok(await _authorsRepository.GetByIdAsync(1));
        }
    }
}
