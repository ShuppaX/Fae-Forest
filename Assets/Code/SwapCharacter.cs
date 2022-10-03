using System.Collections;
using System.Collections.Generic;
using RemixGame.Code;
using UnityEngine;

namespace RemixGame
{
    public class SwapCharacter : MonoBehaviour
    {
        public GameObject Character1, Character2;

        [SerializeField] private Rigidbody2D p1rigid;
        [SerializeField] private Rigidbody2D p2rigid;
        void Start()
        {
            //Starts with first character active
            Character1.gameObject.SetActive(true);
            Character2.gameObject.SetActive(false);
        }

        //changes the player characters back and forth by activating the inactive object

        //swap method for the 1st character into second one
        public void Swap1()
        {
            //maintain position when swapping
            Character2.transform.position = Character1.transform.position;
            //maintain current velocity
            p2rigid.velocity = p1rigid.velocity;
            //object swap
            Character1.gameObject.SetActive(false);
            Character2.gameObject.SetActive(true);

        }

        //swaps second chara back to 1st one
        public void Swap2()
        {
            //maintain position when swapping
            Character1.transform.position = Character2.transform.position;
            //maintain current velocity
            p1rigid.velocity = p2rigid.velocity;
            //object swap
            Character1.gameObject.SetActive(true);
            Character2.gameObject.SetActive(false);
        }
    }
}
