using API.Controllers;
using API.Dto;
using DataAccess;
using DataAccess.Models;
using AutoMapper;

namespace API.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IEnumerable<TaskDto> GetAllTasks()
        {
            throw new NotImplementedException();
        }

        public TaskDto GetTaskById(int id)
        {
            var task = _dbContext.Tasks.FirstOrDefault(t => t.Id == id);

            return _mapper.Map<TaskDto>(task);
        }

        public int CreateTask(CreateTaskDto createTaskDto)
        {
            var task = _mapper.Map<TaskToDo>(createTaskDto);
            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();

            return task.Id;
        }

        public void DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateTask(TaskDto task)
        {
            throw new NotImplementedException();
        }
    }
}
