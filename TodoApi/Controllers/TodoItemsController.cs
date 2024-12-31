using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
using Microsoft.Extensions.Logging;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            _logger.LogInformation("Fetching all Todo items.");
            var todoItems = await _context.TodoItems.ToListAsync();
            _logger.LogInformation("{Count} Todo items retrieved.", todoItems.Count);
            return Ok(todoItems);
        }

        // GET: api/TodoItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            _logger.LogInformation("Fetching Todo item with ID: {Id}.", id);

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                _logger.LogError("Todo item with ID: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Todo item with ID: {Id} retrieved successfully.", id);
            return Ok(todoItem);
        }

        // PUT: api/TodoItems/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                _logger.LogWarning("Mismatch between route ID: {RouteId} and TodoItem ID: {TodoItemId}.", id, todoItem.Id);
                return BadRequest();
            }

            _logger.LogInformation("Updating Todo item with ID: {Id}.", id);
            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Todo item with ID: {Id} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.TodoItems.Any(e => e.Id == id))
                {
                    _logger.LogWarning("Todo item with ID: {Id} does not exist.", id);
                    return NotFound();
                }

                _logger.LogError(ex, "Concurrency error while updating Todo item with ID: {Id}.", id);
                throw;
            }

            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _logger.LogInformation("Creating a new Todo item.");
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Todo item with ID: {Id} created successfully.", todoItem.Id);

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            _logger.LogInformation("Deleting Todo item with ID: {Id}.", id);

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                _logger.LogWarning("Todo item with ID: {Id} not found.", id);
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Todo item with ID: {Id} deleted successfully.", id);

            return NoContent();
        }
    }
}
