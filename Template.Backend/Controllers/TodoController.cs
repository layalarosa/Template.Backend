using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Template.Backend.Dtos;
using Template.Backend.Entities;
using Template.Backend.NewFolder;

namespace Template.Backend.Controllers
{
    [Route("todo")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isadmin")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/todos
        [HttpGet("todolist")]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos()
        {
            var todos = await _context.Todos
                .Select(todo => new TodoDto
                {
                    Id = todo.Id,
                    Title = todo.Title
                })
                .ToListAsync();

            return Ok(todos);
        }

        // GET: todos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoDto>> GetTodoById(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            var todoDto = new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title
            };

            return Ok(todoDto);
        }

        // POST: todos
        [HttpPost]
        public async Task<ActionResult<TodoDto>> CreateTodo([FromBody] TodoCreationDto todoCreationDto)
        {
            var todo = new Todo
            {
                Title = todoCreationDto.Title
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            var todoDto = new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title
            };

            return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todoDto);
        }

        // PUT: todos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoCreationDto todoCreationDto)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            todo.Title = todoCreationDto.Title;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: todos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
