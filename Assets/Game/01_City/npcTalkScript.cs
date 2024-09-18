using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class npcTalkScript : MonoBehaviour
{
    [SerializeField] private MainManager manager;
    /// <summary>
    /// 選択できるかどうか,選択したかどうか
    /// </summary>
    private bool isSelect, isChoice;//選択できるかどうか.選択したか
    private InputAction decision, Cancel;//アクションマップからアクションの取得

    /// <summary>
    /// 表示非表示にするオブジェクト
    /// </summary>
    [SerializeField] private GameObject textObj;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    /// <summary>
    /// 表示したい説明文
    /// </summary>
    [SerializeField] private string text;

    /// <summary>
    /// 音を鳴らす
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// 話しかけたときの音
    /// </summary>
    [SerializeField] private AudioClip talk;

    void Start()
    {
        decision = manager.GetPlayerInput().actions["fire"];
        Cancel = manager.GetPlayerInput().actions["back"];

        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.volume = HoldVariable.SEVolume;

        isSelect = false;
        isChoice = false;

        textObj.SetActive(false);
    }

    private void Update()
    {
        if (isSelect && !isChoice)
        {
            Debug.Log("押せる");
            if (decision.WasPressedThisFrame())
            {
                Time.timeScale = 0f;
                Debug.Log("表示");
                audioSource.PlayOneShot(talk);
                isChoice = true;
                textObj.SetActive(true);
                return;
            }
        }
        else if (isChoice)
        {
            if (decision.WasPressedThisFrame() || Cancel.WasPressedThisFrame())
            {
                Time.timeScale = 1f;
                Debug.Log("非表示");
                audioSource.PlayOneShot(talk);
                isSelect = false;
                isChoice = false;
                textObj.SetActive(false);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("NPC OnTriggerEnter,プレイヤーと当たった");
            textMeshPro.text = text;
            isSelect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("NPC,プレイヤーが離れた");
            textObj.SetActive(false);
            isSelect = false;
            isChoice = false;
        }
    }

}
