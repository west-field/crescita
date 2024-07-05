using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class optionPanelScript : MonoBehaviour
{
    /// <summary>
    /// サウンド表示パネル
    /// </summary>
    [SerializeField] private GameObject soundPanelPrefab;
    /// <summary>
    /// ウィンドウ変更パネル
    /// </summary>
    [SerializeField] private GameObject windowPanelPrefab;
    /// <summary>
    /// 選択している場所にフレームを表示する
    /// </summary>
    [SerializeField] private GameObject selectFrameObject;
    /// <summary>
    /// 作成したパネルプレハブを保持
    /// </summary>
    private GameObject sound,window;

    /// <summary>
    /// サウンドを鳴らす
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// ポーズを開くときの音
    /// </summary>
    [SerializeField] private AudioClip openClosSound;
    /// <summary>
    /// カーソル移動の音
    /// </summary>
    [SerializeField] private AudioClip moveSound;

    /// <summary>
    /// ポーズ画面で選択できる項目
    /// </summary>
    private enum OptionItem
    {
        sound,
        window,
        back,
        end,

        max
    }
    [SerializeField] private TextMeshProUGUI[] text;
    /// <summary>
    /// 選択しているもの
    /// </summary>
    private OptionItem optionItem;

    [SerializeField] private MainManager mainManager;
    private InputAction Navigate, Submit, Cancel, Pause;

    /// <summary>
    /// オプション画面かどうか
    /// </summary>
    private bool isOption;

    /// <summary>
    /// 枠線の拡大縮小
    /// </summary>
    [SerializeField] private Scaling scalingScript;

    private void Start()
    {
        //サウンドの音量を設定した音量に変更する
        audioSource.volume = HoldVariable.SEVolume;
        //表示されたときに音を鳴らす
        audioSource.PlayOneShot(openClosSound);

        //フレームの位置を閉じるの位置にする
        optionItem = OptionItem.back;
        scalingScript.ScalingObjPosition(selectFrameObject.transform, text[(int)optionItem].transform.position);

        //作成する
        sound = Instantiate(soundPanelPrefab, Vector3.zero, Quaternion.identity);
        sound.transform.SetParent(transform, false);
        sound.SetActive(false);
        window = Instantiate(windowPanelPrefab, Vector3.zero, Quaternion.identity);
        window.transform.SetParent(transform, false);
        window.SetActive(false);

        //キー取得
        Navigate = mainManager.GetPlayerInput().actions["move"];
        Submit = mainManager.GetPlayerInput().actions["fire"];
        Cancel = mainManager.GetPlayerInput().actions["back"];
        Pause = mainManager.GetPlayerInput().actions["pause"];

        isOption = true;
    }

    private void Update()
    {
        if(isOption)
        {
            switch (optionItem)
            {
                case OptionItem.sound:
                    SoundDrawUpdate();
                    break;
                case OptionItem.window:
                    WindowDrawUpdate();
                    break;
                case OptionItem.end:
                    GameEndUpdate();
                    break;
                default:
                    break;
            }
        }
        else
        {
            OptionDrawUpdate();
        }
    }
    /// <summary>
    /// ポーズ画面(何も指定していないとき)のアップデート
    /// </summary>
    private void OptionDrawUpdate()
    {
        //選択を変更
        if (Navigate.WasPressedThisFrame())
        {
            var val = Navigate.ReadValue<Vector2>();
            var menuNum = (int)OptionItem.max;
            if (val.y < 0.0f || val.x > 0.0f)
            {
                optionItem = (OptionItem)(((int)optionItem + 1) % menuNum);
            }
            else if (val.x < 0.0f || val.y > 0.0f)
            {
                optionItem = (OptionItem)(((int)optionItem + (menuNum - 1)) % menuNum);
            }

            scalingScript.ScalingObjPosition(selectFrameObject.transform, text[(int)optionItem].transform.position);
        }
        scalingScript.ScalingObj(selectFrameObject.transform);

        //決定を押したとき
        if (Submit.WasPressedThisFrame())
        {
            switch (optionItem)
            {
                case OptionItem.sound:
                    sound.SetActive(true);
                    isOption = true;
                    break;
                case OptionItem.window:
                    window.SetActive(true);
                    isOption = true;
                    break;
                case OptionItem.back:
                    Destroy(sound);
                    Destroy(window);
                    this.gameObject.SetActive(false);
                    break;
                case OptionItem.end:
                    Destroy(sound);
                    Destroy(window);
                    break;
                default:
                    break;
            }
        }
        //キャンセルを押したとき
        else if (Cancel.WasPressedThisFrame())
        {
            //サウンドを鳴らす
            audioSource.PlayOneShot(openClosSound);

            Destroy(sound);
            Destroy(window);
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
            sound.SetActive(false);
            audioSource.PlayOneShot(openClosSound);
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
            window.SetActive(false);
            audioSource.PlayOneShot(openClosSound);
        }
    }

    /// <summary>
    /// ゲーム終了を選んだ時のアップデート
    /// </summary>
    private void GameEndUpdate()
    {
        if (mainManager.GameEnd())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
        }
    }
}
