using System;
using System.Threading.Tasks;
using Joonasw.AzureAdApiSample.Api.Authorization;
using Joonasw.AzureAdApiSample.Api.Data;
using Joonasw.AzureAdApiSample.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Joonasw.AzureAdApiSample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext _db;

        public TodosController(TodoContext db)
        {
            _db = db;
        }

        // GET api/todos
        [HttpGet]
        [Authorize(Policies.ReadTodoItems)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _db.TodoItems.AsNoTracking().ToListAsync());
        }

        // GET api/todos/guid-value
        [HttpGet("{id}")]
        [Authorize(Policies.ReadTodoItems)]
        public async Task<ActionResult<TodoItem>> Get(Guid id)
        {
            return await _db.TodoItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        // POST api/todos
        [HttpPost]
        [Authorize(Policies.WriteTodoItems)]
        public async Task<IActionResult> Post([FromBody]TodoItem model)
        {
            if (User.IsAppOnlyCall() && string.IsNullOrWhiteSpace(model.UserId))
            {
                ModelState.AddModelError(nameof(model.UserId), "User id is required");
                return BadRequest(ModelState);
            }

            model.Id = Guid.NewGuid();
            if (!User.IsAppOnlyCall())
            {
                // If this is done with delegated permissions,
                // we will put the calling user's id on all items
                // created by apps acting instead of them
                // Apps which act with application permissions can specify any user
                model.UserId = User.GetId();
            }

            await _db.TodoItems.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new{id = model.Id}, model);
        }

        // PUT api/todos/guid-value
        [HttpPut("{id}")]
        [Authorize(Policies.WriteTodoItems)]
        public async Task<IActionResult> Put(Guid id, [FromBody]TodoItem model)
        {
            if (User.IsAppOnlyCall() && string.IsNullOrWhiteSpace(model.UserId))
            {
                ModelState.AddModelError(nameof(model.UserId), "User id is required");
                return BadRequest(ModelState);
            }

            model.Id = id;

            TodoItem item = await _db.TodoItems.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            // A delegated caller cannot specify another user id other than the logged in user
            if (!User.IsAppOnlyCall())
            {
                model.UserId = User.GetId();
            }

            item.IsDone = model.IsDone;
            item.Text = model.Text;
            item.UserId = model.UserId;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/todos/guid-value
        [HttpDelete("{id}")]
        [Authorize(Policies.WriteTodoItems)]
        public async Task<IActionResult> Delete(Guid id)
        {
            TodoItem item = await _db.TodoItems.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                // They wanted it deleted, well it does not exist
                // Makes delete idempotent
                return NoContent();
            }

            _db.TodoItems.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
