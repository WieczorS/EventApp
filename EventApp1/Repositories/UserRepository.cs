
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

    public Task<User> GetUserByHashAsync(string hash)
    {
        throw new NotImplementedException();
    }

    public async Task<SetNewPasswordDto> GetUserByTokenAsync(string token)
    {
        var r = new SetNewPasswordDto();
        
        await using (var cmd = new NpgsqlCommand(
                         @"select * from users
                                    where token = @token;", _conn))
        {
            cmd.Parameters.AddWithValue("token", token);
            using (var dr = await cmd.ExecuteReaderAsync())
            {
                if (dr.Read())
                {
                    r.id = (dr["id"]) is int ? (int)(dr["id"]) : 0;
                    r.name = (dr["name"]) as string;
                    r.email = (dr["email"]) as string;
                    r.token = (dr["token"]) as string ?? string.Empty;
                    r.salt = (dr["password_salt"]) as string ?? throw new InvalidOperationException("no password salt");
                    r.tokenExpDate = (dr["token_exp_time"]) is DateTime ? (DateTime)(dr["token_exp_time"]) : default;

                }
            }
        }

        return r;
    }
    
    async Task<User> IUserService.GetUserByEmailAsync(string email)
    {
        var r = new User();
        
        await using (var cmd = new NpgsqlCommand(
                         @"select * from users
                                    where email = @email;", _conn))
        {
            cmd.Parameters.AddWithValue("email", email);
            using (var dr = await cmd.ExecuteReaderAsync())
            {
                if (dr.Read())
                {
                    
                    r.Id = (int)(dr["id"]);
                    r.name = (dr["name"]) as string ?? throw new InvalidOperationException("name is null!");
                    r.surname = (dr["surname"]) as string ?? throw new InvalidOperationException("surname is null!");
                    r.email = (dr["email"]) as string ?? throw new InvalidOperationException("email is null!");
                    r.login = (dr["login"]) as string ?? throw new InvalidOperationException("login is null!");
                    r.password = (dr["password"]) as string ?? throw new InvalidOperationException("password is null!");
                    r.passwordSalt = (dr["password_salt"]) as string ?? throw new InvalidOperationException("dev fucked up");
                }
            }
        }

        return r;
    }

    public async Task CreatePasswordResetToken(int userId)
    {
        var tokenReset = Guid.NewGuid().ToString();
        await using (var cmd = new NpgsqlCommand(@"UPDATE public.users
                                                        SET token = @token, token_exp_time = @date
                                                        WHERE id = @id;", _conn))
        {
            cmd.Parameters.AddWithValue("token", tokenReset);
            cmd.Parameters.AddWithValue("id", userId);
            cmd.Parameters.AddWithValue("date",DateTime.Now.AddMinutes(30));
            await cmd.ExecuteScalarAsync();
        }
        
       
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




    
       


