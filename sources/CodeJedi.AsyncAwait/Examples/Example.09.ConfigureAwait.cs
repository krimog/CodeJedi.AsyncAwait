using System.Threading;
using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void ConfigureAwait()
        {
            Processing.SetState(0, $"1. On est sur le thread UI : {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(2000);
            await MethodWithConfigureAwaitAsync();
            Processing.SetState(1, $"7. On est sur le thread UI : {Thread.CurrentThread.ManagedThreadId}", false);
        }

        private async Task MethodWithConfigureAwaitAsync()
        {
            Processing.SetState(1D/6, $"2. On est toujours sur le thread UI : {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(2000);

            await Task.Run(() =>
            {
                Processing.SetState(2D / 6, $"3. On est sur un autre thread : {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(2000);
            });

            Processing.SetState(3D / 6, $"4. On revient sur le thread UI : {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(2000);

            await Task.Run(() =>
            {
                Processing.SetState(4D / 6, $"5. On est sur un autre thread : {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(2000);
            }).ConfigureAwait(false);

            Processing.SetState(5D / 6, $"6. On est encore sur un autre thread : {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(2000);
        }
    }
}
