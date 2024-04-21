using MK.ExplodingView.Utils;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace MK.ExplodingView.Core
{
    /// <summary>
    /// Can be added to any explodable part to modify its explosion axis.
    /// </summary>
    [DisallowMultipleComponent]
    public class ExplodableModifier : MonoBehaviour
    {
        [SerializeField]
        public uint Order = 0;

        [SerializeField]
        public ModifierProperty ModifierProperty = ModifierProperty.None;

        [SerializeField]
        public ModifierAxis Axis = ModifierAxis.PosX;

        [SerializeField]
        public Vector3 LocalPosition = Vector3.zero;

        [SerializeField]
        public bool AffectChildren = false;
    }
}