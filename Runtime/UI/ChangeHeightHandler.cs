using LandscapeDesignTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LandScapeDesignTool
{
    public class ChangeHeightHandler : MonoBehaviour
    {
        [SerializeField] Text areaName;
        [SerializeField] Button applyButon;

        GameObject _targetArea=null;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.gameObject.tag == "HeightRegulationArea")
                    {
                        if(_targetArea != null)
                        {
                            GameObject.Destroy(_targetArea.GetComponent<Rigidbody>());
                        }
                        areaName.color = Color.green;
                        areaName.text = hit.collider.gameObject.name;

                        _targetArea = hit.collider.gameObject;
                        _targetArea.GetComponent<Collider>().isTrigger = true;
                        Rigidbody rigibody = _targetArea.AddComponent<Rigidbody>();
                        rigibody.useGravity = false;
                        if (_targetArea.GetComponent<CollisionHandler>() == null)
                        {
                            _targetArea.AddComponent<CollisionHandler>();
                        }

                        CollisionHandler handler = _targetArea.AddComponent<CollisionHandler>();
                        if(handler.isApply)
                        {
                            applyButon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "���̍����ɖ߂�";

                        }
                        else
                        {
                            applyButon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "�����ύX";

                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            areaName.color = Color.red;
            areaName.text = "���������G���A��I�����Ă�������";
        }

        public void OnClose()
        {

        }

        public void OnApply()
        {
            float h = _targetArea.GetComponent<HeightRegulationAreaHandler>().GetHeight();
            CollisionHandler handler = _targetArea.GetComponent<CollisionHandler>();
            if (handler.isApply)
            {
                handler.UndoHeight();
            }
            else
            {
                handler.ApplyHeight(h);

            }
            if (handler.isApply)
            {
                applyButon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "���̍����ɖ߂�";
            }
            else
            {
                applyButon.transform.GetChild(0).gameObject.GetComponent<Text>().text = "�����ύX";

            }
        }
    }
}
