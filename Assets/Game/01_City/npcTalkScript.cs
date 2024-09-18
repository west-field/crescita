using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class npcTalkScript : MonoBehaviour
{
    [SerializeField] private MainManager manager;
    /// <summary>
    /// �I���ł��邩�ǂ���,�I���������ǂ���
    /// </summary>
    private bool isSelect, isChoice;//�I���ł��邩�ǂ���.�I��������
    private InputAction decision, Cancel;//�A�N�V�����}�b�v����A�N�V�����̎擾

    /// <summary>
    /// �\����\���ɂ���I�u�W�F�N�g
    /// </summary>
    [SerializeField] private GameObject textObj;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    /// <summary>
    /// �\��������������
    /// </summary>
    [SerializeField] private string text;

    /// <summary>
    /// ����炷
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// �b���������Ƃ��̉�
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
            Debug.Log("������");
            if (decision.WasPressedThisFrame())
            {
                Time.timeScale = 0f;
                Debug.Log("�\��");
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
                Debug.Log("��\��");
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
            Debug.Log("NPC OnTriggerEnter,�v���C���[�Ɠ�������");
            textMeshPro.text = text;
            isSelect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("NPC,�v���C���[�����ꂽ");
            textObj.SetActive(false);
            isSelect = false;
            isChoice = false;
        }
    }

}
