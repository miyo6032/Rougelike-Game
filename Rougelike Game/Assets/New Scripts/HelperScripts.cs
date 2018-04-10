﻿using UnityEngine;

//Random hodgepodge of scripts that help with many different things
public class HelperScripts {

    public static T GetComponentFromChildrenExc<T>(Transform parent)
    where T : Component
    {
        T[] children = parent.GetComponentsInChildren<T>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].transform != parent)
            {
                return children[i];
            }
        }
        return null;
    }

}
