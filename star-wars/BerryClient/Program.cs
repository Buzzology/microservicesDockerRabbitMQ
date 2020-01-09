using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

namespace BerryClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient(
                "StarWarsClient",
                c => c.BaseAddress = new Uri("http://localhost:5000/graphql"));
            serviceCollection.AddStarWarsClient();

            IServiceProvider services = serviceCollection.BuildServiceProvider();
            IStarWarsClient client = services.GetRequiredService<IStarWarsClient>();

            IOperationResult<IGetHero> result = await client.GetHeroAsync(Episode.NewHope);
            Console.WriteLine(((ISomeDroid)result.Data.Hero).Name);

            result = await client.GetHeroAsync(Episode.Empire);
            Console.WriteLine(((ISomeHuman)result.Data.Hero).Name);
        }
    }
}
