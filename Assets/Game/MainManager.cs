using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// メインマネージャー
/// </summary>
public class MainManager : MonoBehaviour
{
    /// <summary>
    /// PlayerInputの取得
    /// </summary>
    [SerializeField] private PlayerInput input;

    /// <summary>
    /// フェードパネル
    /// </summary>
    [SerializeField] private GameObject panelfade;

    /// <summary>
    /// フェードパネルの透明度を変えるため
    /// </summary>
    private Image fadealpha;

    /// <summary>
    /// パネルのアルファ値
    /// </summary>
    private float alpha;

    /// <summary>
    /// フェードスピード
    /// </summary>
    private float fadeSpeed;

    /// <summary>
    /// フェードインフラグ
    /// </summary>
    private bool fadein;
    /// <summary>
    /// フェードアウトフラグ
    /// </summary>
    private bool fadeout;

    /// <summary>
    /// 次のシーン名
    /// </summary>
    private string nextSceneName;

    /// <summary>
    /// サウンド
    /// </summary>
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// フェードイン
    /// </summary>
    private float fadeSeconds;

    /// <summary>
    /// オンラインかオフラインかを指定する
    /// </summary>
    private OfflineMode offlineMode;

    /// <summary>
    /// ロードの進捗状況を表示するUIなど
    /// </summary>
    [SerializeField]　private GameObject loadingUI;

    /// <summary>
    /// ロードの進捗状況を管理するための変数
    /// </summary>
    private AsyncOperation async;

    /// <summary>
    /// シーンをロードしているかどうか
    /// </summary>
    private bool isSceneLoading;

    /// <summary>
    /// ロード画面時表示できる画像の枚数
    /// </summary>
    [SerializeField] private int randMax = 5;

    /// <summary>
    /// シーン変更時に生成するUI
    /// </summary>
    private GameObject map, button, startas;

    private void Awake()
    {
        //サーバーに接続　or　オフラインモードを有効にする
        offlineMode = this.gameObject.GetComponent<OfflineMode>();
        offlineMode.Connect();

        //ロード画像を決める
        int rand = Random.Range(0, 100) % randMax;
        //画像をResourcesから取得する
        Sprite sprite = Resources.Load<Sprite>($"roding_{rand}");
        //画像を表示するUIを取得する
        Image image = loadingUI.GetComponent<Image>();
        //画像を差し替える
        image.sprite = sprite;

        //ロード画像は見えないようにする
        loadingUI.SetActive(false);
    }

    private void Start()
    {
        //イメージを取得
        fadealpha = panelfade.GetComponent<Image>();
        //透明度を1にしておく
        alpha = 1.0f;
        //フェードスピード
        fadeSpeed = 0.05f;

        //初めはフェードイン
        fadein = true;
        fadeout = false;

        //次のシーン名 titleを入れておく
        nextSceneName = "title";

        //音量は0
        audioSource.volume = 0;
        //音量のフェードの速度    最大音量をα値のフェードにかかる時間で割る
        fadeSeconds = HoldVariable.BGMVolume / (alpha / fadeSpeed);
        Debug.Log(fadeSeconds);

        isSceneLoading = false;

        //今のシーン名が title または selectScene ではないとき処理をする
        if (NowGameScene() != "title" && NowGameScene() != "selectScene")
        {
            //ミニマップを作成
            GameObject mapPrefab = (GameObject)Resources.Load("minMap");
            this.map = Instantiate(mapPrefab);

            //ボタンを作成
            GameObject buttonPrefab = (GameObject)Resources.Load("ButtonCanvas");
            this.button = Instantiate(buttonPrefab);

            //ステータスを作成
            GameObject startasPrefab = (GameObject)Resources.Load("PlayerStatasCanvas");
            this.startas = Instantiate(startasPrefab);
        }

        Canvas(false);
    }

    /// <summary>
    /// UIの表示非表示を変更する
    /// </summary>
    /// <param name="active">true:表示 false:非表示</param>
    private void Canvas(bool active)
    {
        //今のシーン名が title または selectScene の時はUIを生成していないので処理を終わる
        if (NowGameScene() == "title" || NowGameScene() == "selectScene")
        {
            return;
        }
        
        map.SetActive(active);
        button.SetActive(active);
        startas.SetActive(active);
    }

