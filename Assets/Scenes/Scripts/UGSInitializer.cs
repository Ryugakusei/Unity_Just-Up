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
                Debug.Log("UGS �ʱ�ȭ �Ϸ� �� �÷��̾� �α��� ����.");
            }
            catch (Exception e)
            {
                Debug.LogError($"UGS �ʱ�ȭ ����: {e.Message}");
            }
        }
    }

    private async Task SignInAnonymously()
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("�÷��̾ �̹� �α��εǾ� �ֽ��ϴ�.");
                return;
            }

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("�÷��̾ �͸����� �α��εǾ����ϴ�.");
        }
        catch (Exception e)
        {
            Debug.LogError($"�͸� �α��� ����: {e.Message}");
        }
    }
}
