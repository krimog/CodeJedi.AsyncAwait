namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public void SynchronousExample()
        {
            // Code exécuté sur le thread UI
            Processing.WriteText("On lance le traitement de manière synchrone");

            // Code exécuté sur le thread UI - l'interface va freezer
            Processing.DoSomeHeavyProcessing();

            // Code exécuté sur le thread UI
            Processing.WriteText("Le traitement est terminé de manière synchrone");
        }
    }
}
