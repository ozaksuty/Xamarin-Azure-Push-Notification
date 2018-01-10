using Android.App;
using Android.Content;
using Gcm.Client;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsAzure.Messaging;

namespace AcikAkademiPN.Droid
{
    [Service]
    public class PushHandlerService : GcmServiceBase
    {
        private NotificationHub Hub { get; set; }
        public static string RegistrationID { get; set; }

        public PushHandlerService() : base(App.SenderID)
        {

        }

        protected override void OnError(Context context, string errorId)
        {
            //Log
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            var msg = new StringBuilder();
            if (intent != null && intent.Extras != null)
            {
                foreach (var item in intent.Extras.KeySet())
                {
                    msg.AppendLine(item + "=" + intent.Extras.Get(item).ToString());
                }
            }

            string messageText = intent.Extras.GetString("message");
            if (!String.IsNullOrEmpty(messageText))
            {
                CreateNotification("Notification", messageText);
            }
            else
            {
                CreateNotification("Notification", msg.ToString());
            }
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            RegistrationID = registrationId;

            Hub = new NotificationHub(App.NotificationHubName,
                App.ListenConnectionString, context);

            try
            {
                Hub.UnregisterAll(registrationId);
            }
            catch (System.Exception ex)
            {
                //Log
            }

            try
            {
                List<string> tags = new List<string>();
                Hub.Register(registrationId, tags.ToArray());
            }
            catch (System.Exception ex)
            {
                //Log
            }
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            //Log
        }

        void CreateNotification(string title, string desc)
        {
            var notificationManager = GetSystemService(Context.NotificationService)
                as NotificationManager;

            var uiIntent = new Intent(this, typeof(MainActivity));
            var notification = new Notification(Android.Resource.Drawable.ButtonMinus, title)
            {
                Flags = NotificationFlags.AutoCancel
            };
            notification.SetLatestEventInfo(this, title, desc,
                PendingIntent.GetActivity(this, 0, uiIntent, 0));
            notificationManager.Notify(1, notification);
            DialogNotify(title, desc);
        }

        void DialogNotify(string title, string desc)
        {
            MainActivity.instance.RunOnUiThread(() =>
            {
                AlertDialog.Builder dlg = new AlertDialog.Builder(MainActivity.instance);
                AlertDialog alert = dlg.Create();
                alert.SetTitle(title);
                alert.SetButton("OK", delegate
                {
                    alert.Dismiss();
                });
                alert.SetMessage(desc);
                alert.Show();
            });
        }
    }
    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE}, 
        Categories = new string[] { App.PackageName })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
        Categories = new string[] { App.PackageName })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
        Categories = new string[] { App.PackageName })]
    public class MyBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
    {
        public static string[] SENDER_IDS = new string[] { App.SenderID };
        public const string TAG = "MyBroadcastReceiver-GCM";
    }
}