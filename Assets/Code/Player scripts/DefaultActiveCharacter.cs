using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class DefaultActiveCharacter : MonoBehaviour
    {
        [SerializeField] private GameObject characterOne;
        [SerializeField] private GameObject characterTwo;

        private void Awake()
        {
            characterOne.SetActive(true);
            characterTwo.SetActive(false);
        }
    }
}
