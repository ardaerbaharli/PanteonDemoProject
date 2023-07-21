using System;
using Grid;
using MapEntities.Buildings;
using MapEntities.Units;
using UI;
using UI.Menus;
using UnityEngine;

public class MapEntityInputHandler : MonoBehaviour
{
    public static MapEntityInputHandler Instance;
    [SerializeField] private LayerMask buildingLayerMask;
    [SerializeField] private LayerMask unitLayerMask;

    [SerializeField] private LayerMask tileLayerMask;

    private Unit _previousSelectedUnit;
    private Building _selectedBuilding;
    private Unit _selectedUnit;

    public Action<Building> onSelectedBuilding;
    public Action<GridTile> onSelectedTile;
    public Action<Unit> onSelectedUnit;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerInputManager.Instance.onRightButtonDown += HandleRightClick;
        PlayerInputManager.Instance.onLeftButtonDown += HandleLeftButtonDown;
    }

    private void HandleLeftButtonDown(Vector3 position)
    {
        if (HandleUnitClick(position))
        {
            MenuController.Instance.HideMenu(MenuType.InformationMenu);
            MenuController.Instance.HideMenu(MenuType.ProductionMenu);

            if (_previousSelectedUnit != null)
                _previousSelectedUnit.Deselect();
            _selectedUnit.Select();

            onSelectedUnit?.Invoke(_selectedUnit);
            return;
        }

        if (HandleBuildingClick(position))
        {
            onSelectedBuilding?.Invoke(_selectedBuilding);
            return;
        }

        if (HandleTileClick(position, out var selectedTile))
        {
            MenuController.Instance.HideMenu(MenuType.InformationMenu);
            MenuController.Instance.HideMenu(MenuType.ProductionMenu);

            onSelectedTile?.Invoke(selectedTile);
        }
    }

    private void HandleRightClick(Vector3 position)
    {
        if (_selectedUnit == null) return;
        if (HandleUnitClick(position))
        {
            MenuController.Instance.HideMenu(MenuType.InformationMenu);
            MenuController.Instance.HideMenu(MenuType.ProductionMenu);

            _previousSelectedUnit.Act(_selectedUnit);

            _previousSelectedUnit.Deselect();
            _previousSelectedUnit = null;
            _selectedUnit = null;

            return;
        }

        if (HandleBuildingClick(position))
        {
            _selectedUnit.Act(_selectedBuilding);

            _selectedUnit.Deselect();
            _selectedUnit = null;
            return;
        }

        if (HandleTileClick(position, out var selectedTile))
        {
            MenuController.Instance.HideMenu(MenuType.InformationMenu);
            MenuController.Instance.HideMenu(MenuType.ProductionMenu);

            _selectedUnit.Move(selectedTile);
            _selectedUnit.Deselect();
            _selectedUnit = null;
            onSelectedTile?.Invoke(selectedTile);
        }
    }


    private bool HandleTileClick(Vector3 position, out GridTile selectedTile)
    {
        var tileHit = Physics2D.Raycast(position, Vector2.zero, 0, tileLayerMask);
        if (tileHit.collider == null)
        {
            selectedTile = null;
            return false;
        }

        selectedTile = tileHit.collider.TryGetComponent<GridTile>(out var t) ? t : null;

        return selectedTile != null;
    }

    private bool HandleBuildingClick(Vector3 position)
    {
        // send a ray at the mouse position on buildingLayerMask
        var buildingHit = Physics2D.Raycast(position, Vector2.zero, 0, buildingLayerMask);
        if (buildingHit.collider == null) return false;

        _selectedBuilding = buildingHit.collider.TryGetComponent<Building>(out var building)
            ? building
            : null;

        return _selectedBuilding != null;
    }

    private bool HandleUnitClick(Vector3 position)
    {
        // send a ray at the mouse position on unitLayerMask
        var unitHit = Physics2D.Raycast(position, Vector2.zero, 0, unitLayerMask);
        if (unitHit.collider == null) return false;

        _previousSelectedUnit = _selectedUnit;
        _selectedUnit = unitHit.collider.TryGetComponent<Unit>(out var u) ? u : null;

        return _selectedUnit != null;
    }
}