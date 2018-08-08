using System;

using Android.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SMSGateway.Droid.Services;
using System.Threading.Tasks;
using Android.Content;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace SMSGateway.Droid
{
    [Activity(Label = "SMS Gateway", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static SMSServiceConnection SmsServiceConnection;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            if (!IsServiceRunning(typeof(SMSService)))
            {
                StartSMSService();
            }
            else
            {
                Console.WriteLine("Stopping service...");
                StopService(new Intent(this, typeof(SMSService)));
                StartSMSService();
            }

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
            BatchRequestPermission();

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        private async void BatchRequestPermission()
        {
            await Task.Delay(1000);
            await RequestPermission(Permission.Storage);
            await RequestPermission(Permission.Sms);
            await RequestPermission(Permission.Phone);
        }

        protected void StartSMSService()
        {
            new Task(() =>
            {
                Console.WriteLine("Starting service...");
                // start our main service
                Application.Context.StartService(new Intent(Application.Context, typeof(SMSService)));

                // create a new service connection so we can get a binder to the service
                SmsServiceConnection = new SMSServiceConnection(null);

                // this event will fire when the Service connectin in the OnServiceConnected call 
                SmsServiceConnection.ServiceConnected += (object sender, SMSServiceConnectedEventArgs e) =>
                {

                    Console.WriteLine("Service Connected");
                    // we will use this event to notify MainActivity when to start updating the UI
                    //SetNotification();
                };

                // bind our service (Android goes and finds the running service by type, and puts a reference
                // on the binder to that service)
                // The Intent tells the OS where to find our Service (the Context) and the Type of Service
                // we're looking for (LocationService)
                Intent SmsServiceIntent = new Intent(Application.Context, typeof(SMSService));
                Console.WriteLine("Calling service binding");

                // Finally, we can bind to the Service using our Intent and the ServiceConnection we
                // created in a previous step.
                Application.Context.BindService(SmsServiceIntent, SmsServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async Task<bool> RequestPermission(Permission permission)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(permission))
                        status = results[permission];
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }

            return false;
        }

        public bool IsServiceRunning(Type cls)
        {
            try
            {
                ActivityManager manager = (ActivityManager)Android.App.Application.Context.GetSystemService(Context.ActivityService);

                foreach (var service in manager.GetRunningServices(int.MaxValue))
                {
                    if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                    {
                        return true;
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }

            return false;
        }
    }
}

