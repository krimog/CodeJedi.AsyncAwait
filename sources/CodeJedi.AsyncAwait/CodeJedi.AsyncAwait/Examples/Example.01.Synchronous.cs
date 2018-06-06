namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public void SynchronousExample()
        {
            // Code exécuté sur le thread UI
            Processing.BeforeProcessing("On lance le traitement de manière synchrone");

            // Code exécuté sur le thread UI - l'interface va freezer
            Processing.DoSomeHeavyProcessing();

            // Code exécuté sur le thread UI
            Processing.AfterProcessing("Le traitement est terminé de manière synchrone");
        }
    }
}
