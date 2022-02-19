using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    private bool modelSelected;

    private GameObject xBotModel;

    private int animationPhase;

    private float crossFadeTime = 0.05f;

    private AudioSource avatarAudioSource;

    public AudioClip gangnumStyleClip;
    public AudioClip sambaDancingClip;
    public AudioClip thrillerClip;
    public AudioClip ymcaClip;
    public AudioClip hotelClip;

    GameObject KeyboardInputField;
    GameObject MicrophoneRecording;

    // Start is called before the first frame update
    void Start()
    {
        // find models in the scene
        xBotModel = GameObject.Find("xbot");

        // get the animation component in the scene
        animator = xBotModel.GetComponent<Animator>();

        avatarAudioSource = xBotModel.GetComponent<AudioSource>();

        KeyboardInputField = GameObject.Find("KeyboardInputField");

        MicrophoneRecording = GameObject.Find("MicrophoneRecordingInfoContainer");

        animationPhase = 0;

        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        animator.SetBool("isWalking", true);

        animator.SetBool("isIdle", false);

        animator.SetInteger("randomAnimation", 0);
        animator.SetBool("isExplaining", false);
        animator.SetBool("isThinking", false);
    }

    public void FinishWalkAnimation(float crossFadeSpeed, bool performWave)
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", true);
        
        if(performWave)
            animator.CrossFade("Waving Intro", crossFadeSpeed);
        else
            animator.CrossFade("Breathing Idle", crossFadeSpeed);
    }

    // Toggle animations
    public void ToggleAnimationPhase()
    {
        switch (animationPhase)
        {
            case 0:
                PerformThinkingAnimation();
                break;

            case 1:
                PerformExplinationAnimation();
                break;

            case 2:
                PerformIDLEAnimation();
                break;
        }
    }

    public void PerformIDLEAnimation()
    {
        animator.SetInteger("randomAnimation", 0);
        animator.SetInteger("touchAnimation", 0);
        animator.SetBool("isExplaining", false);
        animator.SetBool("isIdle", true);
        animator.CrossFade("Breathing Idle", crossFadeTime);
        avatarAudioSource.Stop();
        animationPhase = 0;
    }

    public void PerformThinkingAnimation()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isThinking", true);
        animator.CrossFade("Thinking", crossFadeTime);
        avatarAudioSource.Stop();
        animationPhase = 1;
    }

    public void PerformExplinationAnimation()
    {
        animator.SetBool("isThinking", false);
        animator.SetBool("isExplaining", true);
        animator.CrossFade("Explination", crossFadeTime);
        avatarAudioSource.Stop();
        animationPhase = 2;
    }

    public void PerformTouchAnimationIfIdle()
    {
        AudioClip[] audioClips = { sambaDancingClip, gangnumStyleClip, ymcaClip, thrillerClip, hotelClip };
        string[] animationNames = { "Samba Dancing", "Gangnam Style", "Ymca Dance", "Thriller", "Hotel Dancing" };

        // Perform random animation
        if (animationPhase == 0 && IsAnimatorInCurrentState("Breathing Idle"))
        {
            int animationNum = Random.Range(1, animationNames.Length + 1);

            animator.SetInteger("touchAnimation", animationNum);

            animator.CrossFade(animationNames[animationNum-1], crossFadeTime);
            avatarAudioSource.clip = audioClips[animationNum - 1];

            avatarAudioSource.Play();
        }
    }

    public void PerformRandomAnimationIfIdle()
    {
        string[] animationNames = { "Arm Stretching", "Looking", "Yawn", "Waving" };

        // Perform random animation
        if (animationPhase == 0 && IsAnimatorInCurrentState("Breathing Idle"))
        {
            int animationNum = Random.Range(1, animationNames.Length + 1);

            animator.SetInteger("randomAnimation", animationNum);

            animator.CrossFade(animationNames[animationNum - 1], crossFadeTime);
        }
    }

    public bool IsAnimatorInCurrentState(string state)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    public void MuteAudio()
    {
        avatarAudioSource.volume = 0.0f;
    }

    public void UnmuteAudio()
    {
        avatarAudioSource.volume = 1.0f;
    }

    void OnMouseDown()
    {
        if (IsAnimatorInCurrentState("Thinking") || IsAnimatorInCurrentState("Explination"))
            return;

        if (KeyboardInputField.activeInHierarchy || MicrophoneRecording.activeInHierarchy)
            return;

        if (IsAnimatorInCurrentState("Breathing Idle"))
            PerformTouchAnimationIfIdle();
        else
            PerformIDLEAnimation();
    }
}
