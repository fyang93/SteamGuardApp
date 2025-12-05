using SteamAuth;

namespace SteamGuardApp.Services
{
    public class SteamGuardTimerService : IDisposable
    {
        private PeriodicTimer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTimeOffset? _lastSteamTimeUpdate;
        private static int _steamServerTimeSyncIntervalMinutes = 15;

        public event Func<Task>? OnTickAsync;

        public long SteamTime { get; private set; } = 0;

        public int SecondsUntilChange { get; private set; } = 0;

        public string PercentageUntilChange { get; private set; } = string.Empty;

        public SteamGuardTimerService(TimeSpan interval)
        {
            _timer = new(interval);
            _cancellationTokenSource = new();
            StartTimer();
        }

        public void Dispose()
        {
            this.StopTimer();
        }

        private async Task RunPeriodicTaskAsync()
        {
            var now = DateTimeOffset.UtcNow;

            // 检查是否需要重新获取Steam时间
            var shouldUpdateSteamTime = SteamTime == 0 ||
                                        _lastSteamTimeUpdate == null ||
                                        (now - _lastSteamTimeUpdate.Value).TotalMinutes >= _steamServerTimeSyncIntervalMinutes;

            if (shouldUpdateSteamTime)
            {
                try
                {
                    SteamTime = await TimeAligner.GetSteamTimeAsync();
                }
                catch (Exception ex)
                {
                    // 如果获取Steam时间失败，继续使用本地递增的时间
                    // 可以根据需要添加日志记录
                    if (SteamTime != 0)
                    {
                        SteamTime += 1;
                    }
                    // 如果是第一次获取且失败了，可以考虑重试或使用本地时间
                }
            }
            else
            {
                SteamTime += 1;
            }

            SecondsUntilChange = 30 - (int)(SteamTime - (SteamTime / 30L * 30L));
            PercentageUntilChange = $"{(int)Math.Round(((double)SecondsUntilChange / 30) * 100)}%";
        }

        private async void StartTimer()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
                {
                    await RunPeriodicTaskAsync();
                    OnTickAsync?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                // Timer was canceled
            }
        }

        public void StopTimer()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void RestartTimer(TimeSpan newInterval)
        {
            _cancellationTokenSource?.Cancel();
            _timer = new(newInterval);
            _cancellationTokenSource = new();
            StartTimer();
        }
    }
}