    private void FixedUpdate()
    {
        //フェードイン
        if (fadein)
        {
            FadeIn();
        }
        //フェードアウト
        else if (fadeout)
        {
            FadeOut();
        }
        //ウィンドウモードを変更する
        else if(Input.GetKeyDown(KeyCode.F4))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    /// <summary>
    /// 非同期読み込みをする
    /// </summary>
    public void StartLoad()
    {
        StartCoroutine(Load());
    }

    /// <summary>
    /// ロード画像を表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator Load()
    {
        //ロード画面を表示する
        loadingUI.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        //シーンをロードしていないとき
        if (!isSceneLoading)
        {
            isSceneLoading = true;
            //シーンを非同期でロードする
            async = SceneManager.LoadSceneAsync(nextSceneName);
            Debug.Log(nextSceneName + "シーンをロード");
        }

        //ロードが完了するまで待機する
        while (!async.isDone)
        {
            yield return null;
        }

        //ロード画面を非表示にする
        loadingUI.SetActive(false);
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    private void FadeIn()
    {
        //透明度を変更
        alpha -= fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        //パネルの表示非表示を変更
        ChangePanelEnabled();

        //音のフェード(大きくする)　現在のボリュームが最大音量よりも小さいとき
        if (audioSource.volume < HoldVariable.BGMVolume)
        {
            audioSource.volume += fadeSeconds;
        }

        if (alpha <= 0)
        {
            //音量を最大音量にする
            audioSource.volume = HoldVariable.BGMVolume;
            Debug.Log($"音量：{audioSource.volume}");
            //フェードインを終わる
            fadein = false;
            //UIを非表示にする
            Canvas(true);
            return;
        }
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    private void FadeOut()
    {
        //透明度を変更
        alpha += fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        //パネルの表示非表示を変更
        ChangePanelEnabled();

        //音を小さく
        if (audioSource.volume > 0.0)
        {
            audioSource.volume -= fadeSeconds;
        }

        if (alpha >= 1)
        {
            //音量を0にする
            audioSource.volume = 0;
            
            //フェードアウトを終わる
            fadeout = false;

            //接続を断つ
            offlineMode.Disconnect();

            //UIを非表示にする
            Canvas(false);

            //シーンを変更する
            StartLoad();
            return;
        }
    }

    /// <summary>
    /// オンラインでシーンを変更するときに使用
    /// </summary>
    /// <returns>true:フェードアウトが終わった false:フェードアウトが終わってない</returns>
    public bool OnlineFadeOut()
    {
        //透明度を変更
        alpha += fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        ChangePanelEnabled();

        //音を小さく
        if (audioSource.volume > 0.0)
        {
            audioSource.volume -= fadeSeconds;
        }
        if (alpha >= 1)
        {
            //音量を0にする
            audioSource.volume = 0;
            
            //フェードアウトを終わる
            fadeout = false;
           
            //UIを非表示にする
            Canvas(false);

            return true;
        }
        return false;
    }

    /// <summary>
    /// ゲームを終わるときに使用する
    /// </summary>
    /// <returns>true 終わっても大丈夫: fasle 終わったらダメ</returns>
    public bool GameEnd()
    {
        //α値変更
        alpha += fadeSpeed;

        var color = fadealpha.color;
        color.a = alpha;
        fadealpha.color = color;

        ChangePanelEnabled();

        //音を小さく
        if (audioSource.volume > 0.0)
        {
            audioSource.volume -= fadeSeconds;
        }

        if (alpha >= 1)
        {
            //フェードアウトを終わる
            fadeout = false;

            //接続を断つ
            offlineMode.Disconnect();

            return true;
        }
        return false;
    }

    /// <summary>
    /// フェード用パネルの表示非表示の変更
    /// </summary>
    private void ChangePanelEnabled()
    {
        //α値が0以下のときは非表示
        if (alpha <= 0)
        {
            //表示している時は非表示に変える
            if(panelfade.activeSelf)
            {
                panelfade.SetActive(false);
            }
        }
        //α値が0よりも大きいときは表示
        else
        {
            //非表示にしている時は表示に変える
            if(!panelfade.activeSelf)
            {
                panelfade.SetActive(true);
            }
        }
    }

    /// <summary>
    /// インプットシステムを取得する
    /// </summary>
    /// <returns>PlayerInput</returns>
    public PlayerInput GetPlayerInput()
    {
        return input;
    }

    /// <summary>
    /// シーンを変更する
    /// </summary>
    /// <param name="name">変更したいシーンの名前</param>
    public void ChangeScene(string name)
    {
        //フェードアウトしている時はシーン名を変更せず終了する
        if (fadeout) return;

        Debug.Log(name +"に変更");

        //シーン名を変更
        this.nextSceneName = name;

        //フェードアウトを開始する
        fadeout = true;
        //透明度を0にする
        alpha = 0;
    }

    /// <summary>
    /// シーンを変更しているかどうか
    /// </summary>
    /// <returns>true 変更している: false 変更していない</returns>
    public bool IsChangeScene()
    {
        //フェードアウトを始めたら次のシーンに変更するからfadeoutを返す
        return fadeout;
    }

    /// <summary>
    /// 現在のゲームシーンの名前
    /// </summary>
    /// <returns>ゲームシーン名</returns>
    public string NowGameScene()
    {
        return SceneManager.GetActiveScene().name;
    }

}
