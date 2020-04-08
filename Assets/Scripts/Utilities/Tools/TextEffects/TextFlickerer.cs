using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFlickerer : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlickerText());
    }

    public IEnumerator FlickerText() {
        while (true) {
            //Flicker
            if (gameObject.activeSelf) {
                Text.alpha = 0.8f;
                yield return new WaitForSeconds(0.05f);
                Text.alpha = 1f;
                yield return new WaitForSeconds(0.05f);
                Text.alpha = .7f;
                yield return new WaitForSeconds(0.05f);
                Text.alpha = 1f;

                //Delay
                yield return new WaitForSeconds(Random.Range(0.5f, 5f));
            }
            yield return null;
        }
    }
}
