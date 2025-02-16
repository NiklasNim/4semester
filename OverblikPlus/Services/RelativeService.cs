using System.Collections;
using System.Net.Http.Json;
using OverblikPlus.Models.Dtos.Calendar;
using OverblikPlus.Models.Dtos.Tasks;
using OverblikPlus.Services.Interfaces;

namespace OverblikPlus.Services;

public class RelativeService : IRelativeService
{
    private readonly HttpClient _httpClient;

    public RelativeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ReadTaskDto>> GetTasksForDayForSpecificUser(string userId, DateTime date)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<ReadTaskDto>>($"/api/Relative/{userId}/tasks-for-day?date={date}");
        return response ?? new List<ReadTaskDto>();
    }

    public async Task<IEnumerable<ReadCalendarEventDto>> GetEventsForDayForSpecificUser(string userId, DateTime date)
    {
        var reponse = await _httpClient.GetFromJsonAsync<IEnumerable<ReadCalendarEventDto>>($"/api/Relative/{userId}/events-for-day?date={date}");
        return reponse ?? new List<ReadCalendarEventDto>();
    }
}