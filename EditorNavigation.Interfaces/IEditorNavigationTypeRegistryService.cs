namespace Tvl.VisualStudio.Text.EditorNavigation.Interfaces
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(Contracts.IEditorNavigationTypeRegistryServiceContracts))]
    public interface IEditorNavigationTypeRegistryService
    {
        IEditorNavigationType CreateEditorNavigationType(EditorNavigationTypeDefinition definition, string type, IEnumerable<IEditorNavigationType> baseTypes);
        IEditorNavigationType CreateTransientEditorNavigationType(IEnumerable<IEditorNavigationType> baseTypes);
        IEditorNavigationType CreateTransientEditorNavigationType(params IEditorNavigationType[] baseTypes);
        IEditorNavigationType GetEditorNavigationType(string type);
    }
}
