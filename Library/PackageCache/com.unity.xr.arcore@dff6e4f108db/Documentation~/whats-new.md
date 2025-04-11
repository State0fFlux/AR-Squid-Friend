---
uid: arcore-whats-new
---
# What's new in version 6.0

This release includes the following significant changes:

## Added

- Added support for the following new features introduced in AR Foundation 6.0:
    - Added support for image stabilization, which helps stabilize shaky video from the camera. You can enable image stabilization in your app with the [AR Camera Manager component](xref:arfoundation-camera-components#ar-camera-manager-component).
    - Added support for `XRCameraSubsystem.GetShaderKeywords` to `ARCoreCameraSubsystem` and `ARCoreOcclusionSubsystem`.
- Image libraries with images that score lower than 75 with the 'arcoreimg eval-img' tool will produce a report as a console warning. See the [arcoreimg documentation](https://developers.google.com/ar/develop/augmented-images/arcoreimg) for more details.

## Changed

- Upgraded ARCore version from 1.31 to 1.42.
- Upgraded minimum Unity version from 2021.2 to 2023.3.
- In previous versions of ARCore, the display matrix was column-major and did not include flipping the y-axis before rendering the image onto the device. This y-axis flipping was previously handled by the ARCoreBackground shaders. In order to make the ARCore display matrix consistent with that of ARKit, it is now row-major and flips the y-axis. This matrix is returned by `XRCameraFrame.TryGetDisplayMatrix`. Reference this manual page to understand the [display matrix format and derivation](xref:arfoundation-display-matrix-format-and-derivation).
- ARCoreBackground.shader and ARCoreBackgroundAfterOpaques.shader were changed to correctly render the background using the newly formatted display matrix. Reference this manual page to learn how to write [custom background shaders](xref:arfoundation-custom-background-shaders).
- Changed the behavior of `ARCoreSessionSubsystem.sessionId` to now return a non-empty, unique value per ARCore session. You can access the session id using `XRSessionSubsystem.sessionId`.

## Deprecated

- `ARCoreXRPointCloudSubsystem` has been deprecated and renamed to `ARCorePointCloudSubsystem` for consistency with other subsystems. Unity's API Updater should automatically convert any deprecated APIs references to the new APIs when the project is loaded into the Editor again.

## Removed

| Obsolete API | Recommendation |
| :----------- | :------------- |
| `ARCoreSettingsProvider` | This class is now deprecated. Its internal functionality is replaced by XR Management |
| `ARCoreBeforeSetConfigurationEventArgs.session` | Use `arSession` to access the session. |
| `ARCoreBeforeSetConfigurationEventArgs.config` | Use `arConfig` to access the configuration. |
| `ARCoreBeforeSetConfigurationEventArgs.ARCoreBeforeSetConfigurationEventArgs` | Use `ARCoreBeforeSetConfigurationEventArgs(ArSession, ArConfig)` instead. |
| `ArCameraConfig.Null` | Use `default` instead. |
| `ArCameraConfig.IsNull` | Compare to null instead. |
| `ArCameraConfigFilter.Null` | Use `default` instead. |
| `ArCameraConfigFilter.IsNull` | Compare to null instead. |
| `ArConfig.Null` | Use `default` instead. |
| `ArConfig.IsNull` | Compare to null instead. |
| `ArSession.Null` | Use `default` instead. |
| `ArSession.IsNull` | Compare to null instead. |

For a full list of changes in this version including backwards-compatible bugfixes, refer to the package [changelog](xref:arcore-changelog).
