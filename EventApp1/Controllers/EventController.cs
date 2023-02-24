using EventApp1.Models;
using interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventApp1.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class EventController : Controller
    {

        private readonly IEventRepository _repository;

        public EventController(IEventRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("{n}")]
        public async Task<Event> GetEvent([FromRoute]int n)
        {
            
            return await _repository.GetEventByIdAsync(n);
            
        }
        [HttpPost]
        public async Task<CreatedAtActionResult> InsertAsync(Event eventToCreate)
        {
            var id =  await _repository.InsertAsync(eventToCreate);
            var insertedEvent = await _repository.GetEventByIdAsync(id);
            
            return CreatedAtAction(nameof(GetEvent), new { n = insertedEvent.Id }, insertedEvent);
        }

        [HttpDelete("{id}")]
        public async Task<OkResult> DeleteNews([FromRoute] int idNews)
        {
             await _repository.DeleteEventAsync(idNews);
             return Ok();
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, Event eventToUpdate)
        {
            if (id != eventToUpdate.Id)
            {
                return BadRequest();
            }

   
            await _repository.UpdateAsync(eventToUpdate);
            
            return NoContent();
        }
    }
}