using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using Firebase.Messaging;
using Plugin.Firebase.Core.Platforms.Android;
using System;

namespace MeteoApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        const string TAG = "MainActivity";
        const int NOTIFICATION_REQUEST_CODE = 1234;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Gestisci eventuali dati extra dall'intent
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.Get(key);
                    Log.Debug(TAG, $"Key: {key} Value: {value}");
                }
            }

            // Abilita AutoInit per Firebase Messaging
            RuntimeEnableAutoInit();

            // Sottoscrivi ai topic desiderati
            SubscribeTopics();

            // Registra e visualizza il token FCM
            LogRegToken();

            // Richiedi i permessi per le notifiche
            AskNotificationPermission();
        }

        public void RuntimeEnableAutoInit()
        {
            FirebaseMessaging.Instance.AutoInitEnabled = true;
        }

        public void DeviceGroupUpstream()
        {
            string to = "a_unique_key"; // la chiave di notifica
            int msgId = 0; // Incrementa questo per ogni messaggio

            var message = new RemoteMessage.Builder(to)
                .SetMessageId(msgId.ToString())
                .AddData("hello", "world")
                .Build();

            FirebaseMessaging.Instance.Send(message);
        }

        public void SendUpstream()
        {
            const string SENDER_ID = "YOUR_SENDER_ID";
            int messageId = 0; // Incrementa per ogni messaggio

            FirebaseMessaging fm = FirebaseMessaging.Instance;
            fm.Send(new RemoteMessage.Builder($"{SENDER_ID}@fcm.googleapis.com")
                .SetMessageId(messageId.ToString())
                .AddData("my_message", "Hello World")
                .AddData("my_action", "SAY_HELLO")
                .Build());
        }

        private void SubscribeTopics()
        {
            FirebaseMessaging.Instance.SubscribeToTopic("weather")
                .AddOnCompleteListener(new OnCompleteListener(task =>
                {
                    string msg = "Subscribed";
                    if (!task.IsSuccessful)
                    {
                        msg = "Subscribe failed";
                    }
                    Log.Debug(TAG, msg);
                    Toast.MakeText(this, msg, ToastLength.Short).Show();
                }));
        }

        private void LogRegToken()
        {
            FirebaseMessaging.Instance.GetToken()
                .AddOnCompleteListener(new OnCompleteListener(task =>
                {
                    if (!task.IsSuccessful)
                    {
                        Log.Warn(TAG, "Fetching FCM registration token failed", task.Exception);
                        return;
                    }

                    // Ottieni il nuovo token di registrazione FCM
                    string token = task.Result.ToString();

                    // Log e Toast
                    string msg = "FCM Registration token: " + token;
                    Log.Debug(TAG, msg);
                    Toast.MakeText(this, msg, ToastLength.Short).Show();
                }));
        }

        private void AskNotificationPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) == Permission.Granted)
                {
                    // FCM SDK (e la tua app) può inviare notifiche.
                }
                else if (ShouldShowRequestPermissionRationale(Manifest.Permission.PostNotifications))
                {
                    // Mostra una UI educativa
                    ShowPermissionRationale();
                }
                else
                {
                    // Richiedi direttamente il permesso
                    RequestPermissions(new[] { Manifest.Permission.PostNotifications }, NOTIFICATION_REQUEST_CODE);
                }
            }
        }

        private void ShowPermissionRationale()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Permesso Notifiche");
            builder.SetMessage("L'app ha bisogno del permesso per inviarti notifiche importanti.");
            builder.SetPositiveButton("OK", (senderAlert, args) =>
            {
                RequestPermissions(new[] { Manifest.Permission.PostNotifications }, NOTIFICATION_REQUEST_CODE);
            });
            builder.SetNegativeButton("No grazie", (senderAlert, args) =>
            {
                // L'utente ha rifiutato, continua senza notifiche
            });
            Dialog dialog = builder.Create();
            dialog.Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == NOTIFICATION_REQUEST_CODE)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // Il permesso è stato concesso, la tua app può inviare notifiche.
                }
                else
                {
                    // Il permesso è stato negato, informa l'utente che le notifiche non saranno mostrate.
                }
            }
        }
    }
}
