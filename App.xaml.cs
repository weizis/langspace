using System.Windows;
using LangSpace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LangSpace;

public partial class App : Application
{
    private IHost? _host;

    public IServiceProvider Services => _host?.Services
        ?? throw new InvalidOperationException("Application host is not initialized.");

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            var connectionString = Environment.GetEnvironmentVariable("LANGSPACE_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Set LANGSPACE_CONNECTION_STRING before starting LangSpace.");

            var builder = Host.CreateApplicationBuilder();
            builder.Services.AddDbContextFactory<LangSpaceContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Logging.AddDebug();
            _host = builder.Build();
            _host.Start();
            base.OnStartup(e);
        }
        catch (Exception)
        {
            MessageBox.Show("Unable to initialize LangSpace. Check the database configuration.", "LangSpace", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
