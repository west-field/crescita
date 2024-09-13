using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �G�l�~�[�X�|�i�[
/// </summary>
public class enemySpowner : MonoBehaviour
{
    [SerializeField]�@private GameObject prefab;//�X�|�[�����������G�l�~�[
    private GameObject enemy;

    /*���ɓG���o������܂ł̎���*/
    private const float nextTime = 60.0f;
    //�҂����Ԍv��
    private float elapsedTime;
    //�G�̍ő吔
    private const int maxNumOfEnemiys = 1;

    //�����l�̐����o����������
    private int numberOfEnemys;

    // Start is called before the first frame update
    void Start()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            //�}�X�^�[�N���C�A���g����Ȃ��Ƃ����삵�Ȃ�
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }
        AppearEnemy();
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

        if (enemy == null)
        {
            if (numberOfEnemys - 1 >= 0)
            {
                numberOfEnemys--;
            }
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
            AppearEnemy();
        }
    }

    /// <summary>
    /// �G�o�����\�b�h
    /// </summary>
    void AppearEnemy()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            enemy = PhotonNetwork.InstantiateRoomObject(prefab.name, this.transform.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(prefab, this.transform.position, Quaternion.identity);
        }

        numberOfEnemys++;
        elapsedTime = 0.0f;
    }
}
