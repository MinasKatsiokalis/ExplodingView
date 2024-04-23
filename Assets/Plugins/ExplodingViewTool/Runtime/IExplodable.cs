using UnityEngine;

namespace MK.ExplodingView.Core
{
    public interface IExplodable
    {
        Vector3 OriginalPosition { set; get; }
        Vector3 ExplodedPosition { set; get; }
        void Explode();
    }
}
