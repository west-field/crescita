using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージを選択するための画像
/// </summary>
public class moveImage : MonoBehaviour
{
    /// <summary>
    /// 選択されていないときに表示するテキスト
    /// </summary>
    [SerializeField] private GameObject textObj;
    /// <summary>
    /// 表示する絵
    /// </summary>
    [SerializeField] private Image imageFirst, imageSecond;

    /// <summary>
    /// 最大角度
    /// </summary>
    [SerializeField] private int maxAngle = 360;
    /// <summary>
    /// 現在の角度
    /// </summary>
    private float currentAngle;

    /// <summary>
    /// 足すか引くか
    /// </summary>
    private bool isAdd;

    /// <summary>
    /// 待つ時間
    /// </summary>
    [SerializeField] private float waitTime = 60;
    /// <summary>
    /// 経過時間
    /// </summary>
    private float time;

    private void Start()
    {
        currentAngle = maxAngle;
        isAdd = false;
        time = waitTime;
    }

    private void FixedUpdate()
    {
        //テキストが表示されている時は処理しない
        if(textObj.activeSelf)
        {
            if (currentAngle != maxAngle)
            {
                currentAngle = maxAngle;
            }
            return;
        }

        //0より大きいときは時間を減らす
        if(time >= 0)
        {
            time -= Time.deltaTime * 100;
            return;
        }

        //減らすとき
        if (!isAdd)
        {
            //現在の角度を減らす
            currentAngle -= Time.deltaTime * 100;
            if (0 == imageFirst.fillAmount)
            {
                time = waitTime;
                isAdd = true;
                imageFirst.fillClockwise = false;
            }
        }
        else
        {
            currentAngle += Time.deltaTime * 100;
            if (1 == imageFirst.fillAmount)
            {
                time = waitTime;
                isAdd = false;
                imageFirst.fillClockwise = true;
            }
        }

        //ImageコンポーネントのfillAmountを取得して操作する
        imageFirst.fillAmount = currentAngle / maxAngle;
    }

}
