using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拡大縮小させる
/// </summary>
public class Scaling : MonoBehaviour
{
    /*拡大縮小*/
    /// <summary>
    /// 拡大縮小スピード
    /// </summary>
    private float changeSpeed = 0.0f;
    /// <summary>
    /// 拡大するかどうか
    /// </summary>
    private bool isEnlarge = true;

    private float maxScale = 0.9f, minScale = 0.7f;
    public void Init(float max,float min)
    {
        this.maxScale = max;
        this.minScale = min;
    }

    /// <summary>
    /// 拡大させるオブジェクトの位置を変更させる
    /// </summary>
    /// <param name="scalingTransform">拡大させるオブジェクト</param>
    /// <param name="position">変更位置</param>
    public void ScalingObjPosition(Transform scalingTransform, Vector3 position)
    {
        //今選んでいるオブジェクトの位置とフレームの位置が違うとき
        if (scalingTransform.position != position)
        {
            //位置を合わせる
            scalingTransform.position = position;
            isEnlarge = true;
            //拡大率を初期化
            scalingTransform.localScale = new Vector3(minScale, minScale, minScale);
        }
    }

    /// <summary>
    /// 拡大縮小させる
    /// </summary>
    /// <param name="scalingTransform">拡大縮小させたいオブジェクト</param>
    public void ScalingObj(Transform scalingTransform)
    {
        //フレームを拡大縮小させる
        changeSpeed = Time.unscaledDeltaTime * 0.2f;
        //拡大
        if (isEnlarge)
        {
            scalingTransform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed);
            //フレームの拡大率が0.8以上になった時小さくさせる
            if (scalingTransform.localScale.x >= maxScale)
            {
                isEnlarge = false;
            }
        }
        //縮小
        else
        {
            scalingTransform.localScale -= new Vector3(changeSpeed, changeSpeed, changeSpeed);
            //フレームの拡大率が0.65以下になったら大きくさせる
            if (scalingTransform.localScale.x <= minScale)
            {
                isEnlarge = true;
            }
        }
    }
}
