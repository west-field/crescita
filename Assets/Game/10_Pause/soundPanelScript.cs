using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// サウンドパネル
/// </summary>
public class soundPanelScript : MonoBehaviour
{
    //今再生しているBGMのaudioを取得する、変更しているのが分かるように音を鳴らす用
    private AudioSource BGMaudioSource, SEAudioSource;

    //音量のスライダー
    [SerializeField] private Slider BGMSlider;//BGM
    [SerializeField] private Slider SESlider;//SE

    /*ボタンで音量変更できるようにする*/
    /*キー*/
    private MainManager mainManager;
    private InputAction Navigate;//アクションマップからアクションの取得
    /*音*/
    [SerializeField] private AudioClip moveSound;//カーソル移動音
    /*選択*/
    private bool isBGM;//BGMとSEどちらを選んでいるか
    [SerializeField] private TextMeshProUGUI bgm, se;
    /*増やす*/
    private float add;//どれだけ音量を増やすか

    private void Start()
    {
        GameObject manager = GameObject.Find("Manager");

        //BGMAudioSourceコンポーネントを取得
        BGMaudioSource = manager.GetComponent<AudioSource>();
        BGMaudioSource.volume = HoldVariable.BGMVolume;
        //SEAudioSourceコンポーネントを取得
        SEAudioSource = this.gameObject.GetComponent<AudioSource>();
        SEAudioSource.volume = HoldVariable.SEVolume;
        //スライダーの位置を音量の位置に変更する
        BGMSlider.value = HoldVariable.BGMVolume;
        SESlider.value = HoldVariable.SEVolume;

        Debug.Log($"SE{HoldVariable.SEVolume}::{SEAudioSource.volume}::{SESlider.value}");
        Debug.Log($"BGM{HoldVariable.BGMVolume}::{BGMaudioSource.volume}::{BGMSlider.value}");

        //メインマネージャーからキーを取得する
        mainManager = manager.GetComponent<MainManager>();
        Navigate = mainManager.GetPlayerInput().actions["move"];

        //最初はBGMを選択しておく
        isBGM = true;
        bgm.color = Color.red;
        se.color = Color.black;

        add = 0.0f;
    }

    private void Update()
    {
        ChangeSESoundValume();
        ChangeBGMSoundValume();

        bool isPress = false;

        if (Navigate.WasPressedThisFrame())
        {
            //上下移動　BGMとSEを変更する
            if (Navigate.ReadValue<Vector2>().y > 0.0f || Navigate.ReadValue<Vector2>().y < 0.0f)
            {
                isBGM = !isBGM;
                if (isBGM)
                {
                    bgm.color = Color.red;
                    se.color = Color.black;
                }
                else
                {
                    se.color = Color.red;
                    bgm.color = Color.black;
                }
                //サウンドを鳴らす
                SEAudioSource.PlayOneShot(moveSound);
            }

            //左右移動
            if (Navigate.ReadValue<Vector2>().x > 0.0f || Navigate.ReadValue<Vector2>().x < 0.0f)
            {
                //SEの音量を変更している時にだけSE音を鳴らす
                if (!isBGM)
                {
                    SEAudioSource.PlayOneShot(moveSound);
                }
            }
        }

        if (Navigate.IsPressed())
        {
            Vector2 navigate = Navigate.ReadValue<Vector2>();

            //左右移動
            if (navigate.x > 0.0f || navigate.x < 0.0f)
            {
                isPress = true;
                //最大移動量になっていなかったら
                if(add <= 0.01f && add >= -0.01f)
                {
                    //移動量を増やす
                    add += navigate.x * 0.00005f;
                    Debug.Log(add);
                }
            }
        }

        //左右移動している時
        if (isPress)
        {
            //音量を変更する
            if (!isBGM)
            {
                SESlider.value += add;
                ChangeSESoundValume();
            }
            else
            {
                BGMSlider.value += add;
                ChangeBGMSoundValume();
            }
        }
        else
        {
            //移動量を0に戻す
            if(add != 0.0f)
            {
                add = 0.0f;
            }
        }
    }

    /// <summary>
    /// BGMのボリュームとスライダーのvalueを一致させる
    /// </summary>
    private void ChangeBGMSoundValume()
    {
        //BGM スライダーのvalueとボリュームが一致しないとき
        if (BGMSlider.value != HoldVariable.BGMVolume)
        {
            isBGM = true;
            HoldVariable.BGMVolume = BGMSlider.value;
            BGMaudioSource.volume = BGMSlider.value;
        }
    }
    /// <summary>
    /// SEのボリュームとスライダーのvalueを一致させる
    /// </summary>
    private void ChangeSESoundValume()
    {
        //SE スライダーのvalueとボリュームが一致しないとき
        if (SESlider.value != HoldVariable.SEVolume)
        {
            isBGM = false;
            HoldVariable.SEVolume = SESlider.value;
            SEAudioSource.volume = SESlider.value;
        }
    }
}
