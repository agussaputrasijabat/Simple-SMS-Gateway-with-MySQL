using System;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Widget;
using SMSGateway.Models;

namespace SMSGateway.Droid
{
    [BroadcastReceiver(Exported = true)]
    public class SMSDelivered : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Bundle Extras = intent.Extras;
                string SmsId = intent.GetStringExtra("SMSGatewayId");
                string Message = intent.GetStringExtra("Message");
                string PhoneNumber = intent.GetStringExtra("PhoneNumber");
                
                SMSModel SMS = null;

                if (Services.SMSService.Connect() == System.Data.ConnectionState.Open)
                {
                    SMS = Services.SMSService.Get(int.Parse(SmsId));
                }
                
                switch ((int)ResultCode)
                {
                    case (int)Result.Ok:
                        if (SMS != null)
                        {
                            SMS.Status = "Terkirim";
                            Services.SMSService.UpdateSMS(SMS);
                        }
                        break;
                    case (int)Result.Canceled:
                        if (SMS != null)
                        {
                            SMS.Status = "Belum Terkirim";
                            Services.SMSService.UpdateSMS(SMS);
                        }
                        break;
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
        }
    }
}