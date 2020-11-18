﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AntDesign.Core.HashCodes;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;

namespace AntDesign
{
    public partial class Table<TItem> : AntDomComponentBase, ITable
    {
        private static readonly TItem _fieldModel = (TItem)RuntimeHelpers.GetUninitializedObject(typeof(TItem));
        private static readonly EventCallbackFactory _callbackFactory = new EventCallbackFactory();

        private bool _shouldRender = true;
        private int _parametersHashCode = 0;

        [Parameter]
        public RenderMode RenderMode { get; set; } = RenderMode.Always;

        [Parameter]
        public IEnumerable<TItem> DataSource
        {
            get => _dataSource;
            set
            {
                _waitingReload = true;
                _dataSourceCount = value?.Count() ?? 0;
                _dataSource = value ?? Enumerable.Empty<TItem>();
            }
        }

        [Parameter]
        public RenderFragment<TItem> ChildContent { get; set; }

        [Parameter]
        public RenderFragment<TItem> RowTemplate { get; set; }

        [Parameter]
        public RenderFragment<RowData<TItem>> ExpandTemplate { get; set; }

        [Parameter]
        public Func<RowData<TItem>, bool> RowExpandable { get; set; } = _ => true;

        [Parameter]
        public Func<TItem, IEnumerable<TItem>> TreeChildren { get; set; } = _ => Enumerable.Empty<TItem>();

        [Parameter]
        public EventCallback<QueryModel<TItem>> OnChange { get; set; }

        [Parameter]
        public EventCallback<RowData<TItem>> OnRowClick { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleTemplate { get; set; }

        [Parameter]
        public string Footer { get; set; }

        [Parameter]
        public RenderFragment FooterTemplate { get; set; }

        [Parameter]
        public TableSize Size { get; set; }

        [Parameter]
        public TableLocale Locale { get; set; } = LocaleProvider.CurrentLocale.Table;

        [Parameter]
        public bool Bordered { get; set; } = false;

        [Parameter]
        public string ScrollX { get; set; }

        [Parameter]
        public string ScrollY { get; set; }

        [Parameter]
        public int ScrollBarWidth { get; set; } = 17;

        [Parameter]
        public int IndentSize { get; set; } = 15;

        public ColumnContext ColumnContext { get; set; } = new ColumnContext();

        private IEnumerable<TItem> _showItems;

        private IEnumerable<TItem> _dataSource;

        private bool _waitingReload = false;
        private bool _waitingReloadAndInvokeChange = false;
        private bool _treeMode = false;

        private bool ServerSide => _total > _dataSourceCount;

        bool ITable.TreeMode => _treeMode;

        int ITable.IndentSize => IndentSize;

        public void ReloadData()
        {
            PageIndex = 1;

            FlushCache();

            this.Reload();
        }

        void ITable.Refresh()
        {
            _shouldRender = true;
            StateHasChanged();
        }

        void ITable.ReloadAndInvokeChange()
        {
            ReloadAndInvokeChange();
        }

        private void ReloadAndInvokeChange()
        {
            var queryModel = this.Reload();
            if (OnChange.HasDelegate)
            {
                OnChange.InvokeAsync(queryModel);
            }
        }

        private QueryModel<TItem> Reload()
        {
            var queryModel = new QueryModel<TItem>(PageIndex, PageSize);

            foreach (var col in ColumnContext.Columns)
            {
                if (col is IFieldColumn fieldColumn && fieldColumn.Sortable)
                {
                    queryModel.AddSortModel(fieldColumn.SortModel);
                }
            }

            if (ServerSide)
            {
                _showItems = _dataSource;
            }
            else
            {
                if (_dataSource != null)
                {
                    var query = _dataSource.AsQueryable();
                    foreach (var sort in queryModel.SortModel)
                    {
                        query = sort.Sort(query);
                    }

                    query = query.Skip((PageIndex - 1) * PageSize).Take(PageSize);
                    queryModel.SetQueryableLambda(query);

                    _showItems = query;
                }
            }

            StateHasChanged();

            return queryModel;
        }

        private void SetClass()
        {
            string prefixCls = "ant-table";
            ClassMapper.Add(prefixCls)
                .If($"{prefixCls}-fixed-header", () => ScrollY != null)
                .If($"{prefixCls}-bordered", () => Bordered)
                .If($"{prefixCls}-small", () => Size == TableSize.Small)
                .If($"{prefixCls}-middle", () => Size == TableSize.Middle)
                //.Add( "ant-table ant-table-ping-left ant-table-ping-right ")
                .If($"{prefixCls}-fixed-column {prefixCls}-scroll-horizontal", () => ColumnContext.Columns.Any(x => x.Fixed.IsIn("left", "right")))
                .If($"{prefixCls}-has-fix-left", () => ColumnContext.Columns.Any(x => x.Fixed == "left"))
                .If($"{prefixCls}-has-fix-right {prefixCls}-ping-right ", () => ColumnContext.Columns.Any(x => x.Fixed == "right"))
                ;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (RowTemplate != null)
            {
                ChildContent = RowTemplate;
            }

            if (TreeChildren != null && DataSource.Any(x => TreeChildren(x).Any()))
            {
                _treeMode = true;
            }

            SetClass();

            InitializePagination();

            FlushCache();

            ReloadAndInvokeChange();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (_waitingReloadAndInvokeChange)
            {
                _waitingReloadAndInvokeChange = false;
                _waitingReload = false;

                ReloadAndInvokeChange();
            }
            else if (_waitingReload)
            {
                _waitingReload = false;
                Reload();
            }

            if (!firstRender)
            {
                this.FinishLoadPage();
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (this.RenderMode == RenderMode.ParametersHashCodeChanged)
            {
                var hashCode = this.GetParametersHashCode();
                this._shouldRender = this._parametersHashCode != hashCode;
                this._parametersHashCode = hashCode;
            }
            else
            {
                this._shouldRender = true;
            }
        }

        protected override bool ShouldRender() => this._shouldRender;

        private static void ToggleExpandRow(RowData<TItem> rowData)
        {
            rowData.Expanded = !rowData.Expanded;
        }

        private void RowClick(RowData<TItem> item)
        {
            if (OnRowClick.HasDelegate)
            {
                OnRowClick.InvokeAsync(item);
            }
        }
    }
}
