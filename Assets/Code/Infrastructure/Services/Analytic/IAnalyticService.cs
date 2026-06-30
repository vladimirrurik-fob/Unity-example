namespace Code.Infrastructure.Services.Analytic
{
  public interface IAnalyticService
  {
    void Log(string message);
    void Warmup();
  }
}