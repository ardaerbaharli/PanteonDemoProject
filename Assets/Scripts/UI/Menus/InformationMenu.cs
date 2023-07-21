using System.Collections.Generic;
using Products;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class InformationMenu : Menu
    {
        [SerializeField] private TextMeshProUGUI buildingTypeText;
        [SerializeField] private Image buildingImage;
        [SerializeField] private GameObject toggleButton;

        private List<Product> _shownProducts;


        private void Awake()
        {
            Animator = GetComponent<Animator>();
            _shownProducts = new List<Product>();
        }

        public override void Show()
        {
            base.Show();
            MenuController.Instance.HideMenu(MenuType.ProductionMenu);
            toggleButton.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
            toggleButton.SetActive(false);
            Clear();
        }

        public void Initialize(string buildingName, Sprite buildingSprite, List<Product> productsToShow)
        {
            buildingTypeText.text = buildingName;
            buildingImage.sprite = buildingSprite;

            _shownProducts = productsToShow;
        }

        public void Initialize(string buildingName, Sprite buildingSprite)
        {
            buildingTypeText.text = buildingName;
            buildingImage.sprite = buildingSprite;
        }

        public void Clear()
        {
            _shownProducts.ForEach(product => product.ReturnToPool());
            _shownProducts.Clear();
        }
    }
}