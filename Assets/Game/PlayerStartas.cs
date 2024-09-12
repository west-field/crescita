using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �v���C���[�X�e�[�^�X
/// </summary>
public class PlayerStartas : MonoBehaviourPunCallbacks
{
    /*HP*/
    private int hp;

    //���x���A�b�v������
    private bool isLevelUp;

    //�����l�ŗV�ԂƂ��ɗ��p����
    private HoldVariable.Startas onlineStartas;

    private void Awake()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            // ���g�����������I�u�W�F�N�g�����ɏ������s��
            if (!photonView.IsMine)
            {
                Debug.Log("�X�L�b�v����");
                return;
            }
            onlineStartas.maxHp = 100;
            onlineStartas.attack = 2;
            onlineStartas.defense = 1;
            onlineStartas.weponAttack = 2;//�g�p���Ȃ�
            onlineStartas.armorDefense = 1;//�g�p���Ȃ�
            onlineStartas.level = 1;
            onlineStartas.levelPoint = 0;

            hp = onlineStartas.maxHp;
        }
        else
        {
            hp = HoldVariable.startas.maxHp;
        }
    }

    /// <summary>
    /// ���݂̃X�e�[�^�X���擾����
    /// </summary>
    /// <returns></returns>
    public HoldVariable.Startas GetStartas()
    {
        if (PhotonNetwork.InRoom)
        {
            // ���g�����������I�u�W�F�N�g�����ɏ������s��
            if (photonView.IsMine)
            {
                return onlineStartas;
            }
        }

        return HoldVariable.startas;
    }

    /*--------------------------------HP--------------------------------*/
    /// <summary>
    /// �U�����󂯂�
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    public void Hit(int damage)
    {
        Debug.Log("�U�����󂯂�"+damage);

        //�_���[�W��-�ɂȂ��Ă�����(�G�̍U����-���g�̖h���)
        if (damage <= 0)
        {
            //1�_���[�W�ɂ���
            damage = 1;
        }
        Debug.Log("�U�����󂯂��C��" + damage);
        hp -= damage;

        if (hp < 0)
        {
            hp = 0;
        }

        Debug.Log("�U�����󂯂�" + hp);
    }

    /// <summary>
    /// ��
    /// </summary>
    /// <param name="heals">�񕜗�</param>
    public void Heals(int heals)
    {
        hp += heals;

        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {   
            if (hp > onlineStartas.maxHp)
            {
                hp = onlineStartas.maxHp;
            }
            return;
        }

        if (hp > HoldVariable.startas.maxHp)
        {
            hp = HoldVariable.startas.maxHp;
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
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.maxHp;
        }
        return HoldVariable.startas.maxHp;
    }

    /// <summary>
    /// �����Ă��邩�ǂ���
    /// </summary>
    /// <returns>true �����Ă���: false ����ł���</returns>
    public bool IsAlive()
    {
        return hp > 0;
    }

    /*--------------------------------�U��--------------------------------*/
    /// <summary>
    /// ����̍U���͂��グ��
    /// </summary>
    public void WeponLevelUp()
    {
        HoldVariable.startas.weponAttack *= 2;
        Debug.Log($"PlayerStartas  weponAttackPower:{HoldVariable.startas.weponAttack}");
    }

    /// <summary>
    /// �U���͂��擾
    /// </summary>
    /// <returns>int �U����</returns>
    public int GetAttackPower()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.attack + HoldVariable.startas.weponAttack;
        }
        return HoldVariable.startas.attack + HoldVariable.startas.weponAttack;
    }

    /// <summary>
    /// ���g�̍U���͂��擾
    /// </summary>
    /// <returns>int ���g�̍U����</returns>
    public int GetMyAttackPower()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.attack;
        }
        return HoldVariable.startas.attack;
    }

    /// <summary>
    /// ����U���͂��擾
    /// </summary>
    /// <returns>int ����̍U����</returns>
    public int GetWeponPower()
    {
        return HoldVariable.startas.weponAttack;
    }

    /*--------------------------------�h��--------------------------------*/
    /// <summary>
    /// �h��̖h��͂��グ��
    /// </summary>
    public void ArmorLevelUp()
    {
        HoldVariable.startas.armorDefense *= 2;
        Debug.Log($"PlayerStartas  armorPower:{HoldVariable.startas.armorDefense}");
    }

    /// <summary>
    /// �h��͂��擾
    /// </summary>
    /// <returns>int �h���</returns>
    public int GetDefensePower()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.defense + HoldVariable.startas.armorDefense;
        }
        return HoldVariable.startas.defense+ HoldVariable.startas.armorDefense;
    }

    /// <summary>
    /// ���g�̖h��͂��擾
    /// </summary>
    /// <returns>int ���g�̖h���</returns>
    public int GetMyDefensePower()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.defense;
        }
        return HoldVariable.startas.defense;
    }

    /// <summary>
    /// ����h��͂��擾
    /// </summary>
    /// <returns>int ����̖h���</returns>
    public int GetArmorPower()
    {
        return HoldVariable.startas.armorDefense;
    }

    /*--------------------------------���x��--------------------------------*/
    /// <summary>
    /// ���x���A�b�v
    /// </summary>
    /// <param name="levelPoint">�擾�������x���|�C���g</param>
    public void LevelUp(int levelPoint)
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            Debug.Log(onlineStartas.levelPoint + "+" + levelPoint);
            onlineStartas.levelPoint += levelPoint;

            //1���x���̎��@0 + 100 = 100
            //2���x���̎��@100 + 200 = 300
            var onlineLevelPoint = ((onlineStartas.level - 1) * 100 + onlineStartas.level * 100);

            //���x���|�C���g��level�~10�����傫���Ƃ�
            if (onlineStartas.levelPoint >= onlineLevelPoint)
            {
                //�U����
                onlineStartas.attack += onlineStartas.level;
                //�h���
                onlineStartas.defense += onlineStartas.level;

                //���x�����グ��
                onlineStartas.level++;
                isLevelUp = true;
            }
            
            return;
        }

        Debug.Log(HoldVariable.startas.levelPoint + "+" + levelPoint);
        HoldVariable.startas.levelPoint += levelPoint;

        var holdLevelPoint = ((HoldVariable.startas.level - 1) * 100 + HoldVariable.startas.level * 100);

        //���x���|�C���g��level�~10�����傫���Ƃ�
        if (HoldVariable.startas.levelPoint >= holdLevelPoint)
        {
            //�U����
            HoldVariable.startas.attack += HoldVariable.startas.level;
            //�h���
            HoldVariable.startas.defense += HoldVariable.startas.level;

            //���x�����グ��
            HoldVariable.startas.level++;
            isLevelUp = true;
        }
    }

    /// <summary>
    /// ���݂̃��x�����擾
    /// </summary>
    /// <returns>int ���݂̃��x��</returns>
    public int GetLevel()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.level;
        }
        return HoldVariable.startas.level;
    }

    /// <summary>
    /// ���x���|�C���g���擾
    /// </summary>
    /// <returns>int ���x���|�C���g</returns>
    public int GetLevelPoint()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.levelPoint;
        }
        return HoldVariable.startas.levelPoint;
    }

    /// <summary>
    /// ���x���|�C���g���g�p
    /// </summary>
    /// <param name="useLevelPoint">�g�p���郌�x���|�C���g</param>
    public void UseLevelPoint(int useLevelPoint)
    {
        //���x���|�C���g��ύX
        HoldVariable.startas.levelPoint -= useLevelPoint;

        //���x����ύX
        HoldVariable.startas.level = HoldVariable.startas.levelPoint / 100 + 1;

        //���x����0�������̎�
        if(HoldVariable.startas.level <= 1)
        {
            //1�ɕύX
            HoldVariable.startas.level = 1;
            //���g�̖h��͂�ύX
            HoldVariable.startas.defense = 1;
        }
        else
        {
            //���g�̖h��͂�ύX
            HoldVariable.startas.defense = (HoldVariable.startas.level - 1) * 2;
        }

        //���g�̍U���͂�ύX
        HoldVariable.startas.attack = HoldVariable.startas.level * 2;
    }

    /// <summary>
    /// ���̃��x���܂ł̃|�C���g
    /// </summary>
    /// <returns>int �|�C���g</returns>
    public int GetNextLevelPoint()
    {
        // ���[���ɎQ�����Ă���ꍇ�͏���������
        if (PhotonNetwork.InRoom)
        {
            return ((onlineStartas.level - 1) * 100 + onlineStartas.level * 100) - onlineStartas.levelPoint;
        }
        return ((HoldVariable.startas.level - 1) * 100 + HoldVariable.startas.level * 100) - HoldVariable.startas.levelPoint;
    }

    /// <summary>
    /// ���x���A�b�v�������ǂ���
    /// </summary>
    /// <returns>true ����:false ���Ă��Ȃ�</returns>
    public bool IsLevelUp()
    {
        if(isLevelUp)
        {
            isLevelUp = false;
            return true;
        }
        return false;
    }
}
