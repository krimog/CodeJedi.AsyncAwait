using System.Threading.Tasks;
using System;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
		public async void AsyncAwaitSimpleExample()
        {
            Processing.BeforeProcessing("Start processing with async and await");
            await Task.Run(() => Processing.DoSomeHeavyProcessing());
            Processing.AfterProcessing("Processing finished with async and await");
        }
    }
}
