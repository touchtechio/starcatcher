using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Score : MonoBehaviour {

    public int starCaughtCount;
    public int starReturnCount;
    public static int starCatcherRevivedCount; // keeps track of how many times Plasma has been revived, a voyage counter

    public static int starCaughtLog;
    public static int level;
    private static Constellations constellations;
    private static StarAnimations starAnimations;
    internal static readonly int LEVEL_ONE = 0;
    internal static readonly int LEVEL_TWO = 1;
    internal static readonly int LEVEL_THREE = 2;

    public static float environmentDamageScore;
    public static float cumulativeEnvironmentDamageScore;
    public int totalStarsToBeCaught;
    public int totalDeadStarsToBeReturned;

    public static int randomAdd;
    
    public enum GameState {Dead, Rejuvination, Flourishing, Decline, Dying};

    public static GameState plasmaWorldState;
    private GameState previousWorldState;
    private float rejuvinationTimer;
    public float rejuvinationTimerValue = 5f;
    public static float deadTimer;
    public float deadTimerValue = 60f;  // length of clip one + silence + period + star fall + linger
    private float reduceScoreTimer;
    public float reduceScoreTimerValue = 20f;
    private float pauseBeforeNarrationTimerValue = 10f;

    private static BackingTracks BackingTracks;
    private scoreLog scoreLogger;

    public StarSpawn starSpawn;
    public DeadStarPositionColliders deadStarPositionCollider;
    public DeathNarrationManager deathNarrationManager; 
    public bool constellationMode = true;
    public bool isTutorialScene = false;

    public static Score Instance;
    public bool reduceScoreBool = false;

    // FORMATIONS
    private AnimatorTorus1 torus1Animator;
    private AnimatorSphere1 sphere1Animator;
    private AnimatorSection1 animatorSection;
    private AnimatorSphere3 sphere3Animator;
    private AnimatorSphere2 animatorPlasma;
    private bool hasTriggerSpawnFaintStarAnimation = false;

    public IFormationAnimation[] flourishAnimators;
    private float[] flourishAnimationTimerDurations = {25f, 15f, 25f, 10f}; // torus, sphere streak, sphere around room, section
    private bool[] hasAnimationTriggered = {false, false, false, false};
    private float flourishAnimationTimerValue;
    private float randomStarTimerValue = 10f;
    private float randomStarTimerDuration = 10f;
    private bool[] animationRunTimer = {false, false, false, false};
    private bool hasPlantDyingNarration = false;
    private int nonFlourishAnimatorIndex;

    public int formationIndex = 0;
    public bool inAnimationMode = false;
     
    void Awake(){
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        // set the scene to rejuvenation, with timer fast at the start
        starCaughtCount = 0; //totalStarsToBeCaught; this is to start in dying mode to light up sky
        starCatcherRevivedCount = 0;
        SetLevel(0);
        plasmaWorldState = GameState.Dead;
        deathNarrationManager.TriggerFirstVoyage();
        previousWorldState = plasmaWorldState;
        rejuvinationTimer = rejuvinationTimerValue;
        deadTimer = deadTimerValue;
        reduceScoreTimer = reduceScoreTimerValue;

        constellations = FindObjectOfType<Constellations>();
        starAnimations = FindObjectOfType<StarAnimations>();
        if (null == constellations)
        {
            Debug.Log("ERROR: no constellation manager found");
            constellationMode = false;
        }
        if (null == starAnimations)
        {
            Debug.Log("ERROR: no star animation manager found");
        }

        BackingTracks = FindObjectOfType<BackingTracks>();
        if (null == BackingTracks)
        {
            Debug.LogWarning("WARN: no BackingTracks found");
        }

        // Initialize score logger, star spawn and dead star positions
        scoreLogger = (scoreLog)FindObjectOfType<scoreLog>();
        starSpawn = (StarSpawn)FindObjectOfType<StarSpawn>();
        deadStarPositionCollider = (DeadStarPositionColliders)FindObjectOfType<DeadStarPositionColliders>();

        // Set up formation animations - find componened and disable
        torus1Animator = GameObject.FindObjectOfType(typeof(AnimatorTorus1)) as AnimatorTorus1;
        sphere1Animator = GameObject.FindObjectOfType(typeof(AnimatorSphere1)) as AnimatorSphere1;
        sphere3Animator = GameObject.FindObjectOfType(typeof(AnimatorSphere3)) as AnimatorSphere3;
        animatorSection = GameObject.FindObjectOfType(typeof(AnimatorSection1)) as AnimatorSection1;
        animatorPlasma = GameObject.FindObjectOfType(typeof(AnimatorSphere2)) as AnimatorSphere2;
        flourishAnimators = new IFormationAnimation[] {sphere1Animator, sphere3Animator, animatorSection}; //, torus1Animator};
        for (int i = 0; i < flourishAnimators.Length; i++) {
            flourishAnimationTimerDurations[i] = flourishAnimators[i].GetAnimationLength();
            //Debug.LogError("animator lengths " + animationTimerDurations[i]);
        }
        flourishAnimationTimerValue = flourishAnimationTimerDurations[getCurrentFromationIndex()];
    }

 void Update_Performance() {
    updateStarCaughtOnKeyPress();
    
}
    // void Update_GameStatemachine(){

    void Update(){


        updateStarCaughtOnKeyPress();
        scoreLogger.LogScore();
        // simulate stars caught with hotkey
        if (Input. GetKeyUp("n")){
            catchStarNoConstellation();
        }
        starCaughtLog = starCaughtCount;

        // The tutorial scene also uses the score script, however it doens't need the following actions
        // so they only run in the main scene
        if (isTutorialScene == false) {
            // if (animationRunTimer[getCurrentFromationIndex()] == true) {
            //     runAnimationTimer();
            // }

            if (plasmaWorldState == GameState.Flourishing) {
                if (inAnimationMode == true){
                    runFlourishAnimationTimer();
                } else {
                    runRandomStarTimer();
                }
            }
            // set timers for rejuvination state and dead state
            // during rejuvination, no stars can be caught
            // during dead state, no stars are spawned
            if (plasmaWorldState == GameState.Rejuvination) {
                runRejuvinationTimer();
            }

            if (plasmaWorldState == GameState.Dying) {
                runDyingPauseBeforeNarrationTimer();
                if (inAnimationMode == true)
                {
                    runAnimationTimer();
                } else {
                    runRandomStarTimer();
                }
            }

            // if (plasmaWorldState == GameState.Dead) {
            //     runDeadTimer();
            // }
            checkEffectOnEnvironment();
            if (plasmaWorldState != GameState.Rejuvination && plasmaWorldState != GameState.Dead){
                SetGameState();
            }

            // If on, reduce score over time so that the world regenerates if noone is playing
            if (reduceScoreBool == true) {
                reduceScoreTimer -= Time.deltaTime;
                if (reduceScoreTimer <= 0) {
                    if (starCaughtCount > 0) starCaughtCount --;
                    reduceScoreTimer = reduceScoreTimerValue;
                }
            }
        }
    }


    public Vector3 catchStar()
    {
        starCaughtCount++;

        // can add toggle for game type
        // if (0 == (constellationScore % 10))
        // {
        //     SetLevel(level+1);
        // }
        return constellations.GetNextEmptyPosition().transform.position;
    }

    public void returnStar()
    // TODO; review this code - this triggers a narration, and can only happen once
    {
        Debug.Log( starReturnCount + " stars returned  out of: " + totalDeadStarsToBeReturned + " in " + plasmaWorldState);
        if (starCaughtCount > 0) {
            starReturnCount++;
            starCaughtCount--; // reduces global score
        }
        
        if (starReturnCount == totalDeadStarsToBeReturned && plasmaWorldState == Score.GameState.Dead) {
            Debug.Log("returned 2 stars");
            deathNarrationManager.TriggerReturnOtherStars();
        }
        
       
    }

    public void catchStarNoConstellation()
    {
        starCaughtCount++;
        reduceScoreTimer = reduceScoreTimerValue;
        randomAdd = (int) (UnityEngine.Random.value * 3f);
    }

    public static void SetLevel(int level)
    {
        Score.level = level;
        Score.level %= 3;
        //BackingTracks.UpdateLevel();
    }

    internal static int GetCurrentLevel()
    {
        return level;
    }


    public void checkEffectOnEnvironment()
    {
        // replace formula
        if (starCaughtCount <= totalStarsToBeCaught) {
            environmentDamageScore = (float) starCaughtCount / totalStarsToBeCaught;
            cumulativeEnvironmentDamageScore = environmentDamageScore;
            // cumulativeEnvironmentDamageScore = (float) Mathf.Exp(-1 * Mathf.Pow(3*environmentDamageScore-3.0f,2f));
            // Debug.Log("damage: " + cumulativeEnvironmentDamageScore + "state " + plasmaWorldState);
        } else  {
            cumulativeEnvironmentDamageScore = 1.0f;
        }
    }

    public void SetGameState()
    {
        // Flourishing
        // change states as damage score increases
        if (cumulativeEnvironmentDamageScore < 0.5) {
            plasmaWorldState = GameState.Flourishing;
            // reduceScoreTimerValue = 10f;
            if (plasmaWorldState != previousWorldState)
            {
                Debug.Log("Entering flourish");
                if (starCatcherRevivedCount < 1) {
                    Debug.Log("entering flourish for the first time");
                    deadStarPositionCollider.DestroyDeadStars(); // not sure this is needed, seems to already be happening
                } 
                starAnimations.FullCaughtAnimation();
                starSpawn.StartRandomStars();
                hasAnimationTriggered = new bool[]{false, false, false, false};
            }

            if (cumulativeEnvironmentDamageScore == 0){
                plasmaWorldState = GameState.Rejuvination;
                if (plasmaWorldState != previousWorldState){
                    deathNarrationManager.TriggerFirstVoyage();
                    starSpawn.StartRandomStars();
                    animatorPlasma.Pause();
                    deadStarPositionCollider.DestroyDeadStars();
                }
            }

            if (cumulativeEnvironmentDamageScore > 0.25 && starCatcherRevivedCount > 0){
                animatorPlasma.Restart();
            }
                
            
            // formations 
            // APRIL 1 2023
            // if ((hasAnimationTriggered[getCurrentFromationIndex()] == false) && (cumulativeEnvironmentDamageScore > 0.08)) {
            //     hasAnimationTriggered[getCurrentFromationIndex()] = true;
            //     animationTimerValue = animationTimerDurations[getCurrentFromationIndex()];
            //     animationRunTimer[getCurrentFromationIndex()] = true;
            //     //Debug.Log(hasTorus1Triggered + ", " + cumulativeEnvironmentDamageScore);
            //     starSpawn.StopRandomStars();
            //     // formationIndex++;
            //     flourishAnimators[getCurrentFromationIndex()].Animate();
            //     //Debug.Log("formation "+ getCurrentFromationIndex());
            //     // hasTorus1Triggered = true;
            // }
        }

        // Decline
        else if (cumulativeEnvironmentDamageScore >= 0.5 && cumulativeEnvironmentDamageScore < 0.75)
        {
            plasmaWorldState = GameState.Decline;
            // reduceScoreTimerValue = 12f;
            formationIndex = flourishAnimators.Length - 1;
            nonFlourishAnimatorIndex = 0;
            if (plasmaWorldState != previousWorldState)
            {
                // starAnimations.FullAnimation();
                Debug.Log(" Entering Decline");
                starSpawn.StartRandomStars();        
                deadStarPositionCollider.DestroyDeadStars();
                hasAnimationTriggered = new bool[]{false, false, false, false};
            }

            // formations 
            if (hasAnimationTriggered[nonFlourishAnimatorIndex] == false && cumulativeEnvironmentDamageScore > 0.65){
                animationRunTimer[nonFlourishAnimatorIndex] = true; // starts animation timer
                hasAnimationTriggered[nonFlourishAnimatorIndex] = true; // stops animation from looping
                //Debug.Log(hasTorus1Triggered + ", " + cumulativeEnvironmentDamageScore);
                //starSpawn.StopRandomStars();
                // formationIndex++;
                flourishAnimators[nonFlourishAnimatorIndex].Animate(); // uses all functionality of the animator stuff but calls specific funciton
                deathNarrationManager.TriggerSnowDwarf();
                flourishAnimationTimerValue = flourishAnimationTimerDurations[nonFlourishAnimatorIndex];
                //Debug.Log("formation "+ getCurrentFromationIndex());
                // hasTorus1Triggered = true;
            }
        } 
        
        // Dying
        else if (cumulativeEnvironmentDamageScore >= 0.75 && cumulativeEnvironmentDamageScore < 1)
        {
            plasmaWorldState = GameState.Dying;
            Debug.Log(" Entering Dying" + "previous state " + previousWorldState);
            // reduceScoreTimerValue = 15f;
            nonFlourishAnimatorIndex = 1;
            if (plasmaWorldState != previousWorldState && previousWorldState == GameState.Decline)
            {
                // starAnimations.FullAnimation();
                Debug.Log(" plant naration in dying" + "previous state " + previousWorldState);
                hasPlantDyingNarration = false;
                starSpawn.StartRandomStars();
                deadStarPositionCollider.DestroyDeadStars();
                hasAnimationTriggered = new bool[]{false, false, false, false};
            }

            // formations - connected shower
            if (hasAnimationTriggered[nonFlourishAnimatorIndex] == false && cumulativeEnvironmentDamageScore > 0.9){
                animationRunTimer[nonFlourishAnimatorIndex] = true; // starts animation timer
                hasAnimationTriggered[nonFlourishAnimatorIndex] = true; // stops animation from looping
                //Debug.Log(hasTorus1Triggered + ", " + cumulativeEnvironmentDamageScore);
                //starSpawn.StopRandomStars();
                // formationIndex++;
                flourishAnimators[nonFlourishAnimatorIndex].Animate(); // uses all functionality of the animator stuff but calls specific funciton
                deathNarrationManager.TriggerOuterRing();
                flourishAnimationTimerValue = flourishAnimationTimerDurations[nonFlourishAnimatorIndex];
                //Debug.Log("formation "+ getCurrentFromationIndex());
                // hasTorus1Triggered = true;
            }
        } 

        // DEAD
        else if (cumulativeEnvironmentDamageScore == 1) {
            plasmaWorldState = GameState.Dead;
            starSpawn.DestroyStars();
            deathNarrationManager.TriggerDeath(); 
            this.starReturnCount = 0;
            // totalStarsToBeCaught = 60; // second time through higher stars to catch
            hasAnimationTriggered = new bool[]{true, true, true, true}; // set as true so no flourish animations can get triggered
            // cumulativeEnvironmentDamageScore -= starReturnCount * 0.01f; // revive plants a little as stars are caught
        }
        Debug.Log("score: " + cumulativeEnvironmentDamageScore + "state: " + plasmaWorldState);
        previousWorldState = plasmaWorldState;
    }

    public void runRejuvinationTimer(){
        rejuvinationTimer -= Time.deltaTime;
        if (rejuvinationTimer <= 0 && starCaughtCount > 0)  {
            plasmaWorldState = GameState.Flourishing;
            rejuvinationTimer = rejuvinationTimerValue;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullCaughtAnimation(); // this is a transition animation
            }
        }
    }

    public void runDeadTimer(){
        deadTimer -= Time.deltaTime;
        if ((deadTimer <= 0) && (starReturnCount >= totalDeadStarsToBeReturned))  {
           // deadStarPositionCollider.DestroyDeadStars();
            // plasmaWorldState = GameState.Rejuvination;
            // starReturnCount = 0;
            // starCaughtCount = 0;
            starCatcherRevivedCount++;
            starSpawn.StartRandomStars();
            deadTimer = deadTimerValue;
            SetGameState();
        }
    }

    private void runRandomStarTimer(){
        // continuous on update during flourish state
        randomStarTimerValue -= Time.deltaTime;
        // Debug.Log("timer "+ animationTimerValue);
        if (randomStarTimerValue <= 0) {
            // START ANIMATION AT END OF TIMER
            //starSpawn.StopRandomStars();
            formationIndex++;
            flourishAnimationTimerValue = flourishAnimationTimerDurations[getCurrentFromationIndex()];
            Debug.Log("next animation " + getCurrentFromationIndex() + " timer " + flourishAnimationTimerValue);
            flourishAnimators[getCurrentFromationIndex()].Animate();
            inAnimationMode = true;
            randomStarTimerValue = randomStarTimerDuration;
        }
    }   

    private void runFlourishAnimationTimer(){
        // continuous on update
        flourishAnimationTimerValue -= Time.deltaTime;
        // Debug.Log("timer "+ animationTimerValue);
        if (flourishAnimationTimerValue <= 0) {
            endAnimation(getCurrentFromationIndex());
            Debug.Log("animation timer done");
        }
    }

    private void runAnimationTimer()
    {
        // triggered in dying or decline
        if (hasAnimationTriggered[nonFlourishAnimatorIndex])
        {
            flourishAnimationTimerValue -= Time.deltaTime;
        }
        // Debug.Log("timer "+ animationTimerValue);
        if (flourishAnimationTimerValue <= 0)
        {
            endAnimation(nonFlourishAnimatorIndex);
            Debug.Log("animation timer done");
        }
    }

    private void runDyingPauseBeforeNarrationTimer(){
        // continuous on update
        if (hasPlantDyingNarration == false){
        pauseBeforeNarrationTimerValue -= Time.deltaTime;
        //Debug.Log("puase before narration of plants");
        if (pauseBeforeNarrationTimerValue <= 0) {
            deathNarrationManager.TriggerPlantDying();
            Debug.Log("start narration for dying plants");
            pauseBeforeNarrationTimerValue = 10f;
            hasPlantDyingNarration = true;
        }
        }
    }  
 
