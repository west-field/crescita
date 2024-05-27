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

    /*自身の名前*/
    [SerializeField]　private TextMeshProUGUI myName;

    /*今のレベル*/
    [SerializeField]　private TextMeshProUGUI level;
    [SerializeField]　private TextMeshProUGUI levelPoint;
    [SerializeField]　private TextMeshProUGUI nextLevelPoint;

    /*最大HP*/
    [SerializeField]　private TextMeshProUGUI maxHp;

    /*攻撃力*/
    [SerializeField]　private TextMeshProUGUI attack;

    /*防御力*/
    [SerializeField]　private TextMeshProUGUI defense;

    /*武器攻撃力*/
    [SerializeField]　private TextMeshProUGUI weaponAttack;

    /*防具防御力*/
    [SerializeField]　private TextMeshProUGUI armorPower;

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

        level.text = $"LV{playerStartas.GetLevel()}";
        levelPoint.text = $"{playerStartas.GetLevelPoint()}";
        nextLevelPoint.text = $"あと{playerStartas.GetNextLevelPoint()}";

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
