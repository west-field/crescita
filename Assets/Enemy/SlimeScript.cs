using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// スライム
/// </summary>
public class SlimeScript : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject slime;//本体
    [SerializeField] private EnemyStartas enemyScript;//ステータス

    /*パーティクル*/
    [SerializeField] private ParticleSystem deathParticle;//パーティクル
    [SerializeField] private GameObject hitParticleObject;//当たった時のエフェクトオブジェクト
    private ParticleSystem hitParticle;//攻撃を受けたときのパーティクル

    /*アニメーション*/
    private Animator animator;//アニメーター

    /*挙動*/
    private bool isDeath,isMove;//死んだかどうか、移動しているかどうか、走っているかどうか
    private bool isWallOnTrigger, isReturnCenter;//, isRotating;//壁に当たっているかどうか,中心に戻るか,回転しているか

    /*移動*/
    static float moveSpeed = 0.01f;
    private float speed;

    private Vector3 centerPos;//中心位置(移動範囲を決める際の中央)
    private Vector3 rot;//回転

    private float chargeTime;//移動方向を変える時間
    private float timeCount;//経過時間

    /*攻撃*/
    private int attackCount = 0;

    static private float invincibleTime = 60.0f;//無敵時間
    private float invincibleCount;//無敵経過時間

    /*音*/
    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip attackSound;//攻撃音
    [SerializeField] private AudioClip hitSound;//当たった時の音

    /*HP*/
    [SerializeField] private GameObject hpBarObject;//HPバーのゲームオブジェクト
    private bool isHpBarDraw;//hpバーを表示する
    [SerializeField] private Image hpBars;//HPバー
    private int currentHp;
    private int maxHp;

    [SerializeField] private GameObject miniMap;

    void Start()
    {
        enemyScript.Init(8, 5, 1.3f, 5, 5);

        animator = slime.GetComponent<Animator>();
        animator.SetBool("strong", enemyScript.IsStrong());

        hpBarObject.SetActive(false);

        isDeath = false;
        isMove = false;

        isWallOnTrigger = false;

        speed = moveSpeed;

        centerPos = this.transform.position;

        chargeTime = Random.Range(3.0f, 6.0f);
        timeCount = 0;

        isReturnCenter = false;

        invincibleCount = 0;
        hitParticle = hitParticleObject.GetComponent<ParticleSystem>();

        maxHp = enemyScript.GetMaxHp();
        currentHp = enemyScript.GetNowHp();

        audioSource.volume = HoldVariable.SEVolume;
    }

    private void FixedUpdate()
    {
        if (isDeath) 
        {
            var count = this.transform.childCount;
            if(count <= 0)
            {
                if (PhotonNetwork.InRoom)
                {
                    if(photonView.IsMine)
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

        //hpバーを表示非表示を変更する
        if (isHpBarDraw != hpBarObject.activeSelf)
        {
            hpBarObject.SetActive(isHpBarDraw);
        }
        //hpBarObjectがあるとき
        if (hpBarObject != null)
        {
            //反映させる
            hpBars.fillAmount = (float)currentHp / (float)maxHp;
        }

        //無敵経過時間
        if (invincibleCount > 0.0f)
        {
            invincibleCount--;
        }

        //死んだとき
        if (currentHp <= 0)
        {
            Debug.Log("死んだ");
            isDeath = true;

            animator.SetBool("die", isDeath);
            //当たり判定を消す
            BoxCollider box = slime.GetComponentInChildren<BoxCollider>();
            box.enabled = false;

            if(hpBarObject != null)
            {
                Destroy(hitParticleObject);
                Destroy(hpBarObject);
                Destroy(miniMap);

                deathParticle.Play();
                Destroy(slime);
            }

            GameObject item;
            //アイテム
            var rand = Random.Range(1, 100) % 4;
            switch (rand)
            {
                case 0:
                case 1:
                    Debug.Log("プルプル アイテム生成");
                    item = (GameObject)Resources.Load("redStuffy");
                    Instantiate(item, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z), item.transform.localRotation);
                    break;
                case 2:
                    Debug.Log("コア アイテム生成");
                    item = (GameObject)Resources.Load("redJigglyCore");
                    Instantiate(item, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z), item.transform.localRotation);
                    break;
                default:
                    break;
            }
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        //音量が違うときは音量を合わせる
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //アニメーターが破棄されている時
        if (animator == null)
        {
            //何もしない
            return;
        }

        animator.SetBool("move", isMove);
        //範囲内にプレイヤーがいるとき
        if (enemyScript.GetScopescript().IsScope())
        {
            isHpBarDraw = true;
            //プレイヤーを追いかける
            ChaseThePlayer();
        }
        else if (isReturnCenter)
        {
            isHpBarDraw = false;
            //元の位置に戻る
            ReturnToCenterPos();
        }
        else
        {
            isHpBarDraw = false;
            //上下に移動する
            ForwardAndBackwardMovement();
        }
        //Hpバーを表示している時
        if (hpBarObject.activeSelf)
        {
            //カメラの方向を向く
            hpBarObject.transform.rotation = Quaternion.identity;
        }
    }

    // <summary>
    /// プレイヤーを追いかける
    /// </summary>
    private void ChaseThePlayer()
    {
        isMove = true;//移動している

        //プレイヤーの方向を向く
        rot = enemyScript.GetScopescript().TargetPosition() - this.transform.position;
        rot.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(rot);

        if (attackCount > 0)
        {
            attackCount--;
        }

        //一定の距離以内にプレイヤーがいるとき
        float magnitude = rot.magnitude;
        if (magnitude > -enemyScript.GetScope() && magnitude < enemyScript.GetScope())
        {
            isMove = false;
            //攻撃カウントが0の時　攻撃ができる
            if (attackCount <= 0)
            {
                audioSource.PlayOneShot(attackSound);
                if(!enemyScript.IsStrong())
                {
                    this.transform.position += rot.normalized * (speed * 10);
                }

                animator.SetBool("attack", true);
                attackCount = 120;
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
    /// 元の位置に戻る
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

    /// <summary>
    /// 前後に移動する
    /// </summary>
    private void ForwardAndBackwardMovement()
    {
        isMove = true;//移動している

        timeCount += Time.deltaTime;//時間経過
        bool isChange = false;

        //ランダムで決まった時間よりも大きくなったら
        if (timeCount > chargeTime)
        {
            timeCount = 0;//カウントを０にする
            isChange = true;//変更する
        }
        //壁に当たったら
        else if (isWallOnTrigger)
        {
            isWallOnTrigger = false;//処理が重ならないように falseにする
            isChange = true;//変更する
        }


        if (isChange)
        {
            float angle = 0;//方向転換
            if (this.transform.localRotation.y == 0)
            {
                angle = 180;
            }
            this.transform.localRotation = Quaternion.Euler(0, angle, 0);
        }

        this.transform.position += this.transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)//接触したとき
    {
        if (isDeath) return;

        if (PhotonNetwork.InRoom)
        {
            //マスタークライアントではないとき攻撃が当たった処理をしない
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        //無敵時間が0より大きいとき何もしない
        if (invincibleCount > 0.0f) return;

        //攻撃が当たった時
        if (other.gameObject.tag == "weapon")
        {
            var pos = other.gameObject.transform.position;

            rot = pos - this.transform.position;
            rot.y = 0.0f;

            //プレイヤーの方向を向く            
            this.transform.rotation = Quaternion.LookRotation(rot);

            audioSource.PlayOneShot(hitSound);

            //HPを減らす
            enemyScript.Hit(other.gameObject.transform.root.GetComponent<PlayerStartas>().GetAttackPower());
            currentHp = enemyScript.GetNowHp();

            //ダメージアニメーション
            animator.SetBool("damage", true);

            //無敵時間
            invincibleCount = invincibleTime;
            attackCount += 10;

            //ノックバック
            var rigid = this.gameObject.GetComponent<Rigidbody>();
            rigid.AddForce(-transform.forward * (moveSpeed * 2), ForceMode.VelocityChange);

            //生きていないとき
            if (currentHp <= 0)
            {
                //ルームに参加している場合は処理をする
                if (PhotonNetwork.InRoom)
                {
                    //自分がオーナーではないとき
                    if (!photonView.IsMine)
                    {
                        this.gameObject.GetComponent<PhotonView>().TransferOwnership(other.transform.root.GetComponent<PhotonView>().Owner);
                        Debug.Log("オーナー変更");
                        return;
                    }
                }

                GameObject playerObj = other.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;
                //プレイヤーのレベルを上げる
                playerObj.GetComponent<PlayerStartas>().LevelUp(enemyScript.LevelPoint());
                playerObj.GetComponent<PlayerItem>().CountItem("gold", enemyScript.Gold());
            }
            else
            {
                //武器が当たった位置から自分を向く
                hitParticle.transform.position = pos;

                rot = this.transform.position - hitParticle.transform.position;
                hitParticle.transform.rotation = Quaternion.LookRotation(rot);

                //エフェクトを発生させる
                hitParticle.Play();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)//接触したとき
    {
        if (isDeath) return;
        if (collision.gameObject.tag == "wall")
        {
            isWallOnTrigger = true;
            Debug.Log("当たった");
        }

    }

    //データ送受信
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //このオブジェクトがPhotonViewの所有者である場合
        if (stream.IsWriting)
        {
            stream.SendNext(currentHp);//現在のHP
            stream.SendNext(isHpBarDraw);//HPバーの表示非表示
            stream.SendNext(isDeath);//死んだかどうか
        }
        else
        {
            currentHp = (int)stream.ReceiveNext();//現在のHPを変更する
            isHpBarDraw = (bool)stream.ReceiveNext();//HPバーの表示非表示を変更
            isDeath = (bool)stream.ReceiveNext();//死んだかどうかのフラグを変更
        }
    }
}
