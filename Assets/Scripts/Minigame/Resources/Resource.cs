using System.Collections;
using System;
using UnityEngine;

public enum Emotion {
    Happy = 0,
    Sad = 1,
    Angry = 2
}

/// <summary>
/// Enum used for speech commands that links to the Color enum
/// </summary>
public enum EmotionColor {
    Blue = 0,
    Yellow = 1,
    Red = 2
}

namespace Minigame {

    public class Resource : MonoBehaviour {

        [SerializeField] private float eatDuration = 0.3f;
        [SerializeField] private float spawnDuration = 0.7f;
        [SerializeField] private float removeDuration = 0.5f;
        [SerializeField] private float timeUntillDestroy = 10f;

        public Emotion emotion = Emotion.Happy;

        private bool hasBeenVisible;
        private float lifeTimer;

        private void Start() {
            StartCoroutine(Spawn());
        }

        private void Update() {
            if(lifeTimer < timeUntillDestroy) {
                lifeTimer += Time.deltaTime;
            } else {
                enabled = false;
                StartCoroutine(DestroyObject());
            }
        }

        private IEnumerator Spawn() {
            Vector3 start = Vector3.zero;
            Vector3 target = transform.localScale;
            transform.localScale = start;
            float timer = 0;

            while (timer < spawnDuration) {
                transform.localScale = Vector3.Lerp(start, target, timer / spawnDuration);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public void OnCollected() {
            Resources.Remove(this);
            StartCoroutine(DestroyObject());
        }

        private IEnumerator DestroyObject() {
            Vector3 start = transform.localScale;
            Vector3 target = Vector3.zero;
            float timer = 0;

            GetComponent<BoxCollider>().enabled = false;

            while (timer < eatDuration) {
                transform.localScale = Vector3.Lerp(start, target, timer / eatDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            Resources.Remove(this);
            Destroy(gameObject);
        }

        private void OnBecameVisible() {
            hasBeenVisible = true;
        }

        private void OnBecameInvisible() {
            if (!hasBeenVisible)
                return;
            Destroy(gameObject);
        }
    }
}