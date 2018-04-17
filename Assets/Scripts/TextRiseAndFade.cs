using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class TextRiseAndFade : MonoBehaviour
{
    Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(RiseAndFade());
    }

    IEnumerator RiseAndFade()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            text.CrossFadeAlpha(1f - i, 0f, true);
            text.rectTransform.position = new Vector3(text.rectTransform.position.x, text.rectTransform.position.y + i/40f, text.rectTransform.position.z);
            yield return new WaitForSeconds(0.0001f);
        }
    }
}
