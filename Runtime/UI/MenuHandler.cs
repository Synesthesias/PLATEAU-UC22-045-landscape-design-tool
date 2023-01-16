using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LandScapeDesignTool
{
    public class MenuHandler : MonoBehaviour
    {
        [SerializeField] GameObject menuPanel;
        [SerializeField] GameObject viewpointPanel;
        // Start is called before the first frame update
        void Start()
        {
            menuPanel.SetActive(false);
            viewpointPanel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ToggleMenu()
        {
           menuPanel.SetActive( menuPanel.activeSelf ? false : true);
           if( menuPanel.activeSelf == true)
            {
                viewpointPanel.SetActive(false);
            }


        }

        public void ToggleViewPointPanel()
        {
            viewpointPanel.SetActive(viewpointPanel.activeSelf ? false : true); 
            if (viewpointPanel.activeSelf == false)
            {
                menuPanel.SetActive(true);
            }
        }
    }
}