using EventApp1.Models;

namespace interfaces;


public interface IEventRepository
{
    Task<int> InsertAsync(Event eventToCreate);
    Task<Event> GetEventByIdAsync(int eventId);
    Task UpdateAsync(Event eventToUpdate);
    Task<int> DeleteEventAsync(int eventToDelete);



}