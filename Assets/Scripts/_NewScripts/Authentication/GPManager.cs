using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
public class GPManager : MonoBehaviour
{
    public string token;
    public string error;
    public Action OnGoogleLoginComplete;
    public Action OnGoogleLoginFailed;
    public void Init()
    {
        //Initialize PlayGamesPlatform
        PlayGamesPlatform.Activate();
    }
    private void OnSignInResult(SignInStatus signInStatus)
    {
        if (signInStatus == SignInStatus.Success)
        {
            print("Authenticated. Hello, " + Social.localUser.userName + " (" + Social.localUser.id + ")");
        }
        else
        {
            print("*** Failed to authenticate with " + signInStatus);
        }
    }
    public void LoginGooglePlayGames()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(OnSignInResult);
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
                {
                    Debug.Log("Authorization code: " + code);
                    token = code;
                    await SignInWithGooglePlayGamesAsync(token);
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                });
            }
            else if (success == SignInStatus.Canceled)
            {
                print("cancelled");
            }
            else if (success == SignInStatus.InternalError)
            {
                print("some random error");
            }
            else
            {
                error = "Failed to retrieve Google play games authorization code";
                Debug.Log("Login Unsuccessful");
            }
        });
    }
    async Task SignInWithGooglePlayGamesAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log("Sign In is successful.");
            OnGoogleLoginComplete?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            OnGoogleLoginFailed?.Invoke();
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            OnGoogleLoginFailed?.Invoke();
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
