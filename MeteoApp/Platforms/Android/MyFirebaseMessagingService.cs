using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Firebase.Messaging;

namespace MeteoApp
{
    [Service(Name = "com.google.firebase.example.messaging.MyFirebaseMessagingService", Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            Log.Debug(TAG, "Messaggio ricevuto da: " + message.From);

            if (message.Data.Count > 0)
            {
                Log.Debug(TAG, "Message data payload: " + message.Data);
                // Se hai bisogno di gestire i dati, fallo qui
            }

            if (message.GetNotification() != null)
            {
                Log.Debug(TAG, "Message Notification Body: " + message.GetNotification().Body);
                // Visualizza la notifica
                SendNotification(message.GetNotification().Body);
            }
        }

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Log.Debug(TAG, $"Nuovo token FCM: {token}");
            SendRegistrationToServer(token);
        }

        private void SendRegistrationToServer(string token)
        {
            // Invia il token al tuo server se necessario
        }

        private void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Immutable);

            var channelId = "fcm_default_channel";
            var defaultSoundUri = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
            var notificationBuilder = new AndroidX.Core.App.NotificationCompat.Builder(this, channelId)
                .SetSmallIcon(Resource.Drawable.maui_splash) // Assicurati di avere questa icona
                .SetContentTitle("Messaggio FCM")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetSound(defaultSoundUri)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, "Notifiche Generali", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
            }

            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
