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
            RunAsynchronouslyCommand = new DelegateCommand(example.RunAsynchronously);
            SynchronousAsyncCommand = new DelegateCommand(example.SynchronousAsync);
            WhenAllCommand = new DelegateCommand(example.WhenAll);
            WhenAnyCommand = new DelegateCommand(example.WhenAny);
            ConfigureAwaitCommand = new DelegateCommand(example.ConfigureAwait);
            DeadlockCommand = new DelegateCommand(example.Deadlock);
        }

        public DelegateCommand SynchronousCommand { get; }
        public DelegateCommand BackgroundWorkerCommand { get; }
        public DelegateCommand DispatcherCommand { get; }
        public DelegateCommand AsyncAwaitSimpleCommand { get; }
        public DelegateCommand RunAsynchronouslyCommand { get; }
        public DelegateCommand SynchronousAsyncCommand { get; }
        public DelegateCommand WhenAllCommand { get; }
        public DelegateCommand WhenAnyCommand { get; }
        public DelegateCommand ConfigureAwaitCommand { get; }
        public DelegateCommand DeadlockCommand { get; }

        public Processing Processing => Processing.Instance;
    }
}
