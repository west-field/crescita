using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 敵ステータス
/// </summary>
public class EnemyStartas : MonoBehaviour
{
    /*強い敵かどうか*/
    [SerializeField] private bool isStrong;//強い個体かどうか

    /*HP*/
    private int hp;
    private int maxHP;

    /*攻撃*/
    private int attackPower;

    /*経験値*/
    private int experiencePoint;

    /*お金*/
    private int gold;

    /*視野範囲*/
    [SerializeField] private scopeScript scopeScript;//視認範囲のスクリプト
    private float scope;//攻撃できる範囲

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="maxHp">最大HP</param>
    /// <param name="power">攻撃力</param>
    /// <param name="scope">攻撃できる範囲</param>
    /// <param name="experiencePoint">経験値</param>
    /// <param name="gold">お金</param>
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
            Debug.Log("HP等を変える");
            //マスタークライアントじゃないとき操作しない
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("マスタークライアント");
                var roomNum = PhotonNetwork.CountOfPlayersInRooms;
                roomNum = PhotonNetwork.CountOfPlayersOnMaster;
                Debug.Log(roomNum);

                if (roomNum > 0)
                {
                    Debug.Log("実際に変更");
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
    /// 強い個体かどうか
    /// </summary>
    /// <returns>true 強い: fasle 通常</returns>
    public bool IsStrong()
    {
        return isStrong;
    }

    /// <summary>
    /// 被弾
    /// </summary>
    /// <param name="damage">相手の攻撃力</param>
    public void Hit(int damage)
    {
        hp -= damage;

        if (hp < 0)
        {
            hp = 0;
        }
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="heals">回復量</param>
    public void Heals(int heals)
    {
        hp += heals;

        if (hp > maxHP)
        {
            hp = maxHP;
        }
    }

    /// <summary>
    /// 現在のHPを取得する
    /// </summary>
    /// <returns>int 現在のHP</returns>
    public int GetNowHp()
    {
        return hp;
    }

    /// <summary>
    /// 最大HPを取得する
    /// </summary>
    /// <returns>int 最大HP</returns>
    public int GetMaxHp()
    {
        return maxHP;
    }

    /// <summary>
    /// 生きているかどうか
    /// </summary>
    /// <returns>true 生きてる: false しんでる</returns>
    public bool IsAlive()
    {
        return hp > 0;
    }

    /// <summary>
    /// 視野範囲のスクリプトを取得
    /// </summary>
    /// <returns>scopeScript</returns>
    public scopeScript GetScopescript()
    {
        return scopeScript;
    }

    /// <summary>
    /// 攻撃できる範囲を取得
    /// </summary>
    /// <returns>float 範囲</returns>
    public float GetScope()
    {
        return scope;
    }

    /// <summary>
    /// 攻撃力を取得
    /// </summary>
    /// <returns>int 攻撃力</returns>
    public int GetAttackPower()
    {
        return attackPower;
    }

    /// <summary>
    /// 設定した経験値を取得
    /// </summary>
    /// <returns>int 経験値</returns>
    public int LevelPoint()
    {
        return experiencePoint;
    }

    /// <summary>
    /// 指定したお金を取得
    /// </summary>
    /// <returns>int お金</returns>
    public int Gold()
    {
        return gold;
    }
}
