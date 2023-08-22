using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Case2
{
    public class MoveObject : MonoBehaviour
    {
        [SerializeField] Space curSpace;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Space>() != null)
            {
                curSpace = other.GetComponent<Space>();
            }
        }
    }

}