using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BossGrassland : MonoBehaviour, IPunObservable
{
    /*ステータス*/
    [SerializeField]
    private EnemyStartas enemyStartas;

    /*パーティクル*/
    [SerializeField]
    private BossParticle particle;

    /*アニメーション*/
    Animator animator;//アニメーター

    /*挙動*/
    bool isDeath, isMove;//死んだかどうか、移動しているかどうか
    bool isReturnCenter;//中心に戻るか

    /*移動*/
    static float moveSpeed = 0.01f;//移動スピード
    float speed;//今のスピード

    Vector3 centerPos;//中心位置(移動範囲を決める際の中央)
    Vector3 rot;//回転

    /*攻撃*/
    static private float invincibleTime = 60.0f;//無敵時間
    private float invincibleCount;//無敵経過時間

    private int attackCount = 0;//攻撃できるまでの時間

    private float attackScope;//攻撃できる範囲

    BoxCollider tailBox,jawBox;//攻撃できる

    /*音*/
    [SerializeField]
    private AudioSource audioSource;//サウンド
    [SerializeField]
    private AudioClip attackSound;//攻撃音
    [SerializeField]
    private AudioClip hitSound;//当たった時の音

    /*Hp*/
    [SerializeField]
    private GameObject hpBarObject;//HPバーのゲームオブジェクト
    [SerializeField]
    private Image hpBars;//HPバー
    private int currentHp;
    private int maxHp;

    /*死亡*/
    private int deathCount;

    private GameObject playerObj;//レベルに入れるためだけに作成

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();

        hpBarObject.SetActive(false);

        enemyStartas.Init(50, 20, 1.0f, 100, 100);

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

        MaxHp(enemyStartas.GetMaxHp());
        currentHp = enemyStartas.GetNowHp();
    }

    private void FixedUpdate()
    {
        if (!isDeath)
        {
            //反映させる
            hpBars.fillAmount = (float)currentHp / (float)maxHp;
        }
        else
        {
            deathCount++;
            //Debug.Log(deathCount);
            if (deathCount >= 120)
            {
                Debug.Log("時間経過でボスを削除");
                particle.ParticlePlay();

                GameObject item;
                //アイテム
                var rand = Random.Range(0, 100);

                string[] itemName = { "frogHorn", "frog'sPearl", "frog'sGoldenBeads", "frog'sBlackJewel", "lotusLeaf" };

                var root = this.transform.root.gameObject.transform.position;// - this.transform.position;
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
                    //if (PhotonNetwork.InRoom) //if (PhotonNetwork.IsConnected)
                    //{
                    //    PhotonNetwork.Instantiate(item.name, new Vector3(), item.transform.localRotation);
                    //}
                    //else
                    {
                        Instantiate(item, new Vector3(), item.transform.localRotation);
                    }
                }

                Destroy(this.gameObject);
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
            animator.SetBool("die", isDeath);
            
            tailBox.enabled = false;
            jawBox.enabled = false;
            if (hpBarObject != null)
            {
                Destroy(hpBarObject);
            }

            //プレイヤーのレベルを上げる
            playerObj.GetComponent<PlayerStartas>().LevelUp(enemyStartas.LevelPoint());
            return;
        }

        //ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom) //if (PhotonNetwork.IsConnected)
        {
            return;
        }

        animator.SetBool("move", isMove);

        //範囲内にプレイヤーがいるとき
        if (enemyStartas.GetScopescript().IsScope())
        {
            if (hpBarObject != null)
            {
                hpBarObject.SetActive(true);
            }
            //プレイヤーを追いかける
            ChaseThePlayer();
        }
        else if (isReturnCenter)
        {
            if (hpBarObject != null)
            {
                hpBarObject.SetActive(false);
            }
            ReturnToCenterPos();
        }
        else
        {
            if (hpBarObject != null)
            {
                hpBarObject.SetActive(false);
            }
            //眠る

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

        //Debug.Log(rot.normalized);

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
        //火の玉攻撃の範囲内にいるとき

        //しっぽ攻撃の範囲にいる時

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
            isReturnCenter = false;
        }

        this.transform.position += rot.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)//接触したとき
    {
        //死んでいる時何もしない
        if (isDeath) return;

        //無敵時間が0より大きいとき何もしない
        if (invincibleCount > 0.0f) return;
        
        //攻撃が当たった時
        if (other.gameObject.tag == "weapon")
        {
            //プレイヤーの方向を向く
            rot = other.gameObject.transform.position - this.transform.position;
            rot.y = 0.0f;
            this.transform.rotation = Quaternion.LookRotation(rot);

            playerObj = other.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;

            //生きている時
            if (enemyStartas.IsAlive())
            {
                rot = this.transform.position - other.gameObject.transform.position;
                //武器が当たった位置から自分を向いて、エフェクトを発生させる
                particle.HitParticle(other.gameObject.transform.position, rot);
            }
            else
            {
                playerObj.GetComponent<PlayerItem>().CountItem("gold", enemyStartas.Gold());
                //プレイヤータグを検索
                var obj = GameObject.FindGameObjectsWithTag("enemy");
                //もしあればボックスコライダーを消す
                foreach (var collider in obj)
                {
                    if(collider.GetComponent<BoxCollider>())
                    {
                        collider.GetComponent<BoxCollider>().enabled = false;
                    }
                    break;
                }
            }

            //HPを減らす
            enemyStartas.Hit(other.gameObject.transform.root.GetComponent<PlayerStartas>().GetAttackPower());
            currentHp = enemyStartas.GetNowHp();
            ////反映させる
            //hpBars.fillAmount = (float)enemyStartas.GetNowHp() / (float)enemyStartas.GetMaxHp();

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
            stream.SendNext(isDeath);
        }
        else
        {
            currentHp = (int)stream.ReceiveNext();
            isDeath = (bool)stream.ReceiveNext();
        }
    }
    //最大HPを送る
    [PunRPC]
    private void MaxHp(int hp)
    {
        maxHp = hp;
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
