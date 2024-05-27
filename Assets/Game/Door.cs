using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// ドア用スクリプト
/// (扉を回転させる)
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] private MainManager manager;
    /// <summary>
    /// 選択できるかどうか,選択したかどうか
    /// </summary>
    private bool isSelect, isChoice;//選択できるかどうか.選択したか
    /// <summary>
    /// 扉の回転
    /// </summary>
    [SerializeField] private float rot;

    /// <summary>
    /// 変更シーン名
    /// </summary>
    [SerializeField] private string sceneName;

    [SerializeField] private AudioSource audioSource;//サウンド
    [SerializeField] private AudioClip doorOpenSound;//鳴らしたい音

    private void Start()
    {
        isChoice = false;

        audioSource.volume = HoldVariable.SEVolume;
    }

    private void FixedUpdate()
    {
        //音量が違うときは合わせる
        if(audioSource.volume != HoldVariable.SEVolume)
        {
            audioSource.volume = HoldVariable.SEVolume;
        }

        if(isSelect && !isChoice)
        {
            isSelect = false;

            audioSource.PlayOneShot(doorOpenSound);

            isChoice = true;
            this.transform.Rotate(new Vector3(0, 0, rot));

            manager.ChangeScene(sceneName);//シーンを変更
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag =="Player")
        {
            //選択可能
            isSelect = true;
        }
    }
}
