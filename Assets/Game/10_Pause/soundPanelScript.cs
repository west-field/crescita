using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �T�E���h�p�l��
/// </summary>
public class soundPanelScript : MonoBehaviour
{
    //���Đ����Ă���BGM��audio���擾����A�ύX���Ă���̂�������悤�ɉ���炷�p
    private AudioSource BGMaudioSource, SEAudioSource;

    //���ʂ̃X���C�_�[
    [SerializeField] private Slider BGMSlider;//BGM
    [SerializeField] private Slider SESlider;//SE

    /*�{�^���ŉ��ʕύX�ł���悤�ɂ���*/
    /*�L�[*/
    private MainManager mainManager;
    private InputAction Navigate;//�A�N�V�����}�b�v����A�N�V�����̎擾
    /*��*/
    [SerializeField] private AudioClip moveSound;//�J�[�\���ړ���
    /*�I��*/
    private bool isBGM;//BGM��SE�ǂ����I��ł��邩
    [SerializeField] private TextMeshProUGUI bgm, se;
    /*���₷*/
    private float add;//�ǂꂾ�����ʂ𑝂₷��

    private void Start()
    {
        GameObject manager = GameObject.Find("Manager");

        //BGMAudioSource�R���|�[�l���g���擾
        BGMaudioSource = manager.GetComponent<AudioSource>();
        BGMaudioSource.volume = HoldVariable.BGMVolume;
        //SEAudioSource�R���|�[�l���g���擾
        SEAudioSource = this.gameObject.GetComponent<AudioSource>();
        SEAudioSource.volume = HoldVariable.SEVolume;
        //�X���C�_�[�̈ʒu�����ʂ̈ʒu�ɕύX����
        BGMSlider.value = HoldVariable.BGMVolume;
        SESlider.value = HoldVariable.SEVolume;

        Debug.Log($"SE{HoldVariable.SEVolume}::{SEAudioSource.volume}::{SESlider.value}");
        Debug.Log($"BGM{HoldVariable.BGMVolume}::{BGMaudioSource.volume}::{BGMSlider.value}");

        //���C���}�l�[�W���[����L�[���擾����
        mainManager = manager.GetComponent<MainManager>();
        Navigate = mainManager.GetPlayerInput().actions["move"];

        //�ŏ���BGM��I�����Ă���
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
            //�㉺�ړ��@BGM��SE��ύX����
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
                //�T�E���h��炷
                SEAudioSource.PlayOneShot(moveSound);
            }

            //���E�ړ�
            if (Navigate.ReadValue<Vector2>().x > 0.0f || Navigate.ReadValue<Vector2>().x < 0.0f)
            {
                //SE�̉��ʂ�ύX���Ă��鎞�ɂ���SE����炷
                if (!isBGM)
                {
                    SEAudioSource.PlayOneShot(moveSound);
                }
            }
        }

        if (Navigate.IsPressed())
        {
            Vector2 navigate = Navigate.ReadValue<Vector2>();

            //���E�ړ�
            if (navigate.x > 0.0f || navigate.x < 0.0f)
            {
                isPress = true;
                //�ő�ړ��ʂɂȂ��Ă��Ȃ�������
                if(add <= 0.01f && add >= -0.01f)
                {
                    //�ړ��ʂ𑝂₷
                    add += navigate.x * 0.00005f;
                    Debug.Log(add);
                }
            }
        }

        //���E�ړ����Ă��鎞
        if (isPress)
        {
            //���ʂ�ύX����
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
            //�ړ��ʂ�0�ɖ߂�
            if(add != 0.0f)
            {
                add = 0.0f;
            }
        }
    }

    /// <summary>
    /// BGM�̃{�����[���ƃX���C�_�[��value����v������
    /// </summary>
    private void ChangeBGMSoundValume()
    {
        //BGM �X���C�_�[��value�ƃ{�����[������v���Ȃ��Ƃ�
        if (BGMSlider.value != HoldVariable.BGMVolume)
        {
            isBGM = true;
            HoldVariable.BGMVolume = BGMSlider.value;
            BGMaudioSource.volume = BGMSlider.value;
        }
    }
    /// <summary>
    /// SE�̃{�����[���ƃX���C�_�[��value����v������
    /// </summary>
    private void ChangeSESoundValume()
    {
        //SE �X���C�_�[��value�ƃ{�����[������v���Ȃ��Ƃ�
        if (SESlider.value != HoldVariable.SEVolume)
        {
            isBGM = false;
            HoldVariable.SEVolume = SESlider.value;
            SEAudioSource.volume = SESlider.value;
        }
    }
}
