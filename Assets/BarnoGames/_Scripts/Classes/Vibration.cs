using UnityEngine;

namespace BarnoGames.Runner2020
{
    public static class Vibration
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        private static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        private static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        private static AndroidJavaClass unityPlayer;
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaObject vibrator;
#endif
        private static string vibrateClass = "vibrate";
        // TODO:: SETTINGS ::  ADD DISABLE VIBRATION OPTIONS


        public static bool HasVibrator()
        {
            return IsAndroid();
        }

        public static void Vibrate(long milliseconds = 250)
        {
            if(IsAndroid())
            {
                // 1 SECOND = 1000 MILLISECONS
                // 250 MILLISECONDS ==> 250/1000  = 0.25 seconds
                vibrator.Call(vibrateClass, milliseconds);
            }
            else
            {
                Handheld.Vibrate();
            }
        }

        public static void Cancel()
        {
            if(IsAndroid())
            {
                vibrator.Call("cancel");
            }
        }

        private static bool IsAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

    }
}