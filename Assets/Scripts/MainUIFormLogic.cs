using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityTechnology
{
    public class MainUIFormLogic : MonoBehaviour
    {
        #region Variables
        public Dropdown TechnologyDropdown;
        public Button RunButton;
        public Button ReturnButton;

        private int m_TechnologyIndex = 1;
        #endregion
        
        #region Methods
        public void SwitchTechnology(int _index)
        {
            m_TechnologyIndex = _index + 1;
        }

        public void RunTechnology()
        {
            RunButton.gameObject.SetActive(false);
            TechnologyDropdown.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(true);
            SceneManager.LoadSceneAsync(m_TechnologyIndex, LoadSceneMode.Additive);
        }

        public void ReturnToMain()
        {
            RunButton.gameObject.SetActive(true);
            TechnologyDropdown.gameObject.SetActive(true);
            ReturnButton.gameObject.SetActive(false);
            SceneManager.UnloadSceneAsync(m_TechnologyIndex);
        }
        #endregion
    }
}
