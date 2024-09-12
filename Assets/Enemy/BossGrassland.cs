using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// 草原ステージのボス
/// </summary>
public class BossGrassland : MonoBehaviourPunCallbacks, IPunObservable
{
    /*ステータス*/
    [SerializeField] private EnemyStartas enemyStartas;

    /*パーティクル*/
    [SerializeField] private BossParticle particle;

    /*アニメーション*/
    private Animator animator;//アニメーター

    /*挙動*/
    private bool isDeath, isMove;//死んだかどうか、移動しているかどうか
    private bool isReturnCenter;//中心に戻るか

    /*移動*/
    static float moveSpeed = 0.01f;//移動スピード
    private float speed;//今のスピード

    private Vector3 centerPos;//中心位置(移動範囲を決める際の中央)
    private Vector3 rot;//回転

    /*攻撃*/
    static private float invincibleTime = 60.0f;//無敵時間
    private float invincibleCount;//無敵経過時間

    private int attackCount = 0;//攻撃できるまでの時間

    private float attackScope;//攻撃できる範囲

    private BoxCollider tailBox,jawBox;//攻撃できる

    /*音*/
    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip attackSound;//攻撃音
    [SerializeField] private AudioClip hitSound;//当たった時の音

    /*Hp*/
    [SerializeField] private GameObject hpBarObject;//HPバーのゲームオブジェクト
    private bool isHpBarDraw;
    [SerializeField] private Image hpBars;//HPバー
    private int currentHp;
    private int maxHp;

    /*死亡*/
    private int deathCount;

    /// <summary>
    /// ミニマップに表示するオブジェクトを削除するため
    /// </summary>
    [SerializeField] private GameObject miniMap;

    private void Start()
    {
        animator = this.GetComponent<Animator>();

        hpBarObject.SetActive(false);

        enemyStartas.Init(100, 20, 1.0f, 200, 200);

        isDeath = false;
        isMove = false;
        isReturnCenter = false;
        speed = moveSpeed;

        centerPos = this.transform.position;
        rot = Vector3.zero;

        invincibleCount = 0;

        attackScope = 3.0f;//アタック

        jawBox = this.transform.Find("Root_Pelvis/Spine/Chest/Neck/Head/Jaw/JawTip").GetComponent<BoxCollider>();
        jawBox.enabled = false;
        tailBox = this.transform.Find("Root_Pelvis/Tail01/Tail02/Tail03").GetComponent<BoxCollider>();
        tailBox.enabled = false;

        maxHp = enemyStartas.GetMaxHp();
        currentHp = enemyStartas.GetNowHp();

        deathCount = 0;

        HoldVariable.isDeathBoss = false;
    }

    private void FixedUpdate()
    {
        if (!isDeath)
        {
            //hpバーを表示非表示を変更する
            if (isHpBarDraw != hpBarObject.activeSelf)
            {
                hpBarObject.SetActive(isHpBarDraw);
            }
            //反映させる
            hpBars.fillAmount = (float)currentHp / (float)maxHp;
        }
        else
        {
            deathCount++;

            if (deathCount >= 120)
            {
                Debug.Log("時間経過でボスを削除");

                var root = this.transform.position;
                particle.ParticlePlay(root);

                GameObject item;
                //アイテム
                var rand = Random.Range(0, 100);

                string[] itemName = { "frogHorn", "frog'sPearl", "frog'sGoldenBeads", "frog'sBlackJewel", "lotusLeaf" };

                Vector3[] position = new Vector3[]  {   new Vector3(root.x, root.y + 0.5f, root.z),
                                                        new Vector3(root.x + 0.5f, root.y + 0.5f, root.z),
                                                        new Vector3(root.x - 0.5f, root.y + 0.5f, root.z),
                                                        new Vector3(root.x, root.y + 0.5f, root.z + 0.5f),
                                                        new Vector3(root.x, root.y + 0.5f, root.z - 0.5f)};

                bool isCreate = false;//一度でも作成したかどうか

                int divide = 2;
                int index = 0;
                foreach (var name in itemName)
                {
                    rand = Random.Range(0, 100) % divide;
                    if (rand == 0)
                    {
                        rand = Random.Range(1, 3);

                        for (int i = 0; i < rand; i++)
                        {
                            isCreate = true;
                            item = (GameObject)Resources.Load(name);
                            Instantiate(item, new Vector3(position[index].x, position[index].y + i * 0.3f, position[index].z), item.transform.localRotation);
                            Debug.Log(position[index]);
                        }
                        index++;
                    }

                    divide += 2;
                }

                if (!isCreate)
                {
                    item = (GameObject)Resources.Load("frogHorn");
                    Instantiate(item, new Vector3(), item.transform.localRotation);
                }

                if (PhotonNetwork.InRoom)
                {
                    if (photonView.IsMine)
                    {
                        Debug.Log("消す");
                        PhotonNetwork.Destroy(this.gameObject);
                    }
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }

            return;
        }

        //無敵経過時間
        if (invincibleCount > 0.0f)
        {
            invincibleCount--;
        }

        //生きていないとき
        if (!enemyStartas.IsAlive())
        {
            isDeath = true;
            HoldVariable.isDeathBoss = true;
            animator.SetBool("die", isDeath);

            this.gameObject.tag = "Untagged";

            tailBox.enabled = false;
            jawBox.enabled = false;
            if (hpBarObject != null)
            {
                Destroy(hpBarObject);
            }
            if(miniMap != null)
            {
                Destroy(miniMap);
            }
            return;
        }

        //ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine)
            {
                return;
            }
        }

        //音量が違うときは音量を合わせる
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        animator.SetBool("move", isMove);

        //範囲内にプレイヤーがいるとき
        if (enemyStartas.GetScopescript().IsScope())
        {
            Debug.Log("追いかける");
            isHpBarDraw = true;
            //プレイヤーを追いかける
            ChaseThePlayer();
        }
        else if (isReturnCenter)
        {
            Debug.Log("戻る");
            isHpBarDraw = false;
            ReturnToCenterPos();
        }
        else
        {
            //眠る？
            isHpBarDraw = false;
        }
    }

