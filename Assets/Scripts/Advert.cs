///////////////////////////////////////////////////
/// Advert.cs
/// Chris Dawson 2016
/// 
/// Basic scene script to trigger ads
/// 
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
    int m_AdsSeen;

    //Initial Call
    void Start()
    {
        m_AdsSeen = 0;
        StartAd();
    }

    void StartAd()
    {
        SessionManager _session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        //Not online - allow skip...
        if (_session && _session.m_AdvertManager.IsOnline())
        {
            //iOS/Android ? Hide skip button
#if (UNITY_IOS || UNITY_ANDROID)
            GameObject skipButton = GameObject.Find("SkipButton");
            skipButton.SetActive(false);

            //Call for an Ad to display
			if( _session.m_AdvertStatic)
			{
            	_session.m_AdvertManager.DisplayStaticAd();
			}else{
				_session.m_AdvertManager.DisplayVideoAd(_session.m_AdvertSkippable); //false for non-skippable ads
			}
            //Set the advert callback
            _session.m_AdvertManager.m_Callback = AdvertReturn;
#endif
            m_AdsSeen++;
        }
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

        // have we seen enough ads
        if (m_AdsSeen != Session.m_AdvertCount)
        {
            // start a new ad
            StartAd();
        }
        else
        {
            if (Session.m_AdvertNextScene != "")
                Session.ChangeScene(Session.m_AdvertNextScene);
            else
                Session.ReturnScene("Advert");
        }
    }
}
