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

public class Advert : MonoBehaviour
{
    //Waiting on an advert? Set to true to trigger AD
    private bool m_WaitingOnFullscreenAd = false;

    //What type of ad?
    private bool m_StaticAd = true;

    //Which Video AD mode?
    private bool m_VideoADBasic = true;

    //Hold static ad
    InterstitialAd m_InterstitialAd = null;

    //Initial Call
    void Start()
    {
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

        //iOS/Android ? Hide skip button
#if !(UNITY_IOS || UNITY_ANDROID)
        Button skipButton = GameObject.Find("SkipButton").GetComponent<Button>();
		skipButton.transform.localScale = new Vector2 (1,1);
#endif

#if (UNITY_IOS || UNITY_ANDROID)
        //Show an Ad...
        m_WaitingOnFullscreenAd = true;

		//Request the static
		if (m_StaticAd) 
		{
			//GOOGLE ADS
			RequestStaticAd();
		}
#endif
    }

    //Update
    void Update()
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
		string deviceID = GetIOSAdMobID(); //"15784d9be67b737d7cd7d87fccdeb25f" // Chris iPad 2
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
        m_InterstitialAd.OnAdLoaded += HandleStaticLoaded;
        m_InterstitialAd.OnAdFailedToLoad += HandleStaticFailedToLoad;
        m_InterstitialAd.OnAdOpening += HandleStaticOpened;
        m_InterstitialAd.OnAdClosed += HandleStaticClosed;
        m_InterstitialAd.OnAdLeavingApplication += HandleStaticLeftApplication;

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

		if (!Advertisement.IsReady (NormalZoneId)) {
			Debug.Log ("Ads not ready for default zone");
			return;
		}

        var options = new ShowOptions { resultCallback = HandleShowResult };
		Advertisement.Show (NormalZoneId, options);
		m_WaitingOnFullscreenAd = false;
#endif
    }

    //UNITY AD - Unskippable Video
    private void ShowRewardedAd()
    {
#if UNITY_ADS
        const string RewardedZoneId = "rewardedVideo";

		if (!Advertisement.IsReady (RewardedZoneId)) {
			Debug.Log (string.Format ("Ads not ready for zone '{0}'", RewardedZoneId));
			return;
		}

		var options = new ShowOptions { resultCallback = HandleShowResult };
		Advertisement.Show (RewardedZoneId, options);
		m_WaitingOnFullscreenAd = false;
#endif
    }

    //UNITY AD - callback on end
#if UNITY_ADS
	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError ("The ad failed to be shown.");
			break;
		}
		//Next Scene
		Clicked ();
	}
#endif

    //GOOGLE ADS - callbacks
    public void HandleStaticLoaded(object sender, EventArgs args)
    {
        print("HandleStaticLoaded event received.");
    }
    public void HandleStaticFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleStaticFailedToLoad event received with message: " + args.Message);
        //Next Scene
        Clicked();
    }
    public void HandleStaticOpened(object sender, EventArgs args)
    {
        print("HandleStaticOpened event received");
    }
    void HandleStaticClosing(object sender, EventArgs args)
    {
        print("HandleStaticClosing event received");
    }
    public void HandleStaticClosed(object sender, EventArgs args)
    {
        print("HandleStaticClosed event received");
        //Next Scene
        Clicked();
    }
    public void HandleStaticLeftApplication(object sender, EventArgs args)
    {
        print("HandleStaticLeftApplication event received");
    }

    //This is used by the 'Skip' button (only in non-iOS/Android versions)
    //iOS/Android will call this automatically on end video
    public void Clicked()
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();

        // was this the last level in the zone
        LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
        if (Session.m_CurrentLevel == Data.m_Zones[Session.m_CurrentZone].m_Levels.Length - 1 && !Session.m_AlreadyDone)
            Session.ChangeScene("Zone");
        else
            Session.ChangeScene("Level");
    }

    //GOOGLE ADS - Return Device ID
#if UNITY_ANDROID
	public static string GetAndroidAdMobID() {
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
	public static string GetIOSAdMobID() {
		return Md5Sum(UnityEngine.iOS.Device.advertisingIdentifier);
	}
#endif

    //GOOGLE ADS - Actual Md5Sum return
    //Chris - Must remember this uses cryptography - issues!
    public static string Md5Sum(string strToEncrypt)
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
