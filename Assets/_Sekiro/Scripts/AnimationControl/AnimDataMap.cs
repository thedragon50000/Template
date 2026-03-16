using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimDataMap", menuName = "Scriptable Objects/AnimDataMap")]
public class AnimDataMap : ScriptableObject
{
    [Serializable]
    public class StateData
    {
        public string stateName;
        public int hash;
        public float length;

        // public AnimationClip clip;
    }

    public List<StateData> states = new List<StateData>();
}
