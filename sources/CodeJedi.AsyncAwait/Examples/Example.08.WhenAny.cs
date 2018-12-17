using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeJedi.AsyncAwait.Examples
{
    public partial class Example
    {
        public async void WhenAny()
        {
            int taskCount = 10;
            var runningTasks = new HashSet<Task<int>>();

            Processing.WriteText($"On démarre {taskCount} tâches en parallèle.");
            Processing.IsProcessing = true;
            Processing.Progress = 0;
            for (int i = 0; i < taskCount; i++)
            {
                var startedTask = Processing.DoSomeRandomProcessingAsync();
                runningTasks.Add(startedTask);
            }

            while (runningTasks.Any())
            {
                var finishedTask = await Task.WhenAny(runningTasks);
                runningTasks.Remove(finishedTask);

                Processing.Progress = (double)(taskCount - runningTasks.Count) / taskCount;
                Processing.WriteText($"On a fini {taskCount - runningTasks.Count} tâche. La dernière s'est exécutée sur le thread {finishedTask.Result}.");
            }

            Processing.IsProcessing = false;
            Processing.WriteText($"Les {taskCount} tâches sont terminées.");
        }
    }
}
