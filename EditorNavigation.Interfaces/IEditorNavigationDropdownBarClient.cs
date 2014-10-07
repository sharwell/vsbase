namespace Tvl.VisualStudio.Text.EditorNavigation.Interfaces
{
    using Microsoft.VisualStudio.TextManager.Interop;

    public interface IEditorNavigationDropdownBarClient : IVsDropdownBarClient
    {
        int DropdownCount
        {
            get;
        }
    }
}
