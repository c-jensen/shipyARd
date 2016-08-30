using UnityEngine;
using System.Collections;

public class DoNotDestroyOnLoadScript : MonoBehaviour
{

    // This Script prevents the Game Object from beeing destroyed, if the scene is changed
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {

    }
}
