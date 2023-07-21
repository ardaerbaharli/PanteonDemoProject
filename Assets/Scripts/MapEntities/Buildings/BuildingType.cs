using System.ComponentModel;

namespace MapEntities.Buildings
{
    public enum BuildingType
    {
        [Description("Barracks")] Barracks,
        [Description("Power Plant")] PowerPlant,
        [Description("Headquarters")] Headquarters,
        [Description("Manufactory")] Manufactory
    }
}