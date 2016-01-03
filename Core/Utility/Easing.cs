using UnityEngine;

namespace Elarion {

	public static class Easing {

		public static float Ease(float start, float end, float value, Ease easeType) {
			switch(easeType) {
				case Elarion.Ease.InQuad:
					return EaseInQuad(start, end, value);
				case Elarion.Ease.OutQuad:
					return EaseOutQuad(start, end, value);
				case Elarion.Ease.InOutQuad:
					return EaseInOutQuad(start, end, value);
				case Elarion.Ease.InCubic:
					return EaseInCubic(start, end, value);
				case Elarion.Ease.OutCubic:
					return EaseOutCubic(start, end, value);
				case Elarion.Ease.InOutCubic:
					return EaseInOutCubic(start, end, value);
				case Elarion.Ease.InQuart:
					return EaseInQuart(start, end, value);
				case Elarion.Ease.OutQuart:
					return EaseOutQuart(start, end, value);
				case Elarion.Ease.InOutQuart:
					return EaseInOutQuart(start, end, value);
				case Elarion.Ease.InQuint:
					return EaseInQuint(start, end, value);
				case Elarion.Ease.OutQuint:
					return EaseOutQuint(start, end, value);
				case Elarion.Ease.InOutQuint:
					return EaseInOutQuint(start, end, value);
				case Elarion.Ease.InSine:
					return EaseInSine(start, end, value);
				case Elarion.Ease.OutSine:
					return EaseOutSine(start, end, value);
				case Elarion.Ease.InOutSine:
					return EaseInOutSine(start, end, value);
				case Elarion.Ease.InExpo:
					return EaseInExpo(start, end, value);
				case Elarion.Ease.OutExpo:
					return EaseOutExpo(start, end, value);
				case Elarion.Ease.InOutExpo:
					return EaseInOutExpo(start, end, value);
				case Elarion.Ease.InCirc:
					return EaseInCirc(start, end, value);
				case Elarion.Ease.OutCirc:
					return EaseOutCirc(start, end, value);
				case Elarion.Ease.InOutCirc:
					return EaseInOutCirc(start, end, value);
				case Elarion.Ease.Linear:
					return Linear(start, end, value);
				case Elarion.Ease.Spring:
					return Spring(start, end, value);
				case Elarion.Ease.InBounce:
					return EaseInBounce(start, end, value);
				case Elarion.Ease.OutBounce:
					return EaseOutBounce(start, end, value);
				case Elarion.Ease.InOutBounce:
					return EaseInOutBounce(start, end, value);
				case Elarion.Ease.InBack:
					return EaseInBack(start, end, value);
				case Elarion.Ease.OutBack:
					return EaseOutBack(start, end, value);
				case Elarion.Ease.InOutBack:
					return EaseInOutBack(start, end, value);
				case Elarion.Ease.InElastic:
					return EaseInElastic(start, end, value);
				case Elarion.Ease.OutElastic:
					return EaseOutElastic(start, end, value);
				case Elarion.Ease.InOutElastic:
					return EaseInOutElastic(start, end, value);
				default:
					return Linear(start, end, value);
			}
		}

		private static float Linear(float start, float end, float value) {
			return Mathf.Lerp(start, end, value);
		}

		private static float Spring(float start, float end, float value) {
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}

		private static float EaseInQuad(float start, float end, float value) {
			end -= start;
			return end * value * value + start;
		}

		private static float EaseOutQuad(float start, float end, float value) {
			end -= start;
			return -end * value * (value - 2) + start;
		}

		private static float EaseInOutQuad(float start, float end, float value) {
			value /= .5f;
			end -= start;
			if(value < 1) return end / 2 * value * value + start;
			value--;
			return -end / 2 * (value * (value - 2) - 1) + start;
		}

		private static float EaseInCubic(float start, float end, float value) {
			end -= start;
			return end * value * value * value + start;
		}

		private static float EaseOutCubic(float start, float end, float value) {
			value--;
			end -= start;
			return end * (value * value * value + 1) + start;
		}

		private static float EaseInOutCubic(float start, float end, float value) {
			value /= .5f;
			end -= start;
			if(value < 1) return end / 2 * value * value * value + start;
			value -= 2;
			return end / 2 * (value * value * value + 2) + start;
		}

		private static float EaseInQuart(float start, float end, float value) {
			end -= start;
			return end * value * value * value * value + start;
		}

		private static float EaseOutQuart(float start, float end, float value) {
			value--;
			end -= start;
			return -end * (value * value * value * value - 1) + start;
		}

