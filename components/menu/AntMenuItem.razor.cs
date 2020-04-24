﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace AntBlazor
{
    public partial class AntMenuItem : AntDomComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public bool Selected { get; set; } = false;

        [Parameter]
        public int? PaddingLeft { get; set; }

        [Parameter]
        public bool MatchRouterExact { get; set; } = false;

        [Parameter]
        public bool MatchRouter { get; set; } = false;

        [CascadingParameter]
        public AntMenu Menu { get; set; }

        [CascadingParameter]
        public AntSubMenu SubMenu { get; set; }

        private readonly int _originalPadding = 0;

        private void SetClassMap()
        {
            string prefixName = Menu.IsInDropDown ? "ant-dropdown-menu-item" : "ant-menu-item";
            ClassMapper.Clear()
                .Add(prefixName)
                .If($"{prefixName}-selected", () => Selected)
                .If($"{prefixName}-disabled", () => Disabled);
        }

        internal void SelectedChanged(bool value)
        {
            this.Selected = value;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (this is AntMenuItem item)
            {
                Menu?.MenuItems.Add(item);
                SubMenu?.Items.Add(item);
            }

            int? padding;
            if (Menu.Mode == AntDirectionVHIType.inline)
            {
                if (PaddingLeft != null)
                {
                    padding = PaddingLeft;
                }
                else
                {
                    int level = SubMenu?.Level + 1 ?? 1;
                    padding = level * this.Menu.InlineIndent;
                }
            }
            else
            {
                padding = _originalPadding;
            }

            if (padding != null)
            {
                Style += $"padding-left:{padding}px;";
            }

            SetClassMap();
        }
    }
}
