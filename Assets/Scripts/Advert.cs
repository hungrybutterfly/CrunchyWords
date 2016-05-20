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
#if (UNITY_IOS || UNITY_ANDROID)
        GameObject skipButton = GameObject.Find("SkipButton");
        skipButton.SetActive(false);

        //Call for an Ad to display
        SessionManager _session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (_session)
        {
            //Display and Ad (bool is for true for static or video)
            _session.m_AdvertManager.DisplayAd(_session.m_AdvertStatic);
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
        Session.ChangeScene(Session.m_AdvertReturn);
    }
}
