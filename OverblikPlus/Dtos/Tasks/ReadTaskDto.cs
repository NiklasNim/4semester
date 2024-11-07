using OverblikPlus.Dtos.TaskSteps;

namespace OverblikPlus.Dtos.Tasks;

public class ReadTaskDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Image { get; set; }
    
    public bool RequiresQrCodeScan { get; set; }
    public List<ReadTaskStepDto> Steps { get; set; } = new List<ReadTaskStepDto>();

    public int UserId { get; set; }
    
    public bool IsCompleted { get; set; }

}