using System;
using UnityEngine;

namespace Elarion.Utility.Colors {
    [Serializable]
    public struct ColorHSL {
        [SerializeField]
        private float _h;
        [SerializeField]
        private float _s;
        [SerializeField]
        private float _l;
        [SerializeField]
        private float _a;

        /// <summary>
        /// Hue
        /// </summary>
        public float H => _h;

        /// <summary>
        /// Saturation
        /// </summary>
        public float S => _s;

        /// <summary>
        /// Lightness
        /// </summary>
        public float L => _l;

        /// <summary>
        /// Alpha
        /// </summary>
        public float A => _a;

        public ColorHSL(float h, float s, float l, float a = 1.0f) {
            
            if((float.IsNaN(h) && !float.IsNaN(s)) ||
               float.IsNaN(s) && !float.IsNaN(h)) {
                Debug.LogException(new Exception("Gray ColorHSLs need both hue and saturation set to NaN."));
            }

            _h = float.IsNaN(h) ? h : Mathf.Clamp01(h);
            _s = float.IsNaN(s) ? s : Mathf.Clamp01(s);
            _l = Mathf.Clamp01(l);
            _a = Mathf.Clamp01(a);
        }

        public ColorHSL(ColorHSL hsl) : this(hsl.H, hsl.S, hsl.L, hsl.A) { }

        public ColorHSL(Color rgb) {
            float maxc = Mathf.Max(rgb.r, Mathf.Max(rgb.g, rgb.b));
            float minc = Mathf.Min(rgb.r, Mathf.Min(rgb.g, rgb.b));
            float delta = maxc - minc;

            float l = (maxc + minc) / 2.0f;
            float h = 0.0f;
            float s;

            if(maxc == minc) {
                _a = rgb.a;
                _h = float.NaN;
                _s = float.NaN;
                _l = l;
                return;
            }

            if(l < 0.5f) {
                s = delta / (maxc + minc);
            } else {
                s = delta / (2.0f - maxc - minc);
            }

            float rc = ((maxc - rgb.r) / 6.0f + delta / 2.0f) / delta;
            float gc = ((maxc - rgb.g) / 6.0f + delta / 2.0f) / delta;
            float bc = ((maxc - rgb.b) / 6.0f + delta / 2.0f) / delta;

            if(rgb.r == maxc) {
                h = bc - gc;
            } else if(rgb.g == maxc) {
                h = 1.0f / 3.0f + rc - bc;
            } else if(rgb.b == maxc) {
                h = 2.0f / 3.0f + gc - rc;
            }

            h = NormalizeHue(h);

            _h = h;
            _s = s;
            _l = l;
            _a = rgb.a;
        }

        public ColorHSL Add(float h, float s, float l) {
            return new ColorHSL(NormalizeHue(H + h),
                Mathf.Clamp01(S + s),
                Mathf.Clamp01(L + l));
        }

        public override string ToString() {
            return $"{GetType().Name}({H:0.00},{S:0.00},{L:0.00},{A:0.00})";
        }

        /// <summary>
        /// Converts the color back to the standard unity RGB colorspace.
        /// </summary>
        /// <returns>UnityEngine.Color.Color</returns>
        public Color ToColor() {
            float r;
            float g;
            float b;
            float a;

            if(float.IsNaN(H) || S == 0) {
                r = L;
                g = L;
                b = L;
                a = A;
            } else {
                float m2 = L < 0.5f ? L * (1.0f + S) : L + S - S * L;
                float m1 = 2.0f * L - m2;

                a = A;
                r = 1.0f * HueToRGB(m1, m2, H + 1.0f / 3.0f);
                g = 1.0f * HueToRGB(m1, m2, H);
                b = 1.0f * HueToRGB(m1, m2, H - 1.0f / 3.0f);
            }

            return new Color(r, g, b, a);
        }

        private static float HueToRGB(float m1, float m2, float h) {
            h = NormalizeHue(h);

            if(6.0 * h < 1.0) {
                return m1 + (m2 - m1) * 6.0f * h;
            }

            if(2.0 * h < 1.0) {
                return m2;
            }

            if(3.0 * h < 2.0) {
                return m1 + (m2 - m1) * (2.0f / 3.0f - h) * 6.0f;
            }

            return m1;
        }

        private static float NormalizeHue(float v) {
            const float min = 0.0f;
            const float max = 1.0f;

            if(min <= v && v < max) {
                return v;
            }

            return v - max * Mathf.Floor(v / max);
        }
    }
}