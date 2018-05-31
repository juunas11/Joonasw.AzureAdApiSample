using System;
using System.Collections.Generic;
using System.Linq;
using Joonasw.AzureAdApiSample.Api.Data;
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
            new TodoItem
            {
                Id = Guid.NewGuid(),
                Text = "Implement authentication",
                IsDone = true
            }
        };

        // GET api/todos
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(TodoItems);
        }

        // GET api/todos/guid-value
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            TodoItem item = TodoItems.FirstOrDefault(i => i.Id == id);
            if(item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // POST api/todos
        [HttpPost]
        public IActionResult Post([FromBody]TodoItem model)
        {
            model.Id = Guid.NewGuid();

            TodoItems.Add(model);
            return CreatedAtAction(nameof(Get), new{id = model.Id}, model);
        }

        // PUT api/todos/guid-value
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]TodoItem model)
        {
            model.Id = id;

            TodoItem item = TodoItems.FirstOrDefault(i => i.Id == id);
            if(item == null)
            {
                return NotFound();
            }

            TodoItems.Remove(item);
            TodoItems.Add(model);

            return NoContent();
        }

        // DELETE api/todos/guid-value
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            TodoItem item = TodoItems.FirstOrDefault(i => i.Id == id);
            if(item != null)
            {
                TodoItems.Remove(item);
            }
        }
    }
}
