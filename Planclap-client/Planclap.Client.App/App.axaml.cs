using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommandLine;
using MySqlConnector;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IRepository;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.Implementations;
using Planclap.Client.Infrastructures.Mapper;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Routers;
using Planclap.Client.Presentations.ViewModel;
using Planclap.Client.Views;
using Planclap.Client.Views.Pages;
using Serilog;
using Serilog.Core;

namespace Planclap.Client.App;

public class App : Application
{
    private static readonly Logger Logger = new LoggerConfiguration()
        .WriteTo
        .Console()
        .WriteTo.File("log.txt")
        .CreateLogger();

    private MainWindow? _mainWindow;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Parser.Default.ParseArguments<Options>(desktop.Args)
                .WithNotParsed(_ =>
                {
                    Logger.Error("Errors in command-line args detected");
                    desktop.MainWindow = new Window();
                    desktop.MainWindow.Loaded += (_, _) => desktop.MainWindow.Close();
                })
                .WithParsed(options =>
                {
                    Logger.Information("Launching app");
                    Logger.Information("datetime is {datetime}", options.IsoDatetime);

                    var dateTime = DateTime.Parse(options.IsoDatetime);

                    try
                    {
                        desktop.MainWindow = BuildApp(dateTime, options);
                        Logger.Information("App started with success");
                    }
                    catch (CouldNotReachDatasourceException)
                    {
                        Logger.Error("Les arguments passes sont invalides (impossible d'acceder a la source de donnees)");
                        Environment.Exit(-1);
                    }
                    catch (PlanningNotFoundException)
                    {
                        Logger.Error("Pas de planning pour aujourd'hui");
                        Environment.Exit(-1);
                    }
                    catch (InvalidResourcesException)
                    {
                        Logger.Error("La source de donnees contient des elements invalides");
                        Environment.Exit(-1);
                    }
                    catch (TheaterTooSmallException)
                    {
                        Logger.Error("La salle de cinema est trop petite pour les reservations");
                        Environment.Exit(-1);
                    }
                });
        }

        base.OnFrameworkInitializationCompleted();
    }

    private MainWindow BuildApp(DateTime dateTime, Options options)
    {
        //1. Create services
        var timeService = new TimeService(dateTime);
        var movieService = CreateService(timeService, options);

        //2. Create router
        var router = new DefaultRouter();

        //3. Create ViewModels
        var pageNotifier = new PageNotifier();
        var homePageViewModel = new HomePageViewModel(movieService, timeService, router, pageNotifier);
        var payBookingPageViewModel = new PayBookingPageViewModel(movieService, router, pageNotifier);

        //4. Create pages and bind ViewModels
        var homePage = new HomePage { DataContext = homePageViewModel };

        var payBookingPage = new PayBookingPage { DataContext = payBookingPageViewModel };

        //5. Register pages to the main window
        _mainWindow = new MainWindow { Logger = Logger };

        _mainWindow.AddPage("Home", homePage);
        _mainWindow.AddPage("PayBooking", payBookingPage);

        //6. Register navigation event
        router.Navigating += _mainWindow.OnNavigating;

        //7. Check for integrity
        if (movieService.FetchPlanning().Movies.Count == 0)
        {
            throw new PlanningNotFoundException("No planning found");
        }

        return _mainWindow;
    }

    private static MovieService CreateService(ITimeService timeService, Options options)
    {
        var planning = new Planning(new MovieTheater(8, 10));
        var scheduledMapper = new ScheduledMapper();
        var movieMapper = new MoviesMapper();

        IPlanningRepository planningRepository;
        IMovieRepository movieRepository;

        if (!string.IsNullOrEmpty(options.Dir))
        {
            Logger.Information("Initializing with file");
            Logger.Information("Dir is '{Dir}'", options.Dir);
            planningRepository = new CsvPlanningFileRepository(options.Dir, timeService, new CsvHelper(), scheduledMapper, Logger);
            movieRepository = new JsonMovieFileRepository(options.Dir, movieMapper, timeService, Logger);
        }
        else if (!string.IsNullOrEmpty(options.ConnectionString))
        {
            Logger.Information("Initializing with database");
            Logger.Information("Connection string is '{Bd}'", options.ConnectionString);

            var dbFactory = MySqlConnectorFactory.Instance;
            var queryExecutor = new QuerySetter(timeService);

            planningRepository = new SqlPlanningRepository(dbFactory, options.ConnectionString, scheduledMapper, queryExecutor, Logger);
            movieRepository = new SqlMovieRepository(dbFactory, options.ConnectionString, movieMapper, queryExecutor, Logger);

            try
            {
                using var connection = dbFactory.CreateConnection();
                connection.ConnectionString = options.ConnectionString;
                connection.Open();
                connection.Close();
            }
            catch (Exception)
            {
                throw new CouldNotReachDatasourceException();
            }
        }
        else
        {
            throw new CouldNotReachDatasourceException();
        }

        return new MovieService(movieRepository, planningRepository, planning);
    }
}
