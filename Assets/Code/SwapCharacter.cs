using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class SwapCharacter : MonoBehaviour
    {
        public GameObject Character1, Character2;

        private int currentCharacter = 1;
        
        // Start is called before the first frame update
        void Start()
        {
            Character1.gameObject.SetActive(true);
            Character2.gameObject.SetActive(false);
        }

        public void Swap()
        {
            switch (currentCharacter)
            {
                case 1:
                    currentCharacter = 2;
                    
                    Character1.gameObject.SetActive(false);
                    Character2.gameObject.SetActive(true);
                    break;
                case 2:
                    currentCharacter = 1;
                    Character1.gameObject.SetActive(true);
                    Character2.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
