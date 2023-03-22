using System.Data;
using interfaces;
using EventApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace EventApp1.Repositories;

public class EventRepository:IEventRepository
{
    private readonly NpgsqlConnection _conn;
    



    public EventRepository(NpgsqlConnection conn)           //constructor of connection to DB
    {
         _conn = conn;
        
         if( _conn.State != ConnectionState.Open)
         {
             _conn.Open();
         }
     }

    
    
    
    //CRUD operations
    
    //Create

    public async Task<int> InsertAsync(Event eventToCreate)
    {
        await using (var cmd = new NpgsqlCommand(
                         @"insert into events (name, Description, Start_date, End_date, Location_Id, Organizer_Id)
                                                                 values (@Name, @Description, @StartDate, @EndDate, @LocationId, @OrganizerId)  returning id;",
                         _conn))

        {
            
            if (eventToCreate.Name != null) cmd.Parameters.AddWithValue("Name", eventToCreate.Name);
            if (eventToCreate.Description != null)
                cmd.Parameters.AddWithValue("Description", eventToCreate.Description);
            cmd.Parameters.AddWithValue("StartDate", eventToCreate.StartDate);
            cmd.Parameters.AddWithValue("EndDate", eventToCreate.EndDate);
            cmd.Parameters.AddWithValue("LocationId", eventToCreate.LocationId);
            cmd.Parameters.AddWithValue("OrganizerId", eventToCreate.OrganizerId);
            
            return (int)((await cmd.ExecuteScalarAsync()) ?? throw new InvalidOperationException());
        }        
    }
    
    

    
    //READ

    public async Task<Event> GetEventByIdAsync(int eventId)
    {
        var returningEvent = new Event();

        await using (var cmd = new NpgsqlCommand(
                         @"SELECT id, name, description, start_date, end_date, location_id, organizer_id
                                                        FROM events WHERE Id = @id", _conn))
        {
            cmd.Parameters.AddWithValue("id", eventId);
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    returningEvent.Id = (reader["id"]) is int ? (int)(reader["id"]) : 0;
                    returningEvent.Name = (reader["name"]) as string;
                    returningEvent.Description = (reader["description"]) as string;
                    returningEvent.StartDate = Convert.ToDateTime(reader["start_date"]);
                    returningEvent.EndDate = Convert.ToDateTime(reader["end_date"]);
                    returningEvent.LocationId = (reader["location_id"]) is int ? (int)(reader["location_id"]) : 0;
                    returningEvent.OrganizerId = (reader["organizer_id"]) is int ? (int)(reader["organizer_id"]) : 0;

                }
            }

            return returningEvent;
        }
    }
    
    //UPDATE
//nn
    public async Task UpdateAsync(Event eventToUpdate)
    {
        await using (var cmd = new NpgsqlCommand(@"UPDATE events set
                     name = COALESCE(@Name, events.name),
                     description = COALESCE(@Description, events.description),
                     start_date = COALESCE(@StartDate, events.start_date),
                     end_date = COALESCE(@EndDate, events.end_date), 
                     location_id = COALESCE(@LocationId, events.location_id)
         
                                                            WHERE id = @id",_conn))
        {
            if (eventToUpdate.Name != null) cmd.Parameters.AddWithValue("Name", eventToUpdate.Name);
            if (eventToUpdate.Description != null)
                cmd.Parameters.AddWithValue("Description", eventToUpdate.Description);
            cmd.Parameters.AddWithValue("StartDate", eventToUpdate.StartDate);
            cmd.Parameters.AddWithValue("EndDate", eventToUpdate.EndDate);
            cmd.Parameters.AddWithValue("LocationId", eventToUpdate.LocationId);
            await cmd.ExecuteNonQueryAsync();

        }             
    }

    

    public async Task<int> DeleteEventAsync(int eventToDelete)
    {
        await using (var cmd = new NpgsqlCommand(@"Delete from events where id = @id"))
        {
            cmd.Parameters.AddWithValue("id", eventToDelete);
            return await cmd.ExecuteNonQueryAsync();
        }
    }

   
}