using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void WhenAll()
        {
            Processing.WriteText("On lance un traitement lourd 3 fois en parallèle.");

            await Task.WhenAll(
                Processing.DoSomeHeavyProcessingAsync(),
                Processing.DoSomeHeavyProcessingAsync(),
                Processing.DoSomeHeavyProcessingAsync());

            Processing.WriteText("Les trois traitements sont terminés.");
        }
    }
}
