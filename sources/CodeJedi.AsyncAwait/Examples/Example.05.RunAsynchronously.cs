using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void RunAsynchronously()
        {
            Processing.WriteText("On lance une méthode synchrone via une tâche qu'on attend.");

            // S'il n'y a pas de méthode Async et que le traitement est lourd
            // on peut effecter le traitement sur un autre thread grâce à Task.Run()
            // et attendre la fin grâce à await.
            await Task.Run(() => Processing.DoSomeHeavyProcessing());

            Processing.WriteText("La tâche asynchrone est terminée.");
        }
    }
}
