using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    Vector3 originPos;
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        originPos = transform.localPosition;

        Rect rect = camera.rect;
        float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 19);
        float scaleWidth = 1f / scaleHeight;
        if(scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }
        camera.rect = rect;
    }

    public void Shake()
    {
        StartCoroutine("CameraShake");
    }

    private IEnumerator CameraShake()
    {
        float timer = 0;
        while(timer <= 0.5)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * 0.3f + originPos;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originPos;
    }
}
