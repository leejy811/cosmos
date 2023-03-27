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
        text.text = damage.ToString();
        textColor = text.color;
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        textColor.a = Mathf.Lerp(textColor.a, 0, Time.deltaTime * colorSpeed);
        text.color = textColor;
        if(textColor.a == 0)
        {
            ActiveOff();
        }
    }
    private void ActiveOff()
    {
        this.gameObject.SetActive(false);
        textColor.a = 1;
    }
}
