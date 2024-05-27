using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Photon�@�ڑ��A�ؒf�̐ݒ�
/// �I�t���C�����[�h�ݒ�
/// </summary>
public class OfflineMode : MonoBehaviour
{
    /// <summary>
    /// �I�t���C���ɂ��邩�ǂ���
    /// </summary>
    [SerializeField] private bool isOffline = false;

    /// <summary>
    /// �ڑ�����
    /// </summary>
    public void Connect()
    {
        if(!isOffline)
        {
            //Photon�ɐڑ�����
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            //�I�t���C�����[�h��L���ɂ���
            PhotonNetwork.OfflineMode = true;
        }
    }

    /// <summary>
    /// �ؒf����
    /// </summary>
    public void Disconnect()
    {
        if (!isOffline)
        {
            //�T�[�o�[����ؒf����
            PhotonNetwork.Disconnect();
        }
        else
        {
            //�I�t���C�����[�h�𖳌��ɂ���
            PhotonNetwork.OfflineMode = false;
        }
    }
}
