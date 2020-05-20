﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace AntBlazor
{
    public partial class InputGroup : AntDomComponentBase
    {
        protected const string PrefixCls = "ant-input-group";

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Size { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ClassMapper.Clear()
                .Add(PrefixCls)
                .If($"{PrefixCls}-lg", () => Size == InputSize.Large)
                .If($"{PrefixCls}-sm", () => Size == InputSize.Small)
                .If($"{PrefixCls}-compact", () => Attributes != null && Attributes.ContainsKey("compact"));
        }
    }
}
