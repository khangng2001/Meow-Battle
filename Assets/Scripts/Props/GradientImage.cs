using UnityEngine;
using UnityEngine.Serialization;

namespace Props
{
    public class GradientImage : MonoBehaviour
    {
        public Material gradientMaterial;
 
        void Start () {
            gradientMaterial.SetColor("_Color", Color.black); // the left color
            gradientMaterial.SetColor("_Color2", Color.white); // the right color
        }
    }
}
