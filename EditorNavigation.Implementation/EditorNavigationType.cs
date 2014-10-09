﻿namespace Tvl.VisualStudio.Text.EditorNavigation.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tvl.VisualStudio.Text.EditorNavigation.Interfaces;

    internal class EditorNavigationType : IEditorNavigationType
    {
        private static readonly IEditorNavigationType[] EmptyNavigationTypes = new IEditorNavigationType[0];

        public EditorNavigationType(EditorNavigationTypeDefinition definition, string type, IEnumerable<IEditorNavigationType> baseTypes)
        {
            if (type == null)
                throw new ArgumentNullException();

            baseTypes = baseTypes ?? EmptyNavigationTypes;
            if (baseTypes.Contains(null))
                throw new ArgumentException();
            if (baseTypes.Any(b => b.IsOfType(type)))
                throw new ArgumentException();

            this.Type = type;
            this.BaseTypes = baseTypes.ToArray();
            this.Definition = definition;
        }

        public IEnumerable<IEditorNavigationType> BaseTypes
        {
            get;
            private set;
        }

        public string Type
        {
            get;
            private set;
        }

        public EditorNavigationTypeDefinition Definition
        {
            get;
            private set;
        }

        public bool IsOfType(string type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (this.Type == type)
                return true;

            if (this.BaseTypes.Any(b => b.IsOfType(type)))
                return true;

            return false;
        }
    }
}
