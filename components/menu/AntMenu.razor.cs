﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace AntBlazor
{
    public partial class AntMenu : AntDomComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int InlineIndent { get; set; } = 24;

        [Parameter]
        public string Theme { get; set; } = "light";//'light' | 'dark' = 'light';

        [Parameter]
        public AntDirectionVHIType Mode { get; set; } = AntDirectionVHIType.vertical;

        [Parameter]
        public bool InDropDown { get; set; } = false;

        [Parameter]
        public bool InlineCollapsed { get; set; } = false;

        [Parameter]
        public bool Selectable { get; set; }

        [Parameter]
        public EventCallback<AntMenuItem> Click { get; set; }

        public IList<AntMenuItem> _menuItems = new List<AntMenuItem>();

        public IList<AntSubMenu> _subMenus = new List<AntSubMenu>();

        public IList<AntSubMenu> _openedSubMenus = new List<AntSubMenu>();

        public bool IsInDropDown { get; set; }

        private AntDirectionVHIType _cacheMode;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetClassMap();
            IsInDropDown = InDropDown;
            _cacheMode = Mode;
        }

        private void SetClassMap()
        {
            string prefixName = IsInDropDown ? "ant-dropdown-menu" : "ant-menu";
            ClassMapper.Add(prefixName)
                .Add($"{prefixName}-root")
                .Add($"{prefixName}-{Theme}")
                .Add($"{prefixName}-{Mode}")
                .If($"{prefixName}-inline-collapsed", () => InlineCollapsed);
        }

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSet();
            await UpdateInLineCollapse();
        }

        private async Task UpdateInLineCollapse()
        {
            if (_menuItems.Any())
            {
                if (InlineCollapsed)
                {
                    _openedSubMenus = this._subMenus.Where(x => x.Open).ToList();
                    foreach (var antSubMenu in this._subMenus)
                    {
                        await antSubMenu.SetOpenState(false);
                    }

                    this.Mode = AntDirectionVHIType.vertical;
                }
                else
                {
                    foreach (var subMenu in _openedSubMenus)
                    {
                        await subMenu.SetOpenState(false);
                    }
                    _openedSubMenus.Clear();
                    this.Mode = this._cacheMode;
                }
                StateHasChanged();
            }
        }
    }
}
