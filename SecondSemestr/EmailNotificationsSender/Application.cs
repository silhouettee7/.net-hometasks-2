using EmailSender.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmailNotificationsSender;

public class Application: IDisposable
{
    private ILoggerFactory? _loggerFactory;
    private bool _isLoggingAdded = false;
    private bool _isConfigurationAdded = false;
    public string ConfigFileName { get; init; } = "appsettings.json";
    public string PersonalConfigFileName { get; init; } = "appsettings.user.json";
    public IConfiguration? Configuration { get; private set; }
    
    public Application AddConfiguration()
    {
        if (_isConfigurationAdded)
        {
            return this;
        }
        var configFileExists = File.Exists(ConfigFileName);
        var personalConfigFileExists = File.Exists(PersonalConfigFileName);
        if (!configFileExists && !personalConfigFileExists)
        {
            throw new FileNotFoundException("Could not find configuration file");
        }
        var currentConfigFile = personalConfigFileExists 
            ? PersonalConfigFileName 
            : ConfigFileName;
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(currentConfigFile) ?? Directory.GetCurrentDirectory())
            .AddJsonFile(ConfigFileName, optional: false, reloadOnChange: true)
            .Build();
        
        Configuration = configuration;
        _isConfigurationAdded = true;
        
        return this;
    }

    public Application AddConsoleLogging(LogLevel logLevel = LogLevel.Information)
    {
        if (_isLoggingAdded)
        {
            return this;
        }
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(logLevel);
        });
        _loggerFactory = loggerFactory;
        _isLoggingAdded = true;
        
        return this;
    }

    public ILogger<T> GetLogger<T>()
    {
        if (_loggerFactory is null)
        {
            throw new NullReferenceException("Logging is not configured");
        }
        return _loggerFactory.CreateLogger<T>();
    }
    
    public void Dispose()
    {
        if (_loggerFactory is not null)
        {
            _loggerFactory.Dispose();
        }
    }
}