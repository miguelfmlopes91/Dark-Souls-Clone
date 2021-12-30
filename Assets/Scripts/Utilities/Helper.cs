using UnityEngine;

namespace Utilities
{
    public class Helper : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float vertical;
        private Animator anim;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            anim.SetFloat("vertical", vertical);
        }
    }
}

