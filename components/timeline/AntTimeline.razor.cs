﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace AntBlazor
{
    public partial class AntTimeline : AntDomComponentBase
    {
        /// <summary>
        /// 'left' | 'alternate' | 'right'
        /// </summary>
        [Parameter]
        public string Mode { get; set; }

        [Parameter]
        public bool Reverse { get; set; }

        [Parameter]
        public RenderFragment Pending { get; set; }

        protected virtual RenderFragment LoadingDot { get; }

        private AntTimelineItem PendingItem
        {
            get
            {
                if (this.Pending == null) return null;
                var item = new AntTimelineItem()
                {
                    ChildContent = !_isPendingBoolean ? Pending : null,
                    Dot = PendingDot ?? LoadingDot,
                    Class = "ant-timeline-item-pending"
                };

                item.SetClassMap();
                return item;
            }
        }

        [Parameter]
        public RenderFragment PendingDot { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private IList<AntTimelineItem> _items = new List<AntTimelineItem>();

        protected IList<AntTimelineItem> DisplayItems
        {
            get
            {
                var pitems = PendingItem != null ? new[] { PendingItem } : Array.Empty<AntTimelineItem>();
                if (Reverse)
                {
                    return pitems.Concat(UpdateChildren(_items.Reverse())).ToList();
                }
                else
                {
                    return UpdateChildren(_items).Concat(pitems).ToList();
                }
            }
        }

        private bool _isPendingBoolean;

        protected override void OnInitialized()
        {
            SetClassMap();
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            SetClassMap();
            base.OnParametersSet();
        }

        protected void SetClassMap()
        {
            var prefix = "ant-timeline";
            ClassMapper.Clear()
                .Add(prefix)
                .If($"{prefix}-right", () => Mode == "right")
                .If($"{prefix}-alternate", () => Mode == "alternate")
                .If($"{prefix}-pending", () => Pending != null)
                .If($"{prefix}-reverse", () => Reverse);
        }

        internal void AddItem(AntTimelineItem item)
        {
            this._items.Add(item);
            StateHasChanged();
        }

        protected IEnumerable<AntTimelineItem> UpdateChildren(IEnumerable<AntTimelineItem> items)
        {
            if (!items.Any())
                yield break;

            var length = items.Count();
            for (int i = 0; i < length; i++)
            {
                var item = items.ElementAt(i);
                item.IsLast = i == length - 1;
                item.Position =
                    this.Mode == "left" || Mode == null ? null
                    : this.Mode == "right" ? "right"
                    : this.Mode == "alternate" && i % 2 == 0 ? "left"
                    : "right";

                item.SetClassMap();

                yield return item;
            }
        }
    }
}
