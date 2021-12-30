using UnityEngine;

namespace Utilities
{
    public class Helper : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float vertical;


        [SerializeField] private string animName;
        [SerializeField] private bool playAnim;
        [SerializeField] private bool twoHanded;
        [SerializeField] private bool enableRootMotion;
        
        [SerializeField]private string[] oh_attacks;
        [SerializeField]private string[] th_attacks;
        private Animator anim;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            enableRootMotion = !anim.GetBool("canMove");
            anim.applyRootMotion = enableRootMotion;

            if (enableRootMotion)
            {
                return;
            }
            
            anim.SetBool("two_handed", twoHanded);
            if (playAnim)
            {
                string targetAnim;
                if (!twoHanded)
                {
                    int r = Random.Range(0, oh_attacks.Length);
                    targetAnim = oh_attacks[r];
                }
                else
                {
                    int r = Random.Range(0, th_attacks.Length);
                    targetAnim = th_attacks[r];
                }
                vertical = 0;
                
                anim.CrossFade(targetAnim, 0.2f);
                /*anim.SetBool("canMove", false);
                enableRootMotion = true;*/
                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
        }
    }
}

