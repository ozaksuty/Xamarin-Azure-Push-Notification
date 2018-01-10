using Xamarin.Forms;

namespace AcikAkademiPN
{
    public partial class App : Application
	{
        public const string SenderID = "Firebase Project Sender ID";
        public const string ListenConnectionString = "Azure Notification Hub Listen ConnectionString";
        public const string NotificationHubName = "Azure Notification Hub Name";
        public const string PackageName = "Android PackageName";

        public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}