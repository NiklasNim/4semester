using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskMicroService.dto;
using TaskMicroService.DataAccess;
using TaskMicroService.Entities;

namespace TaskMicroService.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskService(TaskDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReadTaskDto>> GetAllTasks()
        {
            var tasks = await _dbContext.Tasks.ToListAsync();
            return _mapper.Map<List<ReadTaskDto>>(tasks);
        }

        public async Task<ReadTaskDto> GetTaskById(int id)
        {
            var task = await _dbContext.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            return _mapper.Map<ReadTaskDto>(task);
        }

        public async Task<int> CreateTask(CreateTaskDto createTaskDto)
        {
            // if (createTaskDto.UserId.HasValue)
            //{
                //var userExists = await _dbContext.Users.AnyAsync(u => u.Id == createTaskDto.UserId.Value);
                //if (!userExists)
                //{
                    //throw new InvalidOperationException("Invalid UserId");
                //}
            //}
            var task = _mapper.Map<TaskEntity>(createTaskDto);
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return task.Id;
        }

        public async Task DeleteTask(int id)
        {
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task != null)
            {
                _dbContext.Tasks.Remove(task);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            var taskEntity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (taskEntity != null)
            {
                _mapper.Map(updateTaskDto, taskEntity);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        // public async Task<List<TaskStep>> GetStepsForTask(int taskId)
        // {
        //     return await _dbContext.TaskSteps
        //         .Where(step => step.TaskId == taskId)
        //         .OrderBy(step => step.StepNumber)
        //         .ToListAsync();
        // }

    }
}