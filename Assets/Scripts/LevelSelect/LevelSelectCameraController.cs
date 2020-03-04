using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    /** Sensitivity of Camera drag controls */
    public float MouseSensitivity = 1.0f;
    
    /** Time limit for CameraPanTo opetations */
    public float CameraPanToTime = 0.25f;

    /** Used to override CameraPanToTime if the target object is reachable faster than 0.25 at base speed */
    public float BaseCameraSpeed = 35.0f;

    [Header("Configuration Settings")]
    /** Desired offset of camera from parent object */ 
    public Vector3 BoomOffset = new Vector3(0, 0, -25.0f);

    private Vector3 lastPosition;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update() {
 
        if (Input.GetMouseButtonDown(0)) {
            lastPosition = Input.mousePosition;
            StartCoroutine(DragRoutine());
        }

    }

    public IEnumerator PanToTarget(GameObject obj) {
        const float MIN_DIST_SQR = 0.01f;

        var target = obj.transform.position + BoomOffset;

        var baseTime = (transform.position - target).magnitude / BaseCameraSpeed;

        Vector3 cameraVelocity = Vector3.zero;

        while (Vector3.SqrMagnitude(transform.position - target) > MIN_DIST_SQR) {
            Debug.Log("MoveTowards");

            transform.position = Vector3.SmoothDamp(
                transform.position,
                target,
                ref cameraVelocity,
                (baseTime < CameraPanToTime) ? baseTime : CameraPanToTime
            );

            yield return null;
        }

        yield break;
    }

    public void SnapTo(GameObject obj) {
        var pos = obj.transform.position + BoomOffset;
        transform.position += pos;
    }

    IEnumerator DragRoutine() {
        while (Input.GetMouseButton(0)) {
            var delta = (lastPosition - Input.mousePosition) * MouseSensitivity * Time.deltaTime;
            delta.z = 0;
            transform.Translate(delta);
            lastPosition = Input.mousePosition;
            yield return null;
        }

        yield break;
    }

}
