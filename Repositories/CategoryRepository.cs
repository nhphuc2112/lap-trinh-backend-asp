using Dapper;
using Microsoft.Data.SqlClient;
using TodoList.API.Models;

namespace TodoList.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
//lay phan loai theo id
        public async Task<Category> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(
                "sp_GetCategoryById",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
//lay tat ca phan loai
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Category>(
                "sp_GetAllCategories",
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
//tao phan loai
        public async Task<Category> CreateAsync(Category category)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Name", category.Name);
            parameters.Add("@Description", category.Description);
            parameters.Add("@Color", category.Color);
            parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateCategory",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            category.Id = parameters.Get<int>("@Id");
            return category;
        }
//cap nhat phan loai
        public async Task<Category> UpdateAsync(Category category)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Id", category.Id);
            parameters.Add("@Name", category.Name);
            parameters.Add("@Description", category.Description);
            parameters.Add("@Color", category.Color);
            parameters.Add("@UpdatedAt", category.UpdatedAt);
            parameters.Add("@IsActive", category.IsActive);

            await connection.ExecuteAsync(
                "sp_UpdateCategory",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return category;
        }
//xoa phan loai
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(
                "sp_DeleteCategory",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure
            );
            return affected > 0;
        }
//khai bao phan loai theo task
        public async Task<IEnumerable<Category>> GetByTaskIdAsync(int taskId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Category>(
                "sp_GetCategoriesByTaskId",
                new { TaskId = taskId },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
    }
} 