using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void CancelTask()
        {
            var cts = new CancellationTokenSource();
            try
            {
                await Task.WhenAll(
                    CancellableMethodAsync(cts.Token),
                    CancelAfter2Sec(cts));
                Processing.SetState(1, "Le traitement est terminé.", false);
            }
            catch (OperationCanceledException)
            {
                Processing.SetState(Processing.Progress, "Le traitement a été annulé.", false);
            }
        }

        private async Task CancellableMethodAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                Processing.SetState(i / 100D, "On lance un traitement de 10 secondes.");
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(100, cancellationToken);
            }
        }

        private async Task CancelAfter2Sec(CancellationTokenSource cts)
        {
            await Task.Delay(2000);
            cts.Cancel();
        }
    }
}
