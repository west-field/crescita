using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// タイトルマネージャー
/// (名前とパスワードを入力後、次のシーンへ)
/// </summary>
public class titleManager : MonoBehaviour
{
    [SerializeField] private MainManager manager;//メインマネージャー
    private InputAction move,submit, cancel;//アクションマップからアクションの取得

    private bool isEnterName;//ログイン画面を表示させるかどうか
    private bool isCreate;//アカウントを作成するかどうか
    private bool isSelect;//押したかどうか

    [SerializeField] private new GameObject name;

    /*名前、パスワード*/
    [SerializeField] private TMP_InputField enterName, enterPass;//名前の入力,パスワードの入力
    /// <summary>
    /// 名前の入力を選択しているか
    /// </summary>
    private bool isName;
    private bool isLogin;//ログインできるかどうか
    [SerializeField] private rogin roginScript;//通信
    [SerializeField] private TextMeshProUGUI nextText;//InputFieldの下にあるテキスト
    
    /*音*/
    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip directionSound, noGoodSound;//押したときの音,ダメな時の音

    /// <summary>
    /// フレーム
    /// </summary>
    [SerializeField] private GameObject selectFrame;
    /// <summary>
    /// フレームの拡大縮小
    /// </summary>
    [SerializeField] private Scaling scalingSprit;
    /// <summary>
    /// フレームの位置を取得
    /// </summary>
    [SerializeField] private Transform createPos, roginPos;
    /// <summary>
    /// フレームの位置を決める
    /// </summary>
    [SerializeField] private bool isCreateButton;

    void Start()
    {
        move = manager.GetPlayerInput().actions["move"];
        submit = manager.GetPlayerInput().actions["fire"];
        cancel = manager.GetPlayerInput().actions["back"];

        audioSource.volume = HoldVariable.SEVolume;

        scalingSprit.Init(0.9f, 0.7f);

        isCreateButton = false;

        Init();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        enterName.Select();
        isName = false;

        //enterName.text = "crescita";
        //enterPass.text = "12345";
        name.gameObject.SetActive(false);

        isEnterName = false;

        isCreate = false;

        isLogin = false;

        isSelect = false;

        nextText.enabled = false;
        nextText.text = "次へ　Aボタン or enter";
    }

    private void Update()
    {
        if (manager.IsChangeScene()) return;

        scalingSprit.ScalingObj(selectFrame.transform);//拡大縮小

        if (isEnterName)
        {
            NameAndPassword();
            return;
        }

        //選択しているほうにフレームを移動
        if (move.WasPressedThisFrame())
        {
            isCreateButton = !isCreateButton;

            if (isCreateButton)
            {
                scalingSprit.ScalingObjPosition(selectFrame.transform, createPos.position);
            }
            else
            {
                scalingSprit.ScalingObjPosition(selectFrame.transform, roginPos.position);
            }
        }

        //決定を押したとき
        if (submit.WasPressedThisFrame())
        {
            if (isCreateButton)
            {
                CreateButtomClick();
            }
            else
            {
                LoginButtomClick();
            }
        }
    }

    /// <summary>
    /// 名前とパスワードの入力
    /// </summary>
    private void NameAndPassword()
    {
        //戻るを押したとき元の画面に戻る
        if(cancel.WasPressedThisFrame())
        {
            Init();
            return;
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            //名前を入力するフォームを選択していた場合はパスワード入力に変更する
            if(isName)
            {
                enterPass.Select();
                isName = false;
            }
            //パスワードを入力するフォームを選択していた場合は名前を入力するフォームに変更
            else
            {
                enterName.Select();
                isName = true;
            }
        }

        //名前は16文字まで入力できる
        if (enterName.text.Length > 16)
        {
            enterName.text = enterName.text[..16];
            //音を鳴らす
            audioSource.PlayOneShot(noGoodSound);
        }
        //パスワードは10文字まで入力できる
        if (enterPass.text.Length > 10)
        {
            enterPass.text = enterPass.text[..10];
            //音を鳴らす
            audioSource.PlayOneShot(noGoodSound);
        }

        //入力された文字数が2以上の時 && パスワードの数が4以上の時
        if (enterName.text.Length >= 2 && enterPass.text.Length >= 4)
        {
            nextText.enabled = true;//次への文字を表示

            //選択していない　&& ログインしていない　&& 決定を押している
            if (!isSelect && !isLogin && submit.WasPressedThisFrame())
            {
                isSelect = true;

                //作成しないとき
                if (!isCreate)
                { 
                    isLogin = roginScript.Login(enterName.text, enterPass.text);
                    Debug.Log("ログイン"+isLogin);
                }
                else if (isCreate)
                {
                    isLogin = roginScript.Create(enterName.text, enterPass.text);
                    Debug.Log("作成"+isLogin);
                }
            }
            //ログインできたとき
            else if (isLogin)
            {
                enterName.interactable = false;//文字入力ができないように
                enterPass.interactable = false;//文字入力ができないように

                //入力した名前をプレイヤーの名前にする
                HoldVariable.playerName = enterName.text;

                //cityにシーン変更
                manager.ChangeScene("city");
            }

            //ログイン出来てない && 次へのテキストが　(ログイン成功 または 作成)に変わっていた時
            if (!isLogin && (nextText.text == "ログイン成功" || nextText.text == "作成"))
            {
                //ログイン成功に
                isLogin = true;
                //音を鳴らす
                audioSource.PlayOneShot(directionSound);
            }
        }
        else
        {
            nextText.text = "次へ　Aボタン or enter";
            nextText.enabled = false;
            isSelect = false;
        }
    }

    /// <summary>
    /// アカウントを作成する
    /// </summary>
    public void CreateButtomClick()
    {
        //音を鳴らす
        audioSource.PlayOneShot(directionSound);

        isCreate = true;
        name.gameObject.SetActive(true);
        isEnterName = true;
    }
    /// <summary>
    /// アカウントの名前とパスワードでログインする
    /// </summary>
    public void LoginButtomClick()
    {
        //音を鳴らす
        audioSource.PlayOneShot(directionSound);

        isCreate = false;
        name.gameObject.SetActive(true);
        isEnterName = true;
    }
}
