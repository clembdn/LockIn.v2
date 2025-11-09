using UnityEngine;
using System.Collections;

public class IntroSequenceWithWakeUp : MonoBehaviour
{
    [Header("UI & Animation")]
    public Animator fadeAnimator;       // Animator du BlackScreen

    [Header("Sons")]
    public AudioSource sound1;
    public AudioSource sound2;

    [Header("Joueur")]
    public WakeUpAnimation wakeUpScript;  // Ton ancien script WakeUpAnimation

    private void Start()
    {
        // Désactive le réveil au départ
        if(wakeUpScript != null) wakeUpScript.enabled = false;

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        Debug.Log("Intro démarrée...");

        // --- Étape 1 : sons ---
        yield return new WaitForSeconds(1f);
        if(sound1 != null) sound1.Play();
        yield return new WaitForSeconds(2f);
        if(sound2 != null) sound2.Play();

        // --- Étape 2 : fondu noir ---
        if(fadeAnimator != null) fadeAnimator.Play("IntroFade");
        yield return new WaitForSeconds(5f); // durée du fondu

        // --- Étape 3 : activer le réveil ---
        if(wakeUpScript != null) wakeUpScript.enabled = true;

        Debug.Log("WakeUpAnimation activé !");
    }
}
