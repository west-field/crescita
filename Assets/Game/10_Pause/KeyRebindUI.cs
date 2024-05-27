using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// �L�[�ݒ�
/// </summary>
public class KeyRebindUI : MonoBehaviour
{
    //���o�C���h�Ώۂ�Action
    [SerializeField] private InputActionReference actionRef;

    //���݂�Binding�̃p�X��\������e�L�X�g
    [SerializeField] private TMP_Text padPathText;
    //[SerializeField] private TMP_Text keyPathText;

    //���o�C���h���̃}�X�N�p�I�u�W�F�N�g
    [SerializeField] private GameObject mask;

    // ���o�C���h�Ώۂ�Action
    //private InputAction actionKey;
    private InputAction actionPad;

    // ���o�C���h�̔񓯊��I�y���[�V����
    private InputActionRebindingExtensions.RebindingOperation rebindOperationPad;

    //������
    private void Awake()
    {
        //���o�C���h�Ώۂ�Action���ݒ肳��Ă��Ȃ��Ƃ��͏������Ȃ�
        if (actionRef == null) return;
        //InputAction�C���X�^���X��ێ����Ă���
        //actionKey = actionRef.action;
        actionPad = actionRef.action;
        //�L�[�o�C���h�̕\���𔽉f����
        //RefreshDisplayKey();
        RefreshDisplayPad();
    }
    //�㏈��
    private void OnDestroy()
    {
        //�I�y���[�V�����͕K���j������K�v������
        CleanUpOperationPad();
    }

    public void StartRebindingGamepad()
    {
        //����action���ݒ肳��Ă��Ȃ���Ή������Ȃ�
        if (actionPad == null) return;

        //�������o�C���h���Ȃ�A�����I�ɃL�����Z��
        //Cancel���\�b�h�����s�����OnCancel�C�x���g�����΂���
        rebindOperationPad?.Cancel();

        //���o�C���h�O��Action�𖳌�������K�v������
        actionPad.Disable();

        //���o�C���h�Ώۂ�BindingIndex���擾
        var bindingIndex = actionPad.GetBindingIndex(InputBinding.MaskByGroup("Gamepad"));

        //�u���b�L���O�p�̃}�X�N��\��
        if (mask != null)
        {
            mask.SetActive(true);
        }

        //���o�C���h���I�������Ƃ��̏������s�����[�J���֐�
        void OnFinished()
        {
            //�I�y���[�V�����̌㏈��
            CleanUpOperationPad();
            //�ꎞ�I�ɖ���������Action��L��������
            actionPad.Enable();
            //�u���b�L���O�p�̃}�X�N��\��
            if (mask != null)
            {
                mask.SetActive(false);
            }
        }

        //���o�C���h�̃I�y���[�V�������쐬���A�e��R�[���o�b�N�̐ݒ�����{���A�J�n����
        rebindOperationPad =
            actionPad.PerformInteractiveRebinding(bindingIndex).OnComplete(_ => { //���o�C���h�����������Ƃ��̏���
                RefreshDisplayPad();
                OnFinished();
            }).OnCancel(_ => { //���o�C���h���L�����Z�����ꂽ�Ƃ��̏���
                OnFinished();
            }).Start();//�����Ń��o�C���h���J�n����
    }


    // �㏑�����ꂽ�������Z�b�g����
    public void ResetOverrides()
    {
        // Binding�̏㏑����S�ĉ�������
        //actionKey?.RemoveAllBindingOverrides();
        actionPad?.RemoveAllBindingOverrides();
        RefreshDisplayPad();
        //RefreshDisplayKey();
    }

    //���݂̃L�[�o�C���h�\����ύX
    public void RefreshDisplayPad()
    {
        //�A�N�V�������Ȃ��Ƃ�||���݂�Binding�̃p�X��\������e�L�X�g���Ȃ��Ƃ��͏��������Ȃ�
        if (actionPad == null || padPathText == null) return;
        padPathText.text = actionPad.GetBindingDisplayString();
    }

    private void CleanUpOperationPad()
    {
        rebindOperationPad?.Dispose();
        rebindOperationPad = null;
    }
}
