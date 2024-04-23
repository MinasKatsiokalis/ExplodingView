using System;

namespace MK.ExplodingView.Utils
{
    /// <summary>
    /// The direction of the explosion parts.
    /// </summary>
    [Serializable]
    public enum Direction
    {
        FromCenter,
        FromAxis,
        FromPlane,
        FromParent
    }

    /// <summary>
    /// The axis according to which the explosion is going to happen.
    /// </summary>
    [Serializable]
    public enum Axis
    {
        Y,
        X,
        Z,
        YX,
        YZ,
        XZ,
        XYZ
    }

    /// <summary>
    /// Distance factor to be used for the explosion.
    /// </summary>
    [Serializable]
    public enum DistanceFactor
    {
        DistanceFromCenter,
        DistanceFromParent,
        DistanceFromProjectionPoint,
        StaticDistance
    }

    /// <summary>
    /// The reference point for the static distance.
    /// </summary>
    [Serializable]
    public enum StaticDistanceReference
    {   
        Self,
        Parent,
        Center
    }

    /// <summary>
    /// Properties that can be modified by the ExplodableModifier.
    /// </summary>
    [Serializable]
    public enum ModifierProperty
    {
        None,
        Axis,
        LocalPosition
    }

    /// <summary>
    /// The axis of the modifier.
    /// This is enabled when the ModifierProperty is set to Axis.
    /// </summary>
    [Serializable]
    public enum ModifierAxis
    {
        PosX,
        NegX,
        PosY,
        NegY,
        PosZ,
        NegZ
    }

    /// <summary>
    /// Scale factor to be used for the explosion.
    /// </summary>
    [Serializable]
    public enum ScaleFactor
    {
        LargerFurther,
        SmallerFurther,
    }
}
