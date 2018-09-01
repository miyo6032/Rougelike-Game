using UnityEngine;

/// <summary>
/// Holds the state for one piece of dialogue in a conversation
/// </summary>
[System.Serializable]
public class Dialogue
{
    [TextArea(3, 10)]
    public string sentence;

    public string name;
    public Sprite portrait;
}