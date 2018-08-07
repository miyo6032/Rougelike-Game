using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Effect")]
public class Effect : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int duration;
    public bool applyOnce;
    public bool removeAfterDone;
    public bool isPermanent;
    public Modifier[] ModifiersAffected;
}