using UnityEngine;

namespace MonoBeh
{
    public class ResumeController : MonoBehaviour
    {
        private static bool _resume;

        public void DoResume()
        {
            _resume = true;
        }

        public static bool IsResume()
        {
            return _resume;
        }

        public static void ResetResume()
        {
            _resume = false;
        }
    }
}
