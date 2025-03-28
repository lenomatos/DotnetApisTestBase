using PrimeWebApi.Services.Interfaces;

namespace PrimeWebApi.Services
{
    public class PrimeService : IPrimeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PrimeService> _logger;
        private readonly Random _random = new();

        public PrimeService(HttpClient httpClient, ILogger<PrimeService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<int> GenerateRandomNumberAsync(int minValue, int maxValue)
        {
            try
            {
                _logger.LogInformation("Generating random number between {Min} and {Max}", minValue, maxValue);

                // Simulate async work (e.g., database/API call)
                await Task.Delay(100); // 100ms delay

                int number = _random.Next(minValue, maxValue + 1);
                _logger.LogDebug("Generated number: {Number}", number);

                return number;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate random number");
                throw;
            }
        }

        public string CheckNumberType(int number)
        {
            try
            {
                if (number < 0)
                {
                    _logger.LogInformation("Negative number detected: {Number}", number);
                    return "Negative";
                }
                else if (number % 2 == 0)
                {
                    _logger.LogDebug("Even number processed: {Number}", number);
                    return "Even";
                }
                else
                {
                    return "Odd";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckNumberType for input: {Number}", number);
                throw new ArgumentException("Invalid number processing", nameof(number), ex);
            }
        }

        public string GetDayName(int dayNumber)
        {
            try
            {
                _logger.LogDebug("Getting day name for: {DayNumber}", dayNumber);

                return dayNumber switch
                {
                    1 => "Monday",
                    2 => "Tuesday",
                    3 => "Wednesday",
                    4 => "Thursday",
                    5 => "Friday",
                    6 => "Saturday",
                    7 => "Sunday",
                    _ => throw new ArgumentOutOfRangeException(nameof(dayNumber), "Day must be 1-7")
                };
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "Invalid day number: {DayNumber}", dayNumber);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetDayName");
                throw;
            }
        }

        public async Task<string?> GetTodoTitleAsync(int todoId)
        {
            try
            {
                _logger.LogInformation("Fetching todo ID: {TodoId}", todoId);
                var response = await _httpClient.GetAsync($"todos/{todoId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API failed with status: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"API Error: {response.StatusCode}");
                }

                var content = await response.Content.ReadFromJsonAsync<Todo>();
                return content?.Title;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed for todo ID: {TodoId}", todoId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error in GetTodoTitleAsync");
                throw;
            }
        }

        private class Todo
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Completed { get; set; }
        }
    }
}