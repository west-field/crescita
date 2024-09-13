using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �{�^���������Ă��邩�ǂ���
/// (�����Ă���Ƃ���������)
/// </summary>
public class ButtonDisplayScript : MonoBehaviour
{
    /// <summary>
    /// ���C���}�l�[�W���[
    /// </summary>
    private MainManager manager;

    /// <summary>
    /// �{�^���̎��
    /// </summary>
    private enum InputAction
    {
        Run,
        Fire,
        Back,
        Jump,
        Block,
        Sit,
        Pause,
        Map,

        Max
    }
    /// <summary>
    /// �A�N�V����
    /// </summary>
    private UnityEngine.InputSystem.InputAction[] action = new UnityEngine.InputSystem.InputAction[(int)InputAction.Max];

    /// <summary>
    /// �{�^��
    /// </summary>
    [SerializeField] private ChangeButtonSprite[] sprite = new ChangeButtonSprite[(int)InputAction.Max];

    private void Start()
    {
        //�쐬����
        //buttonDisplay = Instantiate(buttonPrefab);
        //MainManager���擾����
        manager = GameObject.Find("Manager").GetComponent<MainManager>();
        //�A�N�V�����}�b�v����A�N�V�������擾����
        action[(int)InputAction.Run] = manager.GetPlayerInput().actions["run"];
        action[(int)InputAction.Fire] = manager.GetPlayerInput().actions["fire"];
        action[(int)InputAction.Back] = manager.GetPlayerInput().actions["back"];
        action[(int)InputAction.Jump] = manager.GetPlayerInput().actions["jump"];
        action[(int)InputAction.Block] = manager.GetPlayerInput().actions["block"];
        action[(int)InputAction.Sit] = manager.GetPlayerInput().actions["sit"];
        action[(int)InputAction.Pause] = manager.GetPlayerInput().actions["pause"];
        action[(int)InputAction.Map] = manager.GetPlayerInput().actions["map"];
    }

    private void Update()
    {
        //�V�[����ύX���Ă��鎞�͏��������Ȃ�
        if (manager.IsChangeScene())
        {
            return;
        }

        for (int i = 0; i < (int)InputAction.Max; i++)
        {
            Pressed(action[i], sprite[i]);
        }
    }

    /// <summary>
    /// ���������Ă��邩
    /// </summary>
    /// <param name="action">�A�N�V����</param>
    /// <param name="gameObject">������Ă��鎞�ɍ�������</param>
    private void Pressed(UnityEngine.InputSystem.InputAction action, ChangeButtonSprite gameObject)
    {
        gameObject.ChangingSprite(action.IsPressed());
    }
}
