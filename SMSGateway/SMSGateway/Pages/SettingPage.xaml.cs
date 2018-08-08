using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SMSGateway.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingPage : ContentPage
	{
		public SettingPage ()
		{
			InitializeComponent ();
            BindingContext = new
            {
                Settings.MySQLServer,
                Settings.MySQLUser,
                Settings.MySQLPassword,
                Settings.MySQLDatabase
            };

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new
            {
                Settings.MySQLServer,
                Settings.MySQLUser,
                Settings.MySQLPassword,
                Settings.MySQLDatabase
            };
        }

        private void EntryCell_Completed(object sender, EventArgs e)
        {
            EntryCell entry = (EntryCell)sender;
            Settings.MySQLServer = entry.Text;
        }

        private void EntryCell_Completed_1(object sender, EventArgs e)
        {
            EntryCell entry = (EntryCell)sender;
            Settings.MySQLUser = entry.Text;
        }

        private void EntryCell_Completed_2(object sender, EventArgs e)
        {
            EntryCell entry = (EntryCell)sender;
            Settings.MySQLPassword = entry.Text;
        }

        private void EntryCell_Completed_3(object sender, EventArgs e)
        {
            EntryCell entry = (EntryCell)sender;
            Settings.MySQLDatabase = entry.Text;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Settings.MySQLServer = MySQLServer.Text;
            Settings.MySQLUser = MySQLUser.Text;
            Settings.MySQLPassword = MySQLPassword.Text;
            Settings.MySQLDatabase = MySQLDatabase.Text;
        }
    }
}