// Script that is attached to the avatar mesh; handles avatar animations and audio

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class AvatarMeshHandler : MonoBehaviour
{
    // References the animator that stores all animation states as well as the transitions 
    // between these animation
    Animator animator;

    // The current animation phase that the avatar is in (e.g. idle, dancing, etc...)
    private int animationPhase;

    // The amount of time that is takes to transition between animations
    private float crossFadeTime = 0.05f;

    // The audio source of the avatar that is used to play audio (e.g. music)
    private AudioSource avatarAudioSource;

    // Clips to play when the avatar is 'dancing'
    public AudioClip gangnumStyleClip;
    public AudioClip sambaDancingClip;
    public AudioClip thrillerClip;
    public AudioClip ymcaClip;
    public AudioClip hotelClip;

    // References the input containers which determine whether it is appropriate for the avatar to 
    // perform specific animations (e.g. if the user is inputting data then the avatar should not dance)
    GameObject KeyboardInputField;
    GameObject MicrophoneRecording;

    // Holds current settings
    protected SettingsHandler currentSettings;

    // Start is called before the first frame update
    void Start()
    {
        // Gets avatar animation handler
        animator = gameObject.GetComponent<Animator>();

        // Gets avatar audio source (used for controlling sounds that the avatar makes)
        avatarAudioSource = gameObject.GetComponent<AudioSource>();

        // Gets the keyboard and microphone input fields, these are used to check if the 
        // user is currently inputting information
        KeyboardInputField = GameObject.Find("KeyboardInputField");
        MicrophoneRecording = GameObject.Find("MicrophoneRecordingInfoContainer");

        string filePath = Path.Combine(Application.persistentDataPath, "data");
        filePath = Path.Combine(filePath, "ApplicationSettings.json");
        currentSettings = new SettingsHandler(filePath);

        // Current animation phase is set to 0, this means that the avatar should default
        // to an idle state
        animationPhase = 0;

        // Sets default animation values
        SetDefaultValues();
    }

    // Sets the default animation values, this ensures that the avatar is in the default 
    // walking intro animation
    public void SetDefaultValues()
    {
        animator.SetBool("isWalking", true);

        animator.SetBool("isIdle", false);

        animator.SetInteger("randomAnimation", 0);
        animator.SetBool("isExplaining", false);
        animator.SetBool("isThinking", false);
    }

    // Used for declaring if the avatar has finished the intro walking animation. The parameters
    // define the speed between the walking animation and the final animation, as well as if the 
    // avatar should wave at the end of the walking animation
    public void FinishWalkAnimation(float crossFadeSpeed, bool performWave)
    {
        // Avatar is set to the idle state in the animator
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", true);
        
        // Performs the wave animation if defined
        if(performWave)
            animator.CrossFade("Waving Intro", crossFadeSpeed);
        // Otherwise perform the idle animation
        else
            animator.CrossFade("Breathing Idle", crossFadeSpeed);
    }

    // Toggle animations between idle, thinking and explaining. These are the three animations that
    // define the main logic for the avatar receiving an input, waiting for a response from Watson,
    // and explaining the results
    public void ToggleAnimationPhase()
    {
        switch (animationPhase)
        {
            // If the avatar is currently idle, then perform the thinking animation
            case 0:
                PerformThinkingAnimation();
                break;

            // If the avatar is currently thinking, then perform the explaining animation
            case 1:
                PerformExplinationAnimation();
                break;

            // If the avatar has finished explaining, then go back to the idle animation
            case 2:
                PerformIDLEAnimation();
                break;
        }
    }

    // Sets appropriate flags for performing the default idle animation
    public void PerformIDLEAnimation()
    {
        // Sets animation flags so that the avatar stays in the idle animation, random and touch animations
        // are set to 0 as the avatar should not perform these animations
        animator.SetInteger("randomAnimation", 0);
        animator.SetInteger("touchAnimation", 0);
        animator.SetBool("isExplaining", false);
        animator.SetBool("isIdle", true);

        // Performs idle animation
        animator.CrossFade("Breathing Idle", crossFadeTime);

        // Any audio is stopped as the avatar is now idle
        avatarAudioSource.Stop();

        // Animation phase is now 0 as the avatar is now idle
        animationPhase = 0;
    }

    // Sets appropriate flags for performing the thinking animation
    public void PerformThinkingAnimation()
    {
        // Sets animation flags so that the avatar stays in the thinking animation
        animator.SetBool("isIdle", false);
        animator.SetBool("isThinking", true);

        // Performs thinking animation
        animator.CrossFade("Thinking", crossFadeTime);

        // Any audio is stopped as the avatar is now thinking and waiting for a response from watson
        avatarAudioSource.Stop();

        // Animation phase is now 1 as the avatar is now thinking
        animationPhase = 1;
    }

    // Sets appropriate flags for performing the explination animation
    public void PerformExplinationAnimation()
    {
        // Sets animation flags so that the avatar stays in the explination animation
        animator.SetBool("isThinking", false);
        animator.SetBool("isExplaining", true);

        // Performs explination animation
        animator.CrossFade("Explination", crossFadeTime);

        // Any audio is stopped as the avatar is now explaining the returned text from watson
        avatarAudioSource.Stop();

        // Animation phase is now 1 as the avatar is now explaining the returned text
        animationPhase = 2;
    }

    // Executed if the user clicks on the avatar. This checks if the avatar is in the idle state (animation).
    // If so, then perform a random dance animation and play the appropriate music for that animation
    public void PerformTouchAnimationIfIdle()
    {
        // Stores the animation states the avatar can transition to if the user 'touches' the avatar, these
        // are randomly selected
        string[] animationNames = { "Samba Dancing", "Gangnam Style", "Ymca Dance", "Thriller", "Hotel Dancing" };

        // Stores references to the music clips for each animation state, these are played if the avatar decides
        // to perform the appropriate animation
        AudioClip[] audioClips = { sambaDancingClip, gangnumStyleClip, ymcaClip, thrillerClip, hotelClip };

        // Perform random animation if the avatar is currently in the idle animation, otherwise no animation
        // is played and the avatar stays idle
        if (animationPhase == 0 && IsAnimatorInCurrentState("Breathing Idle"))
        {
            // Chooses a random 'dance' animation to transition to from the dance animation list
            int animationNum = Random.Range(1, animationNames.Length + 1);

            // Sets the flag in the animator that the avatar is now performing this animation
            animator.SetInteger("touchAnimation", animationNum);

            // Avatar now plays this animation
            animator.CrossFade(animationNames[animationNum-1], crossFadeTime);

            // The audio clip for this animation is selected and played
            avatarAudioSource.clip = audioClips[animationNum - 1];
            avatarAudioSource.Play();
        }
    }

    // If the avatar is idle, then there is a chance that the avatar will perform a 'random' animation.
    // E.G: waving at the user. This function determines whether an animation should occur or whether the 
    // avatar should stay in its idle state 
    public void PerformRandomAnimationIfIdle()
    {
        // List that stores the animation states that can occur if the avatar is idle
        string[] animationNames = { "Arm Stretching", "Looking", "Yawn", "Waving"};

        // Perform random animation if the avatar is idle
        if (animationPhase == 0 && IsAnimatorInCurrentState("Breathing Idle"))
        {
            // Chooses an animation from the list of random animation states
            int animationNum = Random.Range(1, animationNames.Length + 1);

            // Sets the flag in the animator that the avatar is now performing this animation
            animator.SetInteger("randomAnimation", animationNum);

            // Avatar now plays this animation
            animator.CrossFade(animationNames[animationNum - 1], crossFadeTime);
        }
    }

    // Returns if the avatar is in a specific state
    public bool IsAnimatorInCurrentState(string state)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    // Sets the avatar audio to 0 (mute)
    public void MuteAudio()
    {
        avatarAudioSource.volume = 0.0f;
    }

    // Sets the avatar audio to 1 (full volume)
    public void UnmuteAudio()
    {
        avatarAudioSource.volume = 1.0f;
    }

    public float GetCurrentStateDuration()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public string GetCurrentStateName()
    {
        if (animator.GetBool("isExplaining"))
            return "Explination";

        if (animator.GetBool("isThinking"))
            return "Thinking";

        if (animator.GetBool("isWalking"))
            return "Walk";

        return "Breathing Idle";
    }

    public void PlayAnimation(string stateName, float normalisedTime)
    {
        avatarAudioSource.Stop();

        animator.SetBool("isWalking", false);

        switch (stateName)
        {
            case "Thinking":
                animator.SetBool("isIdle", false);
                animator.SetBool("isThinking", true);
                break;

            case "Breathing Idle":
                animator.SetInteger("randomAnimation", 0);
                animator.SetInteger("touchAnimation", 0);
                animator.SetBool("isExplaining", false);
                animator.SetBool("isIdle", true);
                break;

            case "Explination":
                animator.SetBool("isThinking", false);
                animator.SetBool("isExplaining", true);
                break;

            case "Walk":
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdle", false);
                break;

            default:
                SetDefaultValues();
                break;
        }

        Debug.Log(normalisedTime);

        animator.Play(stateName, -1, normalisedTime);
    }

    // Executed if the user clicks on the avatar
    void OnMouseDown()
    {
        // If the avatar is currently in the 'thinking' or 'explination' animation, then 
        // do not change animation
        if (IsAnimatorInCurrentState("Thinking") || IsAnimatorInCurrentState("Explination"))
            return;

        // If the user is currently inputting information, from either the keyboard or microphone,
        // then do not allow the avatar to perform any other animation
        if (KeyboardInputField.activeInHierarchy || MicrophoneRecording.activeInHierarchy)
            return;

        // Get the latest settings data; will be updated if the tutorial is compelted
        currentSettings.ReadJson();

        // User cannot be in the tutorial when making the Avatar dance
        if (!currentSettings.ReturnFieldValue("completeTutorial"))
            return;

        // If the avatar is currently in the idle state, then perform the appropriate animation
        // for if the user 'touches' the avatar
        if (IsAnimatorInCurrentState("Breathing Idle"))
            PerformTouchAnimationIfIdle();

        // If the avatar is not idle, then return the avatar back to its idle state
        else
            PerformIDLEAnimation();
    }
}
