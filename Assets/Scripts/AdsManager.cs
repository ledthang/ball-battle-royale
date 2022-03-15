using System.Collections;
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
    public RewardedAd rewardedAdPlayAgain;
    public RewardedAd rewardedAdRandomSkinColor;
    public RewardedAd rewardedAdRandomIndicatorColor;
    //public bool adsShow = false;
    #region REWARDED ADS UNIT ID

#if UNITY_EDITOR
    const string rewardedAdPlayAgainUnitId = "unused";
    const string rewardedAdRandomSkinColorUnitId = "unusedd";
    const string rewardedAdRandomIndicatorColorUnitId = "unuseddd";
#elif UNITY_ANDROID
/*        const string rewardedAdPlayAgainUnitId = "ca-app-pub-3940256099942544/5224354917"; //SAMPLE
        const string rewardedAdRandomSkinColorUnitId = "ca-app-pub-3940256099942544/5224354917"; //SAMPLE     
        const string rewardedAdRandomIndicatorColorUnitId = "ca-app-pub-3940256099942544/5224354917"; //SAMPLE*/
        const string rewardedAdPlayAgainUnitId = "ca-app-pub-7362583481441359/6502521528";    
        const string rewardedAdRandomSkinColorUnitId = "ca-app-pub-7362583481441359/6781761110";        
        const string rewardedAdRandomIndicatorColorUnitId = "ca-app-pub-7362583481441359/6590189421"; 
#elif UNITY_IPHONE
        const string rewardedAdPlayAgainUnitId = "ca-app-pub-3940256099942544/1712485313"; //SAMPLE
        const string rewardedAdRandomSkinColorUnitId = "ca-app-pub-3940256099942544/1712485313"; //SAMPLE        
        const string rewardedAdRandomIndicatorColorUnitId = "ca-app-pub-3940256099942544/1712485313"; //SAMPLE
#else
        const string rewardedAdPlayAgainUnitId = "unexpected_platform";
        const string rewardedAdRandomSkinColorUnitId = "unexpected_platform";        
        const string rewardedAdRandomIndicatorColorUnitId = "unexpected_platform";
#endif

    #endregion

    public enum RewardedVideoAds
    {
        PlayAgain,
        RandomSkinColor,
        RandomIndicatorColor
    }

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
        this.rewardedAdPlayAgain = RequestAndLoadRewardedAd(RewardedVideoAds.PlayAgain);
        this.rewardedAdRandomSkinColor = RequestAndLoadRewardedAd(RewardedVideoAds.RandomSkinColor);
        this.rewardedAdRandomIndicatorColor = RequestAndLoadRewardedAd(RewardedVideoAds.RandomIndicatorColor);
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

    #region REWARDED ADS

    public RewardedAd RequestAndLoadRewardedAd(RewardedVideoAds type)
    {
        string adUnitId = type switch
        {
            RewardedVideoAds.PlayAgain => rewardedAdPlayAgainUnitId,
            RewardedVideoAds.RandomSkinColor => rewardedAdRandomSkinColorUnitId,
            RewardedVideoAds.RandomIndicatorColor => rewardedAdRandomIndicatorColorUnitId,
            _ => null
        };
        RewardedAd rewardedAd;
        // create new rewarded ad instance
        rewardedAd = new RewardedAd(adUnitId);

        switch (type)
        {
            case RewardedVideoAds.PlayAgain:
                rewardedAd.OnAdOpening += HandleOnAdOpening;
                rewardedAd.OnAdClosed += HandleRewardedAdClosed;
                rewardedAd.OnUserEarnedReward += ReceivePlayAgain;
                break;
            case RewardedVideoAds.RandomSkinColor:
                rewardedAd.OnAdLoaded += EnableRandomSkinColorButton;
                rewardedAd.OnAdOpening += HandleOnAdOpening;
                rewardedAd.OnAdClosed += HandleRewardedAdClosed;
                rewardedAd.OnAdClosed += DisableRandomSkinColorButton;
                rewardedAd.OnUserEarnedReward += ReceiveRandomSkinColor;
                break;
            case RewardedVideoAds.RandomIndicatorColor:
                rewardedAd.OnAdLoaded += EnableRandomIndicatorColorButton;
                rewardedAd.OnAdOpening += HandleOnAdOpening;
                rewardedAd.OnAdClosed += HandleRewardedAdClosed;
                rewardedAd.OnAdClosed += DisableRandomIndicatorColorButton;
                rewardedAd.OnUserEarnedReward += ReceiveRandomIndicatorColor;
                break;
        }


        // Create empty ad request
        RequestRewardedVideoAd(rewardedAd);

        return rewardedAd;
    }

    private void RequestRewardedVideoAd(RewardedAd rewardedAd)
    {
        Debug.Log("request ads");
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }

    public void ShowPlayAgainVideoRewardAd()
    {
        if (this.rewardedAdPlayAgain.IsLoaded())
        {
            this.rewardedAdPlayAgain.Show();
        }
        else
        {
            Debug.Log("rewarded Ad not loaded");
        }
    }
    public void ShowRandomSkinColorVideoRewardAd()
    {
        if (this.rewardedAdRandomSkinColor.IsLoaded())
        {
            this.rewardedAdRandomSkinColor.Show();
        }
        else
        {
            Debug.Log("rewarded Ad not loaded");
        }
    }
    public void ShowRandomIndicatorColorVideoRewardAd()
    {
        if (this.rewardedAdRandomIndicatorColor.IsLoaded())
        {
            this.rewardedAdRandomIndicatorColor.Show();
        }
        else
        {
            Debug.Log("rewarded Ad not loaded");
        }
    }

    public void EnablePlayAgainButton(object sender, EventArgs args)
    {
        //GameManager.Instance.EnablePlayAgainButton();
    }
    public void EnableRandomSkinColorButton(object sender, EventArgs args)
    {
        PlayerManager.Instance.randomSkinColorButton.interactable = true;
    }
    public void EnableRandomIndicatorColorButton(object sender, EventArgs args)
    {
        PlayerManager.Instance.randomIndicatorColorButton.interactable = true;
    }

    public void DisablePlayAgainButton(object sender, EventArgs args)
    {
        //GameManager.Instance.DisablePlayAgainButton();
    }
    public void DisableRandomSkinColorButton(object sender, EventArgs args)
    {
        PlayerManager.Instance.randomSkinColorButton.interactable = false;
    }
    public void DisableRandomIndicatorColorButton(object sender, EventArgs args)
    {
        PlayerManager.Instance.randomIndicatorColorButton.interactable = false;
    }

    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        Time.timeScale = 1;
        AudioManager.Instance.muteSnapshot.TransitionTo(0);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received " + sender);
        RequestRewardedVideoAd((RewardedAd)sender);
        AudioManager.Instance.normalSnapshot.TransitionTo(0);
        //Time.timeScale = 1;
    }

    public void ReceivePlayAgain(object sender, EventArgs args)
    {
        Debug.Log("Rewarded play again");
        GameManager.Instance.PlayAgainButton();
    }

    public void ReceiveRandomSkinColor(object sender, EventArgs args)
    {
        Debug.Log("Rewarded random skin color");
        PlayerManager.Instance.RandomSkinColor();
    }

    public void ReceiveRandomIndicatorColor(object sender, EventArgs args)
    {
        Debug.Log("Rewarded random indicator color");
        PlayerManager.Instance.RandomIndicatorColor();
    }

    public void CheckButton(RewardedAd rewardedAd, Button button)
    {
        if (rewardedAd.IsLoaded())
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    #endregion

    void Update()
    {
    }
}
