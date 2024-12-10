using UnityEngine;
using Complete;
using System;

public class HudManager : MonoBehaviour
{
    [SerializeField] private PlayerStockArea player1StockArea; // Player1のストックHUD
    [SerializeField] private PlayerStockArea player2StockArea; // Player2のストックHUD
    [SerializeField] private Complete.GameManager GameManager; // ゲームマネージャーへの参照

    private TankManager player1TankManager;
    private TankManager player2TankManager;

   
    private void OnEnable()
    {
        if (GameManager != null)
        {
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }
        if (GameManager.m_Tanks != null)
        {
            foreach (var tank in GameManager.m_Tanks)
            {
                if (tank != null)
                {
                    tank.OnWeaponStockChanged += UpdatePlayerStockArea;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (GameManager != null)
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        if (GameManager.m_Tanks != null)
        {
            foreach (var tank in GameManager.m_Tanks)
            {
                if (tank != null)
                {
                    tank.OnWeaponStockChanged -= UpdatePlayerStockArea;
                }
            }
        }
    }

    /// <summary>
    /// ゲームの状態が変化した際にHUDの表示を切り替えます。
    /// </summary>
    /// <param name="newState">現在のゲーム状態</param>
    
    private void HandleGameStateChanged(Complete.GameManager.GameState newState)
    {
        switch (newState)
        {
            case Complete.GameManager.GameState.RoundPlaying:
                ShowHUD(true);
                break;
            case Complete.GameManager.GameState.RoundStarting:
            case Complete.GameManager.GameState.RoundEnding:
                ShowHUD(false);
                break;
        }
    }
    // private void HandleGameStateChanged(Complete.GameManager.GameState newState)
    // {
    //     bool isHUDActive = newState == Complete.GameManager.GameState.RoundPlaying; // プレイ中のみHUDを表示
    //     player1StockArea.gameObject.SetActive(isHUDActive);
    //     player2StockArea.gameObject.SetActive(isHUDActive);
    // }



    /// <summary>
    /// HUDの表示を切り替える
    /// </summary>
    /// <param name="isVisible">表示するかどうか</param>
    private void ShowHUD(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// プレイヤーの砲弾ストック情報を更新する
    /// </summary>
    /// <param name="playerNumber">プレイヤー番号</param>
    /// <param name="newStock">新しいストック数</param>
    private void UpdatePlayerStockArea(int playerNumber, int newStock)
    {
        if (playerNumber == 1)
        {
            player1StockArea.UpdatePlayerStockArea(newStock);
        }
        else if (playerNumber == 2)
        {
            player2StockArea.UpdatePlayerStockArea(newStock);
        }
    }
}
