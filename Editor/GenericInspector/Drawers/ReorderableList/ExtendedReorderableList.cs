/*MIT License

Copyright (c) 2017 Chris Foulston

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.GenericInspector.Drawers.ReorderableList {
    public class ExtendedReorderableList {
        private const float ElementEdgeTop = 1;
        private const float ElementEdgeBot = 3;
        private const float ElementHeightOffset = ElementEdgeTop + ElementEdgeBot;

        private static readonly int SelectionHash = "ReorderableListSelection".GetHashCode();
        private static readonly int DragAndDropHash = "ReorderableListDragAndDrop".GetHashCode();

        private const string EmptyLabel = "List is Empty";
        private const string ArrayError = "{0} is not an Array!";

        public enum ElementDisplayType {
            Auto,
            Expandable,
            SingleLine
        }

        public delegate void DrawHeaderDelegate(Rect rect, GUIContent label);

        public delegate void DrawFooterDelegate(Rect rect);

        public delegate void DrawElementDelegate(Rect rect, SerializedProperty element, GUIContent label, bool selected,
            bool focused);

        public delegate void ActionDelegate(ExtendedReorderableList list);

        public delegate bool ActionBoolDelegate(ExtendedReorderableList list);

        public delegate void AddDropdownDelegate(Rect buttonRect, ExtendedReorderableList list);

        public delegate Object DragDropReferenceDelegate(Object[] references, ExtendedReorderableList list);

        public delegate void DragDropAppendDelegate(Object reference, ExtendedReorderableList list);

        public delegate float GetElementHeightDelegate(SerializedProperty element);

        public delegate float GetElementsHeightDelegate(ExtendedReorderableList list);

        public delegate string GetElementNameDelegate(SerializedProperty element);

        public delegate GUIContent GetElementLabelDelegate(SerializedProperty element);

        public event DrawHeaderDelegate DrawHeaderCallback;
        public event DrawFooterDelegate DrawFooterCallback;
        public event DrawElementDelegate DrawElementCallback;
        public event DrawElementDelegate DrawElementBackgroundCallback;
        public event GetElementHeightDelegate GetElementHeightCallback;
        public event GetElementsHeightDelegate GetElementsHeightCallback;
        public event GetElementNameDelegate GetElementNameCallback;
        public event GetElementLabelDelegate GetElementLabelCallback;
        public event DragDropReferenceDelegate OnValidateDragAndDropCallback;
        public event DragDropAppendDelegate OnAppendDragDropCallback;
        public event ActionDelegate OnReorderCallback;
        public event ActionDelegate OnSelectCallback;
        public event ActionDelegate OnAddCallback;
        public event AddDropdownDelegate OnAddDropdownCallback;
        public event ActionDelegate OnRemoveCallback;
        public event ActionDelegate OnMouseUpCallback;
        public event ActionBoolDelegate OnCanRemoveCallback;
        public event ActionDelegate OnChangedCallback;

        public bool canAdd;
        public bool canRemove;
        public bool draggable;
        public bool sortable;
        public bool expandable;
        public bool multipleSelection;
        public GUIContent label;
        public float headerHeight;
        public float footerHeight;
        public float slideEasing;
        public float verticalSpacing;
        public bool showDefaultBackground;
        public ElementDisplayType elementDisplayType;
        public string elementNameProperty;
        public string elementNameOverride;
        public bool elementLabels;
        public Texture elementIcon;
        public bool sceneReferences;

        public bool Paginate {
            get => _pagination.enabled;
            set => _pagination.enabled = value;
        }

        public int PageSize {
            get => _pagination.fixedPageSize;
            set => _pagination.fixedPageSize = value;
        }

        internal readonly int id;

        private SerializedProperty _list;
        private int _controlID = -1;
        private Rect[] _elementRects;
        private readonly GUIContent _elementLabel;
        private readonly GUIContent _pageInfoContent;
        private readonly GUIContent _pageSizeContent;
        private ListSelection _selection;
        private readonly SlideGroup _slideGroup;
        private int _pressIndex;

        private bool DoPagination => _pagination.enabled && !_list.serializedObject.isEditingMultipleObjects;

        private float ElementSpacing => Mathf.Max(0, verticalSpacing - 2);

        private bool _dragging;
        private float _pressPosition;
        private float _dragPosition;
        private int _dragDirection;
        private DragList _dragList;
        private ListSelection _beforeDragSelection;
        private Pagination _pagination;

        private int _dragDropControlID = -1;

        public ExtendedReorderableList(SerializedProperty list, bool canAdd, bool canRemove, bool draggable,
            ElementDisplayType elementDisplayType, string elementNameProperty, Texture elementIcon)
            : this(list, canAdd, canRemove, draggable, elementDisplayType, elementNameProperty, null, elementIcon) { }

        public ExtendedReorderableList(SerializedProperty list, bool canAdd = true, bool canRemove = true,
            bool draggable = true, ElementDisplayType elementDisplayType = ElementDisplayType.Auto,
            string elementNameProperty = null, string elementNameOverride = null, Texture elementIcon = null, bool sceneReferences = false) {
            if(list == null) {
                throw new MissingListException();
            }

            if(!list.isArray) {
                //check if user passed in a ReorderableArray, if so, that becomes the list object

                SerializedProperty array = list.FindPropertyRelative("array");

                if(array == null || !array.isArray) {
                    throw new InvalidListException();
                }

                _list = array;
            } else {
                _list = list;
            }

            this.canAdd = canAdd;
            this.canRemove = canRemove;
            this.draggable = draggable;
            this.elementDisplayType = elementDisplayType;
            this.elementNameProperty = elementNameProperty;
            this.elementNameOverride = elementNameOverride;
            this.elementIcon = elementIcon;
            this.sceneReferences = sceneReferences;

            id = GetHashCode();
            list.isExpanded = true;
            label = new GUIContent(list.displayName);
            _pageInfoContent = new GUIContent();
            _pageSizeContent = new GUIContent();

#if UNITY_5_6_OR_NEWER
            verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
#else
			verticalSpacing = 2f;
#endif
            headerHeight = 18f;
            footerHeight = 13f;
            slideEasing = 0.15f;
            expandable = true;
            elementLabels = true;
            showDefaultBackground = true;
            multipleSelection = true;
            _pagination = new Pagination();
            _elementLabel = new GUIContent();

            _dragList = new DragList(0);
            _selection = new ListSelection();
            _slideGroup = new SlideGroup();
            _elementRects = new Rect[0];
        }

        //
        // -- PROPERTIES --
        //

        public SerializedProperty List {
            get => _list;
            internal set => _list = value;
        }

        public bool HasList => _list != null && _list.isArray;

        public int Length {
            get {
                if(!HasList) {
                    return 0;
                }

                if(!_list.hasMultipleDifferentValues) {
                    return _list.arraySize;
                }

                //When multiple objects are selected, because of a Unity bug, list.arraySize is never guranteed to actually be the smallest
                //array size. So we have to find it. Not that great since we're creating SerializedObjects here. There has to be a better way!

                int smallerArraySize = _list.arraySize;

                foreach(Object targetObject in _list.serializedObject.targetObjects) {
                    SerializedObject serializedObject = new SerializedObject(targetObject);
                    SerializedProperty property = serializedObject.FindProperty(_list.propertyPath);

                    smallerArraySize = Mathf.Min(property.arraySize, smallerArraySize);
                }

                return smallerArraySize;
            }
        }

        public int VisibleLength => _pagination.GetVisibleLength(Length);

        public int[] Selected {
            get => _selection.ToArray();
            set => _selection = new ListSelection(value);
        }

        public int Index {
            get => _selection.First;
            set => _selection.Select(value);
        }

        public bool IsDragging => _dragging;

        //
        // -- PUBLIC --
        //

        public float GetHeight() {
            if(HasList) {
                float topHeight = DoPagination ? headerHeight * 2 : headerHeight;

                return _list.isExpanded ? topHeight + GetElementsHeight() + footerHeight : headerHeight;
            }

            return EditorGUIUtility.singleLineHeight;
        }

        public void DoLayoutList() {
            Rect position = EditorGUILayout.GetControlRect(false, GetHeight(), EditorStyles.largeLabel);

            DoList(EditorGUI.IndentedRect(position), label);
        }

        public void DoList(Rect rect, GUIContent label) {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect headerRect = rect;
            headerRect.height = headerHeight;

            if(!HasList) {
                DrawEmpty(headerRect, string.Format(ArrayError, label.text), GUIStyle.none, EditorStyles.helpBox);
            } else {
                _controlID = GUIUtility.GetControlID(SelectionHash, FocusType.Keyboard, rect);
                _dragDropControlID = GUIUtility.GetControlID(DragAndDropHash, FocusType.Passive, rect);

                DrawHeader(headerRect, label);

                if(_list.isExpanded) {
                    if(DoPagination) {
                        Rect paginateHeaderRect = headerRect;
                        paginateHeaderRect.y += headerRect.height;

                        DrawPaginationHeader(paginateHeaderRect);

                        headerRect.yMax = paginateHeaderRect.yMax - 1;
                    }

                    Rect elementBackgroundRect = rect;
                    elementBackgroundRect.yMin = headerRect.yMax;
                    elementBackgroundRect.yMax = rect.yMax - footerHeight;

                    Event evt = Event.current;

                    if(_selection.Length > 1) {
                        if(evt.type == EventType.ContextClick && CanSelect(evt.mousePosition)) {
                            HandleMultipleContextClick(evt);
                        }
                    }

                    if(Length > 0) {
                        //update element rects if not dragging. Dragging caches draw rects so no need to update

                        if(!_dragging) {
                            UpdateElementRects(elementBackgroundRect, evt);
                        }

                        if(_elementRects.Length > 0) {
                            int start, end;

                            _pagination.GetVisibleRange(_elementRects.Length, out start, out end);

                            Rect selectableRect = elementBackgroundRect;
                            selectableRect.yMin = _elementRects[start].yMin;
                            selectableRect.yMax = _elementRects[end - 1].yMax;

                            HandlePreSelection(selectableRect, evt);
                            DrawElements(elementBackgroundRect, evt);
                            HandlePostSelection(selectableRect, evt);
                        }
                    } else {
                        DrawEmpty(elementBackgroundRect, EmptyLabel, Style.BoxBackground, Style.VerticalLabel);
                    }

                    Rect footerRect = rect;
                    footerRect.yMin = elementBackgroundRect.yMax;
                    footerRect.xMin = rect.xMax - 58;

                    DrawFooter(footerRect);
                }
            }

            EditorGUI.indentLevel = indent;
        }

        public SerializedProperty AddItem<T>(T item) where T : Object {
            SerializedProperty property = AddItem();

            if(property != null) {
                property.objectReferenceValue = item;
            }

            return property;
        }

        public SerializedProperty AddItem() {
            if(HasList) {
                //TODO Validate add on multiple selected objects

                _list.arraySize++;
                _selection.Select(_list.arraySize - 1);

                SetPageByIndex(_list.arraySize - 1);
                DispatchChange();

                return _list.GetArrayElementAtIndex(_selection.Last);
            }

            throw new InvalidListException();
        }

        public void Remove(int[] selection) {
            System.Array.Sort(selection);

            int i = selection.Length;

            while(--i > -1) {
                RemoveItem(selection[i]);
            }
        }

        public void RemoveItem(int index) {
            if(index >= 0 && index < Length) {
                SerializedProperty property = _list.GetArrayElementAtIndex(index);

                if(property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue) {
                    property.objectReferenceValue = null;
                }

                _list.DeleteArrayElementAtIndex(index);
                _selection.Remove(index);

                //TODO Validate removal on multiple selected objects

                if(Length > 0) {
                    _selection.Select(Mathf.Max(0, index - 1));
                }

                DispatchChange();
            }
        }

        public SerializedProperty GetItem(int index) {
            if(index >= 0 && index < Length) {
                return _list.GetArrayElementAtIndex(index);
            }

            return null;
        }

        public int IndexOf(SerializedProperty element) {
            if(element != null) {
                int i = Length;

                while(--i > -1) {
                    if(SerializedProperty.EqualContents(element, _list.GetArrayElementAtIndex(i))) {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void GrabKeyboardFocus() {
            GUIUtility.keyboardControl = id;
        }

        public bool HasKeyboardControl() {
            return GUIUtility.keyboardControl == id;
        }

        public void ReleaseKeyboardFocus() {
            if(GUIUtility.keyboardControl == id) {
                GUIUtility.keyboardControl = 0;
            }
        }

        public void SetPage(int page) {
            if(DoPagination) {
                _pagination.page = page;
            }
        }

        public void SetPageByIndex(int index) {
            if(DoPagination) {
                _pagination.page = _pagination.GetPageForIndex(index);
            }
        }

        public int GetPage(int index) {
            return DoPagination ? _pagination.page : 0;
        }

        public int GetPageByIndex(int index) {
            return DoPagination ? _pagination.GetPageForIndex(index) : 0;
        }

        //
        // -- PRIVATE --
        //

        private float GetElementsHeight() {
            if(GetElementsHeightCallback != null) {
                return GetElementsHeightCallback(this);
            }

            int i, len = Length;

            if(len == 0) {
                return 28;
            }

            float totalHeight = 0;
            float spacing = ElementSpacing;

            int start, end;

            _pagination.GetVisibleRange(len, out start, out end);

            for(i = start; i < end; i++) {
                totalHeight += GetElementHeight(_list.GetArrayElementAtIndex(i)) + spacing;
            }

            return totalHeight + 7 - spacing;
        }

        private float GetElementHeight(SerializedProperty element) {
            if(GetElementHeightCallback != null) {
                return GetElementHeightCallback(element) + ElementHeightOffset;
            }

            return EditorGUI.GetPropertyHeight(element, GetElementLabel(element, elementLabels),
                       IsElementExpandable(element)) + ElementHeightOffset;
        }

        private Rect GetElementDrawRect(int index, Rect desiredRect) {
            if(slideEasing <= 0) {
                return desiredRect;
            }
            //lerp the drag easing toward slide easing, this creates a stronger easing at the start then slower at the end
            //when dealing with large lists, we can

            return _dragging
                ? _slideGroup.GetRect(_dragList[index].startIndex, desiredRect, slideEasing)
                : _slideGroup.SetRect(index, desiredRect);
        }

        /*
        private Rect GetElementHeaderRect(SerializedProperty element, Rect elementRect) {

            Rect rect = elementRect;
            rect.height = EditorGUIUtility.singleLineHeight + verticalSpacing;

            return rect;
        }
        */

        private Rect GetElementRenderRect(SerializedProperty element, Rect elementRect) {
            float offset = draggable ? 20 : 5;

            Rect rect = elementRect;
            rect.xMin += IsElementExpandable(element) ? offset + 10 : offset;
            rect.xMax -= 5;
            rect.yMin += ElementEdgeTop;
            rect.yMax -= ElementEdgeBot;

            return rect;
        }

        private void DrawHeader(Rect rect, GUIContent label) {
            if(showDefaultBackground && Event.current.type == EventType.Repaint) {
                Style.HeaderBackground.Draw(rect, false, false, false, false);
            }

            HandleDragAndDrop(rect, Event.current);

            bool multiline = elementDisplayType != ElementDisplayType.SingleLine;

            Rect titleRect = rect;
            titleRect.xMin += 6f;
            titleRect.xMax -= multiline ? 95f : 55f;
            titleRect.height -= 2f;
            titleRect.y++;

            label = EditorGUI.BeginProperty(titleRect, label, _list);

            if(DrawHeaderCallback != null) {
                DrawHeaderCallback(titleRect, label);
            } else if(expandable) {
                titleRect.xMin += 10;

                EditorGUI.BeginChangeCheck();

                bool isExpanded = EditorGUI.Foldout(titleRect, _list.isExpanded, label, true);

                if(EditorGUI.EndChangeCheck()) {
                    _list.isExpanded = isExpanded;
                }
            } else {
                GUI.Label(titleRect, label, EditorStyles.label);
            }

            EditorGUI.EndProperty();

            if(multiline) {
                Rect bRect1 = rect;
                bRect1.xMin = rect.xMax - 25;
                bRect1.xMax = rect.xMax - 5;

                if(GUI.Button(bRect1, Style.ExpandButton, Style.PreButton)) {
                    ExpandElements(true);
                }

                Rect bRect2 = rect;
                bRect2.xMin = bRect1.xMin - 20;
                bRect2.xMax = bRect1.xMin;

                if(GUI.Button(bRect2, Style.CollapseButton, Style.PreButton)) {
                    ExpandElements(false);
                }

                rect.xMax = bRect2.xMin + 5;
            }

            //draw sorting options

            if(sortable) {
                Rect sortRect1 = rect;
                sortRect1.xMin = rect.xMax - 25;
                sortRect1.xMax = rect.xMax;

                Rect sortRect2 = rect;
                sortRect2.xMin = sortRect1.xMin - 20;
                sortRect2.xMax = sortRect1.xMin;

                if(EditorGUI.DropdownButton(sortRect1, Style.SortAscending, FocusType.Passive, Style.PreButton)) {
                    SortElements(sortRect1, false);
                }

                if(EditorGUI.DropdownButton(sortRect2, Style.SortDescending, FocusType.Passive, Style.PreButton)) {
                    SortElements(sortRect2, true);
                }
            }
        }

        private void ExpandElements(bool expand) {
            if(!_list.isExpanded && expand) {
                _list.isExpanded = true;
            }

            int i, len = Length;

            for(i = 0; i < len; i++) {
                _list.GetArrayElementAtIndex(i).isExpanded = expand;
            }
        }

        private void SortElements(Rect rect, bool descending) {
            int total = Length;

            //no point in sorting a list with 1 element!

            if(total <= 1) {
                return;
            }

            //the first property tells us what type of items are in the list
            //if generic, then we give the user a list of properties to sort on

            SerializedProperty prop = _list.GetArrayElementAtIndex(0);

            if(prop.propertyType == SerializedPropertyType.Generic) {
                GenericMenu menu = new GenericMenu();

                SerializedProperty property = prop.Copy();
                SerializedProperty end = property.GetEndProperty();

                bool enterChildren = true;

                while(property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, end)) {
                    menu.AddItem(new GUIContent(property.name), false, userData => {
                        //sort based on the property selected then apply the changes

                        ListSort.SortOnProperty(_list, total, descending, (string) userData);

                        ApplyReorder();

                        HandleUtility.Repaint();
                    }, property.name);

                    enterChildren = false;
                }

                menu.DropDown(rect);
            } else {
                //list is not generic, so we just sort directly on the type then apply the changes

                ListSort.SortOnType(_list, total, descending, prop.propertyType);

                ApplyReorder();
            }
        }

        private void DrawEmpty(Rect rect, string label, GUIStyle backgroundStyle, GUIStyle labelStyle) {
            if(showDefaultBackground && Event.current.type == EventType.Repaint) {
                backgroundStyle.Draw(rect, false, false, false, false);
            }

            EditorGUI.LabelField(rect, label, labelStyle);
        }

        private void UpdateElementRects(Rect rect, Event evt) {
            //resize array if elements changed

            int i, len = Length;

            if(len != _elementRects.Length) {
                System.Array.Resize(ref _elementRects, len);
            }

            if(evt.type == EventType.Repaint) {
                //start rect

                Rect elementRect = rect;
                elementRect.yMin = elementRect.yMax = rect.yMin + 2;

                float spacing = ElementSpacing;

                int start, end;

                _pagination.GetVisibleRange(len, out start, out end);

                for(i = start; i < end; i++) {
                    SerializedProperty element = _list.GetArrayElementAtIndex(i);

                    //update the elementRects value for this object. Grab the last elementRect for startPosition

                    elementRect.y = elementRect.yMax;
                    elementRect.height = GetElementHeight(element);
                    _elementRects[i] = elementRect;

                    elementRect.yMax += spacing;
                }
            }
        }

        private void DrawElements(Rect rect, Event evt) {
            //draw list background

            if(showDefaultBackground && evt.type == EventType.Repaint) {
                Style.BoxBackground.Draw(rect, false, false, false, false);
            }

            //if not dragging, draw elements as usual

            if(!_dragging) {
                int start, end;

                _pagination.GetVisibleRange(Length, out start, out end);

                for(int i = start; i < end; i++) {
                    bool selected = _selection.Contains(i);

                    DrawElement(_list.GetArrayElementAtIndex(i), GetElementDrawRect(i, _elementRects[i]), selected,
                        selected && GUIUtility.keyboardControl == _controlID);
                }
            } else if(evt.type == EventType.Repaint) {
                //draw dragging elements only when repainting

                int i, s, len = _dragList.Length;
                int sLen = _selection.Length;

                //first, find the rects of the selected elements, we need to use them for overlap queries

                for(i = 0; i < sLen; i++) {
                    DragElement element = _dragList[i];

                    //update the element desiredRect if selected. Selected elements appear first in the dragList, so other elements later in iteration will have rects to compare

                    element.desiredRect.y = _dragPosition - element.dragOffset;
                    _dragList[i] = element;
                }

                //draw elements, start from the bottom of the list as first elements are the ones selected, so should be drawn last

                i = len;

                while(--i > -1) {
                    DragElement element = _dragList[i];

                    //draw dragging elements last as the loop is backwards

                    if(element.selected) {
                        DrawElement(element.property, element.desiredRect, true, true);
                        continue;
                    }

                    //loop over selection and see what overlaps
                    //if dragging down we start from the bottom of the selection
                    //otherwise we start from the top. This helps to cover multiple selected objects

                    Rect elementRect = element.rect;
                    int elementIndex = element.startIndex;

                    int start = _dragDirection > 0 ? sLen - 1 : 0;
                    int end = _dragDirection > 0 ? -1 : sLen;

                    for(s = start; s != end; s -= _dragDirection) {
                        DragElement selected = _dragList[s];

                        if(selected.Overlaps(elementRect, elementIndex, _dragDirection)) {
                            elementRect.y -= selected.rect.height * _dragDirection;
                            elementIndex += _dragDirection;
                        }
                    }

                    //draw the element with the new rect

                    DrawElement(element.property, GetElementDrawRect(i, elementRect), false, false);

                    //reassign the element back into the dragList

                    element.desiredRect = elementRect;
                    _dragList[i] = element;
                }
            }
        }

        private void DrawElement(SerializedProperty element, Rect rect, bool selected, bool focused) {
            Event evt = Event.current;

            if(DrawElementBackgroundCallback != null) {
                DrawElementBackgroundCallback(rect, element, null, selected, focused);
            } else if(evt.type == EventType.Repaint) {
                Style.ElementBackground.Draw(rect, false, selected, selected, focused);
            }

            if(evt.type == EventType.Repaint && draggable) {
                Style.DraggingHandle.Draw(new Rect(rect.x + 5, rect.y + 6, 10, rect.height - (rect.height - 6)), false,
                    false, false, false);
            }

            GUIContent label = GetElementLabel(element, elementLabels);

            Rect renderRect = GetElementRenderRect(element, rect);

            if(DrawElementCallback != null) {
                DrawElementCallback(renderRect, element, label, selected, focused);
            } else if(sceneReferences) {
                EditorGUI.ObjectField(renderRect, element, label);
            } else {
                EditorGUI.PropertyField(renderRect, element, label, true);
            }

            //handle context click

            int controlId = GUIUtility.GetControlID(label, FocusType.Passive, rect);

            switch(evt.GetTypeForControl(controlId)) {
                case EventType.ContextClick:

                    if(rect.Contains(evt.mousePosition)) {
                        HandleSingleContextClick(evt, element);
                    }

                    break;
            }
        }

        private GUIContent GetElementLabel(SerializedProperty element, bool allowElementLabel) {
            if(!allowElementLabel) {
                return GUIContent.none;
            }

            if(GetElementLabelCallback != null) {
                return GetElementLabelCallback(element);
            }

            string name;

            if(GetElementNameCallback != null) {
                name = GetElementNameCallback(element);
            } else {
                name = GetElementName(element, elementNameProperty, elementNameOverride);
            }

            _elementLabel.text = !string.IsNullOrEmpty(name) ? name : element.displayName;
            _elementLabel.tooltip = element.tooltip;
            _elementLabel.image = elementIcon;

            return _elementLabel;
        }

        private static string GetElementName(SerializedProperty element, string nameProperty, string nameOverride) {
            if(!string.IsNullOrEmpty(nameOverride)) {
                string path = element.propertyPath;

                const string arrayEndDelimeter = "]";
                const char arrayStartDelimeter = '[';

                if(path.EndsWith(arrayEndDelimeter)) {
                    int startIndex = path.LastIndexOf(arrayStartDelimeter) + 1;

                    return string.Format("{0} {1}", nameOverride,
                        path.Substring(startIndex, path.Length - startIndex - 1));
                }

                return nameOverride;
            }

            if(string.IsNullOrEmpty(nameProperty)) {
                return null;
            }

            if(element.propertyType == SerializedPropertyType.ObjectReference && nameProperty == "name") {
                return element.objectReferenceValue ? element.objectReferenceValue.name : null;
            }

            SerializedProperty prop = element.FindPropertyRelative(nameProperty);

            if(prop != null) {
                switch(prop.propertyType) {
                    case SerializedPropertyType.ObjectReference:

                        return prop.objectReferenceValue ? prop.objectReferenceValue.name : null;

                    case SerializedPropertyType.Enum:

                        return prop.enumDisplayNames[prop.enumValueIndex];

                    case SerializedPropertyType.Integer:
                    case SerializedPropertyType.Character:

                        return prop.intValue.ToString();

                    case SerializedPropertyType.LayerMask:

                        return GetLayerMaskName(prop.intValue);

                    case SerializedPropertyType.String:

                        return prop.stringValue;

                    case SerializedPropertyType.Float:

                        return prop.floatValue.ToString();
                }

                return prop.displayName;
            }

            return null;
        }

        private static string GetLayerMaskName(int mask) {
            if(mask == 0) {
                return "Nothing";
            }

            if(mask < 0) {
                return "Everything";
            }

            string name = string.Empty;
            int n = 0;

            for(int i = 0; i < 32; i++) {
                if(((1 << i) & mask) != 0) {
                    if(n == 4) {
                        return "Mixed ...";
                    }

                    name += (n > 0 ? ", " : string.Empty) + LayerMask.LayerToName(i);
                    n++;
                }
            }

            return name;
        }

        private void DrawFooter(Rect rect) {
            if(DrawFooterCallback != null) {
                DrawFooterCallback(rect);
                return;
            }

            if(Event.current.type == EventType.Repaint) {
                Style.FooterBackground.Draw(rect, false, false, false, false);
            }

            Rect addRect = new Rect(rect.xMin + 4f, rect.y - 3f, 25f, 13f);
            Rect subRect = new Rect(rect.xMax - 29f, rect.y - 3f, 25f, 13f);

            EditorGUI.BeginDisabledGroup(!canAdd);

            if(GUI.Button(addRect, OnAddDropdownCallback != null ? Style.IconToolbarPlusMore : Style.IconToolbarPlus,
                Style.PreButton)) {
                if(OnAddDropdownCallback != null) {
                    OnAddDropdownCallback(addRect, this);
                } else if(OnAddCallback != null) {
                    OnAddCallback(this);
                } else {
                    AddItem();
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!CanSelect(_selection) || !canRemove ||
                                         (OnCanRemoveCallback != null && !OnCanRemoveCallback(this)));

            if(GUI.Button(subRect, Style.IconToolbarMinus, Style.PreButton)) {
                if(OnRemoveCallback != null) {
                    OnRemoveCallback(this);
                } else {
                    Remove(_selection.ToArray());
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawPaginationHeader(Rect rect) {
            int total = Length;
            int pages = _pagination.GetPageCount(total);
            int page = Mathf.Clamp(_pagination.page, 0, pages - 1);

            //some actions may have reduced the page count, so we need to check the current page against the clamped one
            //if different, we need to change and repaint

            if(page != _pagination.page) {
                _pagination.page = page;

                HandleUtility.Repaint();
            }

            Rect prevRect = new Rect(rect.xMin + 4f, rect.y - 1f, 17f, 14f);
            Rect popupRect = new Rect(prevRect.xMax, rect.y - 1f, 14f, 14f);
            Rect nextRect = new Rect(popupRect.xMax, rect.y - 1f, 17f, 14f);

            if(Event.current.type == EventType.Repaint) {
                Style.PaginationHeader.Draw(rect, false, true, true, false);
            }

            _pageInfoContent.text = string.Format(Style.PageInfoFormat, _pagination.page + 1, pages);

            Rect pageInfoRect = rect;
            pageInfoRect.width = Style.PaginationText.CalcSize(_pageInfoContent).x;
            pageInfoRect.x = rect.xMax - pageInfoRect.width - 7;
            pageInfoRect.y += 2;

            //draw page info

            GUI.Label(pageInfoRect, _pageInfoContent, Style.PaginationText);

            //draw page buttons and page popup

            if(GUI.Button(prevRect, Style.IconPagePrev, Style.PreButton)) {
                _pagination.page = Mathf.Max(0, _pagination.page - 1);
            }

            if(EditorGUI.DropdownButton(popupRect, Style.IconPagePopup, FocusType.Passive, Style.PreButton)) {
                GenericMenu menu = new GenericMenu();

                for(int i = 0; i < pages; i++) {
                    int pageIndex = i;

                    menu.AddItem(new GUIContent(string.Format("Page {0}", i + 1)), i == _pagination.page,
                        OnPageDropDownSelect, pageIndex);
                }

                menu.DropDown(popupRect);
            }

            if(GUI.Button(nextRect, Style.IconPageNext, Style.PreButton)) {
                _pagination.page = Mathf.Min(pages - 1, _pagination.page + 1);
            }

            //if we're allowed to control the page size manually, show an editor

            bool useFixedPageSize = _pagination.fixedPageSize > 0;

            EditorGUI.BeginDisabledGroup(useFixedPageSize);

            _pageSizeContent.text = total.ToString();

            GUIStyle style = Style.PageSizeTextField;
            Texture icon = Style.ListIcon.image;

            float min = nextRect.xMax + 5;
            float max = pageInfoRect.xMin - 5;
            float space = max - min;
            float labelWidth = icon.width + 2;
            float width = style.CalcSize(_pageSizeContent).x + 50 + labelWidth;

            Rect pageSizeRect = rect;
            pageSizeRect.y--;
            pageSizeRect.x = min + (space - width) / 2;
            pageSizeRect.width = width - labelWidth;

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.SetIconSize(new Vector2(icon.width, icon.height));

            int newPageSize = EditorGUI.DelayedIntField(pageSizeRect, Style.ListIcon,
                useFixedPageSize ? _pagination.fixedPageSize : _pagination.customPageSize, style);

            EditorGUIUtility.labelWidth = 0;
            EditorGUIUtility.SetIconSize(Vector2.zero);

            if(EditorGUI.EndChangeCheck()) {
                _pagination.customPageSize = Mathf.Clamp(newPageSize, 0, total);
                _pagination.page = Mathf.Min(_pagination.GetPageCount(total) - 1, _pagination.page);
            }

            EditorGUI.EndDisabledGroup();
        }

        private void OnPageDropDownSelect(object userData) {
            _pagination.page = (int) userData;
        }

        private void DispatchChange() {
            if(OnChangedCallback != null) {
                OnChangedCallback(this);
            }
        }

        private void HandleSingleContextClick(Event evt, SerializedProperty element) {
            _selection.Select(IndexOf(element));

            GenericMenu menu = new GenericMenu();

            if(element.isInstantiatedPrefab) {
                menu.AddItem(new GUIContent("Revert " + GetElementLabel(element, true).text + " to Prefab"), false,
                    _selection.RevertValues, _list);
                menu.AddSeparator(string.Empty);
            }

            HandleSharedContextClick(evt, menu, "Duplicate Array Element", "Delete Array Element",
                "Move Array Element");
        }

        private void HandleMultipleContextClick(Event evt) {
            GenericMenu menu = new GenericMenu();

            if(_selection.CanRevert(_list)) {
                menu.AddItem(new GUIContent("Revert Values to Prefab"), false, _selection.RevertValues, _list);
                menu.AddSeparator(string.Empty);
            }

            HandleSharedContextClick(evt, menu, "Duplicate Array Elements", "Delete Array Elements",
                "Move Array Elements");
        }

        private void HandleSharedContextClick(Event evt, GenericMenu menu, string duplicateLabel, string deleteLabel,
            string moveLabel) {
            menu.AddItem(new GUIContent(duplicateLabel), false, HandleDuplicate, _list);
            menu.AddItem(new GUIContent(deleteLabel), false, HandleDelete, _list);

            if(DoPagination) {
                int pages = _pagination.GetPageCount(Length);

                if(pages > 1) {
                    for(int i = 0; i < pages; i++) {
                        string label = string.Format("{0}/Page {1}", moveLabel, i + 1);

                        menu.AddItem(new GUIContent(label), i == _pagination.page, HandleMoveElement, i);
                    }
                }
            }

            menu.ShowAsContext();

            evt.Use();
        }

        private void HandleMoveElement(object userData) {
            int toPage = (int) userData;
            int fromPage = _pagination.page;
            int size = _pagination.PageSize;
            int offset = (toPage * size) - (fromPage * size);
            int direction = offset > 0 ? 1 : -1;
            int total = Length;

            //We need to find the actually positions things will move to and not clamp the index
            //because sometimes something wants to move to a negative index, or beyond the length
            //we need to find this overlow and adjust the move offsets based on that

            int overflow = 0;

            for(int i = 0; i < _selection.Length; i++) {
                int desiredIndex = _selection[i] + offset;

                overflow = direction < 0
                    ? Mathf.Min(overflow, desiredIndex)
                    : Mathf.Max(overflow, desiredIndex - total);
            }

            offset -= overflow;

            //copy the current list to prepare for moving

            UpdateDragList(0, 0, total);

            //create a list that will act as our new order

            List<DragElement> orderedList =
                new List<DragElement>(_dragList.Elements.Where(t => !_selection.Contains(t.startIndex)));

            //go through the selection and insert them into the new order based on the page offset

            _selection.Sort();

            for(int i = 0; i < _selection.Length; i++) {
                int selIndex = _selection[i];
                int oldIndex = _dragList.GetIndexFromSelection(selIndex);
                int newIndex = Mathf.Clamp(selIndex + offset, 0, orderedList.Count);

                orderedList.Insert(newIndex, _dragList[oldIndex]);
            }

            //finally, perform the re-order

            _dragList.Elements = orderedList.ToArray();

            ReorderDraggedElements(direction, 0, null);

            //assume we still want to view these items

            _pagination.page = toPage;

            HandleUtility.Repaint();
        }

        private void HandleDelete(object userData) {
            _selection.Delete(userData as SerializedProperty);

            DispatchChange();
        }

        private void HandleDuplicate(object userData) {
            _selection.Duplicate(userData as SerializedProperty);

            DispatchChange();
        }

        private void HandleDragAndDrop(Rect rect, Event evt) {
            switch(evt.GetTypeForControl(_dragDropControlID)) {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if(GUI.enabled && rect.Contains(evt.mousePosition)) {
                        Object[] objectReferences = DragAndDrop.objectReferences;
                        Object[] references = new Object[1];

                        bool acceptDrag = false;

                        foreach(Object object1 in objectReferences) {
                            references[0] = object1;
                            Object object2 = ValidateObjectDragAndDrop(references);

                            if(object2 != null) {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                if(evt.type == EventType.DragPerform) {
                                    if(OnAppendDragDropCallback != null) {
                                        OnAppendDragDropCallback(object2, this);
                                    } else {
                                        AppendDragAndDropValue(object2);
                                    }

                                    acceptDrag = true;
                                    DragAndDrop.activeControlID = 0;
                                } else {
                                    DragAndDrop.activeControlID = _dragDropControlID;
                                }
                            }
                        }

                        if(acceptDrag) {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                        }
                    }

                    break;

                case EventType.DragExited:

                    if(GUI.enabled) {
                        HandleUtility.Repaint();
                    }

                    break;
            }
        }

        private Object ValidateObjectDragAndDrop(Object[] references) {
            if(OnValidateDragAndDropCallback != null) {
                return OnValidateDragAndDropCallback(references, this);
            }

            return Internals.ValidateObjectDragAndDrop(references, _list);
        }

        private void AppendDragAndDropValue(Object obj) {
            Internals.AppendDragAndDropValue(obj, _list);

            DispatchChange();
        }

        private void HandlePreSelection(Rect rect, Event evt) {
            if(evt.type == EventType.MouseDrag && draggable && GUIUtility.hotControl == _controlID) {
                if(_selection.Length > 0 && UpdateDragPosition(evt.mousePosition, rect, _dragList)) {
                    GUIUtility.keyboardControl = _controlID;
                    _dragging = true;
                }

                evt.Use();
            }

            /* TODO This is buggy. The reason for this is to allow selection and dragging of an element using the header, or top row (if any)
             * The main issue here is determining whether the element has an "expandable" drop down arrow, which if it does, will capture the mouse event *without* the code below
             * Because of property drawers and certain property types, it's impossible to know this automatically (without dirty reflection)
             * So if the below code is active and we determine that the property is expandable but isn't actually. Then we'll accidently capture the mouse focus and prevent anything else from receiving it :(
             * So for now, in order to drag or select a row, the user must select empty space on the row. Not a huge deal, and doesn't break functionality.
             * What needs to happen is the drag event needs to occur independent of the event type. But that's messy too, as some controls have horizontal drag sliders :(
            if (evt.type == EventType.MouseDown) {

                //check if we contain the mouse press
                //we also need to check what has current focus. If nothing we can assume control
                //if there's something, check if the header has been pressed if the element is expandable
                //if we did press the header, then override the control

                if (rect.Contains(evt.mousePosition) && IsSelectionButton(evt)) {

                    int index = GetSelectionIndex(evt.mousePosition);

                    if (CanSelect(index)) {

                        SerializedProperty element = list.GetArrayElementAtIndex(index);

                        if (IsElementExpandable(element)) {

                            Rect elementHeaderRect = GetElementHeaderRect(element, elementRects[index]);
                            Rect elementRenderRect = GetElementRenderRect(element, elementRects[index]);

                            Rect elementExpandRect = elementHeaderRect;
                            elementExpandRect.xMin = elementRenderRect.xMin - 10;
                            elementExpandRect.xMax = elementRenderRect.xMin;

                            if (elementHeaderRect.Contains(evt.mousePosition) && !elementExpandRect.Contains(evt.mousePosition)) {

                                DoSelection(index, true, evt);
                                HandleUtility.Repaint();
                            }
                        }
                    }
                }
            }
            */
        }

        private void HandlePostSelection(Rect rect, Event evt) {
            switch(evt.GetTypeForControl(_controlID)) {
                case EventType.MouseDown:

                    if(rect.Contains(evt.mousePosition) && IsSelectionButton(evt)) {
                        int index = GetSelectionIndex(evt.mousePosition);

                        if(CanSelect(index)) {
                            DoSelection(index,
                                GUIUtility.keyboardControl == 0 || GUIUtility.keyboardControl == _controlID ||
                                evt.button == 2, evt);
                        } else {
                            _selection.Clear();
                        }

                        HandleUtility.Repaint();
                    }

                    break;

                case EventType.MouseUp:

                    if(!draggable) {
                        //select the single object if no selection modifier is being performed

                        _selection.SelectWhenNoAction(_pressIndex, evt);

                        if(OnMouseUpCallback != null && IsPositionWithinElement(evt.mousePosition, _selection.Last)) {
                            OnMouseUpCallback(this);
                        }
                    } else if(GUIUtility.hotControl == _controlID) {
                        evt.Use();

                        if(_dragging) {
                            _dragging = false;

                            //move elements in list

                            ReorderDraggedElements(_dragDirection, _dragList.StartIndex,
                                () => _dragList.SortByPosition());
                        } else {
                            //if we didn't drag, then select the original pressed object

                            _selection.SelectWhenNoAction(_pressIndex, evt);

                            if(OnMouseUpCallback != null) {
                                OnMouseUpCallback(this);
                            }
                        }

                        GUIUtility.hotControl = 0;
                    }

                    HandleUtility.Repaint();

                    break;

                case EventType.KeyDown:

                    if(GUIUtility.keyboardControl == _controlID) {
                        if(evt.keyCode == KeyCode.DownArrow && !_dragging) {
                            _selection.Select(Mathf.Min(_selection.Last + 1, Length - 1));
                            evt.Use();
                        } else if(evt.keyCode == KeyCode.UpArrow && !_dragging) {
                            _selection.Select(Mathf.Max(_selection.Last - 1, 0));
                            evt.Use();
                        } else if(evt.keyCode == KeyCode.Escape && GUIUtility.hotControl == _controlID) {
                            GUIUtility.hotControl = 0;

                            if(_dragging) {
                                _dragging = false;
                                _selection = _beforeDragSelection;
                            }

                            evt.Use();
                        }
                    }

                    break;
            }
        }

        private bool IsSelectionButton(Event evt) {
            return evt.button == 0 || evt.button == 2;
        }

        private void DoSelection(int index, bool setKeyboardControl, Event evt) {
            //append selections based on action, this may be a additive (ctrl) or range (shift) selection

            if(multipleSelection) {
                _selection.AppendWithAction(_pressIndex = index, evt);
            } else {
                _selection.Select(_pressIndex = index);
            }

            if(OnSelectCallback != null) {
                OnSelectCallback(this);
            }

            if(draggable) {
                _dragging = false;
                _dragPosition = _pressPosition = evt.mousePosition.y;

                int start, end;

                _pagination.GetVisibleRange(Length, out start, out end);

                UpdateDragList(_dragPosition, start, end);

                _selection.Trim(start, end);

                _beforeDragSelection = _selection.Clone();

                GUIUtility.hotControl = _controlID;
            }

            if(setKeyboardControl) {
                GUIUtility.keyboardControl = _controlID;
            }

            evt.Use();
        }

        private void UpdateDragList(float dragPosition, int start, int end) {
            _dragList.Resize(start, end - start);

            for(int i = start; i < end; i++) {
                SerializedProperty property = _list.GetArrayElementAtIndex(i);
                Rect elementRect = _elementRects[i];

                DragElement dragElement = new DragElement() {
                    property = property,
                    dragOffset = dragPosition - elementRect.y,
                    rect = elementRect,
                    desiredRect = elementRect,
                    selected = _selection.Contains(i),
                    startIndex = i
                };

                _dragList[i - start] = dragElement;
            }

            //finally, sort the dragList by selection, selected objects appear first in the list
            //selection order is preserved as well

            _dragList.SortByIndex();
        }

        private bool UpdateDragPosition(Vector2 position, Rect bounds, DragList dragList) {
            //find new drag position

            int startIndex = 0;
            int endIndex = _selection.Length - 1;

            float minOffset = dragList[startIndex].dragOffset;
            float maxOffset = dragList[endIndex].rect.height - dragList[endIndex].dragOffset;

            _dragPosition = Mathf.Clamp(position.y, bounds.yMin + minOffset, bounds.yMax - maxOffset);

            if(Mathf.Abs(_dragPosition - _pressPosition) > 1) {
                _dragDirection = (int) Mathf.Sign(_dragPosition - _pressPosition);
                return true;
            }

            return false;
        }

        private void ReorderDraggedElements(int direction, int offset, System.Action sortList) {
            //save the current expanded states on all elements. I don't see any other way to do this
            //MoveArrayElement does not move the foldout states, so... fun.

            _dragList.RecordState();

            if(sortList != null) {
                sortList();
            }

            _selection.Sort((a, b) => {
                int d1 = _dragList.GetIndexFromSelection(a);
                int d2 = _dragList.GetIndexFromSelection(b);

                return direction > 0 ? d1.CompareTo(d2) : d2.CompareTo(d1);
            });

            //swap the selected elements in the List

            int s = _selection.Length;

            while(--s > -1) {
                int newIndex = _dragList.GetIndexFromSelection(_selection[s]);
                int listIndex = newIndex + offset;

                _selection[s] = listIndex;

                _list.MoveArrayElement(_dragList[newIndex].startIndex, listIndex);
            }

            //restore expanded states on items

            _dragList.RestoreState(_list);

            //apply and update

            ApplyReorder();
        }

        private void ApplyReorder() {
            _list.serializedObject.ApplyModifiedProperties();
            _list.serializedObject.Update();

            if(OnReorderCallback != null) {
                OnReorderCallback(this);
            }

            DispatchChange();
        }

        private int GetSelectionIndex(Vector2 position) {
            int start, end;

            _pagination.GetVisibleRange(_elementRects.Length, out start, out end);

            for(int i = start; i < end; i++) {
                Rect rect = _elementRects[i];

                if(rect.Contains(position) || (i == 0 && position.y <= rect.yMin) ||
                   (i == end - 1 && position.y >= rect.yMax)) {
                    return i;
                }
            }

            return -1;
        }

        private bool CanSelect(ListSelection selection) {
            return selection.Length > 0 ? selection.All(s => CanSelect(s)) : false;
        }

        private bool CanSelect(int index) {
            return index >= 0 && index < Length;
        }

        private bool CanSelect(Vector2 position) {
            return _selection.Length > 0 ? _selection.Any(s => IsPositionWithinElement(position, s)) : false;
        }

        private bool IsPositionWithinElement(Vector2 position, int index) {
            return CanSelect(index) ? _elementRects[index].Contains(position) : false;
        }

        private bool IsElementExpandable(SerializedProperty element) {
            switch(elementDisplayType) {
                case ElementDisplayType.Auto:

                    return element.hasVisibleChildren && IsTypeExpandable(element.propertyType);

                case ElementDisplayType.Expandable: return true;
                case ElementDisplayType.SingleLine: return false;
            }

            return false;
        }

        private bool IsTypeExpandable(SerializedPropertyType type) {
            switch(type) {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ArraySize:

                    return true;

                default:

                    return false;
            }
        }

        //
        // -- LIST STYLE --
        //

        private static class Style {
            internal const string PageInfoFormat = "{0} / {1}";

            internal static readonly GUIContent IconToolbarPlus;
            internal static readonly GUIContent IconToolbarPlusMore;
            internal static readonly GUIContent IconToolbarMinus;
            internal static readonly GUIContent IconPagePrev;
            internal static readonly GUIContent IconPageNext;
            internal static readonly GUIContent IconPagePopup;

            internal static readonly GUIStyle PaginationText;
            internal static readonly GUIStyle PageSizeTextField;
            internal static readonly GUIStyle DraggingHandle;
            internal static readonly GUIStyle HeaderBackground;
            internal static readonly GUIStyle FooterBackground;
            internal static readonly GUIStyle PaginationHeader;
            internal static readonly GUIStyle BoxBackground;
            internal static readonly GUIStyle PreButton;
            internal static readonly GUIStyle ElementBackground;
            internal static readonly GUIStyle VerticalLabel;
            internal static readonly GUIContent ExpandButton;
            internal static readonly GUIContent CollapseButton;
            internal static readonly GUIContent SortAscending;
            internal static readonly GUIContent SortDescending;

            internal static readonly GUIContent ListIcon;

            static Style() {
                IconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
                IconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
                IconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
                IconPagePrev = EditorGUIUtility.IconContent("Animation.PrevKey", "Previous page");
                IconPageNext = EditorGUIUtility.IconContent("Animation.NextKey", "Next page");
#if UNITY_2018_3_OR_NEWER
                IconPagePopup = EditorGUIUtility.IconContent("ShurikenPopup", "Select page");
#else
				IconPagePopup = EditorGUIUtility.IconContent("MiniPopupNoBg", "Select page");
#endif

                PaginationText = new GUIStyle {
                    margin = new RectOffset(2, 2, 0, 0),
                    fontSize = EditorStyles.miniTextField.fontSize,
                    font = EditorStyles.miniFont,
                    normal = {textColor = EditorStyles.miniTextField.normal.textColor},
                    alignment = TextAnchor.UpperLeft,
                    clipping = TextClipping.Clip
                };

                PageSizeTextField = new GUIStyle("RL Footer") {
                    alignment = TextAnchor.MiddleLeft,
                    clipping = TextClipping.Clip,
                    fixedHeight = 0,
                    padding = new RectOffset(3, 0, 0, 0),
                    overflow = new RectOffset(0, 0, -2, -3),
                    contentOffset = new Vector2(0, -1),
                    font = EditorStyles.miniFont,
                    fontSize = EditorStyles.miniTextField.fontSize,
                    fontStyle = FontStyle.Normal,
                    wordWrap = false
                };

                DraggingHandle = new GUIStyle("RL DragHandle");
                HeaderBackground = new GUIStyle("RL Header");
                FooterBackground = new GUIStyle("RL Footer");
                //paginationHeader = new GUIStyle("RectangleToolHBar");
                PaginationHeader = new GUIStyle("RL Element");
                PaginationHeader.border = new RectOffset(2, 3, 2, 3);
                ElementBackground = new GUIStyle("RL Element");
                ElementBackground.border = new RectOffset(2, 3, 2, 3);
                VerticalLabel = new GUIStyle(EditorStyles.label);
                VerticalLabel.alignment = TextAnchor.UpperLeft;
                VerticalLabel.contentOffset = new Vector2(10, 3);
                BoxBackground = new GUIStyle("RL Background");
                BoxBackground.border = new RectOffset(6, 3, 3, 6);
                PreButton = new GUIStyle("RL FooterButton");

                ExpandButton = EditorGUIUtility.IconContent("winbtn_win_max");
                ExpandButton.tooltip = "Expand All Elements";

                CollapseButton = EditorGUIUtility.IconContent("winbtn_win_min");
                CollapseButton.tooltip = "Collapse All Elements";

                SortAscending = EditorGUIUtility.IconContent("align_vertically_bottom");
                SortAscending.tooltip = "Sort Ascending";

                SortDescending = EditorGUIUtility.IconContent("align_vertically_top");
                SortDescending.tooltip = "Sort Descending";

                ListIcon = EditorGUIUtility.IconContent("align_horizontally_right");
            }
        }

        //
        // -- DRAG LIST --
        //

        private struct DragList {
            private int _startIndex;
            private DragElement[] _elements;
            private int _length;

            internal DragList(int length) {
                _length = length;

                _startIndex = 0;
                _elements = new DragElement[length];
            }

            internal int StartIndex => _startIndex;

            internal int Length => _length;

            internal DragElement[] Elements {
                get => _elements;
                set => _elements = value;
            }

            internal DragElement this[int index] {
                get => _elements[index];
                set => _elements[index] = value;
            }

            internal void Resize(int start, int length) {
                _startIndex = start;

                _length = length;

                if(_elements.Length != length) {
                    System.Array.Resize(ref _elements, length);
                }
            }

            internal void SortByIndex() {
                System.Array.Sort(_elements, (a, b) => {
                    if(b.selected) {
                        return a.selected ? a.startIndex.CompareTo(b.startIndex) : 1;
                    }

                    if(a.selected) {
                        return b.selected ? b.startIndex.CompareTo(a.startIndex) : -1;
                    }

                    return a.startIndex.CompareTo(b.startIndex);
                });
            }

            internal void RecordState() {
                for(int i = 0; i < _length; i++) {
                    _elements[i].RecordState();
                }
            }

            internal void RestoreState(SerializedProperty list) {
                for(int i = 0; i < _length; i++) {
                    _elements[i].RestoreState(list.GetArrayElementAtIndex(i + _startIndex));
                }
            }

            internal void SortByPosition() {
                System.Array.Sort(_elements, (a, b) => a.desiredRect.center.y.CompareTo(b.desiredRect.center.y));
            }

            internal int GetIndexFromSelection(int index) {
                return System.Array.FindIndex(_elements, t => t.startIndex == index);
            }
        }

        //
        // -- DRAG ELEMENT --
        //

        private struct DragElement {
            internal SerializedProperty property;
            internal int startIndex;
            internal float dragOffset;
            internal bool selected;
            internal Rect rect;
            internal Rect desiredRect;

            private bool _isExpanded;
            private Dictionary<int, bool> _states;

            internal bool Overlaps(Rect value, int index, int direction) {
                if(direction < 0 && index < startIndex) {
                    return desiredRect.yMin < value.center.y;
                }

                if(direction > 0 && index > startIndex) {
                    return desiredRect.yMax > value.center.y;
                }

                return false;
            }

            internal void RecordState() {
                _states = new Dictionary<int, bool>();
                _isExpanded = property.isExpanded;

                Iterate(this, property,
                    (DragElement e, SerializedProperty p, int index) => { e._states[index] = p.isExpanded; });
            }

            internal void RestoreState(SerializedProperty property) {
                property.isExpanded = _isExpanded;

                Iterate(this, property,
                    (DragElement e, SerializedProperty p, int index) => { p.isExpanded = e._states[index]; });
            }

            private static void Iterate(DragElement element, SerializedProperty property,
                System.Action<DragElement, SerializedProperty, int> action) {
                SerializedProperty copy = property.Copy();
                SerializedProperty end = copy.GetEndProperty();

                int index = 0;

                while(copy.NextVisible(true) && !SerializedProperty.EqualContents(copy, end)) {
                    if(copy.hasVisibleChildren) {
                        action(element, copy, index);
                        index++;
                    }
                }
            }
        }

        //
        // -- SLIDE GROUP --
        //

        private class SlideGroup {
            private readonly Dictionary<int, Rect> _animIDs;

            public SlideGroup() {
                _animIDs = new Dictionary<int, Rect>();
            }

            public Rect GetRect(int id, Rect r, float easing) {
                if(Event.current.type != EventType.Repaint) {
                    return r;
                }

                if(!_animIDs.ContainsKey(id)) {
                    _animIDs.Add(id, r);
                    return r;
                }

                Rect rect = _animIDs[id];

                if(rect.y != r.y) {
                    float delta = r.y - rect.y;
                    float absDelta = Mathf.Abs(delta);

                    //if the distance between current rect and target is too large, then move the element towards the target rect so it reaches the destination faster

                    if(absDelta > (rect.height * 2)) {
                        r.y = delta > 0 ? r.y - rect.height : r.y + rect.height;
                    } else if(absDelta > 0.5) {
                        r.y = Mathf.Lerp(rect.y, r.y, easing);
                    }

                    _animIDs[id] = r;
                    HandleUtility.Repaint();
                }

                return r;
            }

            public Rect SetRect(int id, Rect rect) {
                if(_animIDs.ContainsKey(id)) {
                    _animIDs[id] = rect;
                } else {
                    _animIDs.Add(id, rect);
                }

                return rect;
            }
        }

        //
        // -- PAGINATION --
        //

        private struct Pagination {
            internal bool enabled;
            internal int fixedPageSize;
            internal int customPageSize;
            internal int page;

            internal bool UsePagination => enabled && PageSize > 0;

            internal int PageSize => fixedPageSize > 0 ? fixedPageSize : customPageSize;

            internal int GetVisibleLength(int total) {
                int start, end;

                if(GetVisibleRange(total, out start, out end)) {
                    return end - start;
                }

                return total;
            }

            internal int GetPageForIndex(int index) {
                return UsePagination ? Mathf.FloorToInt(index / (float) PageSize) : 0;
            }

            internal int GetPageCount(int total) {
                return UsePagination ? Mathf.CeilToInt(total / (float) PageSize) : 1;
            }

            internal bool GetVisibleRange(int total, out int start, out int end) {
                if(UsePagination) {
                    int size = PageSize;

                    start = Mathf.Clamp(page * size, 0, total - 1);
                    end = Mathf.Min(start + size, total);
                    return true;
                }

                start = 0;
                end = total;
                return false;
            }
        }

        //
        // -- SELECTION --
        //

        private class ListSelection : IEnumerable<int> {
            private readonly List<int> _indexes;

            internal int? firstSelected;

            public ListSelection() {
                _indexes = new List<int>();
            }

            public ListSelection(int[] indexes) {
                _indexes = new List<int>(indexes);
            }

            public int First => _indexes.Count > 0 ? _indexes[0] : -1;

            public int Last => _indexes.Count > 0 ? _indexes[_indexes.Count - 1] : -1;

            public int Length => _indexes.Count;

            public int this[int index] {
                get => _indexes[index];
                set {
                    int oldIndex = _indexes[index];

                    _indexes[index] = value;

                    if(oldIndex == firstSelected) {
                        firstSelected = value;
                    }
                }
            }

            public bool Contains(int index) {
                return _indexes.Contains(index);
            }

            public void Clear() {
                _indexes.Clear();
                firstSelected = null;
            }

            public void SelectWhenNoAction(int index, Event evt) {
                if(!EditorGUI.actionKey && !evt.shift) {
                    Select(index);
                }
            }

            public void Select(int index) {
                _indexes.Clear();
                _indexes.Add(index);

                firstSelected = index;
            }

            public void Remove(int index) {
                if(_indexes.Contains(index)) {
                    _indexes.Remove(index);
                }
            }

            public void AppendWithAction(int index, Event evt) {
                if(EditorGUI.actionKey) {
                    if(Contains(index)) {
                        Remove(index);
                    } else {
                        Append(index);
                        firstSelected = index;
                    }
                } else if(evt.shift && _indexes.Count > 0 && firstSelected.HasValue) {
                    _indexes.Clear();

                    AppendRange(firstSelected.Value, index);
                } else if(!Contains(index)) {
                    Select(index);
                }
            }

            public void Sort() {
                if(_indexes.Count > 0) {
                    _indexes.Sort();
                }
            }

            public void Sort(System.Comparison<int> comparison) {
                if(_indexes.Count > 0) {
                    _indexes.Sort(comparison);
                }
            }

            public int[] ToArray() {
                return _indexes.ToArray();
            }

            public ListSelection Clone() {
                ListSelection clone = new ListSelection(ToArray());
                clone.firstSelected = firstSelected;

                return clone;
            }

            internal void Trim(int min, int max) {
                int i = _indexes.Count;

                while(--i > -1) {
                    int index = _indexes[i];

                    if(index < min || index >= max) {
                        if(index == firstSelected && i > 0) {
                            firstSelected = _indexes[i - 1];
                        }

                        _indexes.RemoveAt(i);
                    }
                }
            }

            internal bool CanRevert(SerializedProperty list) {
                if(list.serializedObject.targetObjects.Length == 1) {
                    for(int i = 0; i < Length; i++) {
                        if(list.GetArrayElementAtIndex(this[i]).isInstantiatedPrefab) {
                            return true;
                        }
                    }
                }

                return false;
            }

            internal void RevertValues(object userData) {
                SerializedProperty list = userData as SerializedProperty;

                for(int i = 0; i < Length; i++) {
                    SerializedProperty property = list.GetArrayElementAtIndex(this[i]);

                    if(property.isInstantiatedPrefab) {
                        property.prefabOverride = false;
                    }
                }

                list.serializedObject.ApplyModifiedProperties();
                list.serializedObject.Update();

                HandleUtility.Repaint();
            }

            internal void Duplicate(SerializedProperty list) {
                int offset = 0;

                for(int i = 0; i < Length; i++) {
                    this[i] += offset;

                    list.GetArrayElementAtIndex(this[i]).DuplicateCommand();
                    list.serializedObject.ApplyModifiedProperties();
                    list.serializedObject.Update();

                    offset++;
                }

                HandleUtility.Repaint();
            }

            internal void Delete(SerializedProperty list) {
                Sort();

                int i = Length;

                while(--i > -1) {
                    list.GetArrayElementAtIndex(this[i]).DeleteCommand();
                }

                Clear();

                list.serializedObject.ApplyModifiedProperties();
                list.serializedObject.Update();

                HandleUtility.Repaint();
            }

            private void Append(int index) {
                if(index >= 0 && !_indexes.Contains(index)) {
                    _indexes.Add(index);
                }
            }

            private void AppendRange(int from, int to) {
                int dir = (int) Mathf.Sign(to - from);

                if(dir != 0) {
                    for(int i = from; i != to; i += dir) {
                        Append(i);
                    }
                }

                Append(to);
            }

            public IEnumerator<int> GetEnumerator() {
                return ((IEnumerable<int>) _indexes).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return ((IEnumerable<int>) _indexes).GetEnumerator();
            }
        }

        //
        // -- SORTING --
        //

        private static class ListSort {
            private delegate int SortComparision(SerializedProperty p1, SerializedProperty p2);

            internal static void SortOnProperty(SerializedProperty list, int length, bool descending,
                string propertyName) {
                BubbleSort(list, length, (p1, p2) => {
                    SerializedProperty a = p1.FindPropertyRelative(propertyName);
                    SerializedProperty b = p2.FindPropertyRelative(propertyName);

                    if(a != null && b != null && a.propertyType == b.propertyType) {
                        int comparison = Compare(a, b, descending, a.propertyType);

                        return descending ? -comparison : comparison;
                    }

                    return 0;
                });
            }

            internal static void SortOnType(SerializedProperty list, int length, bool descending,
                SerializedPropertyType type) {
                BubbleSort(list, length, (p1, p2) => {
                    int comparision = Compare(p1, p2, descending, type);

                    return descending ? -comparision : comparision;
                });
            }

            //
            // -- PRIVATE --
            //

            private static void BubbleSort(SerializedProperty list, int length, SortComparision comparision) {
                for(int i = 0; i < length; i++) {
                    SerializedProperty p1 = list.GetArrayElementAtIndex(i);

                    for(int j = i + 1; j < length; j++) {
                        SerializedProperty p2 = list.GetArrayElementAtIndex(j);

                        if(comparision(p1, p2) > 0) {
                            list.MoveArrayElement(j, i);
                        }
                    }
                }
            }

            private static int Compare(SerializedProperty p1, SerializedProperty p2, bool descending,
                SerializedPropertyType type) {
                if(p1 == null || p2 == null) {
                    return 0;
                }

                switch(type) {
                    case SerializedPropertyType.Boolean:

                        return p1.boolValue.CompareTo(p2.boolValue);

                    case SerializedPropertyType.Character:
                    case SerializedPropertyType.Enum:
                    case SerializedPropertyType.Integer:
                    case SerializedPropertyType.LayerMask:

                        return p1.longValue.CompareTo(p2.longValue);

                    case SerializedPropertyType.Color:

                        return p1.colorValue.grayscale.CompareTo(p2.colorValue.grayscale);

                    case SerializedPropertyType.ExposedReference:

                        return CompareObjects(p1.exposedReferenceValue, p2.exposedReferenceValue, descending);

                    case SerializedPropertyType.Float:

                        return p1.doubleValue.CompareTo(p2.doubleValue);

                    case SerializedPropertyType.ObjectReference:

                        return CompareObjects(p1.objectReferenceValue, p2.objectReferenceValue, descending);

                    case SerializedPropertyType.String:

                        return p1.stringValue.CompareTo(p2.stringValue);

                    default:

                        return 0;
                }
            }

            private static int CompareObjects(Object obj1, Object obj2, bool descending) {
                if(obj1 && obj2) {
                    return obj1.name.CompareTo(obj2.name);
                }

                if(obj1) {
                    return @descending ? 1 : -1;
                }

                return descending ? -1 : 1;
            }
        }

        //
        // -- EXCEPTIONS --
        //

        private class InvalidListException : System.InvalidOperationException {
            public InvalidListException() : base("ExtendedReorderableList serializedProperty must be an array") { }
        }

        private class MissingListException : System.ArgumentNullException {
            public MissingListException() : base("ExtendedReorderableList serializedProperty is null") { }
        }

        //
        // -- INTERNAL --
        //

        private static class Internals {
            private static readonly MethodInfo DragDropValidation;
            private static object[] _dragDropValidationParams;
            private static readonly MethodInfo AppendDragDrop;
            private static object[] _appendDragDropParams;

            static Internals() {
                DragDropValidation = System.Type.GetType("UnityEditor.EditorGUI, UnityEditor")
                    .GetMethod("ValidateObjectFieldAssignment", BindingFlags.NonPublic | BindingFlags.Static);
                AppendDragDrop = typeof(SerializedProperty).GetMethod("AppendFoldoutPPtrValue",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            }

            internal static Object ValidateObjectDragAndDrop(Object[] references, SerializedProperty property) {
#if UNITY_2017_1_OR_NEWER
                _dragDropValidationParams = GetParams(ref _dragDropValidationParams, 4);
                _dragDropValidationParams[0] = references;
                _dragDropValidationParams[1] = null;
                _dragDropValidationParams[2] = property;
                _dragDropValidationParams[3] = 0;
#else
				dragDropValidationParams = GetParams(ref dragDropValidationParams, 3);
				dragDropValidationParams[0] = references;
				dragDropValidationParams[1] = null;
				dragDropValidationParams[2] = property;
#endif
                return DragDropValidation.Invoke(null, _dragDropValidationParams) as Object;
            }

            internal static void AppendDragAndDropValue(Object obj, SerializedProperty list) {
                _appendDragDropParams = GetParams(ref _appendDragDropParams, 1);
                _appendDragDropParams[0] = obj;
                AppendDragDrop.Invoke(list, _appendDragDropParams);
            }

            private static object[] GetParams(ref object[] parameters, int count) {
                if(parameters == null) {
                    parameters = new object[count];
                }

                return parameters;
            }
        }
    }
}