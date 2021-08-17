using System;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using Unity.Jobs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DudeiNoise.Utilities
{
    public static class JobsUtility
    {
        #region Public methods

        public static Coroutine ScheduleAsync<T>(this T job, int arrayLength, int interloopBatchCount, MonoBehaviour context, Action<T> OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IJobParallelFor
        {
            JobHandle handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
            return context.StartCoroutine(WaitForJobToFinish(handle, job, OnJobCompleted));
        }
        
        #endregion Public methods
        
        #region Private methods

        private static IEnumerator WaitForJobToFinish<T>(JobHandle jobHandle, T job, Action<T> onJobFinished) where T : struct, IJobParallelFor
        {
            yield return new WaitWhile(() => !jobHandle.IsCompleted);

            jobHandle.Complete();
            onJobFinished?.Invoke(job);
        }

        #endregion Private methods

        #region Editor methods

#if UNITY_EDITOR
        public static EditorCoroutine ScheduleEditorAsync<T>(this T job, int arrayLength, int interloopBatchCount, Object context, Action<T> OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IJobParallelFor
        {
            JobHandle handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
            return EditorCoroutineUtility.StartCoroutine(WaitForJobToFinish(handle, job, OnJobCompleted), context);
        }
        
#endif

        #endregion Editor methods
    }

}
