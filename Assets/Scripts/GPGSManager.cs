using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSManager : MonoBehaviour
{
    void Start()
    {
        InitializeGPGSLogin();
        LoginGPGS();
        
    }

    void InitializeGPGSLogin()
    {
        var config = new PlayGamesClientConfiguration.Builder().RequestIdToken().Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Debug.Log("GPGS init");
    }

    void LoginGPGS()
    {
        Social.localUser.Authenticate(OnGooglePlayGamesLogin);
        Debug.Log("GPGS login in");
    }

    void OnGooglePlayGamesLogin(bool success)
    {
        Debug.Log("GPGS callback");
        if (success)
        {
            // Call Unity Authentication SDK to sign in or link with Google.
            Debug.Log("Login with Google Play Games done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
        }
        else
        {
            Debug.Log("Unsuccessful login");
        }
    }
}
