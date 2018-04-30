using System;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager {

    public ConfidenceLevel confidence = ConfidenceLevel.Low;
    public Action<string> OnPhraseRecognized;

    private PhraseRecognizer recognizer;

    public SpeechManager(string[] keywords, ConfidenceLevel confidenceLevel = ConfidenceLevel.Medium) {
        if (keywords != null) {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += OnPhraseRecognizedFunction;
            recognizer.Start();
        }
    }

    private void OnPhraseRecognizedFunction(PhraseRecognizedEventArgs args) {
        if (OnPhraseRecognized != null)
            OnPhraseRecognized(args.text);
    }

    private void OnApplicationQuit() {
        if (recognizer != null && recognizer.IsRunning) {
            recognizer.OnPhraseRecognized -= OnPhraseRecognizedFunction;
            recognizer.Stop();
        }
    }
    
}