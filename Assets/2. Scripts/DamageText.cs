using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float colorSpeed;
    TextMeshPro text;
    Color textColor;
    public float damage;


    void Start()
    {
        this.GetComponent<MeshRenderer>().sortingLayerName = "Player";
        text = GetComponent<TextMeshPro>();
        text.text = damage.ToString("F1");
        textColor = text.color;
    }

    void Update()
    {
        if (text.text != damage.ToString("F1"))
        {
            text.text = damage.ToString("F1");
        }
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        textColor.a = Mathf.Lerp(textColor.a, 0, Time.deltaTime * colorSpeed);
        text.color = textColor;
        if(textColor.a <= 0.1f)
        {
            this.gameObject.SetActive(false);
            textColor.a = 1;
        }
    }
}
