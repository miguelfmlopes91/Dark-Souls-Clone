using UnityEngine;

namespace Utilities
{
    public class Helper : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float vertical;
        [Range(-1, 1)] [SerializeField] private float horizontal;


        [SerializeField] private bool playAnim;
        [SerializeField] private bool twoHanded;
        [SerializeField] private bool enableRootMotion;
        [SerializeField] private bool useItem;
        [SerializeField] private bool interacting;
        [SerializeField] private bool lockOn;
        
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
            interacting = anim.GetBool("interacting");

            if (!lockOn)
            {
                horizontal = 0;
                vertical = Mathf.Clamp(vertical, 0, 1f);
            }
            
            anim.SetBool("lockOn", lockOn);
            
            if (enableRootMotion)
            {
                return;
            }
            
            if (useItem)
            {
                anim.Play("use_item");
                useItem = false;
            }

            if (interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0, 0.5f);
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
                
                if (vertical > 0.5f)
                {
                    targetAnim = "oh_attack_3";   
                }
                
                vertical = 0;
                
                anim.CrossFade(targetAnim, 0.2f);
                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal", horizontal);
        }
    }
}

