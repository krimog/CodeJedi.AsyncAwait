using System.Threading.Tasks;
using System.Windows.Threading;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public void DispatcherExample()
        {
            // Code exécuté sur le thread UI
            Processing.WriteText("On lance le traitement manuellement");
            var dispatcher = Dispatcher.CurrentDispatcher;

            // Ce code permet de lancer l'exécution dans un autre thread
            Task.Run(() =>
            {
                // Code exécuté sur un autre thread
                Processing.DoSomeHeavyProcessing();

                // On appelle le dispatcher pour retourner sur le thread UI
                dispatcher.Invoke(() =>
                {
                    // Code à nouveau exécuté sur le thread UI
                    Processing.WriteText("Le traitement est terminé et ce texte est affiché grâce au dispatcher");
                });
            });
        }
    }
}
