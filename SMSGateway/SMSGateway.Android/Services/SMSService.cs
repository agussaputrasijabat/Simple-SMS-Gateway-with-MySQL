using System;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using Android.Util;
using Android.Widget;
using MySql.Data.MySqlClient;
using SMSGateway.Models;

namespace SMSGateway.Droid.Services
{
    [Service]
    public class SMSService : Service
    {
        static readonly string TAG = typeof(SMSService).FullName;
        public IBinder Binder { get; set; }
        public Timer SMSTimer;
        private static SmsManager _smsManager;
        public static MySqlConnection _conn;
        private SMSDelivered mReceiver;
        private static Intent SentIntent;

        public override IBinder OnBind(Intent intent)
        {
            Binder = new SMSServiceBinder(this);
            return Binder;
        }

        public override void OnCreate()
        {
            // This method is optional to implement
            base.OnCreate();
            Log.Debug(TAG, "OnCreate");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, "Service started");

            _smsManager = SmsManager.Default;

            if (SMSTimer == null)
            {
                SMSTimer = new Timer(20 * 1000);
                SMSTimer.Elapsed -= SMSTimer_Elapsed;
                SMSTimer.Elapsed += SMSTimer_Elapsed;
                SMSTimer.Start();
            }

            SentIntent = new Intent("SMS_SENT");
            mReceiver = new SMSDelivered();
            RegisterReceiver(mReceiver, new IntentFilter("SMS_SENT"));

            Connect();

            return StartCommandResult.Sticky;
        }

        public static ConnectionState Connect()
        {
            try
            {
                if (_conn == null || _conn.State != ConnectionState.Open)
                {
                    _conn = new MySqlConnection(ConnectionString);
                    _conn.Open();
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
                try { Toast.MakeText(Application.Context, "Connect: " + Ex.Message, ToastLength.Long).Show(); } catch { }
            }

            return _conn.State;
        }

        public static ConnectionState Reconnect()
        {
            if (_conn != null) _conn.Close();
            return Connect();
        }

        private void SMSTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var SMS = Get();
                if (SMS != null)
                {
                    SendSMS(SMS);
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
        }

        public static void SendSMS(SMSModel SMS)
        {
            try
            {
                if (SMS != null)
                {
                    Console.WriteLine("Sending sms to phone number: " + SMS.PhoneNumber);
                    _smsManager = SmsManager.Default;
                    
                    string SmsId = SMS.Id.ToString();
                    if (SentIntent.Extras != null) SentIntent.Extras.Clear();

                    SentIntent.PutExtra("SMSGatewayId", SmsId);
                    SentIntent.PutExtra("Message", SMS.Message);
                    SentIntent.PutExtra("PhoneNumber", SMS.PhoneNumber);
                    
                    var piSent = PendingIntent.GetBroadcast(Application.Context, SMS.Id, SentIntent, 0);
                    var piDelivered = PendingIntent.GetBroadcast(Application.Context, 0, new Intent("SMS_DELIVERED"), 0);

                    SMS.Status = "Mengirim";
                    UpdateSMS(SMS);

                    _smsManager.SendTextMessage(SMS.PhoneNumber, null, SMS.Message, piSent, piDelivered);
                }
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
        }

        public static string ConnectionString
        {
            get
            {
                return $"server={Settings.MySQLServer};user={Settings.MySQLUser};database={Settings.MySQLDatabase};port=3306;password={Settings.MySQLPassword};charset=utf8";
            }
        }

        public static SMSModel Get(int Id = 0)
        {
            try
            {
                Connect();
                string Query = "SELECT * FROM smsgateway WHERE status='Belum Terkirim' limit 1";
                if (Id > 0) Query = "SELECT * FROM smsgateway WHERE id=" + Id;
                using (MySqlCommand Command = new MySqlCommand(Query, _conn))
                {
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            var Log = new SMSModel
                            {
                                Id = (int)Reader[0],
                                PhoneNumber = (string)Reader[1],
                                Message = (string)Reader[2],
                                Status = (string)Reader[3]
                            };

                            return Log;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
            return null;
        }

        public static void UpdateSMS(SMSModel SMS)
        {
            try
            {
                Connect();
                string Query = $"UPDATE smsgateway SET status='{SMS.Status}' WHERE id={SMS.Id}";
                using (MySqlCommand Command = new MySqlCommand(Query, _conn))
                {
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
        }

        public static List<SMSModel> SentList()
        {
            try
            {
                Connect();
                string Query = "SELECT * FROM smsgateway WHERE status IN ('Terkirim')";
                using (MySqlCommand Command = new MySqlCommand(Query, _conn))
                {
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        var Messages = new List<SMSModel>();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                Messages.Add(new SMSModel
                                {
                                    Id = (int)Reader[0],
                                    PhoneNumber = (string)Reader[1],
                                    Message = (string)Reader[2],
                                    Status = (string)Reader[3]
                                });
                            }
                            return Messages;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
            return null;
        }

        public static List<SMSModel> PendingList()
        {
            try
            {
                Connect();
                string Query = "SELECT * FROM smsgateway WHERE status NOT IN ('Terkirim')";
                using (MySqlCommand Command = new MySqlCommand(Query, _conn))
                {
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        var Messages = new List<SMSModel>();

                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                Messages.Add(new SMSModel
                                {
                                    Id = (int)Reader[0],
                                    PhoneNumber = (string)Reader[1],
                                    Message = (string)Reader[2],
                                    Status = (string)Reader[3]
                                });
                            }

                            return Messages;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message + Ex.StackTrace);
            }
            return null;
        }

        public override bool OnUnbind(Intent intent)
        {
            // This method is optional to implement
            Log.Debug(TAG, "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            // This method is optional to implement
            Log.Debug(TAG, "OnDestroy");
            Binder = null;
            UnregisterReceiver(mReceiver);
            base.OnDestroy();
        }
    }
}