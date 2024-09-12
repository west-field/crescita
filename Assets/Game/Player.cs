using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;

/// <summary>
/// プレイヤー
/// </summary>
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    /// <summary>
    /// メインカンバス
    /// (死亡時のテキスト変更のため)
    /// </summary>
    private GameObject mainCanvas;

    /// <summary>
    /// メインマネージャー
    /// </summary>
    private MainManager manager;

    /*キー*/
    /// <summary>
    /// アクション
    /// </summary>
    private InputAction move,run,fire,jump,block,sit, pause;

    /// <summary>
    /// 移動量　アクションから入力値の取得
    /// </summary>
    private Vector2 value;

    /// <summary>
    /// ボタンを押したか
    /// </summary>
    private bool isFire/*攻撃*/, isWalkAnim/*歩いているか*/, isRunAnim/*走っているか*/, 
        isBlock/*ブロックしているか*/, isSit/*座ったか*/, isDeth/*死んだかどうか*/,isPause/*ポーズ画面に行ったかどうか*/;

    /// <summary>
    /// 地面についているかどうか
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// 自身のRigidbody
    /// </summary>
    private Rigidbody rigid;

    ///*ダメージ*/
    private float damageTime;
    private bool isHit;
    /*-----点滅時間-----*/
    /// <summary>
    /// 子のRendererの配列
    /// </summary>
    private Renderer[] childrenRenderer;
    /// <summary>
    /// childrenRenderersが有効か無効か
    /// </summary>
    private bool isEnabledRenderers;
    /// <summary>
    /// ダメージを受けているか
    /// </summary>
    private bool isDamaged;
    /// <summary>
    /// リセットするときのためにコルーチンを保存しておく
    /// </summary>
    private Coroutine flicker;
    /// <summary>
    /// 点滅時間の長さ
    /// </summary>
    private float flickerDuration;
    /// <summary>
    /// ダメージ点滅の合計時間
    /// </summary>
    private float flickerTotalElapsedTime;
    /// <summary>
    /// ダメ―ジ点滅のRenderer
    /// </summary>
    private float flickerElapsedTime;
    /// <summary>
    /// ダメージ点滅のRendererの有効・無効切り替え用のインターバル。
    /// </summary>
    private float flickerInterval;

    /*移動*/
    /// <summary>
    /// 歩き移動スピード
    /// </summary>
    private static float moveSpeed = 4.5f;
    /// <summary>
    /// 現在の移動スピード
    /// </summary>
    private float speed;

    /// <summary>
    /// 移動、回転
    /// </summary>
    private Vector3 vel, lastPos,diff, rot;//移動、最後の位置、移動しているかどうかの計算結果を取得、回転

    /// <summary>
    /// アニメーターを使ってアニメーションを変更する
    /// </summary>
    private Animator animator;

    /*攻撃*/
    /// <summary>
    /// 攻撃する武器を取得する
    /// </summary>
    [SerializeField] private GameObject weapon;
    /// <summary>
    /// 武器の当たり判定(コライダー)を取得する
    /// </summary>
    private BoxCollider weaponCollider;

    /// <summary>
    /// 攻撃ができる時間
    /// </summary>
    float attackTime;
    /// <summary>
    /// 攻撃のインターバル
    /// </summary>
    private static float attackMaxTime = 35.0f;
    /// <summary>
    /// 武器コライダーの有効無効
    /// </summary>
    private bool isWeponEnable;

    /*アイテム*/
    /// <summary>
    /// アイテムスクリプト
    /// </summary>
    private PlayerItem item;
    /// <summary>
    /// 自身のステータススクリプト
    /// </summary>
    private PlayerStartas startas;

    /// <summary>
    /// レベルアップオブジェクト
    /// </summary>
    [SerializeField] private GameObject levelUpObj;
    /// <summary>
    /// レベルアップテキスト
    /// </summary>
    private TextMeshProUGUI levelUpText;
    /// <summary>
    /// テキスト表示時間
    /// </summary>
    private int levelUpCount;

    /*エフェクト*/
    /// <summary>
    /// 攻撃を受けたときのパーティクル
    /// </summary>
    [SerializeField] private ParticleSystem hitParticle;
    /// <summary>
    /// レベルが上がった時のパーティクル
    /// </summary>
    [SerializeField] private ParticleSystem levelUpParticle;
    /// <summary>
    /// 復活するときのエフェクト
    /// </summary>
    [SerializeField] private ParticleSystem revivalParticle;

    /*ポーズ*/
    /// <summary>
    /// ポーズのスクリプト
    /// </summary>
    [SerializeField] private GameObject pausePanelObj;

    /*音*/
    /// <summary>
    /// サウンドを鳴らす
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// 効果音
    /// </summary>
    [SerializeField] private AudioClip attackSound, hitSound,itemGetSound,levelUpSound;//攻撃音,当たった時の音,アイテム入手音、レベルアップ時の音

    /*HP*/
    /// <summary>
    /// HP
    /// </summary>
    private int nowHp, maxHp;

    /// <summary>
    /// 街に戻るまでの時間
    /// </summary>
    private float backCityTime;

    /// <summary>
    /// 復活するための時間
    /// </summary>
    private float revivalTime;
    /// <summary>
    /// 復活するために必要な時間
    /// </summary>
    private float revivaMaxTime;

    /// <summary>
    /// 取得したアイテムの名前を画面に表示する
    /// </summary>
    private GetItemNameDisplay itemNameDisplay;

    void Start()
    {
        //武器のColliderを取得し、当たらないように無効にしておく
        weaponCollider = weapon.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;
        isWeponEnable = false;

        //レベルアップ時フェードインフェードアウトさせるテキストを取得し、見えないようにする
        levelUpText = levelUpObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelUpObj.SetActive(false);
        levelUpCount = 0;

        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成していないオブジェクトの時
            if (!photonView.IsMine)
            {
                //自分以外のカメラとステータススクリプトを無効にする
                this.transform.Find("Camera").gameObject.SetActive(false);
                this.gameObject.GetComponent<PlayerStartas>().enabled = false;
                return;
            }
        }

        //MainCanvasを取得する
        mainCanvas = GameObject.Find("MainCanvas");
        //MainManagerを取得する
        manager = GameObject.Find("Manager").GetComponent<MainManager>();

        //アクションマップからアクションを取得する
        move = manager.GetPlayerInput().actions["move"];
        run = manager.GetPlayerInput().actions["run"];
        fire = manager.GetPlayerInput().actions["fire"];
        jump = manager.GetPlayerInput().actions["jump"];
        block = manager.GetPlayerInput().actions["block"];
        sit = manager.GetPlayerInput().actions["sit"];
        pause = manager.GetPlayerInput().actions["pause"];

        //アクションから入力値の取得
        value = move.ReadValue<Vector2>();
        
        //ボタンを押しているかを初期化
        isFire = false;
        isWalkAnim = false;
        isRunAnim = false;
        isBlock = false;
        isSit = false;
        isDeth = false;
        isPause = false;

        //最初は地面についていることにする
        isGrounded = true;

        //自身のRigidbodyを取得
        rigid = this.gameObject.GetComponent<Rigidbody>();

        //無敵初期化
        damageTime = 0;
        isHit = false;

        //全てのRendererを取得
        childrenRenderer = GetComponentsInChildren<Renderer>();

        //ダメージは受けていない
        isDamaged = false;

        //コルーチンを初期化
        flicker = null;

        //点滅の長さ
        flickerDuration = 1.5f;

        flickerTotalElapsedTime = 0;
        flickerElapsedTime = 0;

        //インターバル
        flickerInterval = 0.075f;

        //歩くスピードに初期化
        speed = moveSpeed;

        //初期化
        vel = Vector3.zero;//移動ベクトル
        lastPos = this.transform.position;//最後の位置
        diff = Vector3.zero;//移動の計算結果
        rot = Vector3.zero;//回転

        //アニメーターを取得する
        animator = this.GetComponent<Animator>();

        //攻撃初期化
        attackTime = 0.0f;

        //自身のオブジェクトからスクリプトを取得
        item = this.gameObject.GetComponent<PlayerItem>();
        startas = this.gameObject.GetComponent<PlayerStartas>();

        //サウンドの音量を変更する
        audioSource.volume = HoldVariable.SEVolume;

        //最大HPを取得する
        maxHp = 0;
        if (PhotonNetwork.InRoom)
        {
            //全てのルーム参加者に送る
            photonView.RPC(nameof(MaxHp), RpcTarget.All, startas.GetMaxHp());
        }
        else
        {
            maxHp = startas.GetMaxHp();
        }
        //現在のHPを取得する
        nowHp = startas.GetNowHp();

        //街に戻るまでの時間を取得
        backCityTime = 5;

        //復活するための時間を初期化
        revivalTime = 0;

        //復活するために必要な時間
        revivaMaxTime = 10;

        //GetItemNameDisplayを追加したオブジェクトがあるかどうか
        if (GameObject.Find("ItemNameDisplay") != null)
        {
            //あるときGetItemNameDisplayを取得する
            itemNameDisplay = GameObject.Find("ItemNameDisplay").GetComponent<GetItemNameDisplay>();
            Debug.Log("ItemNameDisplayがあった");
        }
    }

    private void Update()
    {
        //レベルアップテキストを表示している時
        if (levelUpCount != 0)
        {
            //回転を初期化する
            levelUpObj.transform.rotation = Quaternion.identity;
        }

        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            //自身が生成したオブジェクトではないとき処理を行わない
            if (!photonView.IsMine)
            {
                return;
            }
        }

        //座る
        Sit();

        //ブロックしている時は動けない
        if (isBlock) return;
        //ジャンプする
        Jump();
    }

    private void FixedUpdate()
    {
        //武器の当たり判定を変更する
        weaponCollider.enabled = isWeponEnable;

        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            //自身が生成したオブジェクトではないとき処理を行わない
            if (!photonView.IsMine)
            {
                return;
            }
        }
        
        //音量が違うときは音量を合わせる
        if(audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //hpが0になって、生きていないとき
        if (!startas.IsAlive())
        {
            Death();

            return;
        }

        //アニメーション変更
        animator.SetBool("move", isWalkAnim);
        animator.SetBool("run", isRunAnim);
        animator.SetBool("block", isBlock);

        //シーンを変更している時は処理をしない
        if (manager.IsChangeScene())
        {
            //移動アニメーションを止める
            animator.SetBool("move", false);
            return;
        }

        LevelUp();

        //ポーズ
        if (!isPause && pause.IsPressed())
        {
            // ルームに参加していない場合は処理をする
            if (!PhotonNetwork.InRoom)
            {
                isPause = true;
                Instantiate(pausePanelObj, Vector3.zero, Quaternion.identity);
            }
            return;
        }
        else if (isPause && !pause.IsPressed())
        {
            isPause = false;
        }

        //ダメージを受けたときは何もできないように
        if (isDamaged)
        {
            damageTime -= Time.deltaTime;
            if (damageTime > 0.0f)
            {
                //移動量を初期化
                value = Vector2.zero;
                return;
            }
        }

        //座っている時はジャンプ以外できない
        if (isSit) return;
        Block();
        Fire();
        Move();
        //ブロックしている時は走れない
        if (isBlock) return;
        Run();
    }

    /// <summary>
    /// レベルアップ
    /// </summary>
    private void LevelUp()
    {
        //レベルアップしたとき
        if (startas.IsLevelUp())
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(levelUpSound);

            //パーティクルを再生
            levelUpParticle.Play();

            //レベルアップテキストのオブジェクト
            levelUpObj.transform.rotation = new Quaternion(levelUpObj.transform.rotation.x, 0, 0, 0);//回転を初期化
            levelUpObj.transform.position = this.transform.position + new Vector3(0.0f, 3.0f, 0.0f);//位置を初期化
            levelUpObj.SetActive(true);//表示する

            //テキストのα値を最大にする
            Color color = levelUpText.color;
            color.a = 1.0f;
            levelUpText.color = color;

            //テキストを表示しておく時間
            levelUpCount = 60 * 5;
        }

        //レベルアップテキストを表示している時
        if(levelUpObj.activeSelf)
        {
            //だんだん透明にする
            Color color = levelUpText.color;
            color.a -= 0.01f;
            levelUpText.color = color;

            //時間を減らす
            levelUpCount--;
            Debug.Log(levelUpCount);

            //α値が0よりも小さいとき
            if (color.a <= 0.0f)
            {
                //表示を終わる
                levelUpCount = 0;
            }

            //表示時間が0より小さくなったら
            if (levelUpCount <= 0)
            {
                levelUpCount = 0;

                color.a = 0.0f;
                levelUpText.color = color;
                //非表示にする
                levelUpObj.SetActive(false);
                Debug.Log("レベルアップテキスト:false");
            }
        }
    }


    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        //初期化
        value = Vector2.zero;
        vel = Vector3.zero;

        //アクション開始から終了まで
        isWalkAnim = move.IsPressed();

        //移動ボタンを押していないとき
        if (!isWalkAnim )
        {
            return;
        }

        //入力値の取得
        value = move.ReadValue<Vector2>();

        //移動量計算
        vel = new Vector3(value.x, 0.0f, value.y) * Time.deltaTime * speed;
        //移動量を足す
        this.transform.position += vel;

        //前回から進んだかを取得
        diff = this.transform.position - lastPos;
        //y軸は変更しない
        diff.y = 0;
        //前回の位置の更新
        lastPos = this.transform.position;

        //移動量が少ないときはプレイヤーの方向を変えない
        if(diff.sqrMagnitude <= 0.001f)
        {
            return;
        }

        var playerRot = Quaternion.LookRotation(diff, Vector3.up);//プレイヤーの進行方向を向くクォータニオン
        var diffAngle = Vector3.Angle(transform.forward, diff);//現在の向きと進行方向の角度
        float currentAngularVel = 0.0f;
        var rotAngle = Mathf.SmoothDampAngle(0, diffAngle, ref currentAngularVel, 0.03f, Mathf.Infinity);//現在の回転する角度
        var nextRot = Quaternion.RotateTowards(transform.rotation, playerRot, rotAngle);//どのくらい回転するか
        transform.rotation = nextRot;

    }

    /// <summary>
    /// 走る
    /// </summary>
    private void Run()
    {
        //初期化
        speed = moveSpeed;
        //アクション開始から終了まで
        isRunAnim = run.IsPressed();

        //走るボタンを押していない　|| 歩いていないとき
        if (!isRunAnim || !isWalkAnim)
        {
            return;
        }
        //歩きスピードを1.5倍にする
        speed *= 1.5f;
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    private void Fire()
    {
        if(manager.NowGameScene() != "grassland" && manager.NowGameScene() != "plain")
        {
            return;
        }

        //インターバル
        attackTime -= 1.0f;
        //次に攻撃できるまでのインターバルが0よりも小さいとき　&&　攻撃中の時
        if (attackTime <= 0.0f && isFire)
        {
            //0にする
            attackTime = 0.0f;
            //武器の当たり判定があるとき
            if (isWeponEnable)
            {
                //当たり判定を消す
                isWeponEnable = false;
            }
            //攻撃を終了
            isFire = false;
        }
        else if(attackTime == 15.0f)
        {
            //攻撃音
            audioSource.PlayOneShot(attackSound);
            //武器の当たり判定をtrueにする
            isWeponEnable = true;
            //攻撃している時は防御判定を行わないように
            isBlock = false;
            return;
        }
        //次に攻撃できるまでの時間がまだあるとき
        else if(attackTime > 0.0f)
        {
            return;
        }

        //アクションの開始から終了まで
        isFire = fire.IsPressed();
        //攻撃したとき
        if(isFire)
        {
            animator.SetBool("attack", true);
            attackTime = attackMaxTime;
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void Jump()
    {
        //地面についているとき
        if(isGrounded)
        {
            //ジャンプキーが押されているとき
            if (jump.WasPressedThisFrame())
            {
                //座っていた時
                if (isSit)
                {
                    isSit = false;
                    animator.SetBool("sit", isSit);
                }

                //ジャンプの速度を計算
                var jumpVelocity = Vector3.up * (moveSpeed * 1.0f);
                
                rigid.AddForce(jumpVelocity, ForceMode.Impulse);
                animator.SetBool("jump", true);

                isGrounded = false;
            }
        }
    }

    /// <summary>
    /// ブロック
    /// </summary>
    private void Block()
    {
        //アクションの開始から終了まで
        isBlock = block.IsPressed();

        //ブロックボタンを押していなかったら
        if (!isBlock)
        {
            //処理を抜ける
            return;
        }

        isRunAnim = false;
    }

    /// <summary>
    /// 座る
    /// </summary>
    private void Sit()
    {
        //押していた時
        if (sit.WasPerformedThisFrame())
        {
            //入れ替える
            isSit = !isSit;
            animator.SetBool("sit", isSit);
        }
    }

    /// <summary>
    /// 死んだときの処理
    /// </summary>
    private void Death()
    {
        //isDethがfalseの時
        if (!isDeth)
        {
            //死んだことにする
            isDeth = true;
            //攻撃されたときの点滅時間を終わらせる
            flickerTotalElapsedTime = flickerDuration;
            //死んだアニメーションを再生
            animator.SetBool("death", true);

            //複数人プレイの時は
            if(PhotonNetwork.InRoom)
            {
                //復活までのエフェクトを再生
                revivalParticle.Play();
            }
        }
        //自身のタグを変更して、敵がこちらに来ないようにする
        if (this.tag == "Player")
        {
            this.tag = "Untagged";
        }

        // ルームに参加している場合
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                return;
            }

            //復活までの時間を進める
            revivalTime += Time.deltaTime;

            //復活するまでの時間以上になったら
            if (revivalTime >= revivaMaxTime)
            {
                //時間を初期化
                revivalTime = 0;
                
                //死んでいないことにする
                isDeth = false;
                
                //アニメーションを変更
                animator.SetBool("death", false);
                animator.SetBool("attack", true);
                
                //復活
                startas.Heals(100);
                
                //現在のHPを取得する
                nowHp = startas.GetNowHp();
                
                //自身のタグを変更する
                if (this.tag != "Player")
                {
                    this.tag = "Player";
                }
            }
            
            return;
        }
        else
        {
            //街に戻るまでの時間を減らしていく
            backCityTime -= Time.deltaTime;
            
            //テキストに表示する
            var str = $"あと{(int)backCityTime}秒で\n街にもどります。";
            mainCanvas.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = str;
            
            //街に戻る
            if (backCityTime < 0.0f)
            {
                manager.ChangeScene("city");
                return;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                return;
            }
        }

        //生きていないときは何もしない
        if (!startas.IsAlive())
        {
            return;
        }

        if (!isGrounded)
        {
            if (collision.gameObject.tag != "wall")
            {
                Debug.Log("Player ジャンプできるように");
                isGrounded = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                return;
            }
        }

        //生きていないときは何もしない
        if (!startas.IsAlive())
        {
            return;
        }

        //if (other.gameObject.tag != "wall")
        //{
        //    Debug.Log("Player ジャンプできるように");
        //    isGrounded = true;
        //}

        //アイテムタグかどうか
        if (other.gameObject.tag == "item")
        {
            //アイテムスクリプトがあるか
            Debug.Log("アイテムスクリプト");
            Item itemScript = other.gameObject.GetComponent<Item>();

            if (itemScript != null)
            {
                item.CountItem(itemScript.id, 1);//自身のアイテムに追加する
                Debug.Log("アイテム追加");
                //GetItemNameDisplayがあるかを確認
                if (itemNameDisplay != null)
                {
                    //あった時は取得したアイテム名を渡す
                    itemNameDisplay.ItemNameDisplay(item.ItemName(itemScript.id));
                }

                //取得音
                audioSource.PlayOneShot(itemGetSound);
                //追加したアイテムは消す
                Destroy(other.gameObject);
            }
        }

        //敵と当たった時
        if (other.gameObject.tag == "enemy")
        {
            //当たった時は一瞬硬直
            isWalkAnim = false;

            //移動量を初期化
            value = Vector2.zero;

            ////無敵時間中は何もしない
            if (isDamaged) return;

            //敵の方向を向く   ターゲット - 自身の位置
            rot = other.gameObject.transform.position - this.transform.position;
            rot.y = 0.0f;
            this.transform.rotation = Quaternion.LookRotation(rot);
            //ノックバック
            rigid.AddForce(-transform.forward.normalized * (moveSpeed * 0.2f), ForceMode.VelocityChange);//歩くスピード分

            //攻撃をしていないときは当たったアニメーションをする
            if (!isWeponEnable)
            {
                if(!isHit)
                {
                    //当たったアニメーションにする
                    animator.SetBool("hit", true);
                    isHit = true;
                }
            }
            //攻撃をしている時は攻撃を受けない
            else
            {
                animator.SetBool("hit", false);
                return;
            }

            //ヒット音
            audioSource.PlayOneShot(hitSound);

            //ブロックしていたらHPは減らない
            if (isBlock)
            {
                damageTime = 0;
                return;
            }
            //ダメージ
            Damaged();
            
            //当たったエフェクトを再生
            hitParticle.Play();
            
            //parent 親オブジェクト　root 一番上の親　GetChild(0)　子を取得
            int hit = other.gameObject.transform.root.GetComponent<EnemyStartas>().GetAttackPower() - startas.GetDefensePower();
            startas.Hit(hit);
            nowHp = startas.GetNowHp();

            //無敵時間
            damageTime = 0.5f;

            //ノックバック
            rigid.AddForce(-transform.forward.normalized * (moveSpeed * 0.5f), ForceMode.VelocityChange);//ブロックしていないときはさらにノックバックする

            this.transform.position -= diff * Time.deltaTime * moveSpeed;
        }
    }

    //HPバー送受信
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //このクライアントがPhotonViewの所有者である場合
        if (stream.IsWriting)
        {
            //送信する
            stream.SendNext(nowHp);//現在のHP
            stream.SendNext(isWeponEnable);//武器の当たり判定
        }
        else
        {
            //受信する
            nowHp = (int)stream.ReceiveNext();//現在のHPを変更する
            isWeponEnable = (bool)stream.ReceiveNext();//武器の当たり判定を変更する
        }
    }

    /// <summary>
    /// 最大HPを送る
    /// </summary>
    /// <param name="hp">最大HP</param>
    [PunRPC]
    private void MaxHp(int hp)
    {
        maxHp = hp;
    }

    //--------------------------ダメージ--------------------------//
    /// <summary>
    /// ダメージを受けたときに呼び出す
    /// </summary>
    private void Damaged()
    {
        //ダメージ点滅中は二重に実行しない。
        if (isDamaged)
            return;

        StartFlicker();
    }
    /// <summary>
    /// 表示するかしないかを変更する関数
    /// </summary>
    /// <param name="b">true:表示にする false:非表示にする</param>
    private void SetEnabledRenderers(bool b)
    {
        //一つずつ表示非表示を変更する
        for (int i = 0; i < childrenRenderer.Length; i++)
        {
            childrenRenderer[i].enabled = b;
        }
    }

    /// <summary>
    /// 非同期で点滅を始める
    /// </summary>
    private void StartFlicker()
    {
        //isDamagedで多重実行を防いでいるので、ここで多重実行を弾かなくてもOK。        
        flicker = StartCoroutine(Flicker());
    }

    /// <summary>
    /// 非同期で点滅
    /// </summary>
    /// <returns></returns>
    IEnumerator Flicker()
    {
        //点滅中はダメージを受けている設定に
        isDamaged = true;
        //初期化
        flickerTotalElapsedTime = 0;
        flickerElapsedTime = 0;

        while (true)
        {
            //時間を進める
            flickerTotalElapsedTime += Time.deltaTime;
            flickerElapsedTime += Time.deltaTime;


            //ダメージ点滅のインターバル
            if (flickerInterval <= flickerElapsedTime)
            {
                //ここが被ダメージ点滅の処理。

                flickerElapsedTime = 0;
                //Rendererの有効、無効の反転。
                isEnabledRenderers = !isEnabledRenderers;
                SetEnabledRenderers(isEnabledRenderers);

            }

            //点滅時間の長さ
            if (flickerDuration <= flickerTotalElapsedTime)
            {
                //ここが被ダメージ点滅の終了時の処理。
                Debug.Log("被ダメージ終了");
                isDamaged = false;
                isHit = false;

                //最後には必ずRendererを有効にする(消えっぱなしになるのを防ぐ)。
                isEnabledRenderers = true;
                SetEnabledRenderers(true);

                flicker = null;
                ResetFlicker();
                yield break;
            }

            yield return null;
        }

    }

    /// <summary>
    /// コルーチンのリセット用
    /// </summary>
    private void ResetFlicker()
    {
        if (flicker != null)
        {
            StopCoroutine(flicker);
            flicker = null;
        }
    }
}
