using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBlinker : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    public float Frequency = 0.25f;
    public float CycleDelay = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(BlinkText());
    }

    public IEnumerator BlinkText() {
        while (true) {
            Text.alpha = 1.0f;
            var time = 1 / Frequency;
            var unitsPerSecond = .850f / time;
            while (Text.alpha > 0.15) {
                Text.alpha -= unitsPerSecond * Time.deltaTime;
                yield return null;
            }
            while (Text.alpha < 1) {
                Text.alpha += unitsPerSecond * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(CycleDelay);
        }

        yield break;
    }

}
