using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CodeJedi.AsyncAwait
{
    public class Processing : NotifiableBase
    {
        private const int ProcessingSteps = 100;
        private const int StepTime = 20;
        private readonly Random _randomGenerator = new Random();
        private TextBlock _textBlock;

        public static Processing Instance { get; private set; }
        public int UIThreadId { get; private set; }

        public int ProcessingThreadId { get => _processingThreadId; private set => SetValue(ref _processingThreadId, value); }
        private int _processingThreadId = -1;

        public double Progress { get => _progress; set => SetValue(ref _progress, value); }
        private double _progress;

        public bool IsProcessing { get => _isProcessing; set => SetValue(ref _isProcessing, value); }
        private bool _isProcessing;

        private Processing()
        {
        }

        internal static void Initialize(TextBlock textBlock)
        {
            Instance = new Processing { _textBlock = textBlock, UIThreadId = Thread.CurrentThread.ManagedThreadId };
        }

        public void SetState(double progress, string text, bool isProcessing = true)
        {
            IsProcessing = isProcessing;
            Progress = progress;
            SafeWriteText(text);
        }

        public void SafeWriteText(string text)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _textBlock.Text = text;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => _textBlock.Text = text);
            }
        }

        public void WriteText(string text)
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

        public async Task DoSomeHeavyProcessingAsync()
        {
            IsProcessing = true;
            await Task.Run(() =>
            {
                ProcessingThreadId = Thread.CurrentThread.ManagedThreadId;
                for (int i = 0; i < ProcessingSteps; i++)
                {
                    Progress = (double)i / ProcessingSteps;
                    Thread.Sleep(StepTime);
                }
            });
            Progress = 1;
            IsProcessing = false;
        }

        public async Task DoSomeHeavyProcessingWithConfigureAwaitAsync()
        {
            IsProcessing = true;
            await Task.Run(() =>
            {
                ProcessingThreadId = Thread.CurrentThread.ManagedThreadId;
                for (int i = 0; i < ProcessingSteps; i++)
                {
                    Progress = (double)i / ProcessingSteps;
                    Thread.Sleep(StepTime);
                }
            }).ConfigureAwait(false);
            Progress = 1;
            IsProcessing = false;
        }

        public async Task<int> DoSomeRandomProcessingAsync()
        {
            int processingThreadId = -1;
            await Task.Run(() =>
            {
                processingThreadId = Thread.CurrentThread.ManagedThreadId;
                ProcessingThreadId = processingThreadId;
                Thread.Sleep(_randomGenerator.Next(100, 5000));
            });

            return processingThreadId;
        }
    }
}
