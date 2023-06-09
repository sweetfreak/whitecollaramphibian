using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    void Awake()
    {
        //this is a singleton pattern
        int numScenePersists = FindObjectsOfType<ScenePersist>().Length;

        if (numScenePersists > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public void ResetScenePersist()
    {
        
        Destroy(gameObject);
    }
    
    
    
}
