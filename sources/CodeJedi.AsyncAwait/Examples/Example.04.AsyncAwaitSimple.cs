namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void AsyncAwaitSimpleExample()
        {
            Processing.WriteText("On appelle une méthode asynchrone et on l'attend.");

            // S'il existe une méthode Async, on peut l'attendre grâce à await.
            await Processing.DoSomeHeavyProcessingAsync();
            
            Processing.WriteText("La méthode asynchrone est terminée.");
        }
    }
}
