using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * Interface of all elements operable by UIController
 */
[RequireComponent(typeof(Image))]
public abstract class ControllableUIElement : MonoBehaviour
{
    /** Name used to search for the UI element */
    public string ElementName;

    protected Image ElementGraphic;

    protected bool Pulsing;

    /** Unlock UI element */
    public abstract void Enable();

    /** Lock UI element */
    public abstract void Disable();

    /** 
     * Draw attention to control 
     */
    public virtual void StartHighlight() {
        if (!Pulsing) {
            Pulsing = true;
            StartCoroutine(Pulse());
        }
    }

    /** 
     * Stop drawing attention to control
     */
    public virtual void StopHighlight() {
        Pulsing = false;
        StopCoroutine(Pulse());
        ElementGraphic.color = new Color(1, 1, 1, 1);
    }

    protected IEnumerator Pulse() {
        float lerpPoint = 0.0f;
        Color from = Color.white;
        Color to = Color.grey;

        while (Pulsing) {
            //Approach grey color
            while (true){
                ElementGraphic.color = Color.Lerp(ElementGraphic.color, Color.gray, 0.10f);
                
                var colorDifference = Mathf.Abs(
                    ElementGraphic.color.maxColorComponent - Color.gray.maxColorComponent
                );

                if (colorDifference < 0.001f) { 
                    break;
                }

                yield return null;
            }

            yield return null;

            //Approach white color
            while (true) {
                ElementGraphic.color = Color.Lerp(ElementGraphic.color, Color.white, 0.10f);
                
                var colorDifference = Mathf.Abs(
                    ElementGraphic.color.maxColorComponent - Color.white.maxColorComponent
                );

                if (colorDifference < 0.001f) {
                    break;
                }

                yield return null;
            }

            yield return null;
        }
    }
}
