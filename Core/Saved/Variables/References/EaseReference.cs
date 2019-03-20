﻿using System;

namespace Elarion.Saved.Variables.References {
    [Serializable]
    public class EaseReference : SavedValueReference<SavedEase, Ease> {
        
        public EaseReference(Ease value) : base(value) { }
        
        public static implicit operator Ease(EaseReference reference) {
            return reference?.Value ?? Ease.Linear;
        }
    }
}