		private static float EaseInOutQuart(float start, float end, float value) {
			value /= .5f;
			end -= start;
			if(value < 1) return end / 2 * value * value * value * value + start;
			value -= 2;
			return -end / 2 * (value * value * value * value - 2) + start;
		}

		private static float EaseInQuint(float start, float end, float value) {
			end -= start;
			return end * value * value * value * value * value + start;
		}

		private static float EaseOutQuint(float start, float end, float value) {
			value--;
			end -= start;
			return end * (value * value * value * value * value + 1) + start;
		}

		private static float EaseInOutQuint(float start, float end, float value) {
			value /= .5f;
			end -= start;
			if(value < 1) return end / 2 * value * value * value * value * value + start;
			value -= 2;
			return end / 2 * (value * value * value * value * value + 2) + start;
		}

		private static float EaseInSine(float start, float end, float value) {
			end -= start;
			return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
		}

		private static float EaseOutSine(float start, float end, float value) {
			end -= start;
			return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
		}

		private static float EaseInOutSine(float start, float end, float value) {
			end -= start;
			return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
		}

		private static float EaseInExpo(float start, float end, float value) {
			end -= start;
			return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
		}

		private static float EaseOutExpo(float start, float end, float value) {
			end -= start;
			return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
		}

		private static float EaseInOutExpo(float start, float end, float value) {
			value /= .5f;
			end -= start;
			if(value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
			value--;
			return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
		}

		private static float EaseInCirc(float start, float end, float value) {
			end -= start;
			return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
		}

		private static float EaseOutCirc(float start, float end, float value) {
			value--;
			end -= start;
			return end * Mathf.Sqrt(1 - value * value) + start;
		}

		private static float EaseInOutCirc(float start, float end, float value) {
			value /= .5f;
			end -= start;
			if(value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
			value -= 2;
			return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
		}

		private static float EaseInBounce(float start, float end, float value) {
			end -= start;
			return end - EaseOutBounce(0, end, 1f - value) + start;
		}

		private static float EaseOutBounce(float start, float end, float value) {
			value /= 1f;
			end -= start;
			if(value < (1 / 2.75f)) {
				return end * (7.5625f * value * value) + start;
			} else if(value < (2 / 2.75f)) {
				value -= (1.5f / 2.75f);
				return end * (7.5625f * (value) * value + .75f) + start;
			} else if(value < (2.5 / 2.75)) {
				value -= (2.25f / 2.75f);
				return end * (7.5625f * (value) * value + .9375f) + start;
			} else {
				value -= (2.625f / 2.75f);
				return end * (7.5625f * (value) * value + .984375f) + start;
			}
		}

		private static float EaseInOutBounce(float start, float end, float value) {
			end -= start;
			float d = 1f;
			if(value < d / 2) return EaseInBounce(0, end, value * 2) * 0.5f + start;
			else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
		}

		private static float EaseInBack(float start, float end, float value) {
			end -= start;
			value /= 1;
			float s = 1.70158f;
			return end * (value) * value * ((s + 1) * value - s) + start;
		}

		private static float EaseOutBack(float start, float end, float value) {
			float s = 1.70158f;
			end -= start;
			value = (value / 1) - 1;
			return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
		}

		private static float EaseInOutBack(float start, float end, float value) {
			float s = 1.70158f;
			end -= start;
			value /= .5f;
			if((value) < 1) {
				s *= (1.525f);
				return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
			}
			value -= 2;
			s *= (1.525f);
			return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
		}

		private static float Punch(float amplitude, float value) {
			float s = 9;
			if(value == 0) {
				return 0;
			}
			if(value == 1) {
				return 0;
			}
			float period = 1 * 0.3f;
			s = period / (2 * Mathf.PI) * Mathf.Asin(0);
			return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
		}

		private static float EaseInElastic(float start, float end, float value) {
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;

			if(value == 0) return start;

			if((value /= d) == 1) return start + end;

			if(a == 0f || a < Mathf.Abs(end)) {
				a = end;
				s = p / 4;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		}

		private static float EaseOutElastic(float start, float end, float value) {
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;

			if(value == 0) return start;

			if((value /= d) == 1) return start + end;

			if(a == 0f || a < Mathf.Abs(end)) {
				a = end;
				s = p / 4;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
		}

		private static float EaseInOutElastic(float start, float end, float value) {
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s = 0;
			float a = 0;

			if(value == 0) return start;

			if((value /= d / 2) == 2) return start + end;

			if(a == 0f || a < Mathf.Abs(end)) {
				a = end;
				s = p / 4;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			if(value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
			return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
		}
	}


}
