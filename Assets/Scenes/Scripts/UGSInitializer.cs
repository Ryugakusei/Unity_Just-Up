using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class UGSInitializer : MonoBehaviour
{
    public static UGSInitializer Instance;

    private async void Awake()
    {
        Instance = this;
        await EnsureInitialized();
    }

    public async Task EnsureInitialized()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            try
            {
                await UnityServices.InitializeAsync();
                await SignInAnonymously();
                Debug.Log("UGS 초기화 완료 및 플레이어 로그인 성공.");
            }
            catch (Exception e)
            {
                Debug.LogError($"UGS 초기화 실패: {e.Message}");
            }
        }
    }

    private async Task SignInAnonymously()
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("플레이어가 이미 로그인되어 있습니다.");
                return;
            }

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("플레이어가 익명으로 로그인되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"익명 로그인 실패: {e.Message}");
        }
    }
}
