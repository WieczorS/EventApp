
using System.Data;
using EventApp1.Models;
using NpgsqlTypes;
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
        var r = new User();
        await using (var cmd = new NpgsqlCommand(
                         @"select * from users
                                    where login = @login;", _conn))
        {
            cmd.Parameters.AddWithValue("login", username);
            using (var dr = await cmd.ExecuteReaderAsync())
            {
                if (dr.Read())
                {
                    r.id = (int)(dr["id"]);
                    r.name = (dr["name"]) as string;
                    r.surname = (dr["surname"]) as string;
                    r.email = (dr["email"]) as string;
                    r.login = (dr["login"]) as string;
                    r.password = (dr["login"]) as string;
                    r.passwordSalt = (dr["password_salt"]) as string;
                }
            }
        }

        return r;
    }

}

    
       


