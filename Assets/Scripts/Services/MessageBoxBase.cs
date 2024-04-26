using UnityEngine;

namespace Services
{
    public abstract class MessageBoxBase: MonoBehaviour
    {
        protected abstract void OnSetMessage(string message);
    }
}
