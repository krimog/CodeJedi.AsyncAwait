using System.Threading;
using System.Windows.Controls;

namespace CodeJedi.AsyncAwait
{
    public class Processing : NotifiableBase
    {
        private const int ProcessingSteps = 100;
        private const int StepTime = 20;
        private TextBlock _textBlock;

        public static Processing Instance { get; private set; }
        public int UIThreadId { get; private set; }

        public int ProcessingThreadId { get => _processingThreadId; private set => SetValue(ref _processingThreadId, value); }
        private int _processingThreadId = -1;

        public double Progress { get => _progress; private set => SetValue(ref _progress, value); }
        private double _progress;

        public bool IsProcessing { get => _isProcessing; private set => SetValue(ref _isProcessing, value); }
        private bool _isProcessing;

        private Processing()
        {
        }

        internal static void Initialize(TextBlock textBlock)
        {
            Instance = new Processing { _textBlock = textBlock, UIThreadId = Thread.CurrentThread.ManagedThreadId };
        }

        public void BeforeProcessing(string text)
        {
            _textBlock.Text = text;
        }

        public void DoSomeHeavyProcessing()
        {
            ProcessingThreadId = Thread.CurrentThread.ManagedThreadId;
            IsProcessing = true;
            for (int i = 0; i < ProcessingSteps; i++)
            {
                Progress = (double)i / ProcessingSteps;
                Thread.Sleep(StepTime);
            }
            Progress = 1;
            IsProcessing = false;
        }

        public void AfterProcessing(string text)
        {
            _textBlock.Text = text;
        }
    }
}
