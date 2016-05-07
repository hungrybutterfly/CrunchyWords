///----------------------------------------------
/// Flurry Analytics Plugin
/// Copyright © 2016 Aleksei Kuzin
///----------------------------------------------

using UnityEngine;
using System.Collections;

namespace KHD {

    public class FlurryAnalyticsHelper : MonoBehaviour {

        /// <summary>
        /// iOS API Key.
        /// </summary>
        [SerializeField] private string _iOSApiKey;

        /// <summary>
        /// Android API Key.
        /// </summary>
        [SerializeField] private string _androidApiKey;

        /// <summary>
        /// Enable debug log.
        /// </summary>
        [SerializeField] private bool _enableDebugLog = false;

        /// <summary>
        /// Send crash reports to Flurry.
        /// </summary>
        [SerializeField] private bool _sendCrashReports = true;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            FlurryAnalytics.Instance.SetDebugLogEnabled(_enableDebugLog);

            FlurryAnalytics.Instance.StartSession(_iOSApiKey, _androidApiKey, _sendCrashReports);
        }
    }
}
