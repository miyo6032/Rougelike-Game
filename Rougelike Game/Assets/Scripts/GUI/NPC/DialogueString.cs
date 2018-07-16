using UnityEngine;

/// <summary>
/// Holds info for a conversation with an npc
/// </summary>
[System.Serializable]
public class DialogueString
{
    [TextArea(3, 10)]
    public string[] sentences;
    public string name;
}
