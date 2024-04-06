using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class ExplodablePart : MonoBehaviour, IExplodable
{
    #region Properties

    #region Public Variables

    public Vector3 OriginalPosition { set; get; }
    public Vector3 ExplodedPosition { set; get; }
    public float Duration { set; get; }

    #endregion

    #region Private Variables

    private bool isExploded = false;

    #endregion

    #endregion

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
}
