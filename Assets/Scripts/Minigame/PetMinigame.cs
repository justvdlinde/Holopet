using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minigame;
using UnityEngine.SceneManagement;

namespace Minigame {

    public class PetMinigame : Pet {

        private static PetMinigame instance;
        public static PetMinigame Instance {
            get {
                if (instance == null)
                    instance = FindObjectOfType<PetMinigame>();
                return instance;
            }
        }

        public int happyCountNeededToWin = 6;
        public int happyCount;
        public float happyIncentiveIncreasePerCorrectMotivationalCommand = 25;
        public float happyIncentiveDecreasePerWrongMotivationalCommand = 15;
        public float happyIncentiveIncreasePerCorrectPunishmentCommand = 25;
        public float happyIncentiveIncreasePerWrongPunishmentCommand = 15;

        public float movementSpeedOnAngry, movementSpeedOnSad, movementSpeedOnHappy;

        [SerializeField] private AudioClip[] happyAudio, sadAudio, angryAudio, punishmentAudio, motivationAudio, gameOverAudio;

        public Emotion lastEmotionCollected;

        [SerializeField]
        private float incentiveToCollectHappyResource;
        public float IncentiveToCollectHappyResource {
            get { return incentiveToCollectHappyResource; }
            set { if (value >= 0)
                    incentiveToCollectHappyResource = value;
                if (incentiveToCollectHappyResource > 100)
                    incentiveToCollectHappyResource = 100;
            }
        }
        public System.Action OnGameOver;

        private AudioSource audioSrc;


        private void Awake() {
            instance = this;
            audioSrc = GetComponent<AudioSource>();
        }

        public void OnResourceCollected(Resource resource) {
            resource.OnCollected();
            lastEmotionCollected = resource.emotion;
            Debug.Log("collected " + resource.emotion);
            if (resource.emotion == Emotion.Happy) {
                happyCount++;
                if (happyCount == happyCountNeededToWin)
                    GameOver();
                //else
                    //PlayRandomHappySound();
                Head.MovementSpeed = movementSpeedOnHappy;
            } else {
                if (happyCount > 1)
                    happyCount -= 2;
                if (resource.emotion == Emotion.Angry) {
                    Head.MovementSpeed = movementSpeedOnAngry;
                    //PlayRandomAngrySound();
                } else {
                    Head.MovementSpeed = movementSpeedOnSad;
                    //PlayRandomSadSound();
                }
            }
        }

        public void OnMotivatationRecieved() {
            if (lastEmotionCollected == Emotion.Happy)
                IncentiveToCollectHappyResource += happyIncentiveIncreasePerCorrectMotivationalCommand;
            else
                IncentiveToCollectHappyResource -= happyIncentiveDecreasePerWrongMotivationalCommand;

            if (!audioSrc.isPlaying)
                audioSrc.PlayOneShot(motivationAudio.GetRandom());
        }

        public void OnPunishmentRecieved() {
            if (lastEmotionCollected == Emotion.Happy)
                IncentiveToCollectHappyResource -= happyIncentiveDecreasePerWrongMotivationalCommand;
            else
                IncentiveToCollectHappyResource += happyIncentiveIncreasePerCorrectPunishmentCommand;

            if (!audioSrc.isPlaying)
                audioSrc.PlayOneShot(punishmentAudio.GetRandom());
        }

        public void PlayRandomHappySound() {
            if (!audioSrc.isPlaying)
                audioSrc.PlayOneShot(happyAudio.GetRandom());
        }

        public void PlayRandomAngrySound() {
            if (!audioSrc.isPlaying)
                audioSrc.PlayOneShot(angryAudio.GetRandom());
        }

        public void PlayRandomSadSound() {
            if (!audioSrc.isPlaying)
                audioSrc.PlayOneShot(sadAudio.GetRandom());
        }

        public void PlayRandomGameOverSound() {
            audioSrc.PlayOneShot(gameOverAudio.GetRandom());
        }

        private void GameOver() {
            Debug.Log("Game Over");
            Resources.Clear();
            OnGameOver();
            Head.SlowDown();
            PlayRandomGameOverSound();
            StartCoroutine(MenuCountdown());
        }

        private IEnumerator MenuCountdown() {
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(1);
        }
    }
}