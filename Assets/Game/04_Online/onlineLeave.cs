using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ルームから退出する
/// </summary>
public class onlineLeave : MonoBehaviour
{
    [SerializeField]　private MainManager manager;
    private bool isFirst;

    void Start()
    {
        isFirst = false;
    }

    private void FixedUpdate()
    {
        //シーンを変更している時
        if(manager.IsChangeScene())
        {
            if(!isFirst)
            {
                Debug.Log("Photonから切断");
                isFirst = true;
                // ルームから退出する
                PhotonNetwork.LeaveRoom();
                // ロビーから退出する
                PhotonNetwork.LeaveLobby();
                //Photonから切断
                PhotonNetwork.Disconnect();
            }
        }
    }
}
