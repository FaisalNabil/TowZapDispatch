using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Graphics;
using System.Windows.Input;

namespace TowZap.DriverApp.Controls;

public partial class JobMapView : ContentView
{
    public JobMapView()
    {
        InitializeComponent();
    }

    // Bindable properties

    public static readonly BindableProperty FromLatitudeProperty =
        BindableProperty.Create(nameof(FromLatitude), typeof(double), typeof(JobMapView), 0.0, propertyChanged: OnCoordinatesChanged);

    public static readonly BindableProperty FromLongitudeProperty =
        BindableProperty.Create(nameof(FromLongitude), typeof(double), typeof(JobMapView), 0.0, propertyChanged: OnCoordinatesChanged);

    public static readonly BindableProperty ToLatitudeProperty =
        BindableProperty.Create(nameof(ToLatitude), typeof(double), typeof(JobMapView), 0.0, propertyChanged: OnCoordinatesChanged);

    public static readonly BindableProperty ToLongitudeProperty =
        BindableProperty.Create(nameof(ToLongitude), typeof(double), typeof(JobMapView), 0.0, propertyChanged: OnCoordinatesChanged);

    public double FromLatitude
    {
        get => (double)GetValue(FromLatitudeProperty);
        set => SetValue(FromLatitudeProperty, value);
    }

    public double FromLongitude
    {
        get => (double)GetValue(FromLongitudeProperty);
        set => SetValue(FromLongitudeProperty, value);
    }

    public double ToLatitude
    {
        get => (double)GetValue(ToLatitudeProperty);
        set => SetValue(ToLatitudeProperty, value);
    }

    public double ToLongitude
    {
        get => (double)GetValue(ToLongitudeProperty);
        set => SetValue(ToLongitudeProperty, value);
    }

    private static void OnCoordinatesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JobMapView mapView)
        {
            mapView.UpdateMap();
        }
    }

    private void UpdateMap()
    {
        try
        {
            MapControl.Pins.Clear();
            MapControl.MapElements.Clear();

            var pickup = new Location(FromLatitude, FromLongitude);
            var dropoff = new Location(ToLatitude, ToLongitude);

            MapControl.Pins.Add(new Pin
            {
                Label = "Pickup",
                Address = "From",
                Location = pickup,
                Type = PinType.Place
            });

            MapControl.Pins.Add(new Pin
            {
                Label = "Dropoff",
                Address = "To",
                Location = dropoff,
                Type = PinType.Place
            });

            // Optional: Draw polyline
            var routeLine = new Polyline
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 5
            };
            routeLine.Geopath.Add(pickup);
            routeLine.Geopath.Add(dropoff);

            MapControl.MapElements.Add(routeLine);

            // Auto center
            var centerLat = (FromLatitude + ToLatitude) / 2;
            var centerLng = (FromLongitude + ToLongitude) / 2;

            MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(centerLat, centerLng),
                Distance.FromKilometers(3)
            ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Map Update Failed] {ex.Message}");
        }
    }
}
