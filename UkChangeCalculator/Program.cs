using Microsoft.Extensions.DependencyInjection;
using UkChangeCalculator.Services;

namespace UkChangeCalculator;

class Program
{
    static void Main()
    {
        // Register the services against their interfaces, then let the container
        // build everything. ChangeCalculator receives its IDenominationProvider
        // and ChangeApp receives its IChangeCalculator via constructor injection.
        var services = new ServiceCollection();
        services.AddSingleton<IDenominationProvider, UkDenominationProvider>();
        services.AddSingleton<IChangeCalculator, ChangeCalculator>();
        services.AddSingleton<ChangeApp>();

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ChangeApp>().Run();
    }
}
