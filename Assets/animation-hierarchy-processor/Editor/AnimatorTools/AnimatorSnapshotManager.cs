using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ahp.AnimatorTools
{
    public class AnimatorSnapshotManager
    {
        private Dictionary<Animator, AnimatorSnapshot> _AnimatorSnapshots;
        private Dictionary<Animator, bool> _FindedAnimators;
        
        public void ProcessAnimators()
        {
            ClearFoundedAnimators();
            Animator[] animators = UnityEngine.Object.FindObjectsOfType<Animator>();
            foreach (var animator in animators)
            {
                ProcessAnimator(animator);
            }
            ClearDeleted();
            var values = _AnimatorSnapshots.Values.ToList();
            foreach (var animatorSnapshot in values)
            {
                ProcessSnapshot(animatorSnapshot);
            }
        }

       
        private void ProcessSnapshot(AnimatorSnapshot snapshot)
        {
            var updatedSnapshot = CreateSnapshot(snapshot.Animator);
            var oldTrSnapshots = snapshot.TransformSnapshots;
            var allTransformsUpdated = updatedSnapshot.TransformSnapshots.Keys;
            var clips = AnimationUtility.GetAnimationClips(snapshot.Animator.gameObject);
            foreach (var key in allTransformsUpdated)
            {
                if (oldTrSnapshots.ContainsKey(key))
                {
                    var newTr = updatedSnapshot.TransformSnapshots[key];
                    var oldTr = snapshot.TransformSnapshots[key];
                    if (newTr.childName != oldTr.childName)
                    {
                        foreach (var clip in clips)
                        {
                            ProcessChange(clip, oldTr, newTr);
                        }
                    }

                    if (newTr.path != oldTr.path)
                    {
                        foreach (var clip in clips)
                        {
                            ProcessChange(clip, oldTr, newTr);
                        }
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorApplication.RepaintAnimationWindow();
            _AnimatorSnapshots[snapshot.Animator] = updatedSnapshot;
        }
        
        private void ProcessChange(AnimationClip clip, AnimatorSnapshotTransform old, AnimatorSnapshotTransform updated)
        {
            EditorCurveBinding[] objectCurveBinding = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            EditorCurveBinding[] curveDataBinding = AnimationUtility.GetCurveBindings(clip);
            
            for (int i= 0; i < objectCurveBinding.Length; i++)
            {
                var binding = objectCurveBinding[i];
                if (binding.path == old.path)
                {
                    ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                    binding.path = updated.path;
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
                }
            }

            for (int i = 0; i < curveDataBinding.Length; i++)
            {
                var binding = curveDataBinding[i];
                if (binding.path == old.path)
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                    AnimationUtility.SetEditorCurve(clip, binding, null);
                    binding.path = updated.path;
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }
        }
        private void ProcessAnimator(Animator animator)
        {
            if (_FindedAnimators.ContainsKey(animator))
            {
                _FindedAnimators[animator] = true;
            }

            if (!_AnimatorSnapshots.ContainsKey(animator))
            {
                _AnimatorSnapshots[animator] = CreateSnapshot(animator);
            }
        }
        private AnimatorSnapshot CreateSnapshot(Animator animator)
        {
            var snapshot = new AnimatorSnapshot();
            snapshot.Animator = animator;
            snapshot.InitSnapshotTransform(animator.transform);
            return snapshot;
        }
        private void ClearDeleted()
        {
            foreach(var item in _FindedAnimators.Where(kvp => !kvp.Value).ToList())
            {
                _FindedAnimators.Remove(item.Key);
                _AnimatorSnapshots.Remove(item.Key);
            }
        }
        private void ClearFoundedAnimators()
        {
            foreach (var key in _FindedAnimators.Keys)
            {
                _FindedAnimators[key] = false;
            }
        }

        public AnimatorSnapshotManager()
        {
            _AnimatorSnapshots = new Dictionary<Animator, AnimatorSnapshot>();
            _FindedAnimators = new Dictionary<Animator, bool>();
        }
    }
}