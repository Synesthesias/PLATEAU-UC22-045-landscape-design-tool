using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandScapeDesignTool
{

    public class CollisionHandler : MonoBehaviour
    {

        public float areaHeight = float.MaxValue;
        List<GameObject> targetbjects = new List<GameObject>();
        public bool isApply = false;

        int nobjects;

        // Start is called before the first frame update
        void Start()
        {
            nobjects = 0;
            isApply = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {

            GameObject target = other.gameObject;

            targetbjects.Add(target);

            // Debug.Log(other.gameObject.name+" : height="+bounds.size.y);
        }

        public void ApplyHeight(float h)
        {

            isApply = true;
            foreach ( var target in targetbjects)
            {
                var mesh = target.GetComponent<MeshFilter>().mesh;
                var bounds = mesh.bounds;
                Debug.Log(target.name + " : height=" + bounds.size.y);
                if(  bounds.size.y > h)
                {
                    if (target.GetComponent<TmpHeight>() == null)
                    {
                        target.AddComponent<TmpHeight>();
                    }
                    TmpHeight tmp =target.AddComponent<TmpHeight>();
                    tmp.StoreOrg();
                    float dy = bounds.size.y - h;
                    Vector3 p = target.transform.position;
                    Vector3 np = new Vector3(p.x, p.y - dy, p.z);
                    target.transform.position = np;
                }

                areaHeight = h;
            }

        }

        public void UndoHeight()
        {
            isApply = false;
            foreach (var target in targetbjects)
            {
                TmpHeight tmpH = target.GetComponent<TmpHeight>();
                if (tmpH)
                {
                    tmpH.Restore();
                }
            }
        }
    }
}