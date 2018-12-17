using System;
using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void Progress()
        {
            await MethodWithProgressAsync(new Progress<double>(progress => Processing.SetState(progress, $"Traitement en cours : {Math.Round(progress*100,0)} %", progress != 1)));
            Processing.WriteText("Traitement effectué");
        }

        public async Task MethodWithProgressAsync(IProgress<double> progress)
        {
            int stepCount = 10;
            for (int i = 0; i < stepCount; i++)
            {
                progress?.Report((double)i / stepCount);
                await Task.Delay(300);
            }

            progress?.Report(1);
        }
    }
}