//    private void endFlourishAnimation(){
//        // hasAnimationTriggered[getCurrentFromationIndex()] = true;
//        flourishAnimators[getCurrentFromationIndex()].Reset();
//        // animationRunTimer[getCurrentFromationIndex()] = false;
//        starSpawn.StartRandomStars();
//        inAnimationMode = false;
//        // animationTimerValue = animationTimerDurations[getCurrentFromationIndex()];
//        // flourishAnimators[getCurrentFromationIndex()].Animate();
//    }

    private void endAnimation(int index)
    {
        // hasAnimationTriggered[getCurrentFromationIndex()] = true;
        flourishAnimators[index].Reset();
        // animationRunTimer[getCurrentFromationIndex()] = false;
        starSpawn.StartRandomStars();
        inAnimationMode = false;
        // animationTimerValue = animationTimerDurations[getCurrentFromationIndex()];
        // flourishAnimators[getCurrentFromationIndex()].Animate();
    }

    private int getCurrentFromationIndex() {
        return formationIndex%flourishAnimators.Length;
    }

    public void updateStarCaughtOnKeyPress() {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            plasmaWorldState = GameState.Rejuvination;
            hasAnimationTriggered = new bool[]{false, false, false, false};
            starReturnCount = 0;
            starCaughtCount = 0;
            starCatcherRevivedCount = 0;
            starSpawn.StartRandomStars();
            deadTimer = deadTimerValue;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            starCaughtCount = totalStarsToBeCaught / 3;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            starCaughtCount = totalStarsToBeCaught -1;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            starCaughtCount = totalStarsToBeCaught;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            starCaughtCount ++;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            starCaughtCount --;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.LogWarning("R key was pressed.");
            starSpawn.StartRandomStars();
            hasAnimationTriggered = new bool[] { false, false, false, false };
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.LogWarning("F key was pressed.");
            starSpawn.DestroyStars();
            hasAnimationTriggered = new bool[] { false, false, false, false };
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.LogWarning("X key was pressed.");
            plasmaWorldState = GameState.Decline;
            cumulativeEnvironmentDamageScore = 0.3f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogWarning("S key was pressed.");
            plasmaWorldState = GameState.Decline;
            cumulativeEnvironmentDamageScore = 0.7f;
        }
                if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.LogWarning("W key was pressed.");
            plasmaWorldState = GameState.Decline;
            cumulativeEnvironmentDamageScore = 1.0f;
        }

        if (Input.GetKeyDown(KeyCode.F5)) // allow animations
        {
            hasAnimationTriggered = new bool[]{false, false, false, false};;
        }

        if (Input.GetKeyDown(KeyCode.F6)) // stop animations
        {
            hasAnimationTriggered = new bool[]{true, true, true, true};;
        }
        if (Input.GetKeyDown(KeyCode.F7))
        { // start over
            Start();
        }
    }

}


