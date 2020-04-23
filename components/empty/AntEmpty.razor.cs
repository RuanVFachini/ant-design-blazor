﻿using Microsoft.AspNetCore.Components;
using OneOf;

namespace AntBlazor
{
    public partial class AntEmpty : AntDomComponentBase
    {
        [Parameter] 
        public string PrefixCls { get; set; } = "ant-empty";

        /// <summary>
        /// "ltr"|"rtl"
        /// </summary>
        [Parameter] 
        public string Direction { get; set; } = "ltr";

        [Parameter] 
        public string ImageStyle { get; set; }

        [Parameter] 
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public OneOf<string, bool, RenderFragment> Description { get; set; } = "暂无数据";

        [Parameter]
        public OneOf<string, RenderFragment> Image { get; set; } = AntEmpty.PRESENTED_IMAGE_DEFAULT;

        protected void SetClass()
        {
            this.ClassMapper.Clear()
                .Add(PrefixCls)
                .If($"{PrefixCls}-normal", () => Image.IsT1 && Image.AsT1 == AntEmpty.PRESENTED_IMAGE_SIMPLE)
                .If($"{PrefixCls}-{Direction}", () => Direction.IsIn("ltr", "rlt"))
               ;
        }

        protected override void OnInitialized()
        {
            this.SetClass();
        }

        protected override void OnParametersSet()
        {
            this.SetClass();
        }
    }
}
