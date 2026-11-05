namespace FinalCloudDev.Application.Data;

public interface IDatabaseStartup
{
    Task StartAsync(CancellationToken cancellationToken);
}
