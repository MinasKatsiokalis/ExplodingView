using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using MK.ExplodingView.Utils;

namespace MK.ExplodingView.Core
{
    public class ExplodingViewComponent : MonoBehaviour
    {
        #region Properties
        //List of explodable parts of the model.
        public List<ExplodablePart> Explodables = null;
        //Initialization properties
        public Transform Center = null;
        public Axis DirectionAxis = Axis.Y;
        public bool AddExplodablesAutomatically = true;
        //Exploding properties
        public float ExplosionSpeed = 0.5f;
        public float ExplosionDistance = 1f;
        public DistanceFactor DistanceFactor = DistanceFactor.DistanceFromCenter;
        public bool AddScaleFactor = false;
        public float ScaleFactorMultiplier = 1;
        public bool AddHierarchyFactor = false;
        public float HierarchyFactorMultiplier = 1;
        public bool AddSiblingFactor = false;
        public float SiblingFactorMultiplier = 1;
        //True if the model is in exploded view.
        public bool IsExploded {private set; get;} = false;
        //Debug
        public bool DrawDirectionAxis  = false;

        //Cache the last position and rotation of the model.
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        //Dictionary to store the original and exploded positions of the explodable parts.
        private Dictionary<ExplodablePart, ExplodablePositions> explodablePositionsPoints = new Dictionary<ExplodablePart, ExplodablePositions>();
        //Explodable parts grouped by the order of the ExplodableModifier component.
        private IOrderedEnumerable<IGrouping<uint, ExplodablePart>> explodablesGrouped = null;
        //Pool of the points to avoid constant realocation.
        private Queue<Transform> pointsPool = new Queue<Transform>();
        //Exploding task running flag.
        private bool isExplodngTaskRunning = false;
        private GameObject debug;
        #endregion

        #region Unity Methods
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
        /// THis runs asyncronously.Awaits the exploding to finish.
        /// </summary>
        public async UniTask ExplodingViewAsyncTask()
        {
            if (isExplodngTaskRunning)
                return;
            isExplodngTaskRunning = true;

            await ExplodingViewTask();
            isExplodngTaskRunning = false;
        }

        /// <summary>
        /// Same as <see cref="ExplodingViewAsyncTask"/> but returns void so proggress cannot be tracked. 
        /// Void makes it usable in editor button assignements.
        /// </summary>
        public async void ExplodingView()
        {
            if (isExplodngTaskRunning)
                return;
            isExplodngTaskRunning = true;

            await ExplodingViewTask();
            isExplodngTaskRunning = false;
        }

