using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UISystem
{

    public class ViewController : Singleton<ViewController>
    {

        ScreenName tempSName;
        public ScreenName currentScreen
        {
            get
            {
                return tempSName;
            }
            set
            {
                previesScreen = tempSName;
                tempSName = value;

            }
        }
        public ScreenName previesScreen;
        Screen currentView;
        Screen previousView;
        [SerializeField] ScreenName initScreen;

        [Divider]
        [SerializeField] List<ScreenView> screens = new List<ScreenView>();
        [Divider]
        [SerializeField] List<PopupView> popups = new List<PopupView>();
        [Divider]
        [SerializeField] Popup toast;

        [System.Serializable]
        public struct ScreenView
        {
            public Screen screen;
            public ScreenName screenName;
        }

        [System.Serializable]
        public struct PopupView
        {
            public Popup popup;
            public PopupName popupName;
        }
        void Start() => Init();

        public void ShowPopup(PopupName popupName)
        {
            popups[GetPopupIndex(popupName)].popup.Show();
        }

        public void HidePopup(PopupName popupName)
        {
            popups[GetPopupIndex(popupName)].popup.Hide();
        }

        public void ShowToast(string description, float delay = 3)
        {
            toast.Fill(description);
            toast.Show();
            Helper.Execute(this, () => toast.Hide(), delay);
        }
        public void ChangeScreen(ScreenName screen)
        {
            // if (previousView == currentView)
            // return;
            currentScreen = screen;
            if (currentView != null)
            {
                previousView = currentView;
                previousView.Hide();
                currentView = screens[GetScreenIndex(screen)].screen;
                currentView.Show();

            }
            else
            {
                currentView = screens[GetScreenIndex(screen)].screen;
                currentView.Show();
            }

        }

        public void HidePopups()
        {
            for (int i = 0; i < popups.Count; i++)
            {
                if (popups[i].popup.isActive)
                {
                    popups[i].popup.Hide();
                    return;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int i = 0; i < popups.Count; i++)
                {
                    if (popups[i].popup.isActive)
                    {
                        popups[i].popup.Hide();
                        return;
                    }
                }

                currentView.OnBack();
            }
        }

        int GetScreenIndex(ScreenName screen)
        {
            return screens.FindIndex(
                delegate (ScreenView screenView)
                {
                    return screenView.screenName.Equals(screen);
                });
        }

        int GetPopupIndex(PopupName popup)
        {
            return popups.FindIndex(
                delegate (PopupView popupView)
                {
                    return popupView.popupName.Equals(popup);
                });
        }

        public void RedrawView() => currentView.Redraw();

        private void Init()
        {
            for (int indexOfScreen = 0; indexOfScreen < screens.Count; indexOfScreen++)
            {
                screens[indexOfScreen].screen.Disable();
            }

            if (initScreen != ScreenName.None)
            {
                ChangeScreen(initScreen);
            }
        }

        public T GetScreen<T>(ScreenName sName) => (T)screens[GetScreenIndex(sName)].screen.GetComponent<T>();
        public T GetPopup<T>(PopupName sName) => (T)popups[GetPopupIndex(sName)].popup.GetComponent<T>();
    }
}