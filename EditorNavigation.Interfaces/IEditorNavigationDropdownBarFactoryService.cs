namespace Tvl.VisualStudio.Text.EditorNavigation.Interfaces
{
    using IVsCodeWindow = Microsoft.VisualStudio.TextManager.Interop.IVsCodeWindow;
    using IVsEditorAdaptersFactoryService = Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService;

    public interface IEditorNavigationDropdownBarFactoryService
    {
        IEditorNavigationDropdownBarClient CreateEditorNavigationDropdownBar(IVsCodeWindow codeWindow, IVsEditorAdaptersFactoryService editorAdaptersFactory);
    }
}
