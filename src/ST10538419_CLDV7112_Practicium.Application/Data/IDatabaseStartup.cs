namespace ST10538419_CLDV7112_Practicium.Application.Data;

public interface IDatabaseStartup
{
    Task StartAsync(CancellationToken cancellationToken);
}
