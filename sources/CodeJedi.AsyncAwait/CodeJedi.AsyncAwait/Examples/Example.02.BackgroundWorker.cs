using System.ComponentModel;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        internal void BackgroundWorkerExample()
        {
            var backgroundWorker = new BackgroundWorker();
            // La méthode abonnée sera exécutée sur un autre thread
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            // La méthode abonnée sera exécutée sur le thread UI, une fois le traitement fini.
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            // Code exécuté sur le thread UI
            Processing.BeforeProcessing("On lance le traitement via un BackgroundWorker");

            backgroundWorker.RunWorkerAsync();

            /**************************************/
            /********** CODE À SUPPRIMER **********/
            /**************************************/
            // Code exécuté sur le thread UI
            // Mais puisque RunWorkerAsync n'est pas bloquant, ce texte s'affiche avant la fin du traitement
            Processing.AfterProcessing("Le traitement est terminé");
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Processing.DoSomeHeavyProcessing();

            /**************************************/
            /********** CODE À SUPPRIMER **********/
            /**************************************/
            // Ce code d'affichage n'est pas exécuté sur le thread UI, donc va lever une exception
            Processing.AfterProcessing("Le traitement est terminé");
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Code exécuté sur le thread UI
            // Une fois le traitement fini
            Processing.AfterProcessing("Le traitement du BackgroundWorker est terminé");
        }
    }
}
