using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェードパネルを最後尾に置く
/// </summary>
public class fadePanel : MonoBehaviour
{
    private void OnEnable()
    {
        // ターゲットを最後尾に配置
        this.transform.SetAsLastSibling();
    }
}
