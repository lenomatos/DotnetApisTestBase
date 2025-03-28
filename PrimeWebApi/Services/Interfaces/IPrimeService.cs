namespace PrimeWebApi.Services.Interfaces
{
    public interface IPrimeService
    {
        Task<int> GenerateRandomNumberAsync(int minValue, int maxValue);
        string CheckNumberType(int number);
        string GetDayName(int dayNumber);
        Task<string?> GetTodoTitleAsync(int todoId);
    }
}
