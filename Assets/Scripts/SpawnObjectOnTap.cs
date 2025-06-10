using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnObjectOnTap : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn; 
     private GameObject spawnedObject; 
    [SerializeField] private ARRaycastManager raycastManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); 


    private void Start()
    {
        spawnedObject = null;
    }

    void Update()
    {
       if(Input.touchCount == 0) return;

        if (raycastManager.Raycast(Input.GetTouch(0).position, hits)) {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null) 
                SpawnPrefab(hits[0].pose.position);
            
            else if(Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null) 
                spawnedObject.transform.position = hits[0].pose.position;
               
        }
    }

    void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObject = Instantiate(objectToSpawn,spawnPosition,Quaternion.identity);
    }


      
}