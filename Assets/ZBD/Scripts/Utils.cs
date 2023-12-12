using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace ZBD
{
    public class Utils : MonoBehaviour
    {
        public static Utils Instance;
        public List<Vector2> touchPoints = new List<Vector2>();
        public List<Vector3> accelerometerData = new List<Vector3>();
        public int accelerometerCount;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
        public void ResetTouch()
        {
            touchPoints = new List<Vector2>();
            accelerometerCount = 0;
        }
        private void Update()
        {
            // Check if there is any touch on the screen
            if (Input.touchCount > 0)
            {
                // Loop over all touches and save their position
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    // Save only the touch points when a touch begins
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchPoints.Add(touch.position);
                    }
                }
            }

            if (Input.acceleration.magnitude > 0)
            {

                accelerometerCount++;
            }
        }


        public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool HasUsageStatsPermissionPage()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass pluginClass = new AndroidJavaClass("io.zebedee.zbdsdk.UsageStatsHelper");

                if (pluginClass != null)
                {
                    return pluginClass.CallStatic<bool>("hasUsageStatsPermission", currentActivity);
                }
            }
            return false;
        }

        public void OpenUsageStatsPermissionPage()
        {
            if (Application.platform == RuntimePlatform.Android)
            {

                AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass pluginClass = new AndroidJavaClass("io.zebedee.zbdsdk.UsageStatsHelper");

                if (pluginClass != null)
                {
                    pluginClass.CallStatic("openUsageAccessSettings", currentActivity);
                }

            }
        }

        public string GetAndroidUsageStats()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass pluginClass = new AndroidJavaClass("io.zebedee.zbdsdk.UsageStatsHelper");

                if (pluginClass != null)
                {
                    string usageStats = pluginClass.CallStatic<string>("getUsageStats", currentActivity, (1000 * 3600 * 24));
                    return usageStats;
                }
                else
                {
                    throw new Exception("plugin error");
                }
            }
            throw new Exception("platform not supported");
        }

    }

}