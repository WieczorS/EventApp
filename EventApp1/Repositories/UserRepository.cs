
using System.Data;
using EventApp1.Models;
using interfaces;
using Npgsql;

namespace EventApp1.Repositories;

public class UserRepository:IUserService
{
    private readonly NpgsqlConnection _conn;

    public UserRepository(NpgsqlConnection conn)
    {
        _conn = conn;
        if ( _conn.State == ConnectionState.Closed)
        {
            _conn.Open();
        }
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = new User();
        
        return user;
    }

    async Task<User> IUserService.GetUserAsync(string username)
    {
        await using (var cmd = new NpgsqlCommand(
                         @"select * from users
                                    where login = @login;", _conn))
        {
            cmd.Parameters.AddWithValue("login", username);
            return (User)await cmd.ExecuteScalarAsync();
        }
    }

}

    
       


