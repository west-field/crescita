using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;
using System.Linq;

/// <summary>
/// ポーズパネル
/// </summary>
public class pausePanelScript : MonoBehaviourPunCallbacks
{
    /*表示パネル*/
    /// <summary>
    /// ポーズパネル
    /// </summary>
    [SerializeField] private GameObject pausePanel;
    /// <summary>
    /// アイテム表示パネルプレハブ
    /// </summary>
    [SerializeField] private GameObject itemPanelPrefab;
    /// <summary>
    /// キー設定表示パネルプレハブ
    /// </summary>
    [SerializeField] private GameObject keyConfigPanelPrefab;
    /// <summary>
    /// ステータス表示パネルプレハブ
    /// </summary>
    [SerializeField] private GameObject startasPanelPrefab;
    /// <summary>
    /// サウンド表示パネルプレハブ
    /// </summary>
    [SerializeField] private GameObject soundPanelPrefab;
    /// <summary>
    /// ウィンドウ変更パネルプレハブ
    /// </summary>
    [SerializeField] private GameObject windowPanelPrefab;
    /// <summary>
    /// 選択している場所にフレームを表示する
    /// </summary>
    [SerializeField] private GameObject selectFrameObj;

    /// <summary>
    /// 作成したパネルプレハブを保持
    /// </summary>
    private GameObject item, key,startas,sound, window;

    /*音*/
    /// <summary>
    /// サウンド
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// ポーズを開くときの音,カーソル移動音
    /// </summary>
    [SerializeField] private AudioClip openingClosingSound, moveSound;

    /*選択*/
    /// <summary>
    /// ポーズ画面で選択できる項目
    /// </summary>
    private enum PauseItem
    {
        item,
        startas,
        keyConfig,
        sound,
        window,
        back,
        titleBack,
        gameEnd,

        max
    }
    /// <summary>
    /// 選択しているものを示す
    /// </summary>
    private int selectNum;
    /// <summary>
    /// 選択項目文字を取得
    /// </summary>
    [SerializeField] private TextMeshProUGUI[] text;
    /// <summary>
    /// いま表示している(選択した)もの
    /// </summary>
    private PauseItem drawType;

    /*キー*/
    /// <summary>
    /// メインマネージャー
    /// </summary>
    private MainManager mainManager;
    /// <summary>
    /// アクションマップからアクションの取得
    /// </summary>
    private InputAction Navigate, Submit, Cancel,Pause;

    /// <summary>
    /// ポーズ画面かどうか
    /// </summary>
    private bool isPause;

    //枠線の拡大縮小
    [SerializeField] private Scaling scalingScript;

    void Start()
    {
        //サウンドの音量を設定した音量に変更する
        audioSource.volume = HoldVariable.SEVolume;
        //表示されたときに音を鳴らす
        audioSource.PlayOneShot(openingClosingSound);

        //Canvasゲームオブジェクトを取得する
        GameObject canvas = GameObject.Find("MainCanvas");
        //自身(ポーズパネル)をCanvasの子オブジェクトにする
        this.transform.SetParent(canvas.transform, false);

        scalingScript.Init(0.9f, 0.7f);
        //フレームの位置を戻るの位置にする
        scalingScript.ScalingObjPosition(selectFrameObj.transform, text[(int)PauseItem.back].transform.position);

        //自身(ポーズパネル)を表示
        PanelChange(true);

        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                //Debug.Log("スキップする");
                return;
            }
            item = PhotonNetwork.Instantiate(itemPanelPrefab.name, Vector3.zero, Quaternion.identity);//アイテムパネル
            item.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            item.SetActive(false);//最初は見えないように

            key = PhotonNetwork.Instantiate(keyConfigPanelPrefab.name, Vector3.zero, Quaternion.identity);//キーコンフィグパネル
            key.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            key.SetActive(false);//最初は見えないように

            startas = PhotonNetwork.Instantiate(startasPanelPrefab.name, Vector3.zero, Quaternion.identity);//ステータスパネル
            startas.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            startas.SetActive(false);//最初は見えないように

            sound = PhotonNetwork.Instantiate(soundPanelPrefab.name, Vector3.zero, Quaternion.identity);//サウンドパネル
            sound.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            sound.SetActive(false);//最初は見えないように

