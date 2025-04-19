namespace TowZap.DriverApp.Controls;

public partial class JobPopupView : ContentView
{
    private CancellationTokenSource _autoHideCts;
    private CancellationTokenSource _autoRejectCts;

    public JobPopupView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty JobIdProperty =
        BindableProperty.Create(nameof(JobId), typeof(Guid), typeof(JobPopupView));

    public static readonly BindableProperty JobSummaryProperty =
        BindableProperty.Create(nameof(JobSummary), typeof(string), typeof(JobPopupView));

    public new static readonly BindableProperty IsVisibleProperty =
        BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(JobPopupView), false, propertyChanged: OnVisibleChanged);

    public Guid JobId
    {
        get => (Guid)GetValue(JobIdProperty);
        set => SetValue(JobIdProperty, value);
    }

    public string JobSummary
    {
        get => (string)GetValue(JobSummaryProperty);
        set => SetValue(JobSummaryProperty, value);
    }

    public new bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    public event EventHandler<Guid> Accepted;
    public event EventHandler<Guid> Rejected;

    private void OnAcceptClicked(object sender, EventArgs e)
    {
        CancelTimers();
        Accepted?.Invoke(this, JobId);
        IsVisible = false;
    }

    private void OnRejectClicked(object sender, EventArgs e)
    {
        CancelTimers();
        Rejected?.Invoke(this, JobId);
        IsVisible = false;
    }

    private static void OnVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (JobPopupView)bindable;
        if ((bool)newValue)
        {
            view.StartAutoTimers();
        }
        else
        {
            view.CancelTimers();
        }
    }

    private void StartAutoTimers()
    {
        CancelTimers();

        _autoHideCts = new CancellationTokenSource();
        _autoRejectCts = new CancellationTokenSource();

        // Hide after 10s
        _ = Task.Delay(10000, _autoHideCts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
                MainThread.BeginInvokeOnMainThread(() => this.IsVisible = false);
        });

        // Reject after 1 minute
        _ = Task.Delay(60000, _autoRejectCts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled)
                MainThread.BeginInvokeOnMainThread(() => OnRejectClicked(this, EventArgs.Empty));
        });
    }

    private void CancelTimers()
    {
        _autoHideCts?.Cancel();
        _autoRejectCts?.Cancel();
    }
}
