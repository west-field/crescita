using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �h�A�p�X�N���v�g
/// (������]������)
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] private MainManager manager;
    /// <summary>
    /// �I���ł��邩�ǂ���,�I���������ǂ���
    /// </summary>
    private bool isSelect, isChoice;//�I���ł��邩�ǂ���.�I��������
    /// <summary>
    /// ���̉�]
    /// </summary>
    [SerializeField] private float rot;

    /// <summary>
    /// �ύX�V�[����
    /// </summary>
    [SerializeField] private string sceneName;

    [SerializeField] private AudioSource audioSource;//�T�E���h
    [SerializeField] private AudioClip doorOpenSound;//�炵������

    private void Start()
    {
        isChoice = false;

        audioSource.volume = HoldVariable.SEVolume;
    }

    private void FixedUpdate()
    {
        //���ʂ��Ⴄ�Ƃ��͍��킹��
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

            manager.ChangeScene(sceneName);//�V�[����ύX
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag =="Player")
        {
            //�I���\
            isSelect = true;
        }
    }
}
