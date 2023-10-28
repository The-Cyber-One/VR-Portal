public class PortalDisabler : PortalTraveller
{
    public override void Teleport(Portal inPortal, Portal outPortal)
    {
        // Leave empty so that portal will only disable the collision
    }
}