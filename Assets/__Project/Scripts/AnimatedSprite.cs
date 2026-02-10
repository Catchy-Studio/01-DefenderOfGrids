using UnityEngine;

namespace __Project.Scripts
{
    public class AnimatedSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rend;
        [SerializeField] private Animator animator;

        public enum AnimationState
        {
            Idle =0,
            Attack =1,
            TakeDamage =2
        }

        public SpriteRenderer Rend => rend;

        public Animator Animator => animator;


        public void ChangeAnim()
        {
            
        }
        
    }
}