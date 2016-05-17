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
    //Initial Call
    void Start()
    {
        //iOS/Android ? Hide skip button
#if !(UNITY_IOS || UNITY_ANDROID)
        Button skipButton = GameObject.Find("SkipButton").GetComponent<Button>();
        skipButton.transform.localScale = new Vector2(1, 1);
#else
        //Call for an Ad to display
        SessionManager _session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (_session)
        {
            // was this the last level in the zone
            bool Static = true;
            LevelData Data = GameObject.Find("SessionManager").GetComponent<LevelData>();
            if (_session.m_CurrentLevel == Data.m_Zones[_session.m_CurrentZone].m_Levels.Length - 1 && !_session.m_AlreadyDone)
                Static = false;

            //Display and Ad (bool is for true for static or video)
            _session.m_AdvertManager.DisplayAd(Static);
            //Set the advert callback
            _session.m_AdvertManager.m_Callback = AdvertReturn;
        }
#endif
    }

    //Advert return call (with bool on success)
    public void AdvertReturn(bool _success)
    {
        Clicked();
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
}
