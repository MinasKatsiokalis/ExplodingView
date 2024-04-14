using MK.ExplodingView.Utils;
using System;
using UnityEngine;

namespace MK.ExplodingView.Core
{   
    /// <summary>
    /// Can be added to any explodable part to modify its explosion axis.
    /// </summary>
    public class ExplodableModifier : MonoBehaviour
    {
        public ModifierAxis Axis;
        public uint Order = 0;
        public bool UseSelfDistance = false;
        [SerializeField]
        public float Distance
        {
            get { return distance; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Value cannot be negative.");
                distance = value;
            }
        }
        private float distance = 0;
    }
}