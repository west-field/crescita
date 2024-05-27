using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �����o��������
/// </summary>
public class bushSpowner : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;//�X�|�[�����������I�u�W�F�N�g
    GameObject obj;//���ۂɍ�����I�u�W�F�N�g

    /*���ɏo������܂ł̎���*/
    [SerializeField]
    private float nextTime = 60.0f;
    //�҂����Ԍv��
    private float elapsedTime;
    //�X�|�[����(�ő吔)
    private const int maxNumOfEnemiys = 1;

    //�������o����������
    private int numberOfEnemys;

    void Start()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom) 
        {
            //�}�X�^�[�N���C�A���g����Ȃ��Ƃ����삵�Ȃ�
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }
        SpoenObj();
    }

    private void FixedUpdate()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom) 
        {
            //�}�X�^�[�N���C�A���g����Ȃ��Ƃ����삵�Ȃ�
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        if (obj == null)
        {
            numberOfEnemys--;
        }

        //�o������ő吔�𒴂��Ă����牽�����Ȃ�
        if (numberOfEnemys >= maxNumOfEnemiys)
        {
            return;
        }

        //�o�ߎ��Ԃ𑫂�
        elapsedTime += Time.deltaTime;

        //�o�ߎ��Ԃ���������
        if (elapsedTime >= nextTime)
        {
            elapsedTime = 0.0f;
            SpoenObj();
        }
    }

    //�@�X�|�[��������
    private void SpoenObj()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            obj = PhotonNetwork.Instantiate(prefab.name, this.transform.position, Quaternion.identity);
        }
        else
        {
            obj = Instantiate(prefab, this.transform.position, Quaternion.identity);
        }

        numberOfEnemys++;
        elapsedTime = 0.0f;
    }
}