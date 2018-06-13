using CodeJedi.AsyncAwait.Examples;

namespace CodeJedi.AsyncAwait
{
    public class MainViewModel : NotifiableBase
    {
        public MainViewModel()
        {
            var example = new Example();
            SynchronousCommand = new DelegateCommand(example.SynchronousExample);
            BackgroundWorkerCommand = new DelegateCommand(example.BackgroundWorkerExample);
            DispatcherCommand = new DelegateCommand(example.DispatcherExample);
            AsyncAwaitSimpleCommand = new DelegateCommand(example.AsyncAwaitSimpleExample);
        }

        public DelegateCommand SynchronousCommand { get; }
        public DelegateCommand BackgroundWorkerCommand { get; }
        public DelegateCommand DispatcherCommand { get; }
        public DelegateCommand AsyncAwaitSimpleCommand { get; }

        public Processing Processing => Processing.Instance;
    }
}
