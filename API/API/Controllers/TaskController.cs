using API.Dto;
using API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("id")]
        public IActionResult GetTaskById(int id)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null) 
                return NotFound();
            
            return Ok(task);
        }

        [HttpGet("tasks")]
        public IActionResult GetAllTasks()
        {
            var task = _taskService.GetAllTasks();
            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var taskId =  _taskService.CreateTask(createTaskDto);
            return CreatedAtAction(nameof(GetTaskById), new {id = taskId}, null);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            _taskService.UpdateTask(id, updateTaskDto);
            return NoContent();
        }

        [HttpDelete ("{id}")]
        public IActionResult DeleteTask(int id)
        {
            _taskService.DeleteTask(id);
            return Ok();
        }
    }
    
}


