using UnityEngine;

// Notice this says 'public interface' instead of 'public class'
// and does NOT inherit from MonoBehaviour
public interface IBuffable
{
    // Applies a percentage-based reduction to the attack cooldown
    void ApplySpeedBuff(float buffPercentage);
    
    // Removes the buff and resets to base speed
    void RemoveSpeedBuff(float buffPercentage);
}