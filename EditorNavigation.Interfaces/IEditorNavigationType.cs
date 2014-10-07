namespace Tvl.VisualStudio.Text.EditorNavigation.Interfaces
{
    using System.Collections.Generic;

    public interface IEditorNavigationType
    {
        IEnumerable<IEditorNavigationType> BaseTypes
        {
            get;
        }

        string Type
        {
            get;
        }

        EditorNavigationTypeDefinition Definition
        {
            get;
        }

        bool IsOfType(string type);
    }
}
