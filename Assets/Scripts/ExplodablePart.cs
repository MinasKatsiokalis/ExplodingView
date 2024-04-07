using DG.Tweening;
using System;
using UnityEngine;

namespace MK.ExplodingView
{
    [Serializable]
    public class ExplodablePart : MonoBehaviour, IExplodable
    {
        #region Public Variables

        public Vector3 OriginalPosition { set; get; }
        public Vector3 ExplodedPosition { set; get; }
        public float Duration { set; get; }

        #endregion

        #region Private Variables

        private bool isExploded = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Moves the part to exploded position and back.
        /// </summary>
        public void Explode()
        {
            if (!isExploded)
            {
                transform.DOMove(ExplodedPosition, Duration);
                isExploded = true;
            }
            else
            {
                transform.DOMove(OriginalPosition, Duration);
                isExploded = false;
            }
        }

        #endregion
    }
}
