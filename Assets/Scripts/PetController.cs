using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PetController : MonoBehaviour {

    public Pathfinder pathFinder;
    public PetHead walker;
    public PetAnimationController animController;

    public Vector3 sceneSize = new Vector3(5, 7, 5);

    [SerializeField] private AudioClip[] wakeUpAudio;
    [SerializeField] private AudioClip[] talkingAudio;

    [SerializeField] private VoiceCommand[] commands;

    private State state;
    private AudioSource audioSrc;

    private SpeechManager speechManager;
    private string[] parsedCommands;

    private void Start() {
        SetState(new SleepState(this));
        speechManager = new SpeechManager(GetAllKeywords());
        speechManager.OnPhraseRecognized += ParseCommandToKeyword;
        audioSrc = GetComponent<AudioSource>();
    }

    private string[] GetAllKeywords() {
        List<string> keywords = new List<string>();

        foreach (VoiceCommand v in commands) {
            foreach (string s in v.commands)
                keywords.Add(s);
        }
        return keywords.ToArray();
    }

    private void ParseCommandToKeyword(string text) {
        Debug.Log("Command: <b>" + text + "</b>");

        VoiceCommand command;
        if (CommandsContain(text, out command)) {
            command.onCommandRecieved.Invoke();
        } else {
            Debug.LogWarning("No corresponding command found for " + text);
        }
    }

    public bool CommandsContain(string s, out VoiceCommand command) {
        foreach (VoiceCommand v in commands) {
            if (v.commands.Contains(s)) {
                command = v;
                return true;
            }
        }
        command = null;
        return false;
    }

    public void StartMinigame() {
        Debug.Log("start minigame");
        SceneManager.LoadScene(1);
    }

    public void SetState(State s) {
        if (state != null)
            state.OnStateExit();

        state = s;
        state.OnStateEnter();
    }

    public void WakeUp() {
        if (state.GetType() != typeof(IdleState))
            SetState(new IdleState(this));
        audioSrc.PlayOneShot(wakeUpAudio.GetRandom());
    }

    public void PlayHappySound() {
        if(!audioSrc.isPlaying)
            audioSrc.PlayOneShot(talkingAudio.GetRandom());
    }

    //private void OnGUI() {
    //    if (state.GetType() == typeof(IdleState)) {
    //        IdleState s = state as IdleState;
    //        if (GUI.Button(new Rect(10, 10, 100, 20), "right"))
    //            s.MotionDetected(Direction.Right);
    //        if (GUI.Button(new Rect(10, 30, 100, 20), "Left"))
    //            s.MotionDetected(Direction.Left);
    //        if (GUI.Button(new Rect(10, 50, 100, 20), "front"))
    //            s.MotionDetected(Direction.Front);
    //    }
    //}
}
