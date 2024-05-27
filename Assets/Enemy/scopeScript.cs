using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 範囲
/// </summary>
public class scopeScript : MonoBehaviour
{
    private bool isScope;//範囲内に何かいるかどうか
    private GameObject target;//ターゲット

    private int scorpeTime;//時間が経過したらいったんスコープをfalseにする

    private SphereCollider scopeCollider;

    private void Start()
    {
        isScope = false;
        scorpeTime = 0;
        scopeCollider = this.GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        //isScope が true の時
        if (isScope)
        {
            scopeCollider.enabled = false;
            //2秒たったら
            if (scorpeTime-- <= 0)
            {
                //いったん初期化
                scorpeTime = 0;
                isScope = false;
                target = null;
                scopeCollider.enabled = true;
            }
        }
    }

    /// <summary>
    /// 範囲内にターゲットがいるかどうか
    /// </summary>
    /// <returns>true いる: false いない</returns>
    public bool IsScope()
    {
        return isScope;
    }

    /// <summary>
    /// ターゲットのポジションを取得する
    /// </summary>
    /// <returns>Vector3</returns>
    public Vector3 TargetPosition()
    {
        if (target == null)
            target = gameObject;
        return target.transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        //プレイヤーと当たった時
        if (other.gameObject.tag == "Player")
        {
            if (target == null)
            { 
                target = other.gameObject;
            }
            scorpeTime = 120;
            isScope = true;
        }
    }
}
