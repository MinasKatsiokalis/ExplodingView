using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;
using System.Threading.Tasks;

public class ThreeDModelFunctions : MonoBehaviour
{
    #region Properties
    public List<ExplodablePart> Explodables;
    public float explosionSpeed = 0.1f;
    public float explosionDistance = 1.5f;

    [SerializeField]
    private Transform _center;
    [SerializeField]
    private bool _addExplodableComponent = false;

    [Header("Prefabs")]
    [SerializeField]
    GameObject pointPrefab;

    bool isInExplodedView = false;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        Explodables = new List<ExplodablePart>();
    }

    private async void Start()
    {
        //Line
        Vector3 lineDirection = Vector3.up;
        //Center
        if (_center == null)
            await CalculateCenter();

        if(_addExplodableComponent)
            AddExplodableComponent();
        
        InitializeExplodables(lineDirection);
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Toggle the exploded view of the model.
    /// If in exploded view, return to the original position.
    /// </summary>
    public void ToggleExplodedView()
    {
        Sequence mySequence = DOTween.Sequence();
        foreach (IExplodable explodable in Explodables)
            explodable.Explode();
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Initialize the explodable parts of the model.
    /// Part's exploding direction is calculated based on the <paramref name="lineDirection"/>.
    /// Part's exploding direction is always perpendicular to that direction. 
    /// </summary>
    /// <param name="lineDirection"></param>
    private void InitializeExplodables(Vector3 lineDirection)
    {   
        var rotation = transform.rotation;

        foreach (ExplodablePart explodable in Explodables)
        {
            explodable.OriginalPosition = explodable.transform.position;
            explodable.Duration = explosionSpeed;

            // Calculate the vector from the point on the line to the transform's position
            Vector3 toTransform = explodable.OriginalPosition - _center.position;
            // Project this vector onto the line direction
            Vector3 projection = Vector3.Project(toTransform, _center.up);
            // Calculate the point on the line
            Vector3 pointPosition = _center.position + projection;
            var point = Instantiate(pointPrefab, pointPosition, _center.rotation, explodable.transform.parent);
            
            point.name = explodable.name + "_point";
            //point.transform.LookAt(explodable.OriginalPosition);

            Vector3 explosionDirection = (explodable.OriginalPosition - point.transform.position).normalized;

            float distanceFromCenter = Vector3.Distance(explodable.OriginalPosition, _center.position);
            float distanceFromPoint = Vector3.Distance(explodable.OriginalPosition, point.transform.position);

            explodable.ExplodedPosition = explodable.OriginalPosition + explosionDirection* (explosionDistance* distanceFromCenter);
        }
    }

    #region Helpers
    /// <summary>
    /// Calculate the center of the model.
    /// </summary>
    private async Task CalculateCenter()
    {
        _center = new GameObject("Center").transform;
        _center.SetParent(this.transform);

        List<Vector3> meshes = new List<Vector3>();

        MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
        foreach (var item in meshFilters)
            meshes.Add(item.mesh.bounds.center);

        _center.position = await Task.Run(() => GetAverageCenter(meshes));
        _center.rotation = this.transform.rotation;
    }

    /// <summary>
    /// Calculate the average point of all the <paramref name="meshes"/>' centers.
    /// </summary>
    /// <param name="meshes"></param>
    /// <returns></returns>
    private Vector3 GetAverageCenter(List<Vector3> meshes)
    {
        Vector3 center = Vector3.zero;

        foreach (Vector3 meshCenter in meshes)
            center += meshCenter;

        if (meshes.Count > 0)
            center /= meshes.Count;

        return center;
    }  

    /// <summary>
    /// Add the <see cref="ExplodablePart"/> component to all the meshes in the model.
    /// </summary>
    private void AddExplodableComponent()
    {
        MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in meshRenderers)
        {
            Explodables.Add(item.gameObject.AddComponent<ExplodablePart>());
            item.gameObject.name = "Explodable_" + item.transform.GetSiblingIndex();
        }
    }
    #endregion

    #region Debug
    private void CreateLine()
    {

    }
    #endregion

    #endregion
}