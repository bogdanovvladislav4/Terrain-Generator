using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainProceduralGenerator
{
    public class HideOnPlay : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(false);
        }
    }
}
