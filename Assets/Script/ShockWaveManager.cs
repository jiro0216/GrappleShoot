using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ShockWaveManager : MonoBehaviour
{
    [SerializeField] private float shockWaveTime = 0.75f;

    private Coroutine shockWaveCoroutine;

    private Material material;

    private static int waveDistanceFromCenterID = Shader.PropertyToID("_WaveDistanceFromCenter");

    private void Awake()
    {

        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E pressed");
            CallShockWave(transform.position);
        }
    }

    public void CallShockWave(Vector2 worldPos)
    {

        if (shockWaveCoroutine != null)
            StopCoroutine(shockWaveCoroutine);

        shockWaveCoroutine = StartCoroutine(ShockwaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockwaveAction(float startPos, float endPos)
    {
        material.SetFloat(waveDistanceFromCenterID, startPos);

        float elapsedTime = 0f;

        float lerpedAmount = 0f;

        while (elapsedTime < shockWaveTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, elapsedTime / shockWaveTime);

            material.SetFloat(waveDistanceFromCenterID, lerpedAmount);

            yield return null;
        }
    }


}