            window = PhotonNetwork.Instantiate(windowPanelPrefab.name, Vector3.zero, Quaternion.identity);//ウィンドウ変更パネル
            window.transform.SetParent(canvas.transform, false);
            window.SetActive(false);
        }
        else
        {
            item = Instantiate(itemPanelPrefab, Vector3.zero, Quaternion.identity);//アイテムパネル
            item.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            item.SetActive(false);//最初は見えないように

            key = Instantiate(keyConfigPanelPrefab, Vector3.zero, Quaternion.identity);//キーコンフィグパネル
            key.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            key.SetActive(false);//最初は見えないように

            startas = Instantiate(startasPanelPrefab, Vector3.zero, Quaternion.identity);//ステータスパネル
            startas.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            startas.SetActive(false);//最初は見えないように

            sound = Instantiate(soundPanelPrefab, Vector3.zero, Quaternion.identity);//ステータスパネル
            sound.transform.SetParent(canvas.transform, false);//Canvasの子オブジェクトにする
            sound.SetActive(false);//最初は見えないように

            window = Instantiate(windowPanelPrefab, Vector3.zero, Quaternion.identity);//ウィンドウ変更パネル
            window.transform.SetParent(canvas.transform, false);
            window.SetActive(false);
        }

        //メインマネージャーを取得
        mainManager = GameObject.Find("Manager").GetComponent<MainManager>();

        //キー取得
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];
        Pause = mainManager.GetPlayerInput().actions["pause"];

        //ポーズを表示している
        isPause = true;

        //初期化
        drawType = PauseItem.max;
    }

    private void Update()
    {
        // ルームに参加している場合は処理をする
        if (PhotonNetwork.InRoom)
        {
            // 自身が生成したオブジェクトだけに処理を行う
            if (!photonView.IsMine)
            {
                //Debug.Log("スキップする");
                return;
            }
        }

        //音量が違うときは音量を合わせる
        if (audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        //いま表示している(選択した)もののアップデートをする
        switch (drawType)
        {
            case PauseItem.item:
                ItemDrawUpdate();
                break;
            case PauseItem.startas:
                StartasDrawUpdate();
                break;
            case PauseItem.keyConfig:
                KeyConfigDrawUpdate();
                break;
            case PauseItem.sound:
                SoundDrawUpdate();
                break;
            case PauseItem.window:
                WindowDrawUpdate();
                break;
            case PauseItem.titleBack:
                TitleBackUpdate();
                break;
            case PauseItem.gameEnd:
                GameEndUpdate();
                break;
            default:
                PauseDrawUpdate();
                break;
        }
    }

    /// <summary>
    /// ポーズ画面(何も指定していないとき)のアップデート
    /// </summary>
    private void PauseDrawUpdate()
    {
        //移動ボタンを押していたら
        if (Navigate.WasPressedThisFrame())
        {
            var val = Navigate.ReadValue<Vector2>();

            //左、上を押していた時
            if (val.x < 0.0f || val.y > 0.0f)
            {
                //一つ上に
                selectNum = (selectNum + ((int)PauseItem.max - 1)) % (int)PauseItem.max;
            }
            //右、下を押していた時
            else if (val.x > 0.0f || val.y < 0.0f)
            {
                //一つ下に
                selectNum = (selectNum + 1) % (int)PauseItem.max;
            }

            //サウンドを鳴らす
            audioSource.PlayOneShot(moveSound);
            //選択している項目の色を変える
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;
                    scalingScript.ScalingObjPosition(selectFrameObj.transform, text[i].transform.position);
                }
                else
                {
                    text[i].color = Color.black;
                }
            }
        }

        scalingScript.ScalingObj(selectFrameObj.transform);

        //決定を押したとき
        if (Submit.WasPressedThisFrame())
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);

            //選択したパネルを表示させる
            switch (selectNum)
            {
                case (int)PauseItem.item:
                    item.SetActive(true);
                    drawType = PauseItem.item;
                    selectNum = 0;
                    break;
                case (int)PauseItem.startas:
                    startas.SetActive(true);
                    drawType = PauseItem.startas;
                    selectNum = 0;
                    break;
                case (int)PauseItem.keyConfig:
                    key.SetActive(true);
                    drawType = PauseItem.keyConfig;
                    selectNum = 0;
                    break;
                case (int)PauseItem.sound:
                    sound.SetActive(true);
                    drawType = PauseItem.sound;
                    selectNum = 0;
                    break;
                case (int)PauseItem.window:
                    window.SetActive(true);
                    drawType = PauseItem.window;
                    selectNum = 0;
                    break;
                case (int)PauseItem.back:
                    // ルームに参加している場合は処理をする
                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.Destroy(gameObject);
                        PhotonNetwork.Destroy(item);
                        PhotonNetwork.Destroy(key);
                        PhotonNetwork.Destroy(sound);
                    }
                    else
                    {
                        Destroy(gameObject);
                        Destroy(item);
                        Destroy(key);
                        Destroy(sound);
                    }

                    PanelChange(false);
                    break;
                case (int)PauseItem.titleBack:
                    //アイテム、ステータス情報を保存
                    this.transform.GetComponent<ItemDataLoadOrSave>().SaveItemData(); 
                    this.transform.GetComponent<StartasLoadOrSave>().SaveStartasData();
                    
                    drawType = PauseItem.titleBack;
                    break;
                case (int)PauseItem.gameEnd:
                    //Destroy(gameObject);
                    // ルームに参加している場合は処理をする
                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.Destroy(item);
                        PhotonNetwork.Destroy(key);
                        PhotonNetwork.Destroy(sound);
                    }
                    else
                    {
                        Destroy(item);
                        Destroy(key);
                        Destroy(sound);
                    }

                    this.transform.GetComponent<ItemDataLoadOrSave>().SaveItemData();
                    this.transform.GetComponent<StartasLoadOrSave>().SaveStartasData();

                    drawType = PauseItem.gameEnd;
                    break;
            }
        }
        //キャンセルを押した時
        else if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(openingClosingSound);

            if (!isPause)
            {
                item.SetActive(false);
                isPause = true;
            }
            else
            {
                // ルームに参加している場合は処理をする
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Destroy(gameObject);
                    PhotonNetwork.Destroy(item);
                    PhotonNetwork.Destroy(key);
                    PhotonNetwork.Destroy(sound);
                }
                else
                {
                    Destroy(gameObject);
                    Destroy(item);
                    Destroy(key);
                    Destroy(sound);
                }

                PanelChange(false);
            }
        }
    }

    /// <summary>
    /// アイテムパネル表示時アップデート
    /// </summary>
    private void ItemDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.item;
            //サウンドを鳴らす
            //アイテム画面を見えなくする
            item.SetActive(false);
            //ポーズ画面に戻す
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// ステータスパネル表示時アップデート
    /// </summary>
    private void StartasDrawUpdate()
    {
        if(Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.startas;
            startas.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// キーコンフィグパネル表示時アップデート
    /// </summary>
    private void KeyConfigDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.keyConfig;
            key.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// サウンドパネル表示時アップデート
    /// </summary>
    private void SoundDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            selectNum = (int)PauseItem.sound;
            sound.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// ウィンドウモード変更パネル表示時アップデート
    /// </summary>
    private void WindowDrawUpdate()
    {
        //キャンセルを押したらポーズ画面に戻す    ウィンドウ変更パネルが見えないときポーズ画面に戻す
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame() || window.activeSelf == false)
        {
            selectNum = (int)PauseItem.window;
            window.SetActive(false);
            drawType = PauseItem.max;
            audioSource.PlayOneShot(openingClosingSound);
        }
    }

    /// <summary>
    /// タイトルへ戻る
    /// </summary>
    private void TitleBackUpdate()
    {
        if (mainManager.IsChangeScene()) return;
        Time.timeScale = 1f;

        mainManager.ChangeScene("title");
    }

    /// <summary>
    /// ゲーム終了を選んだ時のアップデート
    /// </summary>
    private void GameEndUpdate()
    {
        if(mainManager.GameEnd())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
        }
    }

    /// <summary>
    /// ポーズパネルを非表示にするときに必要なこと
    /// (true ポーズ画面を表示させる)
    /// (false ポーズ画面を非表示にする)
    /// </summary>
    /// <param name="activ">true:表示 false:非表示</param>
    public void PanelChange(bool activ)
    {
        if (activ)
        {
            Time.timeScale = 0f;

            selectNum = (int)PauseItem.back;//最初は戻るの場所

            //フレームの位置を戻るの位置にする
            scalingScript.ScalingObjPosition(selectFrameObj.transform, new Vector3(800, 275/*-175*/, 0));

            //選択している項目の色を変える
            for (int i = 0; i < text.Length; i++)
            {
                if (i == selectNum)
                {
                    text[i].color = Color.red;
                    //フレームの位置を戻るの位置にする
                    //scalingScript.ScalingObjPosition(selectFrameObj.transform, text[i].transform.position);
                    //Debug.Log(text[i].transform.position);
                    //scalingScript.ScalingObjPosition(selectFrameObj.transform, new Vector3(0, text[i].transform.position.y, 0));
                }
                else
                {
                    text[i].color = Color.black;
                }
            }
        }
        else if (!activ)
        {
            Time.timeScale = 1f;
            //isChange = true;
        }

        scalingScript.ScalingObj(selectFrameObj.transform);

        //いまの表示状態と違うとき
        if (pausePanel.activeSelf != activ)
        {
            audioSource.PlayOneShot(openingClosingSound);
            //切り替える
            pausePanel.SetActive(activ);
        }
    }

}
