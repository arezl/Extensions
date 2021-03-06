﻿namespace System.Web.Mvc
{
    using Text;
    using Collections.Generic;
    using Linq;
    using Linq.Expressions;
    using Dynamic;
    using Data;

    /// <summary>
    /// @Html.CheckBoxList(...) main methods
    /// </summary>
    public static class ListBuilder
    {
        static ListBuilder()
        {
            // reset internal counters
            linkedLabelCount = 0;
            htmlwrapRowbreakCount = 0;
        }

        // counter to count when to insert HTML code that breakes checkbox list
        static int htmlwrapRowbreakCount { get; set; }
        // counter to be used on a label linked to each checkbox in the list
        static int linkedLabelCount { get; set; }

        public const string No_Data_Message = "No Records...";
        public const string Empty_Model_Message = "View Model cannot be null! Please make sure your View Model is created and passed to this View";
        public const string Empty_Name_Message = "Name of the CheckBoxList cannot be null or empty";

        /// <summary>
        /// Model-Based main function
        /// </summary>
        /// <typeparam name="TModel">Current ViewModel</typeparam>
        /// <typeparam name="TItem">ViewModel Item</typeparam>
        /// <typeparam name="TValue">ViewModel Item type of the value</typeparam>
        /// <typeparam name="TKey">ViewModel Item type of the key</typeparam>
        /// <param name="htmlHelper">MVC Html helper class that is being extended</param>
        /// <param name="modelMetadata">Model Metadata</param>
        /// <param name="listName">Name of each checkbox in a list (use this name to POST list values array back to the controller)</param>
        /// <param name="sourceDataExpr">Data list to be used as a source for the list (set in viewmodel)</param>
        /// <param name="valueExpr">Data list value type to be used as checkbox 'Value'</param>
        /// <param name="textToDisplayExpr">Data list value type to be used as checkbox 'Text'</param>
        /// <param name="htmlAttributesExpr">Data list HTML tag attributes, to allow override of htmlAttributes for each checkbox (e.g. 'item => new { data_relation_id = item.RelationID }')</param>
        /// <param name="selectedValuesExpr">Data list of selected items (should be of same data type as a source list)</param>
        /// <param name="htmlAttributes">Each checkbox HTML tag attributes (e.g. 'new { class="somename" }')</param>
        /// <param name="htmlListInfo">Settings for HTML wrapper of the list (e.g. 'new HtmlListInfo2(HtmlTag2.vertical_columns, 2, new { style="color:green;" })')</param>
        /// <param name="disabledValues">String array of values to disable</param>
        /// <param name="position">Direction of the list (e.g. 'Position2.Horizontal' or 'Position2.Vertical')</param>
        /// <returns>HTML string containing checkbox list</returns>
        public static MvcHtmlString CheckBoxList<TModel, TItem, TValue, TKey>(HtmlHelper<TModel> htmlHelper, ModelMetadata modelMetadata, string listName, Expression<Func<TModel, IEnumerable<TItem>>> sourceDataExpr, Expression<Func<TItem, TValue>> valueExpr, Expression<Func<TItem, TKey>> textToDisplayExpr, Expression<Func<TItem, TKey>> htmlAttributesExpr, Expression<Func<TModel, IEnumerable<TItem>>> selectedValuesExpr, object htmlAttributes, HtmlListInfo htmlListInfo, string[] disabledValues, Position position = Position.Horizontal)
        {
            // validation
            if (sourceDataExpr == null || sourceDataExpr.Body.ToString() == "null")
                return MvcHtmlString.Create(No_Data_Message);
            if (htmlHelper.ViewData.Model == null) throw new NoNullAllowedException(Empty_Model_Message);
            if (listName.IsNullOrEmpty()) throw new ArgumentException(Empty_Name_Message, "listName");

            // set properties
            var model = htmlHelper.ViewData.Model;
            var sourceData = sourceDataExpr.Compile()(model).ToList();
            var valueFunc = valueExpr.Compile();
            var textToDisplayFunc = textToDisplayExpr.Compile();
            var selectedItems = new List<TItem>();
            if (selectedValuesExpr != null)
            {
                var _selectedItems = selectedValuesExpr.Compile()(model);
                if (_selectedItems != null) selectedItems = _selectedItems.ToList();
            }
            var selectedValues = selectedItems.Select(s => valueFunc(s).ToString()).ToList();

            // validate source data
            if (!sourceData.Any()) return MvcHtmlString.Create(No_Data_Message);

            // set html properties for each checkbox from model object
            Func<TItem, object, object> _valueHtmlAttributesFunc = (item, baseAttributes) => baseAttributes;
            if (htmlAttributesExpr != null)
            {
                var valueHtmlAttributesFunc = htmlAttributesExpr.Compile();
                _valueHtmlAttributesFunc = (item, baseAttributes) =>
                {
                    var baseAttrDict = baseAttributes.ToDictionary();
                    var itemAttrDict = valueHtmlAttributesFunc(item).ToDictionary();
                    var result = new ExpandoObject();
                    var d = result as IDictionary<string, object>;
                    foreach (var pair in baseAttrDict.Concat(itemAttrDict))
                        d[pair.Key] = pair.Value;
                    return result;
                };
            }

            // if HtmlListInfo is provided, then check for inverse text direction
            var textLayout = TextLayout.Default;
            if (htmlListInfo != null && htmlListInfo.TextLayout == TextLayout.RightToLeft)
                textLayout = htmlListInfo.TextLayout;
            if (position == Position.Vertical_RightToLeft || position == Position.Horizontal_RightToLeft)
                textLayout = TextLayout.RightToLeft;

            // set up table/list html wrapper, if applicable
            var numberOfItems = sourceData.Count;
            var htmlWrapper = CreateHtmlWrapper(htmlListInfo, numberOfItems, position, textLayout);

            // create checkbox list
            var sb = new StringBuilder();
            sb.Append(htmlWrapper.WrapOpen);
            htmlwrapRowbreakCount = 0;

            // create list of checkboxes based on data
            foreach (var item in sourceData)
            {
                // get checkbox value and text
                var itemValue = valueFunc(item).ToString();
                var itemText = textToDisplayFunc(item).ToString();

                // create checkbox element
                sb = CreateCheckBoxListElement(sb, htmlHelper, modelMetadata, htmlWrapper, _valueHtmlAttributesFunc(item, htmlAttributes), selectedValues, disabledValues, listName, itemValue, itemText, textLayout);
            }
            sb.Append(htmlWrapper.WrapClose);

            // return checkbox list
            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Creates an HTML wrapper for the checkbox list
        /// </summary>
        /// <param name="htmlListInfo">Settings for HTML wrapper of the list (e.g. 'new HtmlListInfo2(HtmlTag2.vertical_columns, 2, new { style="color:green;" })')</param>
        /// <param name="numberOfItems">Count of all items in the list</param>
        /// <param name="position">Direction of the list (e.g. 'Position2.Horizontal' or 'Position2.Vertical')</param>
        /// <param name="textLayout">Sets layout of a checkbox for right-to-left languages</param>
        /// <returns>HTML wrapper information</returns>
        private static HtmlWrapperInfo CreateHtmlWrapper(HtmlListInfo htmlListInfo, int numberOfItems, Position position, TextLayout textLayout)
        {
            var htmlWrapInfo = new HtmlWrapperInfo();
            if (htmlListInfo != null)
            {
                // creating custom layouts
                switch (htmlListInfo.HtmlTag)
                {
                // creates user selected number of float sections with
                // vertically sorted checkboxes
                case HtmlTag.VerticalColumns:
                    {
                        if (htmlListInfo.Columns <= 0) htmlListInfo.Columns = 1;
                        // calculate number of rows
                        var rows = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(numberOfItems)
                                                                / Convert.ToDecimal(htmlListInfo.Columns)));
                        if (numberOfItems <= 4 &&
                            (numberOfItems <= htmlListInfo.Columns || numberOfItems - htmlListInfo.Columns == 1))
                            rows = numberOfItems;
                        htmlWrapInfo.SeparatorMaxCount = rows;

                        // create wrapped raw html tag
                        var wrapRow = HtmlElementTag.Div;
                        var wrapHtml_builder = new TagBuilder(wrapRow.ToString());
                        var user_html_attributes = htmlListInfo.HtmlAttributes.ToDictionary();

                        // create raw style and merge it with user provided style (if applicable)
                        var defaultSectionStyle = "float:left;"; // margin-right:30px; line-height:25px;
                        if (textLayout == TextLayout.RightToLeft)
                            defaultSectionStyle += " text-align: right;";

                        object style;
                        user_html_attributes.TryGetValue("style", out style);
                        if (style != null) // if user style is set, use it
                            wrapHtml_builder.MergeAttribute("style", defaultSectionStyle + " " + style);
                        else // if not set, add only default style
                            wrapHtml_builder.MergeAttribute("style", defaultSectionStyle);

                        // merge it with other user provided attributes (e.g.: class)
                        user_html_attributes.Remove("style");
                        wrapHtml_builder.MergeAttributes(user_html_attributes);

                        // build wrapped raw html tag 
                        htmlWrapInfo.WrapOpen = wrapHtml_builder.ToString(TagRenderMode.StartTag);
                        htmlWrapInfo.WrapRowbreak = "</" + wrapRow + "> " +
                                          wrapHtml_builder.ToString(TagRenderMode.StartTag);
                        htmlWrapInfo.WrapClose = wrapHtml_builder.ToString(TagRenderMode.EndTag) +
                                       " <div style=\"clear:both;\"></div>";
                        htmlWrapInfo.AppendToElement = "<br/>";
                    }
                    break;
                // creates an html <table> with checkboxes sorted horizontally
                case HtmlTag.Table:
                    {
                        if (htmlListInfo.Columns <= 0) htmlListInfo.Columns = 1;
                        htmlWrapInfo.SeparatorMaxCount = htmlListInfo.Columns;

                        var wrapHtml_builder = new TagBuilder(HtmlElementTag.Table.ToString());
                        wrapHtml_builder.MergeAttributes(htmlListInfo.HtmlAttributes.ToDictionary());
                        wrapHtml_builder.MergeAttribute("cellspacing", "0"); // for IE7 compatibility

                        var wrapRow = HtmlElementTag.Tr;
                        htmlWrapInfo.WrapElement = HtmlElementTag.Td;
                        htmlWrapInfo.WrapOpen = wrapHtml_builder.ToString(TagRenderMode.StartTag) +
                                      "<" + wrapRow + ">";
                        htmlWrapInfo.WrapRowbreak = "</" + wrapRow + "><" + wrapRow + ">";
                        htmlWrapInfo.WrapClose = "</" + wrapRow + ">" +
                                       wrapHtml_builder.ToString(TagRenderMode.EndTag);
                    }
                    break;
                //// creates an html unordered (bulleted) list of checkboxes in one column
                //case HtmlTag.Ul:
                //    {
                //        var wrapHtml_builder = new TagBuilder(htmlElementTag.ul.ToString());
                //        wrapHtml_builder.MergeAttributes(wrapInfo.htmlAttributes.ToDictionary());
                //        wrapHtml_builder.MergeAttribute("cellspacing", "0"); // for IE7 compatibility

                //        w.wrap_element = htmlElementTag.li;
                //        w.wrap_open = wrapHtml_builder.ToString(TagRenderMode.StartTag);
                //        w.wrap_close = wrapHtml_builder.ToString(TagRenderMode.EndTag);
                //    }
                //    break;
                }
            }
            // default setting creates vertical or horizontal column of checkboxes
            else
            {
                if (position == Position.Horizontal || position == Position.Horizontal_RightToLeft)
                    htmlWrapInfo.AppendToElement = " &nbsp; ";
                if (position == Position.Vertical || position == Position.Vertical_RightToLeft)
                    htmlWrapInfo.AppendToElement = "<br/>";

                if (textLayout == TextLayout.RightToLeft)
                {
                    // lean text to right for right-to-left languages
                    var defaultSectionStyle = "style=\"text-align: right;\"";
                    var wrapRow = HtmlElementTag.Div;
                    htmlWrapInfo.WrapOpen = "<" + wrapRow + " " + defaultSectionStyle + ">";
                    htmlWrapInfo.WrapRowbreak = string.Empty;
                    htmlWrapInfo.WrapClose = "</" + wrapRow + ">";
                }
            }

            // return completed check box list wrapper
            return htmlWrapInfo;
        }

        /// <summary>
        /// Creates an an individual checkbox
        /// </summary>
        /// <param name="sb">String builder of checkbox list</param>
        /// <param name="modelMetadata">Model Metadata</param>
        /// <param name="htmlWrapper">MVC Html helper class that is being extended</param>
        /// <param name="htmlAttributesForCheckBox">Each checkbox HTML tag attributes (e.g. 'new { class="somename" }')</param>
        /// <param name="selectedValues">List of strings of selected values</param>
        /// <param name="disabledValues">List of strings of disabled values</param>
        /// <param name="name">Name of the checkbox list (same for all checkboxes)</param>
        /// <param name="itemValue">Value of the checkbox</param>
        /// <param name="itemText">Text to be displayed next to checkbox</param>
        /// <param name="htmlHelper">HtmlHelper passed from view model</param>
        /// <param name="textLayout">Sets layout of a checkbox for right-to-left languages</param>
        /// <returns>String builder of checkbox list</returns>
        private static StringBuilder CreateCheckBoxListElement(StringBuilder sb, HtmlHelper htmlHelper, ModelMetadata modelMetadata, HtmlWrapperInfo htmlWrapper, object htmlAttributesForCheckBox, IEnumerable<string> selectedValues, IEnumerable<string> disabledValues, string name, string itemValue, string itemText, TextLayout textLayout)
        {
            // get full name from view model
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            // create checkbox tag
            var checkbox_builder = new TagBuilder("input");
            if (selectedValues.Any(x => x == itemValue)) checkbox_builder.MergeAttribute("checked", "checked");
            checkbox_builder.MergeAttributes(htmlAttributesForCheckBox.ToDictionary());
            checkbox_builder.MergeAttribute("type", "checkbox");
            checkbox_builder.MergeAttribute("value", itemValue);
            checkbox_builder.MergeAttribute("name", fullName);

            // create linked label tag
            var link_name = name + linkedLabelCount++;
            checkbox_builder.GenerateId(link_name);
            var linked_label_builder = new TagBuilder("label");
            linked_label_builder.MergeAttribute("for", link_name.Replace(".", "_"));
            linked_label_builder.MergeAttributes(htmlAttributesForCheckBox.ToDictionary());
            linked_label_builder.InnerHtml = itemText;

            // if there are any errors for a named field, we add the css attribute
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
                if (modelState.Errors.Count > 0)
                    checkbox_builder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            checkbox_builder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, modelMetadata));

            // open checkbox tag wrapper
            if (textLayout == TextLayout.RightToLeft)
            {
                // then set style for displaying checkbox for right-to-left languages
                var defaultSectionStyle = "style=\"text-align: right;\"";
                sb.Append(htmlWrapper.WrapElement != HtmlElementTag.None
                            ? "<" + htmlWrapper.WrapElement + " " + defaultSectionStyle + ">"
                            : "");
            }
            else
            {
                sb.Append(htmlWrapper.WrapElement != HtmlElementTag.None
                            ? "<" + htmlWrapper.WrapElement + ">"
                            : "");
            }

            // build hidden tag for disabled checkbox (so the value will post)
            if (disabledValues != null && disabledValues.ToList().Any(x => x == itemValue))
            {
                // set main checkbox to be disabled
                checkbox_builder.MergeAttribute("disabled", "disabled");

                // create a hidden input with checkbox value
                // so it can be posted if checked
                if (selectedValues.Any(x => x == itemValue))
                {
                    var hidden_input_builder = new TagBuilder("input");
                    hidden_input_builder.MergeAttribute("type", "hidden");
                    hidden_input_builder.MergeAttribute("value", itemValue);
                    hidden_input_builder.MergeAttribute("name", name);
                    sb.Append(hidden_input_builder.ToString(TagRenderMode.Normal));
                }
            }

            // create checkbox and tag combination
            if (textLayout == TextLayout.RightToLeft)
            {
                // then display checkbox for right-to-left languages
                sb.Append(linked_label_builder.ToString(TagRenderMode.Normal));
                sb.Append(checkbox_builder.ToString(TagRenderMode.Normal));
            }
            else
            {
                sb.Append(checkbox_builder.ToString(TagRenderMode.Normal));
                sb.Append(linked_label_builder.ToString(TagRenderMode.Normal));
            }

            // close checkbox tag wrapper
            sb.Append(htmlWrapper.WrapElement != HtmlElementTag.None
                        ? "</" + htmlWrapper.WrapElement + ">"
                        : "");

            // add element ending
            sb.Append(htmlWrapper.AppendToElement);

            // add table column break, if applicable
            htmlwrapRowbreakCount += 1;
            if (htmlwrapRowbreakCount == htmlWrapper.SeparatorMaxCount)
            {
                sb.Append(htmlWrapper.WrapRowbreak);
                htmlwrapRowbreakCount = 0;
            }

            // return string builder with checkbox html markup
            return sb;
        }
    }
}
