using System;
using AOT;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARCore
{
    static class ARCoreImageStabilizationUtils
    {
        internal static readonly Matrix4x4 k_OrthographicProjectionGlesNdc = Matrix4x4.Ortho(
            -1f, 1f, -1f, 1f, 0f, 1f);
        internal static Mesh ImageStabilizationMesh { get; private set; }

        [MonoPInvokeCallback(typeof(Action<IntPtr, int, IntPtr, int>))]
        internal static unsafe void UpdateBackgroundGeometry(
            IntPtr verticesPtr, int vertexCount, IntPtr texCoordsPtr, int texCoordsStride)
        {
            var meshNeededCreation = ImageStabilizationMesh == null;
            if (meshNeededCreation)
            {
                ImageStabilizationMesh = new Mesh();

                var desiredVertexLayout = new[]
                {
                    new VertexAttributeDescriptor(VertexAttribute.Position),
                    new VertexAttributeDescriptor(VertexAttribute.TexCoord0)
                };

                // Let's be certain we'll get the vertex buffer layout we want in native code
                ImageStabilizationMesh.SetVertexBufferParams(4, desiredVertexLayout);
                ImageStabilizationMesh.MarkDynamic();
            }

            using var verts = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(
                (void*)verticesPtr, vertexCount, Allocator.None);

            ImageStabilizationMesh.SetVertices(verts);

            if (meshNeededCreation)
            {
                // If the mesh was created in this update call it needs indices to be set as well
                // Should always happen after setting the vertices
                ImageStabilizationMesh.SetIndices(new[]
                {
                    0, 2, 3,
                    3, 1, 0,
                }, MeshTopology.Triangles, 0, false);
            }

            switch (texCoordsStride)
            {
                case 2:
                {
                    using var uvs = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector2>(
                        (void*)texCoordsPtr, vertexCount, Allocator.None);
                    ImageStabilizationMesh.SetUVs(0, uvs);
                    break;
                }

                case 3:
                {
                    using var uvws = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(
                        (void*)texCoordsPtr, vertexCount, Allocator.None);
                    ImageStabilizationMesh.SetUVs(0, uvws);
                    break;
                }
            }
        }
    }
}
