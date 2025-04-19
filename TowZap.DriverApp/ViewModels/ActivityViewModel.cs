using System.Collections.ObjectModel;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.Constants;
using TowZap.DriverApp.Helper;
using TowZap.DriverApp.Enums;
using TowZap.DriverApp.Models.DTOs;
using TowZap.DriverApp.Views;

namespace TowZap.DriverApp.ViewModels
{
    public class ActivityViewModel : BaseViewModel, IInitializable
    {
        #region Fields

        private readonly JobService _jobService;
        private readonly SessionManager _session;

        private GroupingOption _selectedGrouping = GroupingOption.Date;

        #endregion

        #region Constructor

        public ActivityViewModel(JobService jobService, SessionManager session)
        {
            _jobService = jobService;
            _session = session;
        }

        #endregion

        #region Properties

        public ObservableCollection<JobResponse> RecentJobs { get; set; } = new();
        public ObservableCollection<GroupedJobsDTO> GroupedJobs { get; set; } = new();

        public bool IsLoading { get; set; }
        public bool HasJobs => RecentJobs.Any();

        public GroupingOption SelectedGrouping
        {
            get => _selectedGrouping;
            set
            {
                if (SetProperty(ref _selectedGrouping, value))
                    GroupJobs(); // regroup on change
            }
        }

        public List<GroupingOption> GroupingOptions { get; } = Enum
            .GetValues(typeof(GroupingOption))
            .Cast<GroupingOption>()
            .ToList();

        #endregion

        #region Initialization

        public async Task InitializeAsync()
        {
            await LoadJobsAsync();
        }

        #endregion

        #region Job Loading & Grouping

        private async Task LoadJobsAsync()
        {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));

            await _session.InitializeAsync();

            if (_session.Role != UserRoles.Driver)
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
                return;
            }

            var jobs = await _jobService.GetJobsForDriverAsync();

            RecentJobs.Clear();

            foreach (var job in jobs.OrderByDescending(j => j.CreatedAt).Take(20))
            {
                RecentJobs.Add(job);
            }

            GroupJobs();

            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            OnPropertyChanged(nameof(HasJobs));
        }

        private void GroupJobs()
        {
            GroupedJobs.Clear();

            IEnumerable<IGrouping<string, JobResponse>> groups = _selectedGrouping switch
            {
                GroupingOption.Status => RecentJobs.GroupBy(j => j.Status.ToString()),
                GroupingOption.Date => RecentJobs.GroupBy(j => j.CreatedAt.Date.ToString("MMMM dd, yyyy")),
                _ => Enumerable.Empty<IGrouping<string, JobResponse>>()
            };

            foreach (var group in groups)
            {
                GroupedJobs.Add(new GroupedJobsDTO
                {
                    Key = group.Key,
                    Jobs = group.ToList()
                });
            }
        }

        #endregion
    }
}
