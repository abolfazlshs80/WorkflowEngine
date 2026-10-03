namespace WorkflowEngine.Application.Execution;

/// <summary>
/// Retry policy: how many times to retry a failing node and how long to wait between attempts.
/// </summary>
public class RetryPolicy
{
    public int MaxAttempts { get; set; } = 3;
    public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1);

    public async Task<T> ExecuteAsync<T>(Func<int, Task<T>> action, CancellationToken ct)
    {
        Exception? last = null;
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                return await action(attempt);
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                last = ex;
                await Task.Delay(Delay, ct);
            }
        }
        throw last!;
    }
}
