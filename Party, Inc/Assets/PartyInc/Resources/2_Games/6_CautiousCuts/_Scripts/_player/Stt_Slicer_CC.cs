using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PlayInc
{
    namespace Tvtig.Slicer
    {
        /// <summary>
        /// This class and code belongs to Tvtig, aka Thavendren Naicker
        /// https://github.com/Tvtig/UnityLightsaber
        /// https://www.youtube.com/watch?v=BVCNDUcnE1o&ab_channel=Tvtig&t=266s
        /// </summary>
        class Slicer
        {
            /// <summary>
            /// Slice the object by the plane 
            /// </summary>
            /// <param name="plane"></param>
            /// <param name="objectToCut"></param>
            /// <returns></returns>
            public static GameObject[] Slice(Plane plane, GameObject objectToCut)
            {
                //Get the current mesh and its verts and tris
                Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
                var a = mesh.GetSubMesh(0);
                Mono_Sliceable_CC sliceable = objectToCut.GetComponent<Mono_Sliceable_CC>();

                if (sliceable == null)
                {
                    throw new NotSupportedException("Cannot slice non sliceable object, add the sliceable script to the object or inherit from sliceable to support slicing");
                }

                //Create left and right slice of hollow object
                SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid, sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

                GameObject positiveObject = CreateMeshGameObject(objectToCut);
                positiveObject.name = string.Format("{0}_positive", objectToCut.name);

                GameObject negativeObject = CreateMeshGameObject(objectToCut);
                negativeObject.name = string.Format("{0}_negative", objectToCut.name);

                var positiveSideMeshData = slicesMeta.PositiveSideMesh;
                var negativeSideMeshData = slicesMeta.NegativeSideMesh;

                positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
                negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

                SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity);
                SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity);

                return new GameObject[] { positiveObject, negativeObject };
            }

            /// <summary>
            /// Creates the default mesh game object.
            /// </summary>
            /// <param name="originalObject">The original object.</param>
            /// <returns></returns>
            private static GameObject CreateMeshGameObject(GameObject originalObject)
            {
                var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

                GameObject meshGameObject = new GameObject();
                Mono_Sliceable_CC originalMono_Sliceable_CC = originalObject.GetComponent<Mono_Sliceable_CC>();

                meshGameObject.AddComponent<MeshFilter>();
                meshGameObject.AddComponent<MeshRenderer>();
                Mono_Sliceable_CC sliceable = meshGameObject.AddComponent<Mono_Sliceable_CC>();

                sliceable.IsSolid = originalMono_Sliceable_CC.IsSolid;
                sliceable.ReverseWireTriangles = originalMono_Sliceable_CC.ReverseWireTriangles;
                sliceable.UseGravity = originalMono_Sliceable_CC.UseGravity;

                meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

                meshGameObject.transform.localScale = originalObject.transform.localScale;
                meshGameObject.transform.rotation = originalObject.transform.rotation;
                meshGameObject.transform.position = originalObject.transform.position;

                meshGameObject.tag = originalObject.tag;

                return meshGameObject;
            }

            /// <summary>
            /// Add mesh collider and rigid body to game object
            /// </summary>
            /// <param name="gameObject"></param>
            /// <param name="mesh"></param>
            private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity)
            {
                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
                meshCollider.convex = true;

                var rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = useGravity;
            }
        }
    }
}


