using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TodoList.API.Models;
using TodoList.API.DTOs;

namespace TodoList.API.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;
        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }
//lay task theo id
        public async Task<Models.Task> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var task = await connection.QueryFirstOrDefaultAsync<Models.Task>(
                "sp_GetTaskById",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (task != null)
            {
                var categories = await connection.QueryAsync<Category>(
                    "sp_GetCategoriesByTaskId",
                    new { TaskId = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                task.TaskCategories = categories.Select(c => new TaskCategory
                {
                    TaskId = task.Id,
                    CategoryId = c.Id,
                    Task = task,
                    Category = c,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            return task;
        }
//lay tat ca task
        public async Task<IEnumerable<Models.Task>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var tasks = await connection.QueryAsync<Models.Task>(
                "sp_GetAllTasks",
                commandType: System.Data.CommandType.StoredProcedure
            );

            foreach (var task in tasks)
            {
                var categories = await connection.QueryAsync<Category>(
                    "sp_GetCategoriesByTaskId",
                    new { TaskId = task.Id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                task.TaskCategories = categories.Select(c => new TaskCategory
                {
                    TaskId = task.Id,
                    CategoryId = c.Id,
                    Task = task,
                    Category = c,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            return tasks;
        }

        public async Task<(IEnumerable<Models.Task> Tasks, int TotalCount)> GetFilteredAsync(TaskFilterDto filter)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(); // Explicitly open connection

            var parameters = new DynamicParameters();
            // Only add parameters that have values
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                parameters.Add("@SearchTerm", $"%{filter.SearchTerm}%");
            if (filter.Status.HasValue)
                parameters.Add("@Status", filter.Status.Value);
            if (filter.Priority.HasValue)
                parameters.Add("@Priority", filter.Priority.Value);
            if (filter.CategoryId.HasValue)
                parameters.Add("@CategoryId", filter.CategoryId.Value);
            if (filter.DueDateFrom.HasValue)
                parameters.Add("@DueDateFrom", filter.DueDateFrom.Value);
            if (filter.DueDateTo.HasValue)
                parameters.Add("@DueDateTo", filter.DueDateTo.Value);
            //phan trang
            parameters.Add("@Page", filter.Page);
            parameters.Add("@PageSize", filter.PageSize);
            parameters.Add("@TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            try
            {
                var tasks = await connection.QueryAsync<Models.Task>(
                    "sp_GetFilteredTasks",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var totalCount = parameters.Get<int>("@TotalCount");

                foreach (var task in tasks)
                {
                    var categories = await connection.QueryAsync<Category>(
                        "sp_GetCategoriesByTaskId",
                        new { TaskId = task.Id },
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    task.TaskCategories = categories.Select(c => new TaskCategory
                    {
                        TaskId = task.Id,
                        CategoryId = c.Id,
                        Task = task,
                        Category = c,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();
                }

                return (tasks, totalCount);
            }
            catch (Exception ex)
            {
                // khai bao log trong task
                Console.WriteLine($"Error in GetFilteredAsync: {ex.Message}");
                Console.WriteLine($"Parameters: {string.Join(", ", parameters.ParameterNames.Select(p => $"{p}={parameters.Get<object>(p)}"))}");
                throw;
            }
        }
//tao task
        public async Task<Models.Task> CreateAsync(Models.Task task)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Title", task.Title);
                parameters.Add("@Description", task.Description);
                parameters.Add("@Status", task.Status);
                parameters.Add("@Priority", task.Priority);
                parameters.Add("@DueDate", task.DueDate);
                parameters.Add("@UserId", task.UserId);
                parameters.Add("@CreatedAt", task.CreatedAt);
                parameters.Add("@IsDeleted", task.IsDeleted);
                parameters.Add("@Id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "sp_CreateTask",
                    parameters,
                    transaction,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                task.Id = parameters.Get<int>("@Id");

                if (task.TaskCategories != null && task.TaskCategories.Any())
                {
                    foreach (var category in task.TaskCategories)
                    {
                        category.TaskId = task.Id;
                        await connection.ExecuteAsync(
                            "sp_AddTaskCategory",
                            new { TaskId = task.Id, CategoryId = category.CategoryId },
                            transaction,
                            commandType: System.Data.CommandType.StoredProcedure
                        );
                    }
                }

                transaction.Commit();
                return task;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
//cap nhat task
        public async Task<Models.Task> UpdateAsync(Models.Task task)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", task.Id);
                parameters.Add("@Title", task.Title);
                parameters.Add("@Description", task.Description);
                parameters.Add("@Status", task.Status);
                parameters.Add("@Priority", task.Priority);
                parameters.Add("@DueDate", task.DueDate);
                parameters.Add("@UpdatedAt", task.UpdatedAt);

                await connection.ExecuteAsync(
                    "sp_UpdateTask",
                    parameters,
                    transaction,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (task.TaskCategories != null)
                {
                    // xoa category
                    var existingCategories = await connection.QueryAsync<Category>(
                        "sp_GetCategoriesByTaskId",
                        new { TaskId = task.Id },
                        transaction,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    foreach (var category in existingCategories)
                    {
                        await connection.ExecuteAsync(
                            "sp_RemoveTaskCategory",
                            new { TaskId = task.Id, CategoryId = category.Id },
                            transaction,
                            commandType: System.Data.CommandType.StoredProcedure
                        );
                    }

                    // them category
                    if (task.TaskCategories.Any())
                    {
                        // kiem tra xem tat ca category co ton tai khong
                        var categoryIds = task.TaskCategories.Select(tc => tc.CategoryId).ToList();
                        var existingCategoryIds = await connection.QueryAsync<int>(
                            "SELECT Id FROM Categories WHERE Id IN @CategoryIds AND IsActive = 1",
                            new { CategoryIds = categoryIds },
                            transaction
                        );

                        var validCategoryIds = existingCategoryIds.ToHashSet();
                        var invalidCategoryIds = categoryIds.Where(id => !validCategoryIds.Contains(id)).ToList();

                        if (invalidCategoryIds.Any())
                        {
                            throw new InvalidOperationException($"Phan loai voi ID {string.Join(", ", invalidCategoryIds)} khong ton tai hoac khong hoat dong.");
                        }

                        foreach (var category in task.TaskCategories)
                        {
                            await connection.ExecuteAsync(
                                "sp_AddTaskCategory",
                                new { TaskId = task.Id, CategoryId = category.CategoryId },
                                transaction,
                                commandType: System.Data.CommandType.StoredProcedure
                            );
                        }
                    }
                }

                transaction.Commit();
                return task;
            }
            catch (InvalidOperationException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Loi task update: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "sp_DeleteTask",
                new { Id = id },
                commandType: System.Data.CommandType.StoredProcedure
            );
            return affected > 0;
        }
//them category
        public async Task<bool> AddCategoryAsync(int taskId, int categoryId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "sp_AddTaskCategory",
                new { TaskId = taskId, CategoryId = categoryId },
                commandType: System.Data.CommandType.StoredProcedure
            );
            return true;
        }
//xoa category
        public async Task<bool> RemoveCategoryAsync(int taskId, int categoryId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "sp_RemoveTaskCategory",
                new { TaskId = taskId, CategoryId = categoryId },
                commandType: System.Data.CommandType.StoredProcedure
            );
            return affected > 0;
        }
//lay category theo task
        public async Task<IEnumerable<Category>> GetTaskCategoriesAsync(int taskId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryAsync<Category>(
                "sp_GetCategoriesByTaskId",
                new { TaskId = taskId },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
    }
} 