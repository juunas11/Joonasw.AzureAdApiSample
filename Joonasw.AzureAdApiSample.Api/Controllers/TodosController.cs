using System;
using System.Collections.Generic;
using System.Linq;
using Joonasw.AzureAdApiSample.Api.Authorization;
using Joonasw.AzureAdApiSample.Api.Data;
using Joonasw.AzureAdApiSample.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Joonasw.AzureAdApiSample.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        // In-memory data-store for testing.
        private static readonly List<TodoItem> TodoItems = new List<TodoItem>
        {
            new TodoItem //This will only be returned in app-only calls, no one will have that userid
            {
                Id = Guid.NewGuid(),
                Text = "Implement authentication",
                IsDone = true,
                UserId = Guid.NewGuid().ToString()
            }
        };

        // GET api/todos
        [HttpGet]
        [Authorize(Policies.ReadTodoItems)]
        public IActionResult Get()
        {
            if (User.IsDelegatedCall())
            {
                string userId = User.GetId();
                return Ok(TodoItems.Where(i => i.UserId == userId));
            }

            return Ok(TodoItems);
        }

        // GET api/todos/guid-value
        [HttpGet("{id}")]
        [Authorize(Policies.ReadTodoItems)]
        public IActionResult Get(Guid id)
        {
            TodoItem item = TodoItems.FirstOrDefault(i => i.Id == id);

            if(item == null)
            {
                return NotFound();
            }

            // Apps with app permissions can read all items
            // Delegated permissions only allow access to a user's data
            if (User.IsDelegatedCall() && item.UserId != User.GetId())
            {
                return Forbid();
            }

            return Ok(item);
        }

        // POST api/todos
        [HttpPost]
        [Authorize(Policies.WriteTodoItems)]
        public IActionResult Post([FromBody]TodoItem model)
        {
            if (!User.IsDelegatedCall() && string.IsNullOrWhiteSpace(model.UserId))
            {
                ModelState.AddModelError(nameof(model.UserId), "User id is required");
                return BadRequest(ModelState);
            }

            model.Id = Guid.NewGuid();
            if (User.IsDelegatedCall())
            {
                // If this is done with delegated permissions,
                // we will put the calling user's id on all items
                // created by apps acting instead of them
                // Apps which act with application permissions can specify any user
                model.UserId = User.GetId();
            }

            TodoItems.Add(model);
            return CreatedAtAction(nameof(Get), new{id = model.Id}, model);
        }

        // PUT api/todos/guid-value
        [HttpPut("{id}")]
        [Authorize(Policies.WriteTodoItems)]
        public IActionResult Put(Guid id, [FromBody]TodoItem model)
        {
            if (!User.IsDelegatedCall() && string.IsNullOrWhiteSpace(model.UserId))
            {
                ModelState.AddModelError(nameof(model.UserId), "User id is required");
                return BadRequest(ModelState);
            }

            model.Id = id;

            TodoItem item = TodoItems.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            if (User.IsDelegatedCall() && item.UserId != User.GetId())
            {
                return Forbid();
            }

            if (User.IsDelegatedCall())
            {
                model.UserId = User.GetId();
            }

            TodoItems.Remove(item);
            TodoItems.Add(model);

            return NoContent();
        }

        // DELETE api/todos/guid-value
        [HttpDelete("{id}")]
        [Authorize(Policies.WriteTodoItems)]
        public IActionResult Delete(Guid id)
        {
            TodoItem item = TodoItems.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                // They wanted it deleted, well it does not exist
                // Makes delete idempotent
                return NoContent();
            }

            if (User.IsDelegatedCall() && item.UserId != User.GetId())
            {
                // Tried to delete todo item belonging to another user
                return Forbid();
            }

            TodoItems.Remove(item);

            return NoContent();
        }
    }
}
