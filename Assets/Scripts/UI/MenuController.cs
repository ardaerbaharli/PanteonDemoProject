using UnityEngine;
using Utilities;

namespace UI
{
    public enum MenuType
    {
        None,
        ProductionMenu,
        InformationMenu,
        Menus
    }

    public class MenuController : MonoBehaviour
    {
        public DictionaryUnity<MenuType, Menu> pages;
        public static MenuController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }


        public void ToggleMenu(MenuType menu)
        {
            pages[menu].Toggle();
        }

        public void ToggleMenu(MenuType menu, bool value)
        {
            pages[menu].Toggle(value);
        }

        public void ShowMenu(MenuType menu)
        {
            pages[menu].Show();
        }

        public void HideMenu(MenuType menu)
        {
            pages[menu].Hide();
        }

        public Menu GetMenu(MenuType menu)
        {
            return pages[menu];
        }

        public bool IsMenuShown(MenuType menu)
        {
            return pages[menu].IsShown;
        }
    }
}