using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Elarion.Editor.Plugins.AutocompleteSearchField {
    [Serializable]
    public class AutocompleteSearchField {
        private static class Styles {
            public const float resultHeight = 20f;
            public const float resultsBorderWidth = 5f;
            public const float resultsMargin = 12f;
            public const float resultsLabelOffset = 2f;

            public static readonly GUIStyle entryEven;
            public static readonly GUIStyle entryOdd;
            public static readonly GUIStyle labelStyle;
            public static readonly GUIStyle resultsBorderStyle;

            static Styles() {
                entryOdd = new GUIStyle("CN EntryBackOdd");
                entryEven = new GUIStyle("CN EntryBackEven");
                resultsBorderStyle = new GUIStyle("hostview");

                labelStyle = new GUIStyle(EditorStyles.label) {
                    alignment = TextAnchor.MiddleLeft,
                    richText = true
                };
            }
        }

        public Action<string> onInputChanged;
        public Action<string> onConfirm;
        public Action onCancel;
        public string searchString = "";
        public int maxResults = 15;
        public bool inPropertyDrawer = false;

        [SerializeField]
        private List<string> results = new List<string>();

        [SerializeField]
        private int selectedIndex = -1;

        public SearchField searchField;

        private Vector2 previousMousePosition;
        private bool selectedIndexByMouse;

        private bool showResults;

        public void AddResult(string result) {
            results.Add(result);
        }

        public void ClearResults() {
            results.Clear();
        }

        public void OnToolbarGUI() {
            Draw(asToolbar: true);
        }

        public void OnGUI() {
            Draw(asToolbar: false);
        }

        public void Draw(bool asToolbar) {
            Rect rect = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
            Draw(rect, asToolbar);
        }

        public void Draw(Rect rect, bool asToolbar = false) {
            var resultsRect = rect;
            resultsRect.y += 18;
            Draw(rect, resultsRect, asToolbar);
        }

        // this might be optimized at some point - it does draw everything twice
        public void Draw(Rect rect, Rect resultsRect, bool asToolbar) {
            GUILayout.BeginHorizontal();
            DoSearchField(rect, asToolbar);
            GUILayout.EndHorizontal();

            if(results.Count <= 0 || !showResults) {
                return;
            }

            if(inPropertyDrawer) {
                // This handles mouse events properly
                DoResults(resultsRect, false, true);

                // this draws properly on top of everything else
                GenericInspector.GenericInspector.DrawAfterGUI += () => { DoResults(resultsRect, true, false); };
            } else {
                DoResults(resultsRect, true, true);
            }
        }

        private void DoSearchField(Rect rect, bool asToolbar) {
            if(searchField == null) {
                searchField = new SearchField();
                searchField.autoSetFocusOnFindCommand = false;
                searchField.downOrUpArrowKeyPressed += OnDownOrUpArrowKeyPressed;
            }

            var result = asToolbar
                ? searchField.OnToolbarGUI(rect, searchString)
                : searchField.OnGUI(rect, searchString);

            if(result != searchString && onInputChanged != null) {
                onInputChanged(result);
                selectedIndex = -1;
                showResults = true;
            }

            searchString = result;

            if(!inPropertyDrawer && HasSearchbarFocused()) {
                RepaintFocusedWindow();
            }
        }

        private void OnDownOrUpArrowKeyPressed() {
            var current = Event.current;

            if(current.keyCode == KeyCode.UpArrow) {
                current.Use();
                selectedIndex--;
                selectedIndexByMouse = false;
            } else {
                current.Use();
                selectedIndex++;
                selectedIndexByMouse = false;
            }

            if(selectedIndex >= results.Count) selectedIndex = results.Count - 1;
            else if(selectedIndex < 0) selectedIndex = -1;
        }

        private void DoResults(Rect rect, bool draw, bool processEvents) {
            if(results.Count <= 0 || !showResults) return;

            var current = Event.current;
            rect.height = Styles.resultHeight * Mathf.Min(maxResults, results.Count);
            rect.x += Styles.resultsMargin;
            rect.width -= Styles.resultsMargin * 2;

            var elementRect = rect;

            rect.height += Styles.resultsBorderWidth;
            GUI.Label(rect, "", Styles.resultsBorderStyle);

            var mouseIsInResultsRect = rect.Contains(current.mousePosition);

            if(mouseIsInResultsRect && draw) {
                RepaintFocusedWindow();
            }

            var movedMouseInRect = previousMousePosition != current.mousePosition;

            elementRect.x += Styles.resultsBorderWidth;
            elementRect.width -= Styles.resultsBorderWidth * 2;
            elementRect.height = Styles.resultHeight;

            var didJustSelectIndex = false;

            for(var i = 0; i < results.Count && i < maxResults; i++) {
                if(current.type == EventType.Repaint && draw) {
                    var style = i % 2 == 0 ? Styles.entryOdd : Styles.entryEven;

                    style.Draw(elementRect, false, false, i == selectedIndex, false);

                    var labelRect = elementRect;
                    labelRect.x += Styles.resultsLabelOffset;
                    GUI.Label(labelRect, results[i], Styles.labelStyle);
                }

                if(elementRect.Contains(current.mousePosition)) {
                    if(movedMouseInRect) {
                        selectedIndex = i;
                        selectedIndexByMouse = true;
                        didJustSelectIndex = true;
                    }

                    if(current.type == EventType.MouseDown && processEvents) {
                        OnConfirm(results[i]);
                    }
                }

                elementRect.y += Styles.resultHeight;
            }

            if(!processEvents) {
                return;
            }

            if(current.type == EventType.Repaint && !didJustSelectIndex && !mouseIsInResultsRect &&
               selectedIndexByMouse) {
                selectedIndex = -1;
            }

            if((GUIUtility.hotControl != searchField.searchFieldControlID && GUIUtility.hotControl > 0)
               || (current.rawType == EventType.MouseDown && !mouseIsInResultsRect)) {
                showResults = false;
                if(onCancel != null) {
                    onCancel();
                }
            }

            if(current.type == EventType.KeyUp && current.keyCode == KeyCode.Return && selectedIndex >= 0) {
                OnConfirm(results[selectedIndex]);
            }

            if(current.type == EventType.Repaint) {
                previousMousePosition = current.mousePosition;
            }
        }

        private void OnConfirm(string result) {
            searchString = result;
            if(onConfirm != null) onConfirm(result);
            if(onInputChanged != null) onInputChanged(result);
            ClearResults();
            GUIUtility.keyboardControl = 0; // To avoid Unity sometimes not updating the search field text
            RepaintFocusedWindow();
        }

        private bool HasSearchbarFocused() {
            return GUIUtility.keyboardControl == searchField.searchFieldControlID;
        }

        private static void RepaintFocusedWindow() {
            if(EditorWindow.focusedWindow != null) {
                EditorWindow.focusedWindow.Repaint();
            }
        }
    }
}