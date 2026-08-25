class OriginRule : Rule
{
    // Origin Rules are rules concerning the player's origin, things like
    // no people from a certain country or region.

    //TODO: come up with a list of rules this can choose from. 
    
    public OriginRule()
    {
        
    }

    public override bool enforceRule()
    {
        // Implementation for enforcing origin rule
        return true;
    }

}