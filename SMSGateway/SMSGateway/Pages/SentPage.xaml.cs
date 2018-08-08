using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SMSGateway.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SentPage : ContentPage
	{
        public Timer RefreshTimer;
        public SentPage ()
		{
			InitializeComponent ();

            if (RefreshTimer == null)
            {
                RefreshTimer = new Timer(30 * 1000);
                RefreshTimer.Elapsed += RefreshTimer_Elapsed;
                RefreshTimer.Start();
            }

            listView.IsPullToRefreshEnabled = true;
            listView.RefreshCommand = new Command(() => RefreshListview());
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
                var SentList = DependencyService.Get<ISMSSender>().SentList();
                Device.BeginInvokeOnMainThread(() =>
                {
                    listView.ItemsSource = SentList;
                    listView.IsRefreshing = false;
                });
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
                Device.BeginInvokeOnMainThread(()=> listView.IsRefreshing = false);
            }
        }
    }
}