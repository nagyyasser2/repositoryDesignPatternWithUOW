using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core.Interfaces;
using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBaseRepository<Book> _booksRepository;

        public BooksController(IBaseRepository<Book> booksRepository)
        {
            this._booksRepository = booksRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
             return Ok(_booksRepository.GetById(1));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_booksRepository.GetAll());
        }

        [HttpGet("GetByTitle")]
        public IActionResult GetByTitle()
        {
            return Ok(_booksRepository.Find(x => x.Title == "Ninga", new[] { "Author" }));
        }

        [HttpGet("GetAllWithAuthors")]
        public IActionResult GetByTitleWithAuthors()
        {
            return Ok(_booksRepository.FindAll(x => x.Title.Contains("Ninga"), new[] { "Author" }));
        }
    }
}
