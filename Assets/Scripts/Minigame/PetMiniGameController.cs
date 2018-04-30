using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame {

    public enum Direction {
        up = 0,
        right = 1,
        down = 2,
        left = 3
    }

    public class PetMiniGameController : MonoBehaviour {

        [SerializeField] private PetHead head;
        [SerializeField] private float directMovementDistance = 5;
        [SerializeField] private PathfinderMinigame pathfinder;
        [SerializeField] private float maxFindingDistance = 5;

        [Tooltip("Commands for direct movements, such as: 'Go Left'")]
        [SerializeField] private string[] directMovementCommands;
        [Tooltip("Commands for the nearest direction, such a:s 'get the nearest left one'")]
        [SerializeField] private string[] nearestMovementCommands;
        [Tooltip("Commands for the nearest color, such as: 'get the blue one'")]
        [SerializeField] private string[] nearestColorCommands;
        [Tooltip("Commands for the nearest resource")]
        [SerializeField] private string[] nearestResourceCommands;
        [Tooltip("Motivational Commands such as 'Good Job'")]
        [SerializeField] private string[] motivationalCommands;
        [Tooltip("Punishing Commands such as 'No'")]
        [SerializeField] private string[] punishingCommands;
        [Tooltip("Misc commands")]
        [SerializeField] private VoiceCommand[] miscCommands;

        private const string COMMAND_KEYWORD = "<>";
        private SpeechManager speechManager;

        private Vector3[] directions = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
        private List<string> directionalMovementCommandsList;
        private List<string> nearestMovementCommandsList;
        private List<string> nearestColorCommandsList;

        [SerializeField] // for testing
        private string[] allCommands;

        private void Start() {
            allCommands = GetAllKeywords();
            speechManager = new SpeechManager(GetAllKeywords());
            speechManager.OnPhraseRecognized += ParseCommandToKeyword;
            pathfinder.walker.OnTargetReached += pathfinder.SetNewRandomResourceTarget;
        }

        private string[] GetAllKeywords() {
            List<string> keywords = new List<string>();

            //directionalMovementCommandsList = ParseKeywordsToCommands(directMovementCommands, new Direction());
            //keywords.AddRange(directionalMovementCommandsList);
            //nearestMovementCommandsList = ParseKeywordsToCommands(nearestMovementCommands, new Direction());
            //keywords.AddRange(nearestMovementCommandsList);
            //keywords.AddRange(nearestResourceCommands);

            //nearestColorCommandsList = ParseKeywordsToCommands(nearestColorCommands, new EmotionColor());
            //keywords.AddRange(nearestColorCommandsList);

            keywords.AddRange(motivationalCommands);
            keywords.AddRange(punishingCommands);

            foreach (VoiceCommand v in miscCommands) {
                foreach(string s in v.commands)
                keywords.Add(s);
            }
            return keywords.ToArray();
        }

        private List<string> ParseKeywordsToCommands(string[] commands, Enum enumType) {
            List<string> keywords = new List<string>();
            int count = Enum.GetNames(enumType.GetType()).Length;
            foreach (string s in commands) {
                if (s.Contains(COMMAND_KEYWORD)) {
                    for (int i = 0; i < count; i++) {
                        string keyword = Enum.ToObject(enumType.GetType(), i).ToString();
                        string temp = s.Replace(COMMAND_KEYWORD, keyword);
                        keywords.Add(temp);
                    }
                } else {
                    keywords.Add(s);
                }
            }
            return keywords;
        }

        private void ParseCommandToKeyword(string text) {
            Debug.Log("Command: <b>" + text + "</b>");

            //if (directionalMovementCommandsList.Contains(text))
            //    DirectMoveCommandRecieved(text);
            //else if (nearestMovementCommandsList.Contains(text))
            //    NearestMoveCommandRecieved(text);
            //else if (nearestResourceCommands.Contains(text))
            //    NearestResourceCommandRecieved(text);
            //if (nearestColorCommandsList.Contains(text))
            //    NearestColorCommandRecieved(text);
            if (motivationalCommands.Contains(text))
                MotivationalCommandRecieved(text);
            else if (punishingCommands.Contains(text))
                PunishingCommandRecieved(text);
            else {
                VoiceCommand misc;
                if (MiscCommandsContains(text, out misc)) {
                    misc.onCommandRecieved.Invoke();
                } else {
                    Debug.LogWarning("No corresponding command found for " + text);
                }
            }
        }

        private void DirectMoveCommandRecieved(string command) {
            Debug.Log("move command keyword: " + command);
            Direction dirEnum = GetDirectionFromCommand(command);
            Vector3 direction = directions[(int)dirEnum];
            Vector3 targetPosition = head.transform.position + (direction * directMovementDistance);
            pathfinder.SetNewTargetPosition(targetPosition);
        }

        private void NearestMoveCommandRecieved(string command) {
            Debug.Log("nearest movement command keyword: " + command);
            Direction dirEnum = GetDirectionFromCommand(command);
            if (dirEnum == Direction.up || dirEnum == Direction.down)
                return;

            Resource closestResource = Resources.GetNearestResourceHorizontal(transform, dirEnum == Direction.right);

            if (closestResource != null && Vector3.Distance(head.transform.position, closestResource.transform.position) < maxFindingDistance)
                pathfinder.SetNewTargetPosition(closestResource.transform.position);
            else
                Debug.Log("No resource to the " + dirEnum.ToString() + " found within range");
        }

        private void NearestResourceCommandRecieved(string command) {
            if (PetMinigame.Instance.lastEmotionCollected == Emotion.Angry) {
                Debug.Log("nearest resource command keyword: " + command + "is angry so not listening");
                return;
            }

            Debug.Log("nearest resource command keyword: " + command);
            Resource closestResource = Resources.GetNearestResource(head.transform);

            if (closestResource != null && Vector3.Distance(head.transform.position, closestResource.transform.position) < maxFindingDistance)
                pathfinder.SetNewTargetPosition(closestResource.transform.position);
            else
                Debug.Log("No resource found within range");
        }

        private void NearestColorCommandRecieved(string command) {
            Debug.Log("nearest color command keyword: " + command);
            EmotionColor color = GetColorEmotionFromCommand(command);
            Resource closestResource = Resources.GetNearestResourceWithColor(color, head.transform.position);

            head.MovementSpeed = head.movementStartingSpeed;

            if (closestResource != null && Vector3.Distance(head.transform.position, closestResource.transform.position) < maxFindingDistance)
                pathfinder.SetNewTargetPosition(closestResource.transform.position);
            else
                Debug.Log("No resource with " + color.ToString() + " found within range");
        }

        private void MotivationalCommandRecieved(string command) {
            Debug.Log("motivational command keyword: " + command);
            PetMinigame.Instance.OnMotivatationRecieved();
        }

        private void PunishingCommandRecieved(string command) {
            Debug.Log("punishment command keyword: " + command);
            PetMinigame.Instance.OnPunishmentRecieved();
        }

        private Direction GetDirectionFromCommand(string text) {
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                if (text.Contains(dir.ToString()))
                    return dir;
            }

            Debug.LogWarning("No direction found in " + text);
            return Direction.up;
        }

        private EmotionColor GetColorEmotionFromCommand(string text) {
            foreach (EmotionColor dir in Enum.GetValues(typeof(EmotionColor))) {
                if (text.Contains(dir.ToString()))
                    return dir;
            }

            Debug.LogWarning("No emotionColor found in " + text);
            return EmotionColor.Blue;
        }

        public void StopMoving() {
            pathfinder.Stop();
        }

        public void QuitMiniGame() {
            throw new NotImplementedException();
        }

        public void SlowDown() {
            head.SlowDown();
        }

        public bool MiscCommandsContains(string s, out VoiceCommand command) {
            foreach (VoiceCommand v in miscCommands) {
                if (v.commands.Contains(s)) {
                    command = v;
                    return true;
                }
            }
            command = null;
            return false;
        }

        private void OnDestroy() {
            pathfinder.walker.OnTargetReached -= pathfinder.SetNewRandomResourceTarget;
        }

        //private void OnDrawGizmosSelected() {
        //    Gizmos.DrawWireCube(head.transform.position, new Vector3(directMovementDistance * 2, 0, directMovementDistance * 2));
        //    UnityEditor.Handles.DrawWireArc(head.transform.position, head.transform.up, -head.transform.right, 360, maxFindingDistance);
        //}
    }

}