using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ���[������ޏo����
/// </summary>
public class onlineLeave : MonoBehaviour
{
    [SerializeField]�@private MainManager manager;
    private bool isFirst;

    void Start()
    {
        isFirst = false;
    }

    private void FixedUpdate()
    {
        //�V�[����ύX���Ă��鎞
        if(manager.IsChangeScene())
        {
            if(!isFirst)
            {
                Debug.Log("Photon����ؒf");
                isFirst = true;
                // ���[������ޏo����
                PhotonNetwork.LeaveRoom();
                // ���r�[����ޏo����
                PhotonNetwork.LeaveLobby();
                //Photon����ؒf
                PhotonNetwork.Disconnect();
            }
        }
    }
}
