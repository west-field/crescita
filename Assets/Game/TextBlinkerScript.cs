using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// テキストを点滅させる
/// </summary>
public class TextBlinkerScript : MonoBehaviour
{
    private float speed;
    private TextMeshProUGUI textMeshProUGUI;
    private float time;

    private void Awake()
    {
        speed = 1.5f;
        textMeshProUGUI = this.gameObject.GetComponent<TextMeshProUGUI>();
        time = 0;
    }

    private void Update()
    {
        textMeshProUGUI.color = GetAlphaColor(textMeshProUGUI.color);
    }

    private Color GetAlphaColor(Color color)
    {
        time += Time.fixedDeltaTime * speed;
        color.a = Mathf.Sin(time);
        return color;
    }

    private void OnDisable()
    {
        //初期化
        Color color = textMeshProUGUI.color;
        color.a = 1;
        textMeshProUGUI.color = color;
        time = 0;
    }

}
