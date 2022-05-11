using System;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using Unity.Jobs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UNG.Utilities
{
    internal static class JobsUtility
    {
        #region Public methods

        public static JobHandle ScheduleAsync<T>(this T job, int arrayLength, int interloopBatchCount, MonoBehaviour context, Action OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IJobParallelFor
        {
            JobHandle handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
            context.StartCoroutine(WaitForJobToFinish(handle, job, OnJobCompleted));
            return handle;
        }
        
        #endregion Public methods
        
        #region Private methods

        private static IEnumerator WaitForJobToFinish<T>(JobHandle jobHandle, T job, Action onJobFinished) where T : struct, IJobParallelFor
        {
            yield return new WaitWhile(() => !jobHandle.IsCompleted);

            jobHandle.Complete();
            onJobFinished?.Invoke();
        }

        #endregion Private methods

        #region Editor methods

#if UNITY_EDITOR
        public static JobHandle ScheduleEditorAsync<T>(this T job, int arrayLength, int interloopBatchCount, Object context, Action OnJobCompleted = null, JobHandle dependsOn = default) where T : struct, IJobParallelFor
        {
            JobHandle handle = job.Schedule(arrayLength, interloopBatchCount, dependsOn);
            EditorCoroutineUtility.StartCoroutine(WaitForJobToFinish(handle, job, OnJobCompleted), context);
            return handle;
        }
        
#endif

        #endregion Editor methods
    }
}
