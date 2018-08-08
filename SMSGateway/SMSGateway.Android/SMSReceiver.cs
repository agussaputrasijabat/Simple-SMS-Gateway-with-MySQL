using System;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Widget;

namespace SMSGateway.Droid
{
    [BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SMSReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Bundle bundle = intent.Extras;
            string SmsBody = String.Empty;
            if (bundle != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    SmsMessage[] msgs = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                    var smstext = new StringBuilder();
                    foreach (var msg in msgs)
                    {
                        smstext.Append(msg.DisplayMessageBody.ToString());
                    }
                    SmsBody = smstext.ToString();//output the received sms
                }
                else
                {
                    var smsArray = (Java.Lang.Object[])bundle.Get("pdus");
                    SmsMessage[] messages = new SmsMessage[smsArray.Length];
                    for (int i = 0; i < smsArray.Length; i++)
                    {
                        messages[i] = SmsMessage.CreateFromPdu((byte[])smsArray[i]);
                    }
                    StringBuilder content = new StringBuilder();
                    if (messages.Length > 0)
                    {
                        foreach (var message in messages)
                        {
                            content.Append(message.DisplayMessageBody.ToString());
                        }
                    }
                    SmsBody = content.ToString(); //output the received sms
                }
            }

            //Toast.MakeText(context, $"Received message : {SmsBody.ToString()}", ToastLength.Short).Show();
        }
    }
}