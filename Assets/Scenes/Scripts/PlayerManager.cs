using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
    private async void Start()
    {
        await UGSInitializer.Instance.EnsureInitialized();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
