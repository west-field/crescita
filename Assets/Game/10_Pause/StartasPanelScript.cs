using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ステータスパネル
/// </summary>
public class StartasPanelScript : MonoBehaviour
{
    /*自身のステータス*/
    [SerializeField]　private PlayerStartas playerStartas;

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

    ///*自身の名前*/
    //[SerializeField]　private TextMeshProUGUI myName;

    ///*今のレベル*/
    //[SerializeField]　private TextMeshProUGUI level;
    //[SerializeField]　private TextMeshProUGUI levelPoint;
    //[SerializeField]　private TextMeshProUGUI nextLevelPoint;

    ///*最大HP*/
    //[SerializeField]　private TextMeshProUGUI maxHp;

    ///*攻撃力*/
    //[SerializeField]　private TextMeshProUGUI attack;

    ///*防御力*/
    //[SerializeField]　private TextMeshProUGUI defense;

    ///*武器攻撃力*/
    //[SerializeField]　private TextMeshProUGUI weaponAttack;

    ///*防具防御力*/
    //[SerializeField]　private TextMeshProUGUI armorPower;

    //アクティブになった時
    private void OnEnable()
    {
        //プレイヤータグを検索
        var obj = GameObject.FindGameObjectsWithTag("Player");
        //もしあればアイテムを取得する
        foreach (var item in obj)
        {
            //Debug.Log("いた");
            playerStartas = item.GetComponent<PlayerStartas>();
            break;
        }

        startas[(int)Startas.Level].text = $"LV{playerStartas.GetLevel()}";
        startas[(int)Startas.LevelPoint].text = $"{playerStartas.GetLevelPoint()}";
        startas[(int)Startas.NextLevelPoint].text = $"あと{playerStartas.GetNextLevelPoint()}";

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
