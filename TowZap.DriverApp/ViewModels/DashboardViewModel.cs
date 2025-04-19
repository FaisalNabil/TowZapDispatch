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
using TowZap.DriverApp.Views;

namespace TowZap.DriverApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IInitializable
    {
        #region Fields

        private readonly JobService _jobService;
        private readonly GeocodingService _geocoding;
        private readonly SignalRClientService _signalRService;
        private readonly SessionManager _session;

        private JobResponse _currentJob;
        private JobResponse _incomingJob;
        private string _fromAddress = "Loading...";
        private string _toAddress = "Loading...";
        private bool _isJobPopupVisible; 
        private bool _signalRInitialized = false;
        private bool _isProcessing = false;

        #endregion

        #region Constructor

        public DashboardViewModel(
            JobService jobService,
            SignalRClientService signalRService,
            GeocodingService geocoding,
            SessionManager session)
        {
            _jobService = jobService;
            _signalRService = signalRService;
            _geocoding = geocoding;
            _session = session;

            ViewJobCommand = new Command(async () => await ViewJobDetails());
            OpenSettingsCommand = new Command(async () => await OpenSettings());
            AcceptJobCommand = new Command(async () => await AcceptJobAsync());
            DeclineJobCommand = new Command(async () => await DeclineJobAsync());
            //TestCommand = new Command(TestNotification);
        }

        #endregion

        #region Properties

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }

        public bool IsDriver => Role == UserRoles.Driver;
        public bool IsDispatcher => Role == UserRoles.Dispatcher;
        public bool IsCompanyAdmin => Role == UserRoles.CompanyAdministrator;

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

        public string FromAddress
        {
            get => _fromAddress;
            set => SetProperty(ref _fromAddress, value);
        }

        public string ToAddress
        {
            get => _toAddress;
            set => SetProperty(ref _toAddress, value);
        }

        public bool IsJobPopupVisible
        {
            get => _isJobPopupVisible;
            set => SetProperty(ref _isJobPopupVisible, value);
        }

        public bool HasActiveJob => CurrentJob != null && CurrentJob.Status != JobStatus.Completed;
        public bool NoActiveJob => !HasActiveJob;

        #endregion

        #region Commands

        public ICommand ViewJobCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand AcceptJobCommand { get; }
        public ICommand DeclineJobCommand { get; }
        //public ICommand TestCommand { get; }
        #endregion

        #region Initialization

        //private string _jobSummary;
        //public string JobSummary
        //{
        //    get => _jobSummary;
        //    set => SetProperty(ref _jobSummary, value);
        //}
        //private void TestNotification()
        //{
        //    JobSummary = "Unit #123, Toyota Corolla, Abandoned";
        //    IsJobPopupVisible = true;

        //    TriggerLocalNotification();
        //}
        public async Task InitializeAsync()
        {
            await _session.InitializeAsync();

            if (!_session.IsLoggedIn || string.IsNullOrEmpty(_session.Token))
            {
                await Shell.Current.GoToAsync("LoginPage");
                return;
            }

            await LoadUserInfo();
            await LoadCurrentJob();
            await ConnectSignalR();
        }

        private async Task LoadUserInfo()
        {
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

        #endregion

        #region Navigation

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

        #region SignalR Integration

        private async Task ConnectSignalR()
        {
            if (_signalRInitialized) return;
            _signalRInitialized = true;

            var token = _session.Token;
            var hubUrl = $"{ConfigurationService.Get("SignalRHubUrl")}hubs/jobUpdates";

            await _signalRService.InitializeAsync(hubUrl, token, CurrentJob?.Id.ToString() ?? Guid.Empty.ToString());

            _signalRService.On<Guid>("ReceiveJobUpdate", async (jobId) =>
            {
                var job = await _jobService.GetJobByIdAsync(jobId);
                if (job != null)
                {
                    _incomingJob = job;
                    await LoadAddressesAsync(job);
                    IsJobPopupVisible = true;
                    TriggerLocalNotification();
                    OnPropertyChanged(nameof(HasActiveJob));
                    OnPropertyChanged(nameof(NoActiveJob));
                }
            });
        }

        public void ReceiveJobNotification(JobResponse job)
        {
            _incomingJob = job;
            IsJobPopupVisible = true;

            TriggerLocalNotification();
            _ = AutoHandleJobPopupAsync(job);
        }

        private async Task AutoHandleJobPopupAsync(JobResponse job)
        {
            await Task.Delay(60_000);
            IsJobPopupVisible = false;

            await Task.Delay(120_000);
            if (job.Status == JobStatus.Assigned)
            {
                await _jobService.UpdateJobStatusAsync(job.Id, JobStatus.Cancelled);
                OnPropertyChanged(nameof(HasActiveJob));
                OnPropertyChanged(nameof(NoActiveJob));
            }
        }

        #endregion

        #region Job Accept / Decline Actions

        private async Task AcceptJobAsync()
        {
            if (_incomingJob == null) return;

            await _jobService.UpdateJobStatusAsync(_incomingJob.Id, JobStatus.Assigned);
            CurrentJob = _incomingJob;
            _incomingJob = null;

            IsJobPopupVisible = false;
            await LoadCurrentJob(); // optional
            OnPropertyChanged(nameof(HasActiveJob));
            OnPropertyChanged(nameof(NoActiveJob));
        }


        private async Task DeclineJobAsync()
        {
            if (_incomingJob != null)
            {
                await _jobService.UpdateJobStatusAsync(_incomingJob.Id, JobStatus.Declined);
                _incomingJob = null;
                IsJobPopupVisible = false;
            }

            OnPropertyChanged(nameof(HasActiveJob));
            OnPropertyChanged(nameof(NoActiveJob));
        }

        private async Task TriggerLocalNotification()
        {
            try
            {
                await PermissionHelper.RequestNotificationPermissionAsync();
                NotificationHelper.Show("New Tow Job Assigned", "Tap to accept or decline the job.",
    NotificationType.JobWithActions,
    _incomingJob.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shiny Notification failed: " + ex.Message);
            }
        }

        #endregion
    }
}
