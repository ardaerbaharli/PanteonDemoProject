using TMPro;
using UnityEngine;

namespace UI.Menus
{
    public class ProductionMenu : Menu
    {
        [SerializeField] private TextMeshProUGUI toggleMenuText;


        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public override void Show()
        {
            base.Show();
            MenuController.Instance.HideMenu(MenuType.InformationMenu);
            toggleMenuText.text = "Close";
        }

        public override void Hide()
        {
            base.Hide();
            toggleMenuText.text = "Production";
        }
    }
}