        /// <summary>
        /// Calculate the exploding parameters of the model.
        /// This can be called when one or more properties have been changed.
        /// This runs asyncronously.
        /// </summary>
        public async void CalculateExplodingParameters()
        {
            if (isExplodngTaskRunning)
                return;

            if (IsExploded)
                await ExplodingViewAsyncTask();

            DisablePoints();

            if (Explodables.Count > 0)
                InitializeExplodables();

            if (DrawDirectionAxis)
                DrawDebugLine();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Starts the async initialization of the model.
        /// </summary>
        /// <returns></returns>
        private async UniTask AsyncInit()
        {
            if (AddExplodablesAutomatically)
                AddExplodableComponent();

            //Center
            if (Center == null)
                await CalculateCenter();

            if(Explodables.Count > 0)
                InitializeExplodables();

            if (DrawDirectionAxis)
                DrawDebugLine();
        }

        /// <summary>
        /// Awaitable Task responsible to explode the model.
        /// </summary>
        private async UniTask ExplodingViewTask()
        {
            if (transform.position != lastPosition || transform.rotation != lastRotation)
            {
                UpdateExplodables();
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }
            if (!IsExploded)
            {
                foreach (var group in explodablesGrouped.Reverse())
                {
                    var tasks = group.Select(explodable => explodable.ExplodeAsync()).ToArray();
                    await UniTask.WhenAll(tasks);
                }
            }
            else
            {
                foreach (var group in explodablesGrouped)
                {
                    var tasks = group.Select(explodable => explodable.ExplodeAsync()).ToArray();
                    await UniTask.WhenAll(tasks);
                }
            }
            IsExploded = !IsExploded;
        }

        /// <summary>
        /// Initialize the explodable parts of the model.
        /// Part's exploding direction is calculated based on the <paramref name="lineDirection"/>.
        /// Part's exploding direction is always perpendicular to that direction. 
        /// </summary>
        /// <param name="lineDirection"></param>
        private void InitializeExplodables()
        {
            Func<Transform, int> calculateDepth = transform => UtilityFunctions.CalculateHierarchyDepth(transform);
            Func<Transform, float> getMeshScale = transform => transform.GetComponent<MeshFilter>().mesh.bounds.size.magnitude;

            Transform pointsParentObject = new GameObject("Points").transform;
            pointsParentObject.SetParent(this.transform);

            foreach (ExplodablePart explodable in Explodables)
            {   
                //Set the original position and exploding duration of the part.
                explodable.Duration = ExplosionSpeed;
                explodable.OriginalPosition = explodable.transform.position;
                //Calculate the exploded position of the part.
                float? scaleFactor = AddScaleFactor ? (1f/getMeshScale(explodable.transform)) * ScaleFactorMultiplier : null;
                float? hierarchyFactor = AddHierarchyFactor ? calculateDepth(explodable.transform) * HierarchyFactorMultiplier : null;
                float? siblingFactor = AddSiblingFactor ? explodable.transform.GetSiblingIndex() * SiblingFactorMultiplier : null;
                explodable.ExplodedPosition = CalculateExplodedPosition(explodable, scaleFactor, hierarchyFactor, siblingFactor);
                //Save both positions as tranform points into a dictionary.
                explodablePositionsPoints.Add(explodable, SetPartPositionPoints(explodable, pointsParentObject));
            }
            explodablesGrouped = Explodables.GroupBy(item => item.gameObject.GetComponent<ExplodableModifier>()?.Order ?? 0).OrderBy(group => group.Key);
        }

        /// <summary>
        /// Saves the original and exploded transforms points of the explodable part into a dictionary.
        /// </summary>
        /// <param name="explodable"></param>
        private ExplodablePositions SetPartPositionPoints(ExplodablePart explodable, Transform parent = null)
        {
            //Create a Transform to the original & exploded position and add them to the dictionary for further updates.
            Transform originalPoint;
            Transform explodedPoint;
            if (pointsPool.Count > 1)
            {
                originalPoint = pointsPool.Dequeue();
                explodedPoint = pointsPool.Dequeue();
                originalPoint.gameObject.SetActive(true);
                explodedPoint.gameObject.SetActive(true);
            }
            else if (pointsPool.Count == 1)
            {
                originalPoint = pointsPool.Dequeue();
                originalPoint.gameObject.SetActive(true);
                explodedPoint = new GameObject().transform;
            }
            else
            {
                originalPoint = new GameObject().transform;
                explodedPoint = new GameObject().transform;
            }
            originalPoint.name = explodable.name + "_original_point";
            explodedPoint.name = explodable.name + "_exploded_point";
            //Set the parent of the points to the parent object.
            try
            {
                originalPoint.SetParent(parent);
                explodedPoint.SetParent(parent);
            }
            catch (ArgumentNullException)
            {
                Debug.LogWarning("Parent object is null. Setting the parent to the current object.");
                originalPoint.SetParent(this.transform);
                explodedPoint.SetParent(this.transform);
            }
            //Set the position of the points.
            originalPoint.position = explodable.OriginalPosition;
            explodedPoint.position = explodable.ExplodedPosition;
            //Set the rotation of the points.
            originalPoint.rotation = explodedPoint.rotation = explodable.transform.rotation;
            
            return new ExplodablePositions(originalPoint, explodedPoint);
        }

        /// <summary>
        /// Updates the origianl and exploded positions of the explodable parts.
        /// In case the model is moved or rotated, the parts will be updated accordingly.
        /// The update happens only after <see cref="ExplodingViewTask"/> is called.
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
        /// Adds the <see cref="ExplodablePart"/> component to all the meshes in the model.
        /// </summary>
        private void AddExplodableComponent()
        {
            Explodables = new List<ExplodablePart>();

            MeshRenderer[] transforms = transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var item in transforms)
            {   
                if(item == this.transform)
                    continue;

                if(item.gameObject.GetComponent<ExplodablePart>() == null)
                    Explodables.Add(item.gameObject.AddComponent<ExplodablePart>());
            }

            ExplodableModifier[] modifiers = transform.GetComponentsInChildren<ExplodableModifier>();
            foreach (var modifier in modifiers)
            {
                if (modifier.AffectChildren)
                    foreach (var child in modifier.GetComponentsInChildren<MeshRenderer>())
                    {
                        var modifierComponent = child.gameObject.GetComponent<ExplodableModifier>();
                        if (modifierComponent != null)
                            continue;

                        modifierComponent = child.gameObject.AddComponent<ExplodableModifier>();
                        modifierComponent.Order = modifier.Order;
                        modifierComponent.ModifierProperty = modifier.ModifierProperty;
                        modifierComponent.Axis = modifier.Axis;

                        var direction = (modifier.transform.position - child.transform.position).normalized;
                        var distance = Vector3.Distance(modifier.transform.position, child.transform.position);
                        var wolrdPosition = modifier.transform.TransformPoint(modifier.LocalPosition) + direction * distance;

                        modifierComponent.LocalPosition = child.transform.TransformVector(wolrdPosition);
                    }
            }
        }

