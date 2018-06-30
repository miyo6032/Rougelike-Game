using UnityEngine;

/// <summary>
/// A global particle manager to produce enemy damage particles
/// </summary>
public class ParticleManager : MonoBehaviour
{
    private ParticleSystem damageParticles;
    public static ParticleManager instance;

    void Start()
    {
        instance = this;
        damageParticles = HelperScripts.GetComponentFromChildrenExc<ParticleSystem>(transform);
    }

    public void Emit(ParticleSystem.EmitParams emitParams, int amount)
    {
        damageParticles.Emit(emitParams, amount);
    }
}
