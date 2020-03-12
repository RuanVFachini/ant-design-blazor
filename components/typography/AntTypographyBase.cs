﻿using AntBlazor.typography;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntBlazor
{
    public abstract class AntTypographyBase : AntDomComponentBase
    {
        [Inject]
        public HtmlRenderService _service { get; set; }
        [Parameter]
        public bool copyable { get; set; } = false;
        [Parameter]
        public TypographyCopyableConfig copyConfig { get; set; }
        [Parameter]
        public bool delete { get; set; } = false;
        [Parameter]
        public bool disabled { get; set; } = false;
        [Parameter]
        public bool editable { get; set; } = false;
        [Parameter]
        public TypographyEditableConfig editConfig { get; set; }
        [Parameter]
        public bool ellipsis { get; set; } = false;
        [Parameter]
        public TypographyEllipsisConfig ellipsisConfig {get;set;}
        [Parameter]
        public bool mark { get; set; } = false;
        [Parameter]
        public bool underline { get; set; } = false;
        [Parameter]
        public bool strong { get; set; } = false;
        [Parameter]
        public Action onChange { get; set; }
        
        [Parameter]
        public string type { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public async Task Copy()
        {
            if (!copyable)
            {
                return;
            }
            else if (copyConfig is null)
            {
                await this.JsInvokeAsync<object>(JSInteropConstants.copy, await _service.RenderAsync(ChildContent));
            }
            else if (copyConfig.onCopy is null)
            {
                if (string.IsNullOrEmpty(copyConfig.text))
                {
                    await this.JsInvokeAsync<object>(JSInteropConstants.copy, await _service.RenderAsync(ChildContent));
                }
                else
                {
                    await this.JsInvokeAsync<object>(JSInteropConstants.copy, copyConfig.text);
                }
            }
            else
            {
                copyConfig.onCopy.Invoke();
            }
        }
    }

    public class TypographyCopyableConfig
    {
        public string text { get; set; } = string.Empty;
        public Action onCopy { get; set; } = null;
    }

    public class TypographyEditableConfig
    {
        public Action onStart { get; set; }
        public Action<string> onChange { get; set; }
    }

    public class TypographyEllipsisConfig
    {
        public string suffix { get; set; } = "...";
        public int rows { get; set; }
        public Action onExpand { get; set; }
    }
}
