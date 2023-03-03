using EventApp1.Models;
using interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventApp1.Controllers
{
    [ApiController]
    //[Authorize]
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
        /// <summary>
        /// Creates an Event.
        /// </summary>
        /// <param name="eventToCreate"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<CreatedAtActionResult> InsertAsync(Event eventToCreate)
        {
            var id =  await _repository.InsertAsync(eventToCreate);
            var insertedEvent = await _repository.GetEventByIdAsync(id);
            
            return CreatedAtAction(nameof(GetEvent), new { n = insertedEvent.Id }, insertedEvent);
        }
        /// <summary>
        /// Deletes news with given id
        /// </summary>
        /// <param name="idNews"></param>
        /// <returns></returns>
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