using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using Random = UnityEngine.Random;

namespace Minigame {

    public static class Resources {

        private static List<Resource> allResources = new List<Resource>();
        public static List<Resource> AllResources {
            get { return allResources; }
            private set { allResources = value; }
        }

        public static Action OnResourceRemoved;

        public static void Add(Resource r) {
            AllResources.Add(r);
        }

        public static void Remove(Resource r) {
            if (allResources.Contains(r))
                AllResources.Remove(r);
            if (OnResourceRemoved != null) {
                OnResourceRemoved.Invoke();
            }
        }

        public static void Clear() {
            OnResourceRemoved = null;
            for (int i = allResources.Count - 1; i >= 0; i--) {
                allResources[i].OnCollected();
            }
        }

        public static Resource GetRandom() {
            return allResources.GetRandom();
        }

        public static Resource GetRandomDependingOnIncentive(float happyIncentive, Vector3 position) {
            float chanceHappy = happyIncentive / 100;
            float rnd = Random.value;
            if (rnd < chanceHappy) {
                return GetNearestResourceWithColor(EmotionColor.Blue, position);
            } else {
                if (rnd > 0.5f)
                    return GetNearestResourceWithColor(EmotionColor.Red, position);
                else
                    return GetNearestResourceWithColor(EmotionColor.Yellow, position);
            }
        }

        public static Resource GetNearestResource(Transform t) {
            float closestDist = Mathf.Infinity;
            Resource closestResource = null;
            foreach (Resource r in allResources) {
                float dist = Vector3.Distance(r.transform.position, t.position);
                if (dist < closestDist) {
                    closestDist = dist;
                    closestResource = r;
                }
            }
            return closestResource;
        }

        public static Resource GetNearestResourceWithColor(EmotionColor color, Vector3 position) {
            Emotion correspondingEmotion = (Emotion)((int)color);
            List<Resource> resourcesWithCorrespondingColor = AllResources.Where(x => x.emotion == correspondingEmotion).ToList();

            if (resourcesWithCorrespondingColor.Count == 0)
                return null;

            float closestDist = Mathf.Infinity;
            Resource closestResource = null;
            foreach (Resource r in resourcesWithCorrespondingColor) {
                if (r == null)
                    continue;
                float dist = Vector3.Distance(r.transform.position, position);
                if (dist < closestDist) {
                    closestDist = dist;
                    closestResource = r;
                }
            }

            return closestResource;
        }

        public static Resource GetNearestResourceHorizontal(Transform t, bool getRight) {
            List<Resource> correspondingResources = AllResources.Where(x => Common.IsRightFromObject(t, x.transform) == getRight).ToList();
            float closestDist = Mathf.Infinity;
            Resource closestResource = null;
            foreach (Resource r in correspondingResources) {
                //if ((getRight && !Common.IsRightFromObject(t, r.transform)) || (!getRight && Common.IsRightFromObject(t, r.transform)))
                //    continue;

                float dist = Vector3.Distance(r.transform.position, t.position);
                if (dist < closestDist) {
                    closestDist = dist;
                    closestResource = r;
                }
            }
            return closestResource;
        }
    }
}