        /// <summary>
        /// Calculates the center of the model.
        /// </summary>
        private async UniTask CalculateCenter()
        {       
            Debug.Log("Calculating center");
            if(Center == null)
            {
                Center = new GameObject("Center").transform;
                Center.SetParent(this.transform);
                Center.SetSiblingIndex(0);
            }
            Vector3[] transformCenters = transform.GetComponentsInChildren<MeshFilter>().Select(item => item.transform.position).ToArray();
            Center.position = await UniTask.RunOnThreadPool(() => UtilityFunctions.GetAverageCenter(transformCenters));
            Center.rotation = this.transform.rotation;
        }

        /// <summary>
        /// Calculates the exploded position of the explodable part.
        /// </summary>
        /// <param name="explodable"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="hierarchyFactor"></param>
        /// <returns>Vector3 position</returns>
        private Vector3 CalculateExplodedPosition(ExplodablePart explodable, float? scaleFactor = null, float? hierarchyFactor = null, float? siblingFactor = null)
        {
            Func<IExplodable, Vector3> getProjectionPoint = explodable => UtilityFunctions.GetProjectionPoint(Center, explodable.OriginalPosition, DirectionAxis);
            Func<Vector3, Vector3, float> getDistance = (point1, point2) => Vector3.Distance(point1, point2);
            Func<Transform, ModifierAxis, Vector3> convertAxis2Direction = (transform, axis) => UtilityFunctions.AxisToDirection(transform, axis);
            Func<Vector3, Vector3, Vector3> getDirection = (point1, point2) => (point1 - point2).normalized;

            Vector3 originalPosition = explodable.OriginalPosition;
            Vector3 projectionPoint = getProjectionPoint(explodable);
            Vector3 explosionDirection = (getDirection(originalPosition, projectionPoint)); 

            float distanceFactor = DistanceFactor == DistanceFactor.None ? 1 : (DistanceFactor == DistanceFactor.DistanceFromCenter) ?
                getDistance(originalPosition, Center.position) : getDistance(originalPosition, projectionPoint);

            float additionalFactors = (scaleFactor != null ? scaleFactor.Value * this.transform.localScale.magnitude : 0) + (hierarchyFactor ?? 0) + (siblingFactor ?? 0);

            if (explodable.TryGetComponent<ExplodableModifier>(out var modifier))
            {
                if (modifier.ModifierProperty == ModifierProperty.LocalPosition)
                    return explodable.transform.TransformPoint(modifier.LocalPosition);
                else if (modifier.ModifierProperty == ModifierProperty.Axis)
                    explosionDirection = convertAxis2Direction(explodable.transform, modifier.Axis);
            }
            else
            {
                var tempExplosionPosition = originalPosition + (explosionDirection * ExplosionDistance) * (distanceFactor + additionalFactors);
                var tempExplosionDirection = getDirection(tempExplosionPosition, projectionPoint);
                explosionDirection = UtilityFunctions.GetPrimaryTransformDirectionAxis(explodable.transform, tempExplosionDirection);
            }
            return originalPosition + (explosionDirection * ExplosionDistance) * (distanceFactor + additionalFactors);
        }

        /// <summary>
        /// Disables the original and exploded position points of the explodable parts.
        /// Returns the points to the pool for later usage.
        /// Empties the dictionary.
        /// </summary>
        private void DisablePoints()
        {
            foreach (var item in explodablePositionsPoints)
            {
                item.Value.OriginalPositionPoint.gameObject.SetActive(false);
                item.Value.ExplodedPositionPoint.gameObject.SetActive(false);
                pointsPool.Enqueue(item.Value.OriginalPositionPoint);
                pointsPool.Enqueue(item.Value.ExplodedPositionPoint);

                if (item.Equals(explodablePositionsPoints.Last()))
                    Destroy(item.Value.OriginalPositionPoint.parent.gameObject);
            }
            explodablePositionsPoints.Clear();
        }
        #endregion

        #region Debug
        /// <summary>
        /// Draws a line to visualize the direction of the exploding parts.
        /// </summary>
        private void DrawDebugLine()
        {   
            Destroy(debug);
            debug = new GameObject("Debug");
            debug.transform.SetParent(this.transform);

            var axes = DirectionAxis.ToString().ToCharArray().Select(c => (Axis)Enum.Parse(typeof(Axis), c.ToString()));

            foreach (var axis in axes)
            {
                GameObject debugLine = new GameObject("DebugLine"+axis);
                debugLine.transform.SetParent(debug.transform);
                LineRenderer lineRenderer = debugLine.gameObject.AddComponent<LineRenderer>();

                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.positionCount = 2;

                Vector3 direction = UtilityFunctions.AxisToDirection(Center, axis);
                lineRenderer.SetPosition(0, Center.position + direction * 3);
                lineRenderer.SetPosition(1, Center.position - direction * 3);
            }
        }
        #endregion

        #endregion
    }
}