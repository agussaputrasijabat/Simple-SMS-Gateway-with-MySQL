using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SMSGateway.Droid.Services
{
    public class SMSServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public event EventHandler<SMSServiceConnectedEventArgs> ServiceConnected = delegate { };

        public SMSServiceBinder Binder
        {
            get { return this.binder; }
            set { this.binder = value; }
        }
        protected SMSServiceBinder binder;

        public SMSServiceConnection(SMSServiceBinder binder)
        {
            if (binder != null)
            {
                this.binder = binder;
            }
        }

        // This gets called when a client tries to bind to the Service with an Intent and an 
        // instance of the ServiceConnection. The system will locate a binder associated with the 
        // running Service 
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            // cast the binder located by the OS as our local binder subclass
            SMSServiceBinder serviceBinder = service as SMSServiceBinder;
            if (serviceBinder != null)
            {
                this.binder = serviceBinder;
                this.binder.IsBound = true;
                Log.Debug("SMSServiceConnection", "OnServiceConnected Called");
                // raise the service connected event
                this.ServiceConnected(this, new SMSServiceConnectedEventArgs() { Binder = service });

                // now that the Service is bound, we can start gathering some location data
                //serviceBinder.Service.StartLocationUpdates();
            }
        }

        // This will be called when the Service unbinds, or when the app crashes
        public void OnServiceDisconnected(ComponentName name)
        {
            this.binder.IsBound = false;
            Log.Debug("SMSServiceConnection", "Service unbound");
        }
    }
}