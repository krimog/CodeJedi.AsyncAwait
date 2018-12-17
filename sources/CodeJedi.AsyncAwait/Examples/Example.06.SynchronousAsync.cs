using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void SynchronousAsync()
        {
            Processing.WriteText("On appelle une méthode Async et on l'attend.");

            await PossiblySynchronousMethodAsync(true);
        }

        private async Task PossiblySynchronousMethodAsync(bool shouldBeSynchronous)
        {
            if (shouldBeSynchronous)
            {
                // Sur ce chemin d'exécution, aucun await n'est fait, 
                // donc le traitement est effectué de manière synchrone.
                Processing.WriteText("Le traitement est synchrone.");
                Processing.DoSomeHeavyProcessing();
                Processing.WriteText("Fin du traitement synchrone.");
            }
            else
            {
                Processing.WriteText("Le traitement est asynchrone.");
                await Processing.DoSomeHeavyProcessingAsync();
                Processing.WriteText("Fin du traitement asynchrone.");
            }
        }
    }
}
