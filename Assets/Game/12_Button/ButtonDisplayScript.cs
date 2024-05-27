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
    /// アクション
    /// </summary>
    private InputAction[] action = new InputAction[(int)inputAction.max];

    /// <summary>
    /// ボタン
    /// </summary>
    [SerializeField] private ChangeButtonSprite[] sprite = new ChangeButtonSprite[(int)inputAction.max];

    private void Start()
    {
        //作成する
        //buttonDisplay = Instantiate(buttonPrefab);
        //MainManagerを取得する
        manager = GameObject.Find("Manager").GetComponent<MainManager>();
        //アクションマップからアクションを取得する
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
        //シーンを変更している時は処理をしない
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
    /// 押し続けているか
    /// </summary>
    /// <param name="action">アクション</param>
    /// <param name="gameObject">押されている時に黒くする</param>
    private void Pressed(InputAction action, ChangeButtonSprite gameObject)
    {
        gameObject.ChangingSprite(action.IsPressed());
    }
}
