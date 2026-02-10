using _NueCore.Common.Utility;
using UnityEngine;

namespace _NueExtras.TutorialSystem._Hands
{
    public class HandSwipeTutorialUI : MonoBehaviour
    {
        [SerializeField] private Transform handTransform;
        [SerializeField] private GameObject openModel;
        [SerializeField] private GameObject closeModel;

        [SerializeField] private Transform point1;
        [SerializeField] private Transform point2;
        
        [SerializeField] private float swipeDuration = 1f;

        private float _timer = 0f;
        
        

        private void Awake()
        {
            openModel.SetActive(true);
            closeModel.SetActive(false);
        }


        private void Update()
        {
            if(point1 && point2)
            {
                _timer += Time.deltaTime;
                if(_timer > swipeDuration)
                {
                    _timer = swipeDuration;
                }
                var t = EaseHelper.EaseInOutSine(_timer, swipeDuration);
                handTransform.position = Vector3.Lerp(point1.position, point2.position, t);
                
                if(t >= .2f && t <= .8f)
                {
                    openModel.SetActive(false);
                    closeModel.SetActive(true);
                }
                else
                {
                    openModel.SetActive(true);
                    closeModel.SetActive(false);
                }
                
                if (_timer >= swipeDuration)
                {
                    _timer = 0f;
                }
            }
            
            
            
            
        }
    }
}