using Dapper;
using Microsoft.Data.SqlClient;
using TodoList.API.Models;
using TodoList.API.Database;

namespace TodoList.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
//lay user theo id
        public async Task<User> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserById",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
//lay user theo username
        public async Task<User> GetByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByUsername",
                new { Username = username },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
//lay user theo email
        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByEmail",
                new { Email = email },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
//lay tat ca user
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<User>(
                "sp_GetAllUsers",
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
//tao user
        public async Task<User> CreateAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Username", user.Username);
            parameters.Add("@PasswordHash", user.PasswordHash);
            parameters.Add("@Email", user.Email);
            parameters.Add("@CreatedAt", user.CreatedAt);
            parameters.Add("@IsActive", user.IsActive);
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateUser",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            user.Id = parameters.Get<int>("@Id");
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Id", user.Id);
            parameters.Add("@Username", user.Username);
            parameters.Add("@PasswordHash", user.PasswordHash);
            parameters.Add("@Email", user.Email);
            parameters.Add("@UpdatedAt", user.UpdatedAt);
            parameters.Add("@IsActive", user.IsActive);

            await connection.ExecuteAsync(
                "sp_UpdateUser",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(
                "sp_DeleteUser",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure
            );
            return affected > 0;
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.ExecuteScalarAsync<int>(
                "sp_CheckUserExists",
                new { Username = username, Email = email },
                commandType: System.Data.CommandType.StoredProcedure
            );
            return count > 0;
        }
    }
} 