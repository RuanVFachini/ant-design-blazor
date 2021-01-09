﻿using System.Linq;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;

namespace AntDesign
{
    public class ColumnBase : AntDomComponentBase, IColumn
    {
        [CascadingParameter]
        public ITable Table { get; set; }

        [CascadingParameter(Name = "IsInitialize")]
        public bool IsInitialize { get; set; }

        [CascadingParameter(Name = "IsHeader")]
        public bool IsHeader { get; set; }

        [CascadingParameter(Name = "IsColGroup")]
        public bool IsColGroup { get; set; }

        [CascadingParameter(Name = "IsPlaceholder")]
        public bool IsPlaceholder { get; set; }

        [CascadingParameter]
        public ColumnContext Context { get; set; }

        [CascadingParameter(Name = "RowData")]
        public RowData RowData { get; set; }

        [CascadingParameter(Name = "IsMeasure")]
        public bool IsMeasure { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleTemplate { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public string HeaderStyle { get; set; }

        [Parameter]
        public int RowSpan { get; set; } = 1;

        [Parameter]
        public int ColSpan { get; set; } = 1;

        [Parameter]
        public int HeaderColSpan { get; set; } = 1;

        [Parameter]
        public string Fixed { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public int ColIndex { get; set; }

        protected string FixedStyle
        {
            get
            {
                if (Fixed == null || Context == null)
                {
                    return "";
                }

                var fixedWidth = ((CssSizeLength)Width).Value * (Fixed == "left" ? ColIndex : Context.Columns.Count - ColIndex - 1);
                if (IsHeader && Table.ScrollY != null && Table.ScrollX != null && Fixed == "right" && Context.Columns.Count - ColIndex - 1 == 0)
                {
                    fixedWidth += Table.ScrollBarWidth;
                }

                return $"position: sticky; {Fixed}: {(CssSizeLength)fixedWidth}";
            }
        }

        private void SetClass()
        {
            ClassMapper
                .Add("ant-table-cell")
                .GetIf(() => $"ant-table-cell-fix-{Fixed}", () => Fixed.IsIn("right", "left"))
                .If($"ant-table-cell-fix-right-first", () => Fixed == "right" && Context?.Columns.FirstOrDefault(x => x.Fixed == "right")?.ColIndex == this.ColIndex)
                .If($"ant-table-cell-fix-left-last", () => Fixed == "left" && Context?.Columns.LastOrDefault(x => x.Fixed == "left")?.ColIndex == this.ColIndex)
                .If($"ant-table-cell-with-append", () => ColIndex == 1 && Table.TreeMode)
                ;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (IsInitialize)
            {
                Context?.AddHeaderColumn(this);
                if (Fixed == "left")
                {
                    Table.HasFixLeft();
                }
                else if (Fixed == "right")
                {
                    Table.HasFixRight();
                }
            }
            else if (IsColGroup && Width == null)
            {
                Context?.AddColGroup(this);
            }
            else
            {
                Context?.AddRowColumn(this);
            }

            SetClass();
        }

        protected override void Dispose(bool disposing)
        {
            if (Context != null)
            {
                Context.Columns.Remove(this);
            }
            base.Dispose(disposing);
        }
    }
}
