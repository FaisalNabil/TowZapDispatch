using TowZap.DriverApp.Models.DTOs;

namespace TowZap.DriverApp.Controls;

public partial class StatusTimeline : ContentView
{
    public static readonly BindableProperty StatusHistoryProperty =
        BindableProperty.Create(nameof(StatusHistory), typeof(List<DriverStatusHistoryItemDTO>), typeof(StatusTimeline), null, propertyChanged: OnStatusChanged);

    public List<DriverStatusHistoryItemDTO> StatusHistory
    {
        get => (List<DriverStatusHistoryItemDTO>)GetValue(StatusHistoryProperty);
        set => SetValue(StatusHistoryProperty, value);
    }

    private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StatusTimeline timeline && newValue is List<DriverStatusHistoryItemDTO> list)
        {
            timeline.BuildTimeline(list);
        }
    }

    private void BuildTimeline(List<DriverStatusHistoryItemDTO> statuses)
    {
        TimelineStack.Children.Clear();
        foreach (var item in statuses.OrderByDescending(x => x.Timestamp))
        {
            TimelineStack.Children.Add(new Label
            {
                Text = $"{item.Status} - {item.Timestamp:g}",
                FontSize = 14,
                TextColor = Colors.Gray
            });
        }
    }
    public StatusTimeline()
	{
		InitializeComponent();
	}
}