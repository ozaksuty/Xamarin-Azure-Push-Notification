using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using WindowsAzure.Messaging;

namespace AcikAkademiPN.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private SBNotificationHub Hub { get; set; }
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge
                    | UIUserNotificationType.Sound, new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationType = UIRemoteNotificationType
                    .Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType
                    .Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationType);
            }

            return base.FinishedLaunching(app, options);
        }

        public override void DidReceiveRemoteNotification(UIApplication application,
            NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            ProcessNotification(userInfo);
        }

        public override void ReceivedRemoteNotification(UIApplication application,
            NSDictionary userInfo)
        {
            ProcessNotification(userInfo);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application,
            NSData deviceToken)
        {
            Hub = new SBNotificationHub(App.ListenConnectionString, App.NotificationHubName);

            Hub.UnregisterAllAsync(deviceToken, (error) =>
            {
                if (error != null)
                {
                    Console.WriteLine("UnregisterAllAsync - error!");
                    return;
                }

                Hub.RegisterNativeAsync(deviceToken, null, (errorCallback) =>
                {
                    if (errorCallback != null)
                        Console.WriteLine("RegisterNativeAsync - errorCallback!");
                });
            });
        }

        void ProcessNotification(NSDictionary options)
        {
            if (options != null && options.ContainsKey(new NSString("aps")))
            {
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

                string alert = "";

                if (aps.ContainsKey(new NSString("alert")))
                {
                    alert = (aps[new NSString("alert")] as NSString).ToString();
                }

                if (!String.IsNullOrEmpty(alert))
                {
                    UIAlertView aView = new UIAlertView(
                        "Notification", alert, null, "OK", null);
                    aView.Show();
                }
            }
        }
    }
}