using Nox7atra.ahp.AnimatorTools;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ahp
{
    [InitializeOnLoad]
    public static class HierarchyMonitor
    {
        private static AnimatorSnapshotManager _animatorSnapshotManager;
        static HierarchyMonitor()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            _animatorSnapshotManager = new AnimatorSnapshotManager();
        }

        private static void OnHierarchyChanged()
        {
            _animatorSnapshotManager.ProcessAnimators();
        }
    }
}