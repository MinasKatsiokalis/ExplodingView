using UnityEngine;

public interface IExplodable
{
    Vector3 OriginalPosition { set;  get; }
    Vector3 ExplodedPosition { set;  get; }
    public void Explode();
}
