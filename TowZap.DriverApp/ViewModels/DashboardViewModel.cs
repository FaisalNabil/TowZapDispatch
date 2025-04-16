using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Helper;
using TowZap.DriverApp.Constants;

namespace TowZap.DriverApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly JobService _jobService;
        private readonly GeocodingService _geocoding;
        private readonly SignalRClientService _signalRService;
        private readonly SessionManager _session;

        public DashboardViewModel(JobService jobService, SignalRClientService signalRService, GeocodingService geocoding, SessionManager session)
        {
            _jobService = jobService;
            _signalRService = signalRService;
            _geocoding = geocoding;
            _session = session;
            _ = InitializeAsync();

            ViewJobCommand = new Command(async () => await ViewJobDetails());
            OpenSettingsCommand = new Command(async () => await OpenSettings());
            AcceptJobCommand = new Command(async () => await AcceptJobAsync());
            DeclineJobCommand = new Command(async () => await DeclineJobAsync());

        }

        #region Properties
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }

        public bool IsDriver => Role == UserRoles.Driver;
        public bool IsDispatcher => Role == UserRoles.Dispatcher;
        public bool IsCompanyAdmin => Role == UserRoles.CompanyAdministrator;

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
        public ICommand OpenSettingsCommand { get; }
        public ICommand AcceptJobCommand { get; }
        public ICommand DeclineJobCommand { get; }

        #endregion

        #region Core Logic
        private async Task InitializeAsync()
        {
            await LoadUserInfo();
            await LoadCurrentJob();
            await ConnectSignalR();
        }
        private async Task LoadUserInfo()
        {
            await _session.InitializeAsync();

            FullName = _session.FullName ?? "User";
            CompanyName = _session.CompanyName ?? "Company";
            Role = _session.Role ?? "Unknown";

            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(CompanyName));
            OnPropertyChanged(nameof(Role));
            OnPropertyChanged(nameof(IsDriver));
            OnPropertyChanged(nameof(IsDispatcher));
            OnPropertyChanged(nameof(IsCompanyAdmin));
        }

        private async Task LoadCurrentJob()
        {
            CurrentJob = await _jobService.GetCurrentJobAsync();

            if (CurrentJob != null)
                await LoadAddressesAsync(CurrentJob);
        }

        private async Task LoadAddressesAsync(JobResponse job)
        {
            FromAddress = await _geocoding.ReverseGeocodeAsync(job.FromLatitude, job.FromLongitude);
            ToAddress = await _geocoding.ReverseGeocodeAsync(job.ToLatitude, job.ToLongitude);
        }

        private async Task ViewJobDetails()
        {
            if (CurrentJob != null)
                await Shell.Current.GoToAsync($"JobDetailPage?jobId={CurrentJob.Id}");
        }

        private async Task OpenSettings()
        {
            await Shell.Current.GoToAsync("//SettingsPage");
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
