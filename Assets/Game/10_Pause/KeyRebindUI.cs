using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// キー設定
/// </summary>
public class KeyRebindUI : MonoBehaviour
{
    //リバインド対象のAction
    [SerializeField] private InputActionReference actionRef;

    //現在のBindingのパスを表示するテキスト
    [SerializeField] private TMP_Text padPathText;
    //[SerializeField] private TMP_Text keyPathText;

    //リバインド中のマスク用オブジェクト
    [SerializeField] private GameObject mask;

    // リバインド対象のAction
    //private InputAction actionKey;
    private InputAction actionPad;

    // リバインドの非同期オペレーション
    private InputActionRebindingExtensions.RebindingOperation rebindOperationPad;

    //初期化
    private void Awake()
    {
        //リバインド対象のActionが設定されていないときは処理しない
        if (actionRef == null) return;
        //InputActionインスタンスを保持しておく
        //actionKey = actionRef.action;
        actionPad = actionRef.action;
        //キーバインドの表示を反映する
        //RefreshDisplayKey();
        RefreshDisplayPad();
    }
    //後処理
    private void OnDestroy()
    {
        //オペレーションは必ず破棄する必要がある
        CleanUpOperationPad();
    }

    public void StartRebindingGamepad()
    {
        //もしactionが設定されていなければ何もしない
        if (actionPad == null) return;

        //もしリバインド中なら、強制的にキャンセル
        //Cancelメソッドを実行するとOnCancelイベントが発火する
        rebindOperationPad?.Cancel();

        //リバインド前にActionを無効かする必要がある
        actionPad.Disable();

        //リバインド対象のBindingIndexを取得
        var bindingIndex = actionPad.GetBindingIndex(InputBinding.MaskByGroup("Gamepad"));

        //ブロッキング用のマスクを表示
        if (mask != null)
        {
            mask.SetActive(true);
        }

        //リバインドが終了したときの処理を行うローカル関数
        void OnFinished()
        {
            //オペレーションの後処理
            CleanUpOperationPad();
            //一時的に無効化したActionを有効化する
            actionPad.Enable();
            //ブロッキング用のマスクを表示
            if (mask != null)
            {
                mask.SetActive(false);
            }
        }

        //リバインドのオペレーションを作成し、各種コールバックの設定を実施し、開始する
        rebindOperationPad =
            actionPad.PerformInteractiveRebinding(bindingIndex).OnComplete(_ => { //リバインドが完了したときの処理
                RefreshDisplayPad();
                OnFinished();
            }).OnCancel(_ => { //リバインドがキャンセルされたときの処理
                OnFinished();
            }).Start();//ここでリバインドを開始する
    }


    // 上書きされた情報をリセットする
    public void ResetOverrides()
    {
        // Bindingの上書きを全て解除する
        //actionKey?.RemoveAllBindingOverrides();
        actionPad?.RemoveAllBindingOverrides();
        RefreshDisplayPad();
        //RefreshDisplayKey();
    }

    //現在のキーバインド表示を変更
    public void RefreshDisplayPad()
    {
        //アクションがないとき||現在のBindingのパスを表示するテキストがないときは処理をしない
        if (actionPad == null || padPathText == null) return;
        padPathText.text = actionPad.GetBindingDisplayString();
    }

    private void CleanUpOperationPad()
    {
        rebindOperationPad?.Dispose();
        rebindOperationPad = null;
    }
}
