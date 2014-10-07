namespace Tvl.VisualStudio.Text.EditorNavigation.Implementation
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IEditorNavigationTypeDefinitionMetadata
    {
        string Name
        {
            get;
        }

        [DefaultValue(null)]
        IEnumerable<string> BaseDefinition
        {
            get;
        }
    }
}
