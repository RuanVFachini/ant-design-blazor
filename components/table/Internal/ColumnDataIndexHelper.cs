﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using AntDesign.core.Helpers;
using AntDesign.TableModels;

namespace AntDesign.Internal
{
    internal static class ColumnDataIndexHelper<TProp>
    {
        private static readonly ConcurrentDictionary<ColumnCacheKey, ColumnCacheItem> _dataIndexCache = new();

        internal static ColumnCacheItem GetDataIndexConfig(Column<TProp> column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            var cacheKey = ColumnCacheKey.Create(column);
            return _dataIndexCache.GetOrAdd(cacheKey, CreateDataIndexConfig);
        }

        private static ColumnCacheItem CreateDataIndexConfig(ColumnCacheKey key)
        {
            var (itemType, propType, dataIndex, sortable, sort, sorterCompare) = key;
            Func<RowData, TProp> getValue = null;
            ITableSortModel sortModel = null;
            var properties = dataIndex?.Split(".");
            if (properties is { Length: > 0 })
            {
                var isNullable = propType.IsValueType && Nullable.GetUnderlyingType(propType) != null;
                var rowDataType = typeof(RowData);
                var rowData1Type = typeof(RowData<>).MakeGenericType(itemType);
                var rowDataExp = Expression.Parameter(rowDataType);
                var rowData1Exp = Expression.TypeAs(rowDataExp, rowData1Type);
                var dataMemberExp = Expression.Property(rowData1Exp, nameof(RowData<object>.Data));

                Expression memberExp = isNullable
                                           ? PropertyAccessHelper.AccessNullableProperty(dataMemberExp, properties)
                                           : PropertyAccessHelper.AccessProperty(dataMemberExp, properties);
                getValue = Expression.Lambda<Func<RowData, TProp>>(memberExp, rowDataExp).Compile();

                if (sortable)
                {
                    var propertySelector = isNullable
                                               ? PropertyAccessHelper.BuildNullablePropertyAccessExpression(itemType, properties)
                                               : PropertyAccessHelper.BuildPropertyAccessExpression(itemType, properties);
                    sortModel = new DataIndexSortModel<TProp>(dataIndex, propertySelector, 1, sort, sorterCompare);
                }
            }

            return new ColumnCacheItem(getValue, sortModel);
        }

        internal readonly struct ColumnCacheKey
        {
            internal readonly Type ItemType;

            internal readonly Type PropType;

            internal readonly string DataIndex;

            internal readonly bool Sortable;

            internal readonly string Sort;

            internal readonly Func<TProp, TProp, int> SorterCompare;

            internal static ColumnCacheKey Create(Column<TProp> column)
            {
                return new(column.ItemType, typeof(TProp), column.DataIndex, column.Sortable, column.Sort, column.SorterCompare);
            }

            internal ColumnCacheKey(Type itemType, Type propType, string dataIndex, bool sortable, string sort, Func<TProp, TProp, int> sorterCompare)
            {
                ItemType = itemType;
                PropType = propType;
                DataIndex = dataIndex;
                Sortable = sortable;
                Sort = sort;
                SorterCompare = sorterCompare;
            }

            internal void Deconstruct(out Type itemType, out Type propType, out string dataIndex, out bool sortable, out string sort, out Func<TProp, TProp, int> sorterCompare)
            {
                itemType = ItemType;
                propType = PropType;
                dataIndex = DataIndex;
                sortable = Sortable;
                sort = Sort;
                sorterCompare = SorterCompare;
            }
        }

        internal readonly struct ColumnCacheItem
        {
            internal readonly Func<RowData, TProp> GetValue;

            internal readonly ITableSortModel SortModel;

            internal ColumnCacheItem(Func<RowData, TProp> getValue, ITableSortModel sortModel)
            {
                GetValue = getValue;
                SortModel = sortModel;
            }

            internal void Deconstruct(out Func<RowData, TProp> getValue, out ITableSortModel sortModel)
            {
                getValue = GetValue;
                sortModel = SortModel;
            }
        }
    }
}
