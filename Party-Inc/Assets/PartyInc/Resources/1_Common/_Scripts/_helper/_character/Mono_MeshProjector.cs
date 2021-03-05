using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    ///DO NOT TOUCH WITHOUT CONSULTING

    /// <summary>
    /// A class that projects this gameobjects childrens mesh on a Physics collider.
    /// Gives the illusion of a mesh UV Map, but its actually done physically and on runtime.
    /// </summary>
    [ExecuteInEditMode]
    public class Mono_MeshProjector : MonoBehaviour
    {
        private bool _initialized = false;
        private bool _cantProject = false;

        private Mesh[] _childrenMeshes;
        private MeshFilter[] _childrenMeshFilters;
        private Transform[] _childrenTransforms;
        private Transform[] _childrenOldParents;

        private Vector3[][] _planeVertices;
        private Vector3[][] _planeNormals;

        private Vector3[] _lastProjection;
        private Vector3[] _lastNormals;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize everything that is necessary.
            Init();

            // Only project at once if we are not in the editor.
            if (!Application.isEditor) Project();
        }

        /// <summary>
        /// This method gets all the necessary meshes and mesh filters to do the projection.
        /// </summary>
        private void Init()
        {
            if (_initialized) return;

            // Get children transfoms, and mesh filters.
            _childrenMeshFilters = GetComponentsInChildren<MeshFilter>();
            _childrenTransforms = GetComponentsInChildren<Transform>();
            _childrenMeshes = new Mesh[_childrenMeshFilters.Length];

            // Get the childrens original transform parents.
            _childrenOldParents = new Transform[_childrenTransforms.Length];
            for (int i = 0; i < _childrenOldParents.Length; i++)
            {
                _childrenOldParents[i] = _childrenTransforms[i].parent;
            }

            // Sets the meshes from the mesh filters
            for (int i = 0; i < _childrenMeshFilters.Length; i++)
            {
                // Shift one place to skip root transform.
                _childrenMeshFilters[i].transform.parent = _childrenOldParents[i + 1];

                _childrenMeshes[i] = _childrenMeshFilters[i].sharedMesh;
            }

            _planeVertices = new Vector3[_childrenMeshes.Length][];
            _planeNormals = new Vector3[_childrenMeshes.Length][];

            for (int i = 0; i < _childrenMeshes.Length; i++)
            {
                _planeVertices[i] = _childrenMeshes[i].vertices;
                _planeNormals[i] = _childrenMeshes[i].normals;
            }

            _initialized = true;
        }

        /// <summary>
        /// Changes the meshes vertices and normals to the set arrays.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="newVertices"></param>
        /// <param name="newNormals"></param>
        private void ChangeVertices(Mesh mesh, Vector3[] newVertices, Vector3[] newNormals)
        {
            mesh.vertices = newVertices;
            mesh.normals = newNormals;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Undoes the projection and returns to the original state.
        /// </summary>
        public void Undo()
        {
            if (_cantProject)
            {
                _cantProject = false;
                for (int i = 0; i < _childrenMeshes.Length; i++)
                {
                    UndoChildren(_childrenMeshes[i], _childrenMeshFilters[i], i);
                }

                transform.parent = null;
            }
        }

        /// <summary>
        /// Undo for a singular child.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="meshFilter"></param>
        /// <param name="i"></param>
        private void UndoChildren(Mesh mesh, MeshFilter meshFilter, int i)
        {
            ChangeVertices(mesh, _lastProjection, _lastNormals);

            // Shift one place to skip root transform.
            meshFilter.transform.parent = _childrenOldParents[i + 1];
        }

        /// <summary>
        /// Projects all children on the collider hit by the raycasts.
        /// Gives the illusion of a UV Map, but its not.
        /// </summary>
        public void Project()
        {
            // Initialize just in case.
            Init();

            // Only project if you are allowed to
            if (!_cantProject)
            {
                // Make sure there is an object in the first place.
                RaycastHit hitOne;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitOne, 20f, 1 << 8))
                {
                    transform.parent = hitOne.transform;
                }
                else return;

                // Cant project
                _cantProject = true;

                // Project each child
                for (int i = 0; i < _childrenMeshes.Length; i++)
                {
                    ProjectChildren(_childrenMeshes[i], _childrenMeshFilters[i], _planeVertices[i], _planeNormals[i], _childrenTransforms[i + 1], _childrenOldParents[i + 1]);
                }
            }
        }

        /// <summary>
        /// Project for a singular child.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="meshFilter"></param>
        /// <param name="vertices"></param>
        /// <param name="normals"></param>
        /// <param name="trans"></param>
        /// <param name="oldTrans"></param>
        private void ProjectChildren(Mesh mesh, MeshFilter meshFilter, Vector3[] vertices, Vector3[] normals, Transform trans, Transform oldTrans)
        {
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector3> newNormals = new List<Vector3>();

            foreach (Vector3 vertex in vertices)
            {
                // CENTER: obj.position - transform.TransformPoint(vertex)
                RaycastHit hit;
                if (Physics.Raycast(trans.TransformPoint(vertex), trans.TransformDirection(Vector3.down), out hit, 20f, 1 << 8))
                {
                    newVertices.Add(trans.InverseTransformPoint(hit.point + new Vector3(0f, 0, 0.03f)));
                    newNormals.Add(trans.InverseTransformDirection(hit.normal));

                    // Debug rays
                    Debug.DrawRay(trans.TransformPoint(vertex), trans.TransformDirection(Vector3.down) * hit.distance, Color.green, 0f);
                    Debug.DrawRay(hit.point + new Vector3(0f, 0.02f, 0f), hit.normal, Color.blue, 0f);
                }
                else
                {
                    // Debug rays
                    Debug.DrawRay(trans.TransformPoint(vertex), trans.TransformDirection(Vector3.down) * 20f, Color.red, 0f);
                }
            }

            if (newVertices.Count == vertices.Length)
            {
                _lastProjection = vertices;
                _lastNormals = normals;

                trans.parent = oldTrans;
                meshFilter.transform.parent = oldTrans;

                ChangeVertices(mesh, newVertices.ToArray(), newNormals.ToArray());
            }
        }

        private void OnValidate()
        {
            // If something changes in the editor, reinitialize.
            _initialized = false;
        }
    }
}


