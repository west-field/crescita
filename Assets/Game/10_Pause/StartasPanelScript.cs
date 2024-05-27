using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// �X�e�[�^�X�p�l��
/// </summary>
public class StartasPanelScript : MonoBehaviour
{
    /*���g�̃X�e�[�^�X*/
    [SerializeField]�@private PlayerStartas playerStartas;

    /*���g�̖��O*/
    [SerializeField]�@private TextMeshProUGUI myName;

    /*���̃��x��*/
    [SerializeField]�@private TextMeshProUGUI level;
    [SerializeField]�@private TextMeshProUGUI levelPoint;
    [SerializeField]�@private TextMeshProUGUI nextLevelPoint;

    /*�ő�HP*/
    [SerializeField]�@private TextMeshProUGUI maxHp;

    /*�U����*/
    [SerializeField]�@private TextMeshProUGUI attack;

    /*�h���*/
    [SerializeField]�@private TextMeshProUGUI defense;

    /*����U����*/
    [SerializeField]�@private TextMeshProUGUI weaponAttack;

    /*�h��h���*/
    [SerializeField]�@private TextMeshProUGUI armorPower;

    //�A�N�e�B�u�ɂȂ�����
    private void OnEnable()
    {
        //�v���C���[�^�O������
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //��������΃A�C�e�����擾����
        foreach (var item in obj)
        {
            //Debug.Log("����");
            playerStartas = item.GetComponent<PlayerStartas>();
            break;
        }

        level.text = $"LV{playerStartas.GetLevel()}";
        levelPoint.text = $"{playerStartas.GetLevelPoint()}";
        nextLevelPoint.text = $"����{playerStartas.GetNextLevelPoint()}";

        maxHp.text = $"{playerStartas.GetMaxHp()}";

        attack.text = $"{playerStartas.GetMyAttackPower()}";

        defense.text = $"{playerStartas.GetMyDefensePower()}";

        weaponAttack.text = $"{playerStartas.GetWeponPower()}";

        armorPower.text = $"{playerStartas.GetArmorPower()}";
    }

    private void Start()
    {
        myName.text = HoldVariable.playerName;
    }
}
