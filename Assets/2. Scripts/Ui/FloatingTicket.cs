using UnityEngine;
using DG.Tweening;

public class FloatingTicket : MonoBehaviour
{
    public float moveAmount=3;
    public float rotateAmount = 3000;
    public float colorSpeed;
    Color textColor;
    SpriteRenderer sprite;


    private void OnEnable()     
    {
        sprite =transform.GetChild(0).GetComponent<SpriteRenderer>();
        textColor = sprite.color;
        Sequence sequence = DOTween.Sequence();
        sequence.
            Append(transform.DOMoveY(moveAmount, 3)).
            Join(transform.DORotate(new Vector3(0,rotateAmount, 0), 3,RotateMode.FastBeyond360).SetEase(Ease.OutQuint)).SetAutoKill(false);
    }

    void Update()
    {
        textColor.a = Mathf.Lerp(textColor.a, 0, Time.deltaTime * colorSpeed);
        sprite.color = textColor;
        if (textColor.a <= 0.1f)
        {
            //DOTween.KillAll();
            textColor.a = 1;
            this.gameObject.SetActive(false);
        }
    }
}
