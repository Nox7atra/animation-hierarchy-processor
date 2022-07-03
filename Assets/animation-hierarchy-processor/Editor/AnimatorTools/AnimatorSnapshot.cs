using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.ahp.AnimatorTools
{
    public class AnimatorSnapshot
    {
        public Dictionary<Transform, AnimatorSnapshotTransform> TransformSnapshots;
        public Animator Animator;

        public void InitSnapshotTransform(Transform root)
        {
            TransformSnapshots = new  Dictionary<Transform, AnimatorSnapshotTransform>();
            var count = root.childCount;
            for (int i = 0; i < count; i++)
            {
                var child = root.GetChild(i);
                CreateAnimatorSnapshotTransformRecursive(child, child.name);
            }
        }

        private void CreateAnimatorSnapshotTransformRecursive(Transform transform, string path)
        {
            var snapshotTr = new AnimatorSnapshotTransform();
            snapshotTr.parent = transform.parent;
            snapshotTr.child = transform;
            snapshotTr.childName = transform.name;
            snapshotTr.path = path;
            TransformSnapshots.Add(transform, snapshotTr);
            var count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                var child = transform.GetChild(i);
                CreateAnimatorSnapshotTransformRecursive(child, path + "/" + child.name);
            }
        }
    }

    public class AnimatorSnapshotTransform
    {
        public Transform parent;
        public Transform child;
        public string childName;
        public string path;
    }
}
