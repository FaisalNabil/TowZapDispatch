using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.Helper;
using TowZap.DriverApp.Models.DTOs;

namespace TowZap.DriverApp.ViewModels
{
    [QueryProperty(nameof(JobId), "jobId")]
    public class JobDetailViewModel : BaseViewModel
    {
        private readonly JobService _jobService;
        private readonly GeocodingService _geocodingService;
        private readonly SignalRClientService _signalRService;
        private readonly SessionManager _session;

        public JobDetailViewModel(JobService jobService, GeocodingService geocodingService, SignalRClientService signalRService, SessionManager session)
        {
            _jobService = jobService;
            _geocodingService = geocodingService;
            _signalRService = signalRService;
            StatusOptions = new List<MetaEnumDTO>();
            UpdateStatusCommand = new Command(async () => await UpdateStatusAsync());
            _session = session;
        }

        public Guid JobId { get; set; }

        public JobResponse Job { get; set; }

        public string FromAddress { get; set; } = "";
        public string ToAddress { get; set; } = "";
        public string VehicleSummary => $"{Job?.Make} {Job?.Model} - {Job?.PlateNumber}";

        public List<MetaEnumDTO> StatusOptions { get; set; }

        private MetaEnumDTO _selectedStatus;
        public MetaEnumDTO SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }
        private List<DriverStatusHistoryItemDTO> _jobStatusHistory = new();
        public List<DriverStatusHistoryItemDTO> JobStatusHistory
        {
            get => _jobStatusHistory;
            set => SetProperty(ref _jobStatusHistory, value);
        }
        public Location PickupLocation => Job != null ? new(Job.FromLatitude, Job.FromLongitude) : null;
        public Location DropoffLocation => Job != null ? new(Job.ToLatitude, Job.ToLongitude) : null;

        public ICommand UpdateStatusCommand { get; }

        public async Task InitializeAsync()
        {
            Job = await _jobService.GetJobByIdAsync(JobId);
            if (Job == null)
            {
                await Shell.Current.DisplayAlert("Error", "Job not found", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }
            JobStatusHistory = await _jobService.GetJobStatusHistoryAsync(Job.Id);

            StatusOptions = await _jobService.GetJobStatusOptionsAsync();
            SelectedStatus = StatusOptions.FirstOrDefault(s => s.Value == Job.Status.ToString());

            FromAddress = await _geocodingService.ReverseGeocodeAsync(Job.FromLatitude, Job.FromLongitude);
            ToAddress = await _geocodingService.ReverseGeocodeAsync(Job.ToLatitude, Job.ToLongitude);

            OnPropertyChanged(nameof(Job));
            OnPropertyChanged(nameof(JobStatusHistory));
            OnPropertyChanged(nameof(FromAddress));
            OnPropertyChanged(nameof(ToAddress));
            OnPropertyChanged(nameof(StatusOptions));
            OnPropertyChanged(nameof(SelectedStatus));
            OnPropertyChanged(nameof(PickupLocation));
            OnPropertyChanged(nameof(DropoffLocation));

            await ConnectSignalR(JobId);
        }

        private async Task UpdateStatusAsync()
        {
            if (Job == null || SelectedStatus == null)
                return;

            if (Enum.TryParse<JobStatus>(SelectedStatus.Value, out var status))
            {
                var success = await _jobService.UpdateJobStatusAsync(Job.Id, status);
                if (success)
                {
                    Job.Status = status;
                    await Shell.Current.DisplayAlert("Updated", "Job status updated successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to update status.", "OK");
                }

                OnPropertyChanged(nameof(Job));
            }
            SelectedStatus = StatusOptions.FirstOrDefault(s => s.Value == Job.Status.ToString());
            OnPropertyChanged(nameof(SelectedStatus));
        }

        #region SignalR
        public async Task ConnectSignalR(Guid jobId)
        {
            var token = _session.Token;
            var hubUrl = $"{ConfigurationService.Get("SignalRHubUrl")}hubs/jobUpdates";


            await _signalRService.InitializeAsync(hubUrl, token, jobId.ToString());

            _signalRService.On<string>("JobStatusUpdated", async (idStr) =>
            {
                if (Guid.TryParse(idStr, out var updatedJobId) && updatedJobId == JobId)
                {
                    Job = await _jobService.GetJobByIdAsync(updatedJobId);
                    JobStatusHistory = await _jobService.GetJobStatusHistoryAsync(updatedJobId);
                    OnPropertyChanged(nameof(Job));
                    OnPropertyChanged(nameof(JobStatusHistory));
                }
            });
        }
        #endregion
    }
}
