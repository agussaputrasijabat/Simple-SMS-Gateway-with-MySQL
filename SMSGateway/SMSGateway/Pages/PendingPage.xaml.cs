using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Messaging;
using System.Timers;

namespace SMSGateway.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PendingPage : ContentPage
	{
        public Timer RefreshTimer;
		public PendingPage ()
		{
			InitializeComponent ();

            if (RefreshTimer == null)
            {
                RefreshTimer = new Timer(40 *1000);
                RefreshTimer.Elapsed += RefreshTimer_Elapsed;
                RefreshTimer.Start();
            }

            listView.IsPullToRefreshEnabled = true;
            listView.RefreshCommand = new Command(()=> RefreshListview());
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(2000);
            RefreshListview();
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshListview();
        }

        private void RefreshListview()
        {
            try
            {
                var PendingList = DependencyService.Get<ISMSSender>().PendingList();
                Device.BeginInvokeOnMainThread(() =>
                {
                    listView.ItemsSource = PendingList;
                    listView.IsRefreshing = false;
                });
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
                Device.BeginInvokeOnMainThread(() => listView.IsRefreshing = false);
            }
        }
    }
}