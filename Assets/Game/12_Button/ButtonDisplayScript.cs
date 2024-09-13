using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ボタンを押しているかどうか
/// (押しているとき黒くする)
/// </summary>
public class ButtonDisplayScript : MonoBehaviour
{
    /// <summary>
    /// メインマネージャー
    /// </summary>
    private MainManager manager;

    /// <summary>
    /// ボタンの種類
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
    /// アクション
    /// </summary>
    private UnityEngine.InputSystem.InputAction[] action = new UnityEngine.InputSystem.InputAction[(int)InputAction.Max];

    /// <summary>
    /// ボタン
    /// </summary>
    [SerializeField] private ChangeButtonSprite[] sprite = new ChangeButtonSprite[(int)InputAction.Max];

    private void Start()
    {
        //作成する
        //buttonDisplay = Instantiate(buttonPrefab);
        //MainManagerを取得する
        manager = GameObject.Find("Manager").GetComponent<MainManager>();
        //アクションマップからアクションを取得する
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
        //シーンを変更している時は処理をしない
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
    /// 押し続けているか
    /// </summary>
    /// <param name="action">アクション</param>
    /// <param name="gameObject">押されている時に黒くする</param>
    private void Pressed(UnityEngine.InputSystem.InputAction action, ChangeButtonSprite gameObject)
    {
        gameObject.ChangingSprite(action.IsPressed());
    }
}
