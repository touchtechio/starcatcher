using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public int starCaughtCount;
    public int starReturnCount;

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
    private static GameState previousWorldState;
    private float rejuvinationTimer;
    public float rejuvinationTimerValue = 5f;
    public static float deadTimer;
    public float deadTimerValue = 60f;  // length of clip one + silence + period + star fall + linger
    private float reduceScoreTimer;
    public float reduceScoreTimerValue = 10f;

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
    private bool hasTorus1Triggered = false;
    private float animationTimerTorus1Value = 15f;
    private float animationTimerTorus1;
    private bool torus1AnimationRunTimer = false;
    private float animationTimerSphere1Value = 10f;
    private float animationTimerSphere1;
    private bool hasSphere1Triggered = false;
    private bool sphere1AnimationRunTimer = false;
    private AnimatorSphere1 sphere1Animator;

    public IFormationAnimation[] flourishAnimators;

    public int formationIndex = 0;
     
    void Awake(){
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        // set the scene to rejuvenation, with timer fast at the start
        starCaughtCount = 0;
        SetLevel(0);
        plasmaWorldState = GameState.Rejuvination;
        previousWorldState = plasmaWorldState;
        rejuvinationTimer = reduceScoreTimerValue;
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
        animationTimerTorus1 = animationTimerTorus1Value;
        torus1Animator = GameObject.FindObjectOfType(typeof(AnimatorTorus1)) as AnimatorTorus1;
        //torusAnimatorComponent = (Animator)torus1Animator.GetComponent<Animator>();
        // torusAnimatorComponent.enabled = false;

        animationTimerSphere1 = animationTimerSphere1Value;
        sphere1Animator = GameObject.FindObjectOfType(typeof(AnimatorSphere1)) as AnimatorSphere1;
        flourishAnimators = new IFormationAnimation[] {sphere1Animator, torus1Animator};
    }

    void Update(){

        updateStarCaughtOnKeyPress();
        scoreLogger.LogScore();
        // simulate stars caught with hotkey
        if (Input. GetKeyUp("n")){
            catchStarNoConstellation();
        }
        starCaughtLog = starCaughtCount;

        // set timers for rejuvination state and dead state
        // during rejuvination, no stars can be caught
        // during dead state, no stars are spawned
        if (plasmaWorldState == GameState.Rejuvination) {
            setRejuvinationTimer();
        }

        if (plasmaWorldState == GameState.Flourishing) {

        }

        if (plasmaWorldState == GameState.Dead) {
            setDeadTimer();
        }

        // The tutorial scene also uses the score script, however it doens't need the following actions
        // so they only run in the main scene
        if (isTutorialScene == false) {
            if (torus1AnimationRunTimer == true) {
                runTorus1AnimationTimer();
                }

            if (sphere1AnimationRunTimer == true) {
                runSphere1AnimationTimer();
            }

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

        //TODO: add toggle for game type
        // if (0 == (constellationScore % 10))
        // {
        //     SetLevel(level+1);
        // }
        return constellations.GetNextEmptyPosition().transform.position;
    }

    public void returnStar()
    {
        starReturnCount++;
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

    public void updateStarCaughtOnKeyPress() {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            plasmaWorldState = GameState.Rejuvination;
            starReturnCount = 0;
            starCaughtCount = 0;
            starSpawn.StartRandomStars();
            deadTimer = deadTimerValue;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            starCaughtCount = totalStarsToBeCaught / 2;
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
    }


    public void checkEffectOnEnvironment()
    {
        // TODO: replace formula
        if (starCaughtCount <= totalStarsToBeCaught) {
            environmentDamageScore = (float) starCaughtCount / totalStarsToBeCaught;
            //cumulativeEnvironmentDamageScore = environmentDamageScore;
            cumulativeEnvironmentDamageScore = (float) Mathf.Exp(-1 * Mathf.Pow(3*environmentDamageScore-3.0f,2f));
            //Debug.Log("damage: " + cumulativeEnvironmentDamageScore);
        } else  {
            cumulativeEnvironmentDamageScore = 1.0f;
        }
    }

    public void SetGameState()
    {

        // Flourishing
        // change states as damage score increases
        if (cumulativeEnvironmentDamageScore < 0.2) {
            plasmaWorldState = GameState.Flourishing;
            reduceScoreTimerValue = 10f;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullCaughtAnimation();
                starSpawn.StartRandomStars();
                deadStarPositionCollider.DestroyDeadStars();
                hasSphere1Triggered = false;
            }
            
            // formations 
            if (hasTorus1Triggered == false && cumulativeEnvironmentDamageScore > 0.1) {
                //Debug.Log(hasTorus1Triggered + ", " + cumulativeEnvironmentDamageScore);
                starSpawn.StopRandomStars();
                formationIndex++;
                flourishAnimators[formationIndex%flourishAnimators.Length].Animate();
                // hasTorus1Triggered = true;
                torus1AnimationRunTimer = true;
            }
        }

        // Decline
        else if (cumulativeEnvironmentDamageScore >= 0.2 && cumulativeEnvironmentDamageScore < 0.7)
        {
            plasmaWorldState = GameState.Decline;
            reduceScoreTimerValue = 12f;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullAnimation();
                starSpawn.StartRandomStars();        
                deadStarPositionCollider.DestroyDeadStars();
                hasTorus1Triggered = false;
            }

            // formations 
            if (hasSphere1Triggered == false && cumulativeEnvironmentDamageScore > 0.4){
                starSpawn.StopRandomStars();
                sphere1Animator.Animate();
                //hasSphere1Triggered = true;
                sphere1AnimationRunTimer = true;
                //Invoke("endSphere1Animation", 1f); // Invoke triggers for the duration of the delay set
            }
        } 
        
        // Dying
        else if (cumulativeEnvironmentDamageScore >= 0.7 && cumulativeEnvironmentDamageScore < 1)
        {
            plasmaWorldState = GameState.Dying;
            reduceScoreTimerValue = 15f;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullAnimation();
                starSpawn.StartRandomStars();
                deadStarPositionCollider.DestroyDeadStars();
                hasSphere1Triggered = false;
            }

        // DEAD
        } else if (cumulativeEnvironmentDamageScore == 1) {
            plasmaWorldState = GameState.Dead;
            starSpawn.DestroyStars();
            deathNarrationManager.TriggerDeath(); 
            this.starReturnCount = 0;

            
        }
        // Debug.Log("score: " + cumulativeEnvironmentDamageScore + "state: " + plasmaWorldState);
        previousWorldState = plasmaWorldState;
    }

    public void setRejuvinationTimer(){
        rejuvinationTimer -= Time.deltaTime;
        if (rejuvinationTimer <= 0)  {
            plasmaWorldState = GameState.Flourishing;
            rejuvinationTimer = rejuvinationTimerValue;
            if (plasmaWorldState != previousWorldState)
            {
                starAnimations.FullCaughtAnimation();
            }
            previousWorldState = plasmaWorldState;
        }
    }

    public void setDeadTimer(){
        deadTimer -= Time.deltaTime;
        if ((deadTimer <= 0) && (starReturnCount >= totalDeadStarsToBeReturned))  {
            deadStarPositionCollider.DestroyDeadStars();
            plasmaWorldState = GameState.Rejuvination;
            starReturnCount = 0;
            starCaughtCount = 0;
            starSpawn.StartRandomStars();
            deadTimer = deadTimerValue;
            previousWorldState = plasmaWorldState;
        }
    }

    private void runTorus1AnimationTimer(){
        // continuous on update
        animationTimerTorus1 -= Time.deltaTime;
        //Debug.Log("timer "+ animationTimerTorus1);
        if (animationTimerTorus1 <= 0) {
            starSpawn.StartRandomStars();
            animationTimerTorus1 = animationTimerTorus1Value;
            torus1AnimationRunTimer = false;
            hasTorus1Triggered = true;
            formationIndex++;
            flourishAnimators[formationIndex%flourishAnimators.Length].Animate();
        }
    }

    private void runSphere1AnimationTimer(){
        // continuous on update
        animationTimerSphere1 -= Time.deltaTime;
        //Debug.Log("timer "+ animationTimerSphere1);
        if (animationTimerSphere1 <= 0) {
            endSphere1Animation();
        }
    }

    private void endSphere1Animation(){
        Debug.Log("sphere timer done");
        hasSphere1Triggered = true;
        starSpawn.StartRandomStars();
        animationTimerSphere1 = animationTimerSphere1Value;
        sphere1AnimationRunTimer = false;
        sphere1Animator.Animate();
    }
}


