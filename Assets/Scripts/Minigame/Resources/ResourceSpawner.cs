using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Minigame {

    public class ResourceSpawner : MonoBehaviour {

        private static ResourceSpawner instance;
        public static ResourceSpawner Instance { get { return instance; } }

        [SerializeField] private Resource[] resourcePrefabs;
        [SerializeField] private float timeBetweenSpawnsMin = 1, timeBetweenSpawnsMax = 2;
        [SerializeField] private int wantedAmountInScene = 10;

        public Vector3 range = new Vector3(3, 3, 3);

        private Emotion lastSpawned;

        private void Awake() {
            instance = this;
            Resources.OnResourceRemoved += SpawnResourceAtRandomPosition;
            for (int i = 0; i < wantedAmountInScene; i++)
                SpawnResourceAtRandomPosition();

            PetMinigame.Instance.OnGameOver += () => { enabled = false; };
        }

        private void SpawnResourceAtRandomPosition() {
            if (Resources.AllResources.Count > wantedAmountInScene)
                return;

            List<Resource> resourcesEligibleForSpawn = new List<Resource>();
            foreach (Resource p in resourcePrefabs) {
                if(p.emotion != lastSpawned)
                    resourcesEligibleForSpawn.Add(p);
            }
            Vector3 rndSpawnLocation = Common.GetRandomPositionWithinRange(range, transform.position);
            Resource r = Instantiate(resourcesEligibleForSpawn.GetRandom(), rndSpawnLocation, Random.rotation);
            lastSpawned = r.emotion;
            Resources.Add(r);
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, range);
        }

        private void OnDestroy() {
            Resources.OnResourceRemoved -= SpawnResourceAtRandomPosition;
        }
    }
}