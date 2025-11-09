using UnityEngine;
using System.Collections;

public class WakeUpAnimation : MonoBehaviour
{
    public float wakeUpDuration = 3f; // durée du redressement
    public float liftHeight = 1f;     // combien il se soulève

    [Header("UI & Animation")]
    public Animator fadeAnimator;    // Animator de ton BlackScreen
    public AudioSource sound1;       // Premier son
    public AudioSource sound2;       // Deuxième son

    private Vector3 startPos;
    private Vector3 endPos;
    private Quaternion startRot;
    private Quaternion endRot;

    private void Start()
    {
        if(fadeAnimator != null)
        {
            fadeAnimator.Play("IntroFade");
        }

        startPos = transform.position;
        startRot = transform.rotation;

        endPos = startPos + Vector3.up * liftHeight;
        endRot = Quaternion.Euler(0, startRot.eulerAngles.y, 0);

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        // --- Jouer le son 1 et attendre qu'il se termine ---
        if(sound1 != null) 
        {
            sound1.Play();
            yield return new WaitWhile(() => sound1.isPlaying);
        }

        // --- Jouer le son 2 après le son 1 ---
        if(sound2 != null)
        {
            sound2.Play();
            yield return new WaitWhile(() => sound2.isPlaying);
        }

        // --- Attente avant de commencer le réveil (si nécessaire) ---
        yield return new WaitForSeconds(6f);

        float elapsed = 0f;

        while (elapsed < wakeUpDuration)
        {
            float t = elapsed / wakeUpDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        Debug.Log("Le joueur est debout !");
    }
}
