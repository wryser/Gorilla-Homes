using System.Collections;
using UnityEngine;

namespace GorillaHomes
{
    public class Forward : GorillaPressableButton
    {
        private Material pressedMat = Resources.Load<Material>("objects/treeroom/materials/pressed");
        private Material unpressedMat = Resources.Load<Material>("objects/treeroom/materials/plastic");

        public void Awake()
        {
            transform.GetComponent<Renderer>().material = this.unpressedMat;
        }

        public override void ButtonActivation()
        {
            base.ButtonActivation();
            StartCoroutine(Press());
        }

        private IEnumerator Press()
        {
            transform.GetComponent<Renderer>().material = this.pressedMat;
            Plugin.instance.Forward();
            yield return (object)new WaitForSeconds(0.25f);
            transform.GetComponent<Renderer>().material = this.unpressedMat;
        }
    }
}