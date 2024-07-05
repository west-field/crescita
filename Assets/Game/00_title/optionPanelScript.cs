using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class optionPanelScript : MonoBehaviour
{
    /// <summary>
    /// �T�E���h�\���p�l��
    /// </summary>
    [SerializeField] private GameObject soundPanelPrefab;
    /// <summary>
    /// �E�B���h�E�ύX�p�l��
    /// </summary>
    [SerializeField] private GameObject windowPanelPrefab;
    /// <summary>
    /// �I�����Ă���ꏊ�Ƀt���[����\������
    /// </summary>
    [SerializeField] private GameObject selectFrameObject;
    /// <summary>
    /// �쐬�����p�l���v���n�u��ێ�
    /// </summary>
    private GameObject sound,window;

    /// <summary>
    /// �T�E���h��炷
    /// </summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>
    /// �|�[�Y���J���Ƃ��̉�
    /// </summary>
    [SerializeField] private AudioClip openClosSound;
    /// <summary>
    /// �J�[�\���ړ��̉�
    /// </summary>
    [SerializeField] private AudioClip moveSound;

    /// <summary>
    /// �|�[�Y��ʂőI���ł��鍀��
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
    /// �I�����Ă������
    /// </summary>
    private OptionItem optionItem;

    [SerializeField] private MainManager mainManager;
    private InputAction Navigate, Submit, Cancel, Pause;

    /// <summary>
    /// �I�v�V������ʂ��ǂ���
    /// </summary>
    private bool isOption;

    /// <summary>
    /// �g���̊g��k��
    /// </summary>
    [SerializeField] private Scaling scalingScript;

    private void Start()
    {
        //�T�E���h�̉��ʂ�ݒ肵�����ʂɕύX����
        audioSource.volume = HoldVariable.SEVolume;
        //�\�����ꂽ�Ƃ��ɉ���炷
        audioSource.PlayOneShot(openClosSound);

        //�t���[���̈ʒu�����̈ʒu�ɂ���
        optionItem = OptionItem.back;
        scalingScript.ScalingObjPosition(selectFrameObject.transform, text[(int)optionItem].transform.position);

        //�쐬����
        sound = Instantiate(soundPanelPrefab, Vector3.zero, Quaternion.identity);
        sound.transform.SetParent(transform, false);
        sound.SetActive(false);
        window = Instantiate(windowPanelPrefab, Vector3.zero, Quaternion.identity);
        window.transform.SetParent(transform, false);
        window.SetActive(false);

        //�L�[�擾
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
    /// �|�[�Y���(�����w�肵�Ă��Ȃ��Ƃ�)�̃A�b�v�f�[�g
    /// </summary>
    private void OptionDrawUpdate()
    {
        //�I����ύX
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

        //������������Ƃ�
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
        //�L�����Z�����������Ƃ�
        else if (Cancel.WasPressedThisFrame())
        {
            //�T�E���h��炷
            audioSource.PlayOneShot(openClosSound);

            Destroy(sound);
            Destroy(window);
        }
    }

    /// <summary>
    /// �T�E���h�p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void SoundDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame())
        {
            sound.SetActive(false);
            audioSource.PlayOneShot(openClosSound);
        }
    }
    /// <summary>
    /// �E�B���h�E���[�h�ύX�p�l���\�����A�b�v�f�[�g
    /// </summary>
    private void WindowDrawUpdate()
    {
        //�L�����Z������������|�[�Y��ʂɖ߂�    �E�B���h�E�ύX�p�l���������Ȃ��Ƃ��|�[�Y��ʂɖ߂�
        if (Cancel.WasPressedThisFrame() || Pause.WasPressedThisFrame() || window.activeSelf == false)
        {
            window.SetActive(false);
            audioSource.PlayOneShot(openClosSound);
        }
    }

    /// <summary>
    /// �Q�[���I����I�񂾎��̃A�b�v�f�[�g
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
