﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntDesign.Forms;
using AntDesign.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AntDesign
{
    public partial class Form<TModel> : AntDomComponentBase, IForm
    {
        private readonly string _prefixCls = "ant-form";

        [Parameter]
        public string Layout { get; set; } = FormLayout.Horizontal;

        [Parameter]
        public RenderFragment<TModel> ChildContent { get; set; }

        [Parameter]
        public ColLayoutParam LabelCol { get; set; }

        [Parameter]
        public ColLayoutParam WrapperCol { get; set; }

        [Parameter]
        public TModel Model { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnFinish { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnFinishFailed { get; set; }

        private EditContext _editContext;
        private IList<IFormItem> _formItems = new List<IFormItem>();
        private IList<IValueAccessor> _controls = new List<IValueAccessor>();

        internal Dictionary<string, object> FieldDefaultValues { get; private set; }

        ColLayoutParam IForm.WrapperCol => WrapperCol;

        ColLayoutParam IForm.LabelCol => LabelCol;

        EditContext IForm.EditContext => _editContext;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _editContext = new EditContext(Model);
            _editContext.OnFieldChanged += HandleFieldChanged;

            FieldDefaultValues = Model.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(Model));

            FieldDefaultValues.ForEach(item =>
            {
                Console.WriteLine($"___________FieldDefaultValues,{item.Key},{item.Value}");
            });
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            SetClass();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _editContext.OnFieldChanged -= HandleFieldChanged;
        }

        protected void SetClass()
        {
            this.ClassMapper.Clear()
                .Add(_prefixCls)
                .Add($"{_prefixCls}-{Layout.ToLower()}")
               ;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            _editContext.Validate();
            StateHasChanged();
        }

        public void HandleValidSubmit()
        {
        }

        public void Reset()
        {
            _editContext.OnFieldChanged -= HandleFieldChanged;

            _formItems.ForEach(item => item.Reset());

            _editContext.OnFieldChanged += HandleFieldChanged;
        }

        void IForm.AddFormItem(IFormItem formItem)
        {
            _formItems.Add(formItem);
        }

        void IForm.AddControl(IValueAccessor valueAccessor)
        {
            this._controls.Add(valueAccessor);
        }
    }
}
