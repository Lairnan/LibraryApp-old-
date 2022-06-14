using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryApp;

public interface ITransient {}
public interface IScoped {}
public interface ISingleton {}

public static class Ioc
{
    private static readonly IServiceProvider Provider;
    public static T Resolve<T>() where T : class => Provider.GetRequiredService<T>();

    static Ioc()
    {
        var services = new ServiceCollection();

        services.Scan(s => 
            s.FromAssemblyOf<ITransient>()
                .AddClasses(x=>x.AssignableTo<ITransient>()).AsSelf().WithTransientLifetime()
                .AddClasses(x=>x.AssignableTo<IScoped>()).AsSelf().WithScopedLifetime()
                .AddClasses(x=>x.AssignableTo<ISingleton>()).AsSelf().WithSingletonLifetime()
        );

        Provider = services.BuildServiceProvider();

        foreach (var service in services.Where(s=>s.Lifetime == ServiceLifetime.Singleton))
        {
            Provider.GetRequiredService(service.ServiceType);
        }
    }
}