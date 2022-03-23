/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using UnityEngine.Events;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        MobileAds.Initialize(InitializationStatus => { });
        this.RequestBanner();
        this.bannerView.Hide();
        this.RequestAndLoadInterstitialAd();
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    #region BANNER ADS
    private void RequestBanner()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //sample
        string adUnitId = "ca-app-pub-7362583481441359/1943193946";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716"; //sample
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    public void ShowBannerAd()
    {
        this.bannerView.Show();
    }

    public void HideBannerAd()
    {
        this.bannerView.Hide();
    }
    #endregion

    #region INTERSTITIAL ADS

    public void RequestAndLoadInterstitialAd()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //sample
        string adUnitId = "ca-app-pub-7362583481441359/6754868626";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910"; //sample
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial before using it
        DestroyInterstitialAd();
        interstitialAd = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitialAd.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitialAd.OnAdClosed += HandleOnAdClosed;

        // Load an interstitial ad
        interstitialAd.LoadAd(this.CreateAdRequest());
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet");
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.ToString());
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        this.RequestAndLoadInterstitialAd();
    }


    #endregion
}
*/