    /// <summary>
    /// プレイヤーを追いかける
    /// </summary>
    private void ChaseThePlayer()
    {
        isMove = true;//移動している

        //プレイヤーの方向を向く
        rot = enemyStartas.GetScopescript().TargetPosition() - this.transform.position;
        rot.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(rot);

        if (attackCount > 0)
        {
           attackCount--;
        }

        //一定の距離以内にプレイヤーがいるとき

        //通常攻撃の範囲内にいるとき
        float magnitude = rot.magnitude;
        if(magnitude > -attackScope && magnitude < attackScope)
        {
            isMove = false;
            //攻撃カウントが0の時　攻撃ができる
            if (attackCount <= 0)
            {
                jawBox.enabled = true;
                animator.SetBool("attack", true);
                attackCount = 120;
            }
            else if(attackCount == 60)
            {
                jawBox.enabled = false;
            }
        }
        //いないときは移動する
        else
        {
            this.transform.position += rot.normalized * speed;
        }

        if (!isReturnCenter)
        {
            isReturnCenter = true;
        }
    }

    /// <summary>
    /// 中心に戻る
    /// </summary>
    private void ReturnToCenterPos()
    {
        isMove = true;//移動している

        //元の位置の方向を向く
        rot = centerPos - this.transform.position;
        rot.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(rot);

        if (rot.magnitude < 1.0f)
        {
            isMove = false;
            isReturnCenter = false;
        }

        this.transform.position += rot.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)//接触したとき
    {
        //死んでいる時何もしない
        if (isDeath) return;

        if (PhotonNetwork.InRoom)
        {
            //マスタークライアントではないとき攻撃が当たった処理をしない
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        //無敵時間が0より大きいとき何もしない
        if (invincibleCount > 0.0f) return;
        
        //攻撃が当たった時
        if (other.gameObject.tag == "weapon")
        {
            var playerObj = other.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;

            //生きている時
            if (enemyStartas.IsAlive())
            {
                rot = this.transform.position - other.gameObject.transform.position;
                //武器が当たった位置から自分を向いて、エフェクトを発生させる
                particle.HitParticle(other.gameObject.transform.position, rot);
            }
            else
            {
                //プレイヤーのレベルを上げる
                playerObj.GetComponent<PlayerStartas>().LevelUp(enemyStartas.LevelPoint());
                playerObj.GetComponent<PlayerItem>().CountItem("gold", enemyStartas.Gold());
            }

            //HPを減らす
            enemyStartas.Hit(other.gameObject.transform.root.GetComponent<PlayerStartas>().GetAttackPower());
            currentHp = enemyStartas.GetNowHp();

            //音を鳴らす
            audioSource.PlayOneShot(hitSound);
            //ダメージアニメーション
            animator.SetBool("damage", true);
            invincibleCount = invincibleTime;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHp);
            stream.SendNext(jawBox.enabled);
            stream.SendNext(isHpBarDraw);
            stream.SendNext(isDeath);
        }
        else
        {
            currentHp = (int)stream.ReceiveNext();
            jawBox.enabled = (bool)stream.ReceiveNext();
            isHpBarDraw = (bool)stream.ReceiveNext();
            isDeath = (bool)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// 倒されたかどうか
    /// </summary>
    /// <returns>true 倒された: false まだ生きている</returns>
    public bool IsDeath()
    {
        return isDeath;
    }
}
