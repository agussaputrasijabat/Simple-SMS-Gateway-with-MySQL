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
    public class SMSServiceBinder : Binder
    {
        public SMSService Service
        {
            get { return this.service; }
        }
        protected SMSService service;

        public bool IsBound { get; set; }

        public SMSServiceBinder(SMSService service)
        {
            this.service = service;
        }
    }
}