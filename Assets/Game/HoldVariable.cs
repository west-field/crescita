using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldVariable : MonoBehaviour
{
    public static Vector3 playerPosision { get; set; }//プレイヤーの位置
    public static Quaternion playerRotate { get; set; }//プレイヤーの回転

    public static string playerName { get; set; }//プレイヤーのニックネーム

    public static float BGMVolume { get; set; }//サウンドの音量
    public static float SEVolume { get; set; }//サウンドの音量

    public static List<ItemData> itemDataList = new List<ItemData>();

    [SerializeField] private ItemDataBase itemDataBase;
    public static string id;

    public static bool isDeathBoss;

    /*攻撃力、防御力、レベル*/
    public struct Startas
    {
        public int maxHp;//自身の最大HP
        public int attack;//攻撃力
        public int defense;//防御力
        public int weponAttack;//武器
        public int armorDefense;//防具
        public int level;//レベル
        public int levelPoint;//レベルポイント
    };
    public static Startas startas;

    private void Awake()
    {
        BGMVolume = 0.5f;
        SEVolume = 0.5f;
    }

    void Start()
    {
        playerPosision = Vector3.zero;

        playerRotate = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        foreach (var item in itemDataBase.GetItemLists())
        {
            ItemData itemdata = new ItemData(item.Id, 0);
            itemDataList.Add(itemdata);
        }
        isDeathBoss = false;
    }
}
