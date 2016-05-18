///////////////////////////////////////////////////
/// Advert.cs
/// Chris Dawson 2016
/// 
/// Handles both static/interstatial ads(Google AdMob) and Video ads(UnityAds)
/// For AdMob schemes see - https://apps.admob.com/#home
/// For Unity - see Services in Unity - https://dashboard.unityads.unity3d.com/
/// 
/// Things to remember :
/// iOS - You must take the admob framework into xcode build, also it must be in the folder itself
///////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdvertManager : MonoBehaviour
{
    //Waiting on an advert? Set to true to trigger AD
    private bool m_WaitingOnFullscreenAd = false;

    //What type of ad?
    private bool m_StaticAd = true;

    //Which Video AD mode?
    private bool m_VideoADBasic = true;

    //Hold static ad
    InterstitialAd m_InterstitialAd = null;

    //Request made?
    private bool m_RequestMade = false;

    //The callback from the ad (returns true/false depending on success)
    public Action<bool> m_Callback { get; set; }


    //Request an ad ready for later
    public void RequestAd()
    {
        m_RequestMade = true;
        //Request the static
        //GOOGLE ADS
        RequestStaticAd();
        //NOTE: Only static requires this
    }

    //Display Ad now
    public void DisplayAd(bool _static)
    {
        m_StaticAd = _static;
        if (!m_RequestMade)
        {
            Debug.Log("SLOW!! YOU MUST REQUEST AN AD PREVIOUS TO THIS!");
            RequestAd();
        }
#if (UNITY_IOS || UNITY_ANDROID)
        //Show an Ad...
        m_WaitingOnFullscreenAd = true;

        // send metrics
        int Static = 0;
        if (m_StaticAd) Static = 1;
        int VideoADBasic = 0;
        if (m_VideoADBasic) VideoADBasic = 1;
        SessionManager.MetricsLogEventWithParameters("Advert", new Dictionary<string, string>()
        {
            { "Static", Static.ToString() },
            { "VideoADBasic", VideoADBasic.ToString() },
        });
#endif
    }

    //Update
    private void Update()
    {
        //Waiting on Ad?
        if (m_WaitingOnFullscreenAd)
        {
            //Are we static or video?
            if (m_StaticAd)
            {
                //GOOGLE ADS
                ShowStaticAd();
            }
            else
            {
                //UNITY ADS           
                if (m_VideoADBasic)
                {
                    //SKIPPABLE
                    ShowDefaultAd();
                }
                else
                {
                    //NON_SKIPPABLE
                    ShowRewardedAd();
                }
            }
        }
    }

    //GOOGLE AD - Static - Request
    private void RequestStaticAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-5274247676971508/7231635075"; //Android Interstitial ID
        string deviceID = GetAndroidAdMobID();
#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-5274247676971508/1324702270"; //iOS Interstitial ID
		string deviceID = GetIOSAdMobID();
#else
        string adUnitId = "unexpected_platform";
        string deviceID = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        m_InterstitialAd = new InterstitialAd(adUnitId);
        // Create an ad request.
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice(deviceID)
            .Build();

        // Callbacks
        //m_InterstitialAd.OnAdLoaded += HandleStaticLoaded;
        m_InterstitialAd.OnAdFailedToLoad += HandleStaticFailedToLoad;
        //m_InterstitialAd.OnAdOpening += HandleStaticOpened;
        m_InterstitialAd.OnAdClosed += HandleStaticClosed;
        //m_InterstitialAd.OnAdLeavingApplication += HandleStaticLeftApplication;

        // Load the interstitial with the request.
        m_InterstitialAd.LoadAd(request);
    }

    //GOOGLE AD - Static - Show
    private void ShowStaticAd()
    {
        if (m_InterstitialAd.IsLoaded())
        {
            m_InterstitialAd.Show();
            m_WaitingOnFullscreenAd = false;
            m_RequestMade = false;
        }
        else
        {
            print("Static is not ready yet.");
        }
    }

    //UNITY AD - Basic Video (Skippable)
    private void ShowDefaultAd()
    {
#if UNITY_ADS
        const string NormalZoneId = "video";

        if (!Advertisement.IsReady(NormalZoneId))
        {
            Debug.Log("Ads not ready for default zone");
            return;
        }

        m_WaitingOnFullscreenAd = false;
        m_RequestMade = false;
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(NormalZoneId, options);        
#endif
    }

    //UNITY AD - Unskippable Video
    private void ShowRewardedAd()
    {
#if UNITY_ADS
        const string RewardedZoneId = "rewardedVideo";

        if (!Advertisement.IsReady(RewardedZoneId))
        {
            Debug.Log(string.Format("Ads not ready for zone '{0}'", RewardedZoneId));
            return;
        }

        m_WaitingOnFullscreenAd = false;
        m_RequestMade = false;
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(RewardedZoneId, options);        
#endif
    }

    //UNITY AD - callback on end
#if UNITY_ADS
    private void HandleShowResult(ShowResult result)
    {
        bool success = false;
        switch (result)
        {
            case ShowResult.Finished:
                //Debug.Log("The ad was successfully shown.");
                success = true;
                break;
            case ShowResult.Skipped:
                //Debug.Log("The ad was skipped before reaching the end.");
                success = true;
                break;
            case ShowResult.Failed:
                //Debug.LogError("The ad failed to be shown.");
                success = false;
                break;
        }
        //Next Scene
        m_Callback(success);
        //Next Ad
        RequestAd();
    }
#endif

    //GOOGLE ADS - callbacks
    public void HandleStaticFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //print("HandleStaticFailedToLoad event received with message: " + args.Message);
        //Next Scene
        m_Callback(false);
        //Next Ad
        RequestAd();
    }
    public void HandleStaticClosed(object sender, EventArgs args)
    {
        //Next Scene
        m_Callback(true);
        //Next Ad
        RequestAd();
    }

    //Return General DeviceID
    public string GetDeviceID()
    {
        //Rely on Unity
        return SystemInfo.deviceUniqueIdentifier;
    }

    //GOOGLE ADS - Return Device ID
#if UNITY_ANDROID
    private string GetAndroidAdMobID()
    {
        UnityEngine.AndroidJavaClass up = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer");
        UnityEngine.AndroidJavaObject currentActivity = up.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity");
        UnityEngine.AndroidJavaObject contentResolver = currentActivity.Call<UnityEngine.AndroidJavaObject>("getContentResolver");
        UnityEngine.AndroidJavaObject secure = new UnityEngine.AndroidJavaObject("android.provider.Settings$Secure");
        string deviceID = secure.CallStatic<string>("getString", contentResolver, "android_id");
        return Md5Sum(deviceID).ToUpper();
    }
#endif

    //GOOGLE ADS - Return Device ID
#if UNITY_IPHONE
	private string GetIOSAdMobID() {
		return Md5Sum(UnityEngine.iOS.Device.advertisingIdentifier);
	}
#endif

    //GOOGLE ADS - Actual Md5Sum return
    //Chris - Must remember this uses cryptography - issues!
    private string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }
}
