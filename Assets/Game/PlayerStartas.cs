using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// プレイヤーステータス
/// </summary>
public class PlayerStartas : MonoBehaviourPunCallbacks
{
    /*HP*/
    private int hp;

    //レベルアップしたか
    private bool isLevelUp;

    //複数人で遊ぶときに利用する
    private HoldVariable.Startas onlineStartas;

    private void Awake()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                Debug.Log("スキップする");
                return;
            }
            onlineStartas.maxHp = 100;
            onlineStartas.attack = 2;
            onlineStartas.defense = 1;
            onlineStartas.weponAttack = 2;//使用しない
            onlineStartas.armorDefense = 1;//使用しない
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
    /// 現在のステータスを取得する
    /// </summary>
    /// <returns></returns>
    public HoldVariable.Startas GetStartas()
    {
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (photonView.IsMine)
            {
                return onlineStartas;
            }
        }

        return HoldVariable.startas;
    }

    /*--------------------------------HP--------------------------------*/
    /// <summary>
    /// 攻撃を受けた
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void Hit(int damage)
    {
        Debug.Log("攻撃を受けた"+damage);

        //ダメージが-になっていたら(敵の攻撃力-自身の防御力)
        if (damage <= 0)
        {
            //1ダメージにする
            damage = 1;
        }
        Debug.Log("攻撃を受けた修正" + damage);
        hp -= damage;

        if (hp < 0)
        {
            hp = 0;
        }

        Debug.Log("攻撃を受けた" + hp);
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="heals">回復量</param>
    public void Heals(int heals)
    {
        hp += heals;

        // ルームに参加している場合は処理をする
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
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.maxHp;
        }
        return HoldVariable.startas.maxHp;
    }

    /// <summary>
    /// 生きているかどうか
    /// </summary>
    /// <returns>true 生きている: false 死んでいる</returns>
    public bool IsAlive()
    {
        return hp > 0;
    }

    /*--------------------------------攻撃--------------------------------*/
    /// <summary>
    /// 武器の攻撃力を上げる
    /// </summary>
    public void WeponLevelUp()
    {
        HoldVariable.startas.weponAttack *= 2;
        Debug.Log($"PlayerStartas  weponAttackPower:{HoldVariable.startas.weponAttack}");
    }

    /// <summary>
    /// 攻撃力を取得
    /// </summary>
    /// <returns>int 攻撃力</returns>
    public int GetAttackPower()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.attack + HoldVariable.startas.weponAttack;
        }
        return HoldVariable.startas.attack + HoldVariable.startas.weponAttack;
    }

    /// <summary>
    /// 自身の攻撃力を取得
    /// </summary>
    /// <returns>int 自身の攻撃力</returns>
    public int GetMyAttackPower()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.attack;
        }
        return HoldVariable.startas.attack;
    }

    /// <summary>
    /// 武器攻撃力を取得
    /// </summary>
    /// <returns>int 武器の攻撃力</returns>
    public int GetWeponPower()
    {
        return HoldVariable.startas.weponAttack;
    }

    /*--------------------------------防御--------------------------------*/
    /// <summary>
    /// 防具の防御力を上げる
    /// </summary>
    public void ArmorLevelUp()
    {
        HoldVariable.startas.armorDefense *= 2;
        Debug.Log($"PlayerStartas  armorPower:{HoldVariable.startas.armorDefense}");
    }

    /// <summary>
    /// 防御力を取得
    /// </summary>
    /// <returns>int 防御力</returns>
    public int GetDefensePower()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.defense + HoldVariable.startas.armorDefense;
        }
        return HoldVariable.startas.defense+ HoldVariable.startas.armorDefense;
    }

    /// <summary>
    /// 自身の防御力を取得
    /// </summary>
    /// <returns>int 自身の防御力</returns>
    public int GetMyDefensePower()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.defense;
        }
        return HoldVariable.startas.defense;
    }

    /// <summary>
    /// 武具防御力を取得
    /// </summary>
    /// <returns>int 武具の防御力</returns>
    public int GetArmorPower()
    {
        return HoldVariable.startas.armorDefense;
    }

    /*--------------------------------レベル--------------------------------*/
    /// <summary>
    /// レベルアップ
    /// </summary>
    /// <param name="levelPoint">取得したレベルポイント</param>
    public void LevelUp(int levelPoint)
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            Debug.Log(onlineStartas.levelPoint + "+" + levelPoint);
            onlineStartas.levelPoint += levelPoint;

            //1レベルの時　0 + 100 = 100
            //2レベルの時　100 + 200 = 300
            var onlineLevelPoint = ((onlineStartas.level - 1) * 100 + onlineStartas.level * 100);

            //レベルポイントがlevel×10よりも大きいとき
            if (onlineStartas.levelPoint >= onlineLevelPoint)
            {
                //攻撃力
                onlineStartas.attack += onlineStartas.level;
                //防御力
                onlineStartas.defense += onlineStartas.level;

                //レベルを上げる
                onlineStartas.level++;
                isLevelUp = true;
            }
            
            return;
        }

        Debug.Log(HoldVariable.startas.levelPoint + "+" + levelPoint);
        HoldVariable.startas.levelPoint += levelPoint;

        var holdLevelPoint = ((HoldVariable.startas.level - 1) * 100 + HoldVariable.startas.level * 100);

        //レベルポイントがlevel×10よりも大きいとき
        if (HoldVariable.startas.levelPoint >= holdLevelPoint)
        {
            //攻撃力
            HoldVariable.startas.attack += HoldVariable.startas.level;
            //防御力
            HoldVariable.startas.defense += HoldVariable.startas.level;

            //レベルを上げる
            HoldVariable.startas.level++;
            isLevelUp = true;
        }
    }

    /// <summary>
    /// 現在のレベルを取得
    /// </summary>
    /// <returns>int 現在のレベル</returns>
    public int GetLevel()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.level;
        }
        return HoldVariable.startas.level;
    }

    /// <summary>
    /// レベルポイントを取得
    /// </summary>
    /// <returns>int レベルポイント</returns>
    public int GetLevelPoint()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return onlineStartas.levelPoint;
        }
        return HoldVariable.startas.levelPoint;
    }

    /// <summary>
    /// レベルポイントを使用
    /// </summary>
    /// <param name="useLevelPoint">使用するレベルポイント</param>
    public void UseLevelPoint(int useLevelPoint)
    {
        //レベルポイントを変更
        HoldVariable.startas.levelPoint -= useLevelPoint;

        //レベルを変更
        HoldVariable.startas.level = HoldVariable.startas.levelPoint / 100 + 1;

        //レベルが0よりも下の時
        if(HoldVariable.startas.level <= 1)
        {
            //1に変更
            HoldVariable.startas.level = 1;
            //自身の防御力を変更
            HoldVariable.startas.defense = 1;
        }
        else
        {
            //自身の防御力を変更
            HoldVariable.startas.defense = (HoldVariable.startas.level - 1) * 2;
        }

        //自身の攻撃力を変更
        HoldVariable.startas.attack = HoldVariable.startas.level * 2;
    }

    /// <summary>
    /// 次のレベルまでのポイント
    /// </summary>
    /// <returns>int ポイント</returns>
    public int GetNextLevelPoint()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            return ((onlineStartas.level - 1) * 100 + onlineStartas.level * 100) - onlineStartas.levelPoint;
        }
        return ((HoldVariable.startas.level - 1) * 100 + HoldVariable.startas.level * 100) - HoldVariable.startas.levelPoint;
    }

    /// <summary>
    /// レベルアップしたかどうか
    /// </summary>
    /// <returns>true した:false していない</returns>
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
