//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Class that handles spawning sync objects on keyboard presses, for the SpawningTest scene.
    /// </summary>
    public class SyncObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private PrefabSpawnManager spawnManager;

        [SerializeField]
        [Tooltip("Optional transform target, for when you want to spawn the object on a specific parent.  If this value is not set, then the spawned objects will be spawned on this game object.")]
        private Transform spawnParentTransform;

        private void Awake()
        {
            if (spawnManager == null)
            {
                Debug.LogError("You need to reference the spawn manager on SyncObjectSpawner.");
            }

            // If we don't have a spawn parent transform, then spawn the object on this transform.
            if (spawnParentTransform == null)
            {
                spawnParentTransform = transform;
            }
        }

        public Vector3 UserRelativeLocation(int offset = 0)
        {
            Vector3 spawnerPos = GameObject.FindGameObjectWithTag("spawner").transform.position;
            Vector3 userPos = (Camera.main.transform.position + (Camera.main.transform.forward * offset));

            return (userPos - spawnerPos);
        }

        public void SpawnBasicSyncObject()
        {
            Vector3 position = Random.onUnitSphere * 2;
            Quaternion rotation = Random.rotation;

            var spawnedObject = new SyncSpawnedObject();

            spawnManager.Spawn(spawnedObject, position, rotation, spawnParentTransform.gameObject, "SpawnedObject", false);
        }

        public void SpawnCustomSyncObject()
        {
            Vector3 position = Random.onUnitSphere * 2;
            Quaternion rotation = Random.rotation;

            var spawnedObject = new SyncSpawnTestSphere();
            spawnedObject.TestFloat.Value = Random.Range(0f, 100f);

            spawnManager.Spawn(spawnedObject, position, rotation, spawnParentTransform.gameObject, "SpawnTestSphere", false);
        }

        public void SpawnUnityChan()
        {
            Vector3 position = UserRelativeLocation(2);
            position.y = 0; //im not quite sure what her height is position.y - 1;

            //rotation
            var rotation = Camera.main.transform.rotation;
            /*
            var rot = Camera.main.transform.eulerAngles;
            rot.y = rot.y + 180; //rotate it 180 to face you
            rot.x = 0;
            rot.z = 0;
            Quaternion rotation = Quaternion.Euler(rot);
            */
            var spawnObject = new SyncSpawnUnityChan();
            spawnObject.TestFloat.Value = Random.Range(0f, 100f);

            spawnManager.Spawn(spawnObject, position, rotation, spawnParentTransform.gameObject, "SpawnUnityChan", false);
        }

        /// <summary>
        /// Deletes any sync object that inherits from SyncSpawnObject.
        /// </summary>
        public void DeleteSyncObject()
        {
            GameObject hitObject = GazeManager.Instance.HitObject;
            if (hitObject != null)
            {
                var syncModelAccessor = hitObject.GetComponent<DefaultSyncModelAccessor>();
                if (syncModelAccessor != null)
                {
                    var syncSpawnObject = (SyncSpawnedObject)syncModelAccessor.SyncModel;
                    spawnManager.Delete(syncSpawnObject);
                }
            }
        }

        //Doesnt actually reset the world, but keeps the anchor position and deletes all objects spawned so far
        public void ResetWorld()
        {
            GameObject spawner = GameObject.FindGameObjectWithTag("spawner");
            for (int i = 0; i < spawner.transform.childCount; i++)
            {
                Debug.Log(spawner.transform.GetChild(i).tag);
                if (spawner.transform.GetChild(i).tag != "nodelete" || spawner.transform.GetChild(i).tag != "floor")
                {
                    var model = spawner.transform.GetChild(i).GetComponent<DefaultSyncModelAccessor>();
                    if (model != null)
                    {
                        spawnManager.Delete(((SyncSpawnedObject)(model.SyncModel)));
                    }
                    else
                    {
                        GameObject.Destroy(spawner.transform.GetChild(i).gameObject);
                    }
                }
            }
        }

        //Deletes everything and then reloads scene
        public void ResetScene()
        {
            ResetWorld();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
