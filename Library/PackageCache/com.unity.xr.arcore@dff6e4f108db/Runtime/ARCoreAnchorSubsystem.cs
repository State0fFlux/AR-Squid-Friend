using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// The ARCore implementation of the
    /// [XRAnchorSubsystem](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem).
    /// Do not create this directly. Use the
    /// [SubsystemManager](xref:UnityEngine.SubsystemManager)
    /// instead.
    /// </summary>
    [Preserve]
    public sealed class ARCoreAnchorSubsystem : XRAnchorSubsystem
    {
        class ARCoreProvider : Provider
        {
            public override void Start() => UnityARCore_anchors_start();

            public override void Stop() => UnityARCore_anchors_stop();

            public override void Destroy() => UnityARCore_anchors_onDestroy();

            public override unsafe TrackableChanges<XRAnchor> GetChanges(
                XRAnchor defaultAnchor,
                Allocator allocator)
            {
                int addedCount, updatedCount, removedCount, elementSize;
                void* addedPtr, updatedPtr, removedPtr;
                var context = UnityARCore_anchors_acquireChanges(
                    out addedPtr, out addedCount,
                    out updatedPtr, out updatedCount,
                    out removedPtr, out removedCount,
                    out elementSize);

                try
                {
                    return new TrackableChanges<XRAnchor>(
                        addedPtr, addedCount,
                        updatedPtr, updatedCount,
                        removedPtr, removedCount,
                        defaultAnchor, elementSize,
                        allocator);
                }
                finally
                {
                    UnityARCore_anchors_releaseChanges(context);
                }

            }

            public override bool TryAddAnchor(
                Pose pose,
                out XRAnchor anchor)
            {
                return UnityARCore_anchors_tryAdd(pose, out anchor);
            }

            public override bool TryAttachAnchor(
                TrackableId attachedToId,
                Pose pose,
                out XRAnchor anchor)
            {
                return UnityARCore_anchors_tryAttach(attachedToId, pose, out anchor);
            }

            public override bool TryRemoveAnchor(TrackableId anchorId)
            {
                return UnityARCore_anchors_tryRemove(anchorId);
            }

            [DllImport(Constants.k_LibraryName)]
            static extern void UnityARCore_anchors_start();

            [DllImport(Constants.k_LibraryName)]
            static extern void UnityARCore_anchors_stop();

            [DllImport(Constants.k_LibraryName)]
            static extern void UnityARCore_anchors_onDestroy();

            [DllImport(Constants.k_LibraryName)]
            static extern unsafe void* UnityARCore_anchors_acquireChanges(
                out void* addedPtr, out int addedCount,
                out void* updatedPtr, out int updatedCount,
                out void* removedPtr, out int removedCount,
                out int elementSize);

            [DllImport(Constants.k_LibraryName)]
            static extern unsafe void UnityARCore_anchors_releaseChanges(
                void* changes);

            [DllImport(Constants.k_LibraryName)]
            static extern bool UnityARCore_anchors_tryAdd(
                Pose pose,
                out XRAnchor anchor);

            [DllImport(Constants.k_LibraryName)]
            static extern bool UnityARCore_anchors_tryAttach(
                TrackableId trackableToAffix,
                Pose pose,
                out XRAnchor anchor);

            [DllImport(Constants.k_LibraryName)]
            static extern bool UnityARCore_anchors_tryRemove(TrackableId anchorId);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            if (!Api.platformAndroid || !Api.loaderPresent)
                return;

            var cinfo = new XRAnchorSubsystemDescriptor.Cinfo
            {
                id = "ARCore-Anchor",
                providerType = typeof(ARCoreAnchorSubsystem.ARCoreProvider),
                subsystemTypeOverride = typeof(ARCoreAnchorSubsystem),
                supportsTrackableAttachments = true,
                supportsSynchronousAdd = true,
                supportsSaveAnchor = false,
                supportsLoadAnchor = false,
                supportsEraseAnchor = false,
                supportsGetSavedAnchorIds = false,
                supportsAsyncCancellation = false,
            };

            XRAnchorSubsystemDescriptor.Register(cinfo);
        }
    }
}
