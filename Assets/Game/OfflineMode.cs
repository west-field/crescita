using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Photon　接続、切断の設定
/// オフラインモード設定
/// </summary>
public class OfflineMode : MonoBehaviour
{
    /// <summary>
    /// オフラインにするかどうか
    /// </summary>
    [SerializeField] private bool isOffline = false;

    /// <summary>
    /// 接続する
    /// </summary>
    public void Connect()
    {
        if(!isOffline)
        {
            //Photonに接続する
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            //オフラインモードを有効にする
            PhotonNetwork.OfflineMode = true;
        }
    }

    /// <summary>
    /// 切断する
    /// </summary>
    public void Disconnect()
    {
        if (!isOffline)
        {
            //サーバーから切断する
            PhotonNetwork.Disconnect();
        }
        else
        {
            //オフラインモードを無効にする
            PhotonNetwork.OfflineMode = false;
        }
    }
}
