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
    private enum inputAction
    {
        run,
        fire,
        back,
        jump,
        block,
        sit,
        pause,
        map,

        max
    }
    /// <summary>
    /// �A�N�V����
    /// </summary>
    private InputAction[] action = new InputAction[(int)inputAction.max];

    /// <summary>
    /// �{�^��
    /// </summary>
    [SerializeField] private ChangeButtonSprite[] sprite = new ChangeButtonSprite[(int)inputAction.max];

    private void Start()
    {
        //�쐬����
        //buttonDisplay = Instantiate(buttonPrefab);
        //MainManager���擾����
        manager = GameObject.Find("Manager").GetComponent<MainManager>();
        //�A�N�V�����}�b�v����A�N�V�������擾����
        action[(int)inputAction.run] = manager.GetPlayerInput().actions["run"];
        action[(int)inputAction.fire] = manager.GetPlayerInput().actions["fire"];
        action[(int)inputAction.back] = manager.GetPlayerInput().actions["back"];
        action[(int)inputAction.jump] = manager.GetPlayerInput().actions["jump"];
        action[(int)inputAction.block] = manager.GetPlayerInput().actions["block"];
        action[(int)inputAction.sit] = manager.GetPlayerInput().actions["sit"];
        action[(int)inputAction.pause] = manager.GetPlayerInput().actions["pause"];
        action[(int)inputAction.map] = manager.GetPlayerInput().actions["map"];
    }

    private void Update()
    {
        //�V�[����ύX���Ă��鎞�͏��������Ȃ�
        if (manager.IsChangeScene())
        {
            return;
        }

        for (int i = 0; i < (int)inputAction.max; i++)
        {
            Pressed(action[i], sprite[i]);
        }
    }

    /// <summary>
    /// ���������Ă��邩
    /// </summary>
    /// <param name="action">�A�N�V����</param>
    /// <param name="gameObject">������Ă��鎞�ɍ�������</param>
    private void Pressed(InputAction action, ChangeButtonSprite gameObject)
    {
        gameObject.ChangingSprite(action.IsPressed());
    }
}
