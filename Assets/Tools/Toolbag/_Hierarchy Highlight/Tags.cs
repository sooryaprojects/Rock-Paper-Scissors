using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Tags
{
    [SerializeField]
    public List<string> tagNames = new List<string>();
    [SerializeField]
    public List<Color> tagColors = new List<Color>();

    //TODO
    //get rid of tagNames and use instance IDs to identify the highlighted objects
    //maintain a list of all the IDs.

    //multiple objects can have same tags?

    public List<int> instanceID = new List<int>();
}

