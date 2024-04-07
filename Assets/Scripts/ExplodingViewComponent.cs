using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System;
using MK.ExplodingView.Utils;
using System.Drawing;

namespace MK.ExplodingView
{
    public class ExplodingViewComponent : MonoBehaviour
    {
        #region Properties
        [Header("\nInitialization Properties\n")]
        [Tooltip("Transform to count as the center of exploding view. If no Trasform is set, the center is calculated as the average of the mesh parts and has orientation based on the current game object.")]
        public Transform Center;
        [Tooltip("Direction of the line that the parts will explode along. This takes as reference the <color=cyan>Center</color> axis")]
        public Axis DirectionAxis;
        [Tooltip("If enabled, the exploding parts will be calculated automaticaly. Otherwise, they have to be added manualy by adding the <color=cyan>ExplodablePart.cs</color> component and drag-n-drop them in the <color=cyan>Explodables</color> list.")]
        public bool AddExplodablesAutomatically = false;

        [Header("\nExploding Properties\n")]
        [Tooltip("Speed of explosion in seconds.")]
        public float ExplosionSpeed = 0.1f;
        [Tooltip("Explosion distance factor.")]
        [Range(0.1f, 5f)]
        public float ExplosionDistance = 1.5f;
        [Tooltip("The parts that should be moving during exploded view.")]
        public List<ExplodablePart> Explodables;

        [Header("\nDebug\n")]
        [SerializeField]
        GameObject pointPrefab;
        [SerializeField]
        bool debug = false;

        private Dictionary<ExplodablePart, ExplodablePositions> explodablePositionsPoints;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        #endregion

        #region Unity Functions
        private void Awake()
        {   
            if(Explodables == null)
                Explodables = new List<ExplodablePart>();
        }

        private void Start()
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            
            AsyncInit().Forget();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Toggle the exploded view of the model.
        /// If in exploded view, return to the original position.
        /// </summary>
        public void ToggleExplodingView()
        {
            if (transform.position != lastPosition || transform.rotation != lastRotation)
            {
                UpdateExplodables();
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }

            foreach (IExplodable explodable in Explodables)
                explodable.Explode();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Starts the async initialization of the model.
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid AsyncInit()
        {
            //Center
            if (Center == null)
                await CalculateCenter();

            if (AddExplodablesAutomatically)
                AddExplodableComponent();

            if(Explodables.Count > 0)
                InitializeExplodables();

            if(debug)
                DrawDebugLine();
        }

        /// <summary>
        /// Initialize the explodable parts of the model.
        /// Part's exploding direction is calculated based on the <paramref name="lineDirection"/>.
        /// Part's exploding direction is always perpendicular to that direction. 
        /// </summary>
        /// <param name="lineDirection"></param>
        private void InitializeExplodables()
        {
            explodablePositionsPoints = new Dictionary<ExplodablePart, ExplodablePositions>();

            foreach (ExplodablePart explodable in Explodables)
            {   
                explodable.Duration = ExplosionSpeed;
                //Set the original position and exploding duration of the part.
                explodable.OriginalPosition = explodable.transform.position;
                //Calculate the point on the line that is closest to the original position of the part.
                Vector3 pointPosition = UtilityFunctions.GetPointOnLine(explodable.OriginalPosition, Center.position, UtilityFunctions.AxisToDirection(Center, DirectionAxis));
                //Calculate the exploded position of the part.
                explodable.ExplodedPosition = CalculateExplodedPosition(explodable.OriginalPosition, pointPosition);

                //Create a Transform to the original position and added to the dictionary for further updates.
                Transform originalPoint = new GameObject(explodable.name + "_original_point").transform;
                originalPoint.SetParent(explodable.transform.parent);
                originalPoint.position = explodable.OriginalPosition;
                originalPoint.rotation = explodable.transform.rotation;

                //Create a Transform to the exploded position and added to the dictionary for further updates.
                Transform explodedPoint = new GameObject(explodable.name + "_exploded_point").transform;
                explodedPoint.SetParent(explodable.transform.parent);
                explodedPoint.position = explodable.ExplodedPosition;
                explodedPoint.rotation = explodable.transform.rotation;
                
                //Add the part and its original and exploded positions to the dictionary.
                explodablePositionsPoints.Add(explodable, new ExplodablePositions(originalPoint,explodedPoint));
            }
        }

        /// <summary>
        /// Updates the origianl and exploded positions of the explodable parts.
        /// </summary>
        private void UpdateExplodables()
        {
            foreach (ExplodablePart explodable in Explodables)
            {   
                explodable.Duration = ExplosionSpeed;
                explodable.OriginalPosition = explodablePositionsPoints[explodable].OriginalPositionPoint.position;
                explodable.ExplodedPosition = explodablePositionsPoints[explodable].ExplodedPositionPoint.position;
            }
        }

        #region Helpers

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

        /// <summary>
        /// Calculate the center of the model.
        /// </summary>
        private async UniTask CalculateCenter()
        {
            Center = new GameObject("Center").transform;
            Center.SetParent(this.transform);

            List<Vector3> meshes = new List<Vector3>();

            MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
            foreach (var item in meshFilters)
                meshes.Add(item.mesh.bounds.center);

            Center.position = await UniTask.RunOnThreadPool(() => UtilityFunctions.GetAverageCenter(meshes));
            Center.rotation = this.transform.rotation;
        }

        /// <summary>
        /// Calculates the final exploded position of the part.
        /// </summary>
        /// <param name="originalPosition"></param>
        /// <param name="projectionPosition"></param>
        /// <returns>Vector3 position</returns>
        private Vector3 CalculateExplodedPosition(Vector3 originalPosition, Vector3 projectionPosition)
        {
            Vector3 explosionDirection = (originalPosition - projectionPosition).normalized;
            float distanceFromCenter = Vector3.Distance(originalPosition, Center.position);
            return originalPosition + explosionDirection * (ExplosionDistance * distanceFromCenter);
        }

        #endregion

        #region Debug
        private void DrawDebugLine()
        {
            LineRenderer lineRenderer = transform.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, Center.position + UtilityFunctions.AxisToDirection(Center, DirectionAxis) * 3);
            lineRenderer.SetPosition(1, Center.position);
            lineRenderer.SetPosition(2, Center.position - UtilityFunctions.AxisToDirection(Center, DirectionAxis) * 3);
        }
        #endregion

        #endregion
    }
}
