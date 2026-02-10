namespace NueGames.NTooltip
{
    public interface ITooltipTarget_Collider : ITooltipTarget
    { 
        void OnMouseEnter(); 
        void OnMouseExit();
    }
}