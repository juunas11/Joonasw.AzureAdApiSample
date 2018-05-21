using System;
using System.Collections.Generic;
using System.Linq;
using Joonasw.AzureAdApiSample.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Joonasw.AzureAdApiSample.Api.Controllers
{
    [Route("api/[controller]")]
    public class TodosController : Controller
    {
        // In-memory data-store for testing.
        private readonly List<TodoItem> _todoItems;

        public TodosController()
        {
            _todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Implement authentication",
                    IsDone = true
                }
            };
        }

        // GET api/todos
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_todoItems);
        }

        // GET api/todos/guid-value
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var item = _todoItems.FirstOrDefault(i => i.Id == id);
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

            _todoItems.Add(model);
            return CreatedAtAction(nameof(Get), new{id = model.Id}, model);
        }

        // PUT api/todos/guid-value
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]TodoItem model)
        {
            model.Id = id;

            var item = _todoItems.FirstOrDefault(i => i.Id == id);
            if(item == null)
            {
                return NotFound();
            }

            _todoItems.Remove(item);
            _todoItems.Add(model);

            return NoContent();
        }

        // DELETE api/todos/guid-value
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            var item = _todoItems.FirstOrDefault(i => i.Id == id);
            if(item != null)
            {
                _todoItems.Remove(item);
            }
        }
    }
}
