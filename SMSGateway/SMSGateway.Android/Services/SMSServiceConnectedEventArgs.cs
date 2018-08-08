using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SMSGateway.Droid.Services
{
    public class SMSServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}