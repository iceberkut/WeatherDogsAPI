using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingIndicatorView : MonoBehaviour
    {
        [SerializeField] private Image spinnerImage;
        [SerializeField] private float rotationSpeed = 360f;

        private void Update()
        {
            spinnerImage.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }
    }
} 