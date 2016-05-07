﻿///----------------------------------------------
/// Flurry Analytics Plugin
/// Copyright © 2016 Aleksei Kuzin
///----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace KHD {

    [AddComponentMenu("")]
    public class FlurryAnalytics : SingletonCrossSceneAutoCreate<FlurryAnalytics> {

        public enum Gender {
            Female = 0,
            Male
        }

        /// <summary>
        /// On Destroy.
        /// </summary>
        protected override void OnDestroy() {
            base.OnDestroy();

#if UNITY_ANDROID
            FlurryAnalyticsAndroid.Dispose();
#endif
        }

        /// <summary>
        /// Start Flurry session.
        /// </summary>
        /// <param name="iOSApiKey">iOS API key.</param>
        /// <param name="androidApiKey">Android API key.</param>
        public void StartSession(string iOSApiKey, string androidApiKey, bool sendErrorsToFlurry) {
#if UNITY_IOS
            FlurryAnalyticsIOS.StartSession(iOSApiKey, sendErrorsToFlurry);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.Init(androidApiKey, sendErrorsToFlurry);
            FlurryAnalyticsAndroid.OnStartSession();
#endif
        }

        /// <summary>
        /// Enable/Disable Flurry SDK debug logs.
        /// 
        /// By default logs are disabled.
        /// Should be called before StartSession.
        /// </summary>
        public void SetDebugLogEnabled(bool enabled) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetDebugLogEnabled(enabled);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetLogEnabled(enabled);
#endif
        }

        /// <summary>
        /// Explicitly set app version..
        /// Should be called before StartSession.
        /// </summary>
        /// <param name="appVersion">App version.</param>
        public void SetAppVersion(string appVersion) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetAppVersion(appVersion);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetVersionName(appVersion);
#endif
        }

        /// <summary>
        /// Sets the time the app may be in the background before starting a new session upon resume.
        /// Default value 10 seconds.
        ///
        /// Should be called before StartSession.
        /// </summary>
        public void SetSessionContinueSeconds(int seconds) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetSessionContinueSeconds(seconds);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetContinueSessionMillis(seconds * 1000);
#endif
        }

        /// <summary>
        /// Enables Flurry Pulse.
        /// Please see https://developer.yahoo.com/flurry-pulse/ for more details.
        /// 
        /// Should be called before StartSession.
        /// </summary>
        public void SetPulseEnabled(bool enabled) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetPulseEnabled(enabled);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetPulseEnabled(enabled);
#endif
        }

        /// <summary>
        /// Assign a unique id for a user in your app.
        /// </summary>
        public void SetUserId(string userId) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetUserId(userId);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetUserId(userId);
#endif
        }
        
        /// <summary>
        /// Use this method to capture the age of your user.
        /// </summary>
        public void SetAge(int age) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetAge(age);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetAge(age);
#endif
        }
        
        /// <summary>
        /// Set user's gender.
        /// </summary>
        public void SetGender(FlurryAnalytics.Gender gender) {
#if UNITY_IOS
            FlurryAnalyticsIOS.SetGender(gender);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.SetGender(gender);
#endif
        }

        /// <summary>
        /// Records a custom event specified by eventName.
        /// </summary>
        /// <param name="eventName">
        /// Name of the event. For maximum effectiveness, we recommend using a naming scheme
        /// that can be easily understood by non-technical people in your business domain.
        /// </param>
        public void LogEvent(string eventName) {
#if UNITY_IOS
            FlurryAnalyticsIOS.LogEvent(eventName, false);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.LogEvent(eventName, false);
#endif
        }

        /// <summary>
        /// Records a timed event specified by eventName.
        /// </summary>
        /// <param name="eventName">
        /// Name of the event. For maximum effectiveness, we recommend using a naming scheme
        /// that can be easily understood by non-technical people in your business domain.
        /// </param>
        /// <param name="isTimed">If set to <c>true</c> event will be timed.
        /// Call EndTimedEvent to stop timed event.</param>
        public void LogEvent(string eventName, bool isTimed) {
#if UNITY_IOS
            FlurryAnalyticsIOS.LogEvent(eventName, isTimed);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.LogEvent(eventName, isTimed);
#endif
        }

        /// <summary>
        /// Records a custom parameterized event specified by eventName with parameters.
        /// A maximum of 10 parameter names may be associated with any event.
        /// </summary>
        /// <param name="eventName">
        /// Name of the event. For maximum effectiveness, we recommend using a naming scheme
        /// that can be easily understood by non-technical people in your business domain.
        /// </param>
        /// <param name="parameters">An immutable copy of map containing Name-Value pairs of parameters.</param>
        public void LogEventWithParameters(string eventName, Dictionary<string, string> parameters) {
#if UNITY_IOS
            FlurryAnalyticsIOS.LogEventWithParameters(eventName, parameters, false);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.LogEventWithParameters(eventName, parameters, false);
#endif
        }

        /// <summary>
        /// Records a custom parameterized timed event specified by eventName with parameters.
        /// A maximum of 10 parameter names may be associated with any event.
        /// </summary>
        /// <param name="eventName">
        /// Name of the event. For maximum effectiveness, we recommend using a naming scheme
        /// that can be easily understood by non-technical people in your business domain.
        /// </param>
        /// <param name="parameters">An immutable copy of map containing Name-Value pairs of parameters.</param>
        /// <param name="isTimed">If set to <c>true</c> event will be timed.
        /// Call EndTimedEvent to stop timed event.</param>
        public void LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed) {
#if UNITY_IOS
            FlurryAnalyticsIOS.LogEventWithParameters(eventName, parameters, isTimed);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.LogEventWithParameters(eventName, parameters, isTimed);
#endif
        }
        
        /// <summary>
        /// Ends a timed event specified by eventName and optionally updates parameters with parameters.
        /// </summary>
        /// <param name="eventName">
        /// Name of the event. For maximum effectiveness, we recommend using a naming scheme
        /// that can be easily understood by non-technical people in your business domain.
        /// </param>
        /// <param name="parameters">An immutable copy of map containing Name-Value pairs of parameters.</param>
        public void EndTimedEvent(string eventName, Dictionary<string, string> parameters = null) {
#if UNITY_IOS
            FlurryAnalyticsIOS.EndTimedEvent(eventName, parameters);
#elif UNITY_ANDROID
            FlurryAnalyticsAndroid.EndTimedEvent(eventName, parameters);
#endif
        }
    }
}
