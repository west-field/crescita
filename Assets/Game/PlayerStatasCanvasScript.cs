using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 画面に表示するステータス
/// </summary>
public class PlayerStatasCanvasScript : MonoBehaviour
{
    /// <summary>
    /// レベル表示テキスト
    /// </summary>
    [SerializeField] private TextMeshProUGUI levelText;
    /// <summary>
    /// レベルアップポイント表示テキスト
    /// </summary>
    [SerializeField] private TextMeshProUGUI levelPointText;

    //比較のため持っておく
    private int level, levelPoint;

    //Hpバーを変更させる
    /// <summary>
    /// HPバー
    /// </summary>
    [SerializeField] private Image hpBars;

    /// <summary>
    /// 現在のhpを取得するため
    /// </summary>
    private PlayerStartas startas;

    void Start()
    {
        levelText.text = "";
        levelPointText.text = "";
        level = 0;
        levelPoint = 0;

        var obje = GameObject.FindGameObjectsWithTag("Player");
        //取得したプレイヤーを見る
        foreach(var ob in obje)
        {
            //ステータスがnullでないとき
            if(ob.GetComponent<PlayerStartas>() != null)
            {
                //取得する
                startas = ob.GetComponent<PlayerStartas>();
                Debug.Log("playerStartas あった");
                break;
            }
            Debug.Log("playerStartas ない");
        }

        ////レベル表示と今のレベルが違う時
        //if (level != HoldVariable.startas.level)
        //{
        //    level = HoldVariable.startas.level;
        //    //今のレベルを表示する
        //    levelText.text = $"{HoldVariable.startas.level}";
        //}

        //if (levelPoint != HoldVariable.startas.levelPoint)
        //{
        //    levelPoint = HoldVariable.startas.levelPoint;
        //    //経験値を計算する
        //    var point = ((HoldVariable.startas.level - 1) * 100 + HoldVariable.startas.level * 100) - HoldVariable.startas.levelPoint;

        //    //レベルアップのためのポイントを表示する
        //    levelPointText.text = $"あと{point}";
        //}
        
        //レベルを表示させる
        level = startas.GetStartas().level;
        //今のレベルを表示する
        levelText.text = $"{startas.GetStartas().level}";

        //経験値を表示させる
        levelPoint = startas.GetStartas().levelPoint;
        //経験値を計算する
        var point = ((startas.GetStartas().level - 1) * 100 + startas.GetStartas().level * 100) - startas.GetStartas().levelPoint;
        //レベルアップのためのポイントを表示する
        levelPointText.text = $"あと{point}";

    }

    private void FixedUpdate()
    {
        if(startas != null)
        {
            //レベル表示と今のレベルが違う時
            if (level != startas.GetStartas().level)
            {
                level = startas.GetStartas().level;
                //今のレベルを表示する
                levelText.text = $"{startas.GetStartas().level}";
            }

            if (levelPoint != startas.GetStartas().levelPoint)
            {
                levelPoint = startas.GetStartas().levelPoint;
                //経験値を計算する
                var point = ((startas.GetStartas().level - 1) * 100 + startas.GetStartas().level * 100) - startas.GetStartas().levelPoint;

                //レベルアップのためのポイントを表示する
                levelPointText.text = $"あと{point}";
            }

            hpBars.fillAmount = (float)startas.GetNowHp() / (float)startas.GetStartas().maxHp;
        }
        else
        {
            //Debug.Log("ない");
            var obj = GameObject.FindGameObjectsWithTag("Player");

            //取得したプレイヤーを見る
            foreach (var ob in obj)
            {
                //ステータスがnullでないとき
                if (ob.GetComponent<PlayerStartas>() != null)
                {
                    //取得する
                    startas = ob.GetComponent<PlayerStartas>();
                    Debug.Log("あった");
                    break;
                }
                Debug.Log("ない");
            }
        }
    }
}
