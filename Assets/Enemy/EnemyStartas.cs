using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �G�X�e�[�^�X
/// </summary>
public class EnemyStartas : MonoBehaviour
{
    /*�����G���ǂ���*/
    [SerializeField] private bool isStrong;//�����̂��ǂ���

    /*HP*/
    private int hp;
    private int maxHP;

    /*�U��*/
    private int attackPower;

    /*�o���l*/
    private int experiencePoint;

    /*����*/
    private int gold;

    /*����͈�*/
    [SerializeField] private scopeScript scopeScript;//���F�͈͂̃X�N���v�g
    private float scope;//�U���ł���͈�

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="maxHp">�ő�HP</param>
    /// <param name="power">�U����</param>
    /// <param name="scope">�U���ł���͈�</param>
    /// <param name="experiencePoint">�o���l</param>
    /// <param name="gold">����</param>
    public void Init(int maxHp,int power,float scope,int experiencePoint, int gold)
    {
        this.hp = maxHp;
        this.scope = scope;
        this.attackPower = power;
        this.experiencePoint = experiencePoint;
        this.gold = gold;
        if (isStrong)
        {
            this.hp *= 2;
            this.scope *= 2;
            this.attackPower *= 2;
            this.experiencePoint *= 2;
            this.gold *= 2;
        }

        if(PhotonNetwork.InRoom)
        {
            Debug.Log("HP����ς���");
            //�}�X�^�[�N���C�A���g����Ȃ��Ƃ����삵�Ȃ�
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("�}�X�^�[�N���C�A���g");
                var roomNum = PhotonNetwork.CountOfPlayersInRooms;
                roomNum = PhotonNetwork.CountOfPlayersOnMaster;
                Debug.Log(roomNum);

                if (roomNum > 0)
                {
                    Debug.Log("���ۂɕύX");
                    this.hp *= roomNum;
                    this.attackPower *= 2;
                    this.experiencePoint *= 2;
                    this.gold *= roomNum;
                }
            }
        }

        maxHP = hp;
    }

    /// <summary>
    /// �����̂��ǂ���
    /// </summary>
    /// <returns>true ����: fasle �ʏ�</returns>
    public bool IsStrong()
    {
        return isStrong;
    }

    /// <summary>
    /// ��e
    /// </summary>
    /// <param name="damage">����̍U����</param>
    public void Hit(int damage)
    {
        hp -= damage;

        if (hp < 0)
        {
            hp = 0;
        }
    }

    /// <summary>
    /// ��
    /// </summary>
    /// <param name="heals">�񕜗�</param>
    public void Heals(int heals)
    {
        hp += heals;

        if (hp > maxHP)
        {
            hp = maxHP;
        }
    }

    /// <summary>
    /// ���݂�HP���擾����
    /// </summary>
    /// <returns>int ���݂�HP</returns>
    public int GetNowHp()
    {
        return hp;
    }

    /// <summary>
    /// �ő�HP���擾����
    /// </summary>
    /// <returns>int �ő�HP</returns>
    public int GetMaxHp()
    {
        return maxHP;
    }

    /// <summary>
    /// �����Ă��邩�ǂ���
    /// </summary>
    /// <returns>true �����Ă�: false ����ł�</returns>
    public bool IsAlive()
    {
        return hp > 0;
    }

    /// <summary>
    /// ����͈͂̃X�N���v�g���擾
    /// </summary>
    /// <returns>scopeScript</returns>
    public scopeScript GetScopescript()
    {
        return scopeScript;
    }

    /// <summary>
    /// �U���ł���͈͂��擾
    /// </summary>
    /// <returns>float �͈�</returns>
    public float GetScope()
    {
        return scope;
    }

    /// <summary>
    /// �U���͂��擾
    /// </summary>
    /// <returns>int �U����</returns>
    public int GetAttackPower()
    {
        return attackPower;
    }

    /// <summary>
    /// �ݒ肵���o���l���擾
    /// </summary>
    /// <returns>int �o���l</returns>
    public int LevelPoint()
    {
        return experiencePoint;
    }

    /// <summary>
    /// �w�肵���������擾
    /// </summary>
    /// <returns>int ����</returns>
    public int Gold()
    {
        return gold;
    }
}
