using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void Deadlock()
        {
            /**************************************/
            /********** CODE À SUPPRIMER **********/
            /**************************************/
            DeadlockMethodAsync().Wait();

            // Première solution : appeler le code de manière asynchrone
            await DeadlockMethodAsync();

            // Deuxième solution : utiliser un ConfigureAwait(false) 
            // sur TOUS les await de la méthode et de toutes les sous-méthodes
            DeadlockMethodWithConfigureAwaitAsync().Wait();

            // Troisième solution : appeler la méthode sur un autre thread
            Task.Run(() => DeadlockMethodAsync()).Wait();
        }

        private async Task DeadlockMethodAsync()
        {
            await Processing.DoSomeHeavyProcessingAsync();
        }

        private async Task DeadlockMethodWithConfigureAwaitAsync()
        {
            await Processing.DoSomeHeavyProcessingWithConfigureAwaitAsync().ConfigureAwait(false);
        }
    }
}
