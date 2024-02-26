using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.OuterSpace
{
    public class OuterSpaceViewModel : ModuleViewModelBase
    {
        public string ImagePath { get; set; }

        public string Report { get; set; }

        public List<PersonInSpace> PeopleInSpace { get; set; }

        public List<RocketLaunch> UpcomingRocketLaunches { get; set; }

		public OuterSpaceViewModel() : base("OuterSpace") { }

		public OuterSpaceViewModel(ApiStatus status) : base("OuterSpace", status) { }

        public OuterSpaceViewModel(params ApiStatus[] statuses) : base("OuterSpace", statuses)
        {
        }
    }
}
