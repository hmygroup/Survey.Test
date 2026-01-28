using Serilog;

namespace SurveyApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure Serilog
        ConfigureLogging();

        try
        {
            // Build the service provider
            _serviceProvider = ConfigureServices();

            // Load theme preference
            var themeService = _serviceProvider.GetRequiredService<ThemeService>();
            themeService.LoadThemePreference();

            // Create and show the main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            Log.Information("Application started successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start");
            MessageBox.Show($"Failed to start application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Application shutting down");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void ConfigureLogging()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SurveyApp",
            "Logs",
            "app-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("Logging configured");
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configure HttpClient for API
        services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register API Services
        services.AddHttpClient<QuestionaryService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<QuestionService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<AnswerService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<QuestionResponseService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<QuestionTypeService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<PolicyService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5049/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register Infrastructure Services
        services.AddSingleton<NavigationService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<QuestionEditorFactory>();
        services.AddTransient<ReactiveValidationService>();
        services.AddSingleton<CommandHistoryManager>();
        
        // Register Caching Services
        services.AddMemoryCache();
        services.AddSingleton<GraphCacheService>();

        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<QuestionaryListViewModel>();
        services.AddTransient<QuestionaryDialogViewModel>();
        services.AddTransient<QuestionEditorViewModel>();
        services.AddTransient<QuestionDialogViewModel>();
        services.AddTransient<ConstraintEditorViewModel>();

        // Register Views
        services.AddTransient<MainWindow>();
        services.AddTransient<HomeView>();
        services.AddTransient<QuestionaryListView>();
        services.AddTransient<QuestionaryDialogWindow>();
        services.AddTransient<QuestionEditorView>();
        services.AddTransient<QuestionDialogWindow>();

        // Add Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        Log.Information("Services configured");

        return services.BuildServiceProvider();
    }
}
