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

    private enum Startas
    {
        Name,
        Level,
        LevelPoint,
        NextLevelPoint,
        MaxHp,
        Attack,
        Defense,
        WeaponAttack,
        ArmorPower,

        Max
    }

    [SerializeField] private TextMeshProUGUI[] startas = new TextMeshProUGUI[(int)Startas.Max];

    ///*���g�̖��O*/
    //[SerializeField]�@private TextMeshProUGUI myName;

    ///*���̃��x��*/
    //[SerializeField]�@private TextMeshProUGUI level;
    //[SerializeField]�@private TextMeshProUGUI levelPoint;
    //[SerializeField]�@private TextMeshProUGUI nextLevelPoint;

    ///*�ő�HP*/
    //[SerializeField]�@private TextMeshProUGUI maxHp;

    ///*�U����*/
    //[SerializeField]�@private TextMeshProUGUI attack;

    ///*�h���*/
    //[SerializeField]�@private TextMeshProUGUI defense;

    ///*����U����*/
    //[SerializeField]�@private TextMeshProUGUI weaponAttack;

    ///*�h��h���*/
    //[SerializeField]�@private TextMeshProUGUI armorPower;

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

        startas[(int)Startas.Level].text = $"LV{playerStartas.GetLevel()}";
        startas[(int)Startas.LevelPoint].text = $"{playerStartas.GetLevelPoint()}";
        startas[(int)Startas.NextLevelPoint].text = $"����{playerStartas.GetNextLevelPoint()}";

        startas[(int)Startas.MaxHp].text = $"{playerStartas.GetMaxHp()}";

        startas[(int)Startas.Attack].text = $"{playerStartas.GetMyAttackPower()}";

        startas[(int)Startas.Defense].text = $"{playerStartas.GetMyDefensePower()}";

        startas[(int)Startas.WeaponAttack].text = $"{playerStartas.GetWeponPower()}";

        startas[(int)Startas.ArmorPower].text = $"{playerStartas.GetArmorPower()}";
    }

    private void Start()
    {
        startas[(int)Startas.Name].text = HoldVariable.playerName;
    }
}
