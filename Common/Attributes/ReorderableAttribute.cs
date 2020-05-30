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

using System;
using UnityEngine;

namespace Elarion.Attributes {
    /// <summary>
    /// Use on arrays to make them easily reorderable in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReorderableAttribute : PropertyAttribute {
        public bool Add { get; set; } = true;
        public bool Remove { get; set; } = true;
        public bool Draggable { get; set; } = true;
        public bool SingleLine { get; set; } = false;
        public bool Paginate { get; set; } = false;
        public bool Sortable { get; set; } = true;
        public bool SceneReferences { get; set; } = false;
        public int PageSize { get; set; } = 10;
        public string ElementNameProperty { get; set; } = null;
        public string ElementNameOverride { get; set; } = null;
        public string ElementIconPath { get; set; } = null;
    }
}