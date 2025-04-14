using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Helper;

namespace TowZap.DriverApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly JobService _jobService;
        private readonly SignalRClientService _signalRService;

        public DashboardViewModel(JobService jobService, SignalRClientService signalRService)
        {
            _jobService = jobService;
            _signalRService = signalRService;
            _ = InitializeAsync();

            ViewJobCommand = new Command(async () => await ViewJobDetails());
            LogoutCommand = new Command(async () => await Logout());
            AcceptJobCommand = new Command(async () => await AcceptJobAsync());
            DeclineJobCommand = new Command(async () => await DeclineJobAsync());

        }

        #region Properties

        private JobResponse _currentJob;
        public JobResponse CurrentJob
        {
            get => _currentJob;
            set
            {
                if (SetProperty(ref _currentJob, value))
                {
                    OnPropertyChanged(nameof(HasActiveJob));
                    OnPropertyChanged(nameof(NoActiveJob));
                }
            }
        }

        private string _fromAddress = "Loading...";
        public string FromAddress
        {
            get => _fromAddress;
            set => SetProperty(ref _fromAddress, value);
        }

        private string _toAddress = "Loading...";
        public string ToAddress
        {
            get => _toAddress;
            set => SetProperty(ref _toAddress, value);
        }

        private bool _isJobPopupVisible;
        public bool IsJobPopupVisible
        {
            get => _isJobPopupVisible;
            set => SetProperty(ref _isJobPopupVisible, value);
        }

        public bool HasActiveJob => CurrentJob != null && CurrentJob.Status != JobStatus.Completed;
        public bool NoActiveJob => !HasActiveJob;

        private Guid _incomingJobId;

        #endregion

        #region Commands

        public ICommand ViewJobCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand AcceptJobCommand { get; }
        public ICommand DeclineJobCommand { get; }

        #endregion

        #region Core Logic
        private async Task InitializeAsync()
        {
            await LoadCurrentJob();
            await ConnectSignalR();
        }

        private async Task LoadCurrentJob()
        {
            CurrentJob = await _jobService.GetCurrentJobAsync();

            if (CurrentJob != null)
                await LoadAddressesAsync(CurrentJob);
        }

        private async Task LoadAddressesAsync(JobResponse job)
        {
            FromAddress = await ReverseGeocodeAsync(job.FromLatitude, job.FromLongitude);
            ToAddress = await ReverseGeocodeAsync(job.ToLatitude, job.ToLongitude);
        }

        private async Task<string> ReverseGeocodeAsync(double lat, double lng)
        {
            try
            {
                using var http = new HttpClient();
                var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lng}";
                http.DefaultRequestHeaders.Add("User-Agent", "TowZapDriverApp");

                var response = await http.GetStringAsync(url);
                using var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("display_name", out var name))
                    return name.GetString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reverse geocode failed: {ex.Message}");
            }

            return $"{lat}, {lng}";
        }

        private async Task ViewJobDetails()
        {
            if (CurrentJob != null)
                await Shell.Current.GoToAsync($"//JobDetailPage?jobId={CurrentJob.Id}");
        }

        private async Task Logout()
        {
            Preferences.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        #endregion

        #region SignalR
        private async Task ConnectSignalR()
        {
            var token = Preferences.Get("auth_token", "");
            var hubUrl = $"{ConfigurationService.Get("ApiBaseUrl")}/hubs/jobUpdates";


            await _signalRService.InitializeAsync(hubUrl, token);

            _signalRService.On<Guid>("ReceiveJobUpdate", async (jobId) =>
            {
                var job = await _jobService.GetJobByIdAsync(jobId);
                if (job != null)
                {
                    CurrentJob = job;
                    await LoadAddressesAsync(job);
                    IsJobPopupVisible = true;
                    TriggerLocalNotification();

                    OnPropertyChanged(nameof(HasActiveJob));
                    OnPropertyChanged(nameof(NoActiveJob));
                }
            });

        }

        #endregion

        #region Accept/Reject

        private async Task AcceptJobAsync()
        {
            if (CurrentJob != null)
            {
                await _jobService.UpdateJobStatusAsync(CurrentJob.Id, JobStatus.EnRoute);
                IsJobPopupVisible = false;
                await LoadCurrentJob(); // refresh job details
            }
        }

        private async Task DeclineJobAsync()
        {
            if (CurrentJob != null)
            {
                await _jobService.UpdateJobStatusAsync(CurrentJob.Id, JobStatus.Cancelled);
                IsJobPopupVisible = false;
                CurrentJob = null;
                OnPropertyChanged(nameof(HasActiveJob));
                OnPropertyChanged(nameof(NoActiveJob));
            }
        }

        private async Task TriggerLocalNotification()
        {
            try
            {
                await PermissionHelper.RequestNotificationPermissionAsync();
                NotificationHelper.Show("New Tow Job Assigned", "Tap to accept or decline the job.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shiny Notification failed: " + ex.Message);
            }
        }

        #endregion
    }
}
