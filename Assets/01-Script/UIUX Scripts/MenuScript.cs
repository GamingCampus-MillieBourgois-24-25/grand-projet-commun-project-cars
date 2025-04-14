using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject Screen;
    [SerializeField] GameObject Menu;
    [SerializeField] TransitionManagement script;

    public Transform cameraTargetPosition;
    public Transform cameraTargetPositionGarage;
    public Transform cameraTargetPositionPersonalize;
    public float lerpDuration = 2.0f;
    public bool IsTransitioning = false;

    public void GoGarage()
    {
        StartCoroutine(WaitToGoGarage());
        StartCoroutine(SetColor(Color.white));
    }

    public void GoMenu()
    {
        StartCoroutine(WaitToGoMenu());
        StartCoroutine(SetColor(new Color(50f / 255f, 50f / 255f, 53f / 255f)));
    }

    public IEnumerator SetColor(Color choice)
    {
        Renderer renderer = Screen.GetComponent<Renderer>();
        Color startColor = renderer.material.color;
        Color endColor = choice;
        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            renderer.material.color = Color.Lerp(startColor, endColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        renderer.material.color = endColor;
    }

    public IEnumerator WaitToGoMenu()
    {
        script.Wait();
        Transform cam = Camera.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        Vector3 targetPos = cameraTargetPosition.position;
        Quaternion targetRot = cameraTargetPosition.rotation;

        float elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            float t = elapsed / lerpDuration;
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = targetPos;
        cam.rotation = targetRot;
        Screen.GetComponent<VideoPlayer>().Stop();
        Menu.SetActive(true);
        yield return new WaitForSeconds(.5f);
        script.Go();
    }

    public IEnumerator WaitToGoGarage()
    {
        script.Wait();
        Menu.SetActive(false);
        Screen.GetComponent<VideoPlayer>().Play();
        Transform cam = Camera.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        Vector3 targetPos = cameraTargetPositionGarage.position;
        Quaternion targetRot = cameraTargetPositionGarage.rotation;

        float elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            float t = elapsed / lerpDuration;
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = targetPos;
        cam.rotation = targetRot;
        yield return new WaitForSeconds(.5f);
        script.Go();
    }

    public IEnumerator WaitToGoPersonalize()
    {
        script.Wait();
        Menu.SetActive(false);
        Screen.GetComponent<VideoPlayer>().Play();
        Transform cam = Camera.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        Vector3 targetPos = cameraTargetPositionPersonalize.position;
        Quaternion targetRot = cameraTargetPositionPersonalize.rotation;

        float elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            float t = elapsed / lerpDuration;
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = targetPos;
        cam.rotation = targetRot;
        yield return new WaitForSeconds(.5f);
        script.Go();
    }
}
