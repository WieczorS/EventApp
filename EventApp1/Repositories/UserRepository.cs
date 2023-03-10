
using System.Data;
using EventApp1.Models;
using NpgsqlTypes;
using interfaces;
using Npgsql;


namespace EventApp1.Repositories;

public class UserRepository:IUserService
{
    private readonly NpgsqlConnection _conn;
    private readonly IPasswordServices _passwordServices;

    public UserRepository(NpgsqlConnection conn, IPasswordServices repo)
    {
        _passwordServices = repo;
        _conn = conn;
        if ( _conn.State == ConnectionState.Closed)
        {
            _conn.Open();
        }
    }

    public async Task<User> AuthorizeUserAsync(string username, string password)
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
                    
                    r.Id = (int)(dr["id"]);
                    r.name = (dr["name"]) as string;
                    r.surname = (dr["surname"]) as string;
                    r.email = (dr["email"]) as string;
                    r.login = (dr["login"]) as string;
                    r.password = (dr["password"]) as string;
                    r.passwordSalt = (dr["password_salt"]) as string;
                }
            }
        }

        return r;
    }

    public async Task AddUserAsync(UserAddDto user)
    {
        var newUser = new User();
        var pwdSalt = _passwordServices.GenerateRandomSalt();
        var passwordHash = _passwordServices.HashPassword(pwdSalt, user.password);
        await using (var cmd = new NpgsqlCommand(
                         @"INSERT INTO users (name, surname, user_type_id, company_id, email, login, password, password_salt) 
                                VALUES 
                                (@name, @surname, @userTypeId, @companyId, @email, @login, @password, @passwordSalt)
                    returning id;", _conn))
        {
            
            cmd.Parameters.AddWithValue("name", user.name);
            cmd.Parameters.AddWithValue("surname", user.surname);
            cmd.Parameters.AddWithValue("userTypeId", user.UserTypeId);
            cmd.Parameters.AddWithValue("companyId", user.CompanyId);
            cmd.Parameters.AddWithValue("email", user.email);
            cmd.Parameters.AddWithValue("login", user.login);
            cmd.Parameters.AddWithValue("password", passwordHash);
            cmd.Parameters.AddWithValue("passwordSalt", Convert.ToBase64String(pwdSalt)); 
            await cmd.ExecuteScalarAsync();
            
        }
        
    }

}




    
       


