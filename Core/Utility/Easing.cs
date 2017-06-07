using UnityEngine;

/*
 * TERMS OF USE - EASING EQUATIONS
 * Open source under the BSD License.
 * Copyright (c)2001 Robert Penner
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Elarion {

	public static class Easing {

		/// <summary>
		/// Returns the function associated to the easingFunction enum.
		/// </summary>
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

		/// <summary>
		/// Executes the derivative function of the appropriate easing function. If you use an easing function for position then this
		/// function can get you the speed at a given time (normalized).
		/// </summary>
		public static float EaseDerivative(float start, float end, float value, Ease easeType) {
			switch(easeType) {
				case Elarion.Ease.InQuad:
					return EaseInQuadD(start, end, value);
				case Elarion.Ease.OutQuad:
					return EaseOutQuadD(start, end, value);
				case Elarion.Ease.InOutQuad:
					return EaseInOutQuadD(start, end, value);
				case Elarion.Ease.InCubic:
					return EaseInCubicD(start, end, value);
				case Elarion.Ease.OutCubic:
					return EaseOutCubicD(start, end, value);
				case Elarion.Ease.InOutCubic:
					return EaseInOutCubicD(start, end, value);
				case Elarion.Ease.InQuart:
					return EaseInQuartD(start, end, value);
				case Elarion.Ease.OutQuart:
					return EaseOutQuartD(start, end, value);
				case Elarion.Ease.InOutQuart:
					return EaseInOutQuartD(start, end, value);
				case Elarion.Ease.InQuint:
					return EaseInQuintD(start, end, value);
				case Elarion.Ease.OutQuint:
					return EaseOutQuintD(start, end, value);
				case Elarion.Ease.InOutQuint:
					return EaseInOutQuintD(start, end, value);
				case Elarion.Ease.InSine:
					return EaseInSineD(start, end, value);
				case Elarion.Ease.OutSine:
					return EaseOutSineD(start, end, value);
				case Elarion.Ease.InOutSine:
					return EaseInOutSineD(start, end, value);
				case Elarion.Ease.InExpo:
					return EaseInExpoD(start, end, value);
				case Elarion.Ease.OutExpo:
					return EaseOutExpoD(start, end, value);
				case Elarion.Ease.InOutExpo:
					return EaseInOutExpoD(start, end, value);
				case Elarion.Ease.InCirc:
					return EaseInCircD(start, end, value);
				case Elarion.Ease.OutCirc:
					return EaseOutCircD(start, end, value);
				case Elarion.Ease.InOutCirc:
					return EaseInOutCircD(start, end, value);
				case Elarion.Ease.Linear:
					return LinearD(start, end, value);
				case Elarion.Ease.Spring:
					return SpringD(start, end, value);
				case Elarion.Ease.InBounce:
					return EaseInBounceD(start, end, value);
				case Elarion.Ease.OutBounce:
					return EaseOutBounceD(start, end, value);
				case Elarion.Ease.InOutBounce:
					return EaseInOutBounceD(start, end, value);
				case Elarion.Ease.InBack:
					return EaseInBackD(start, end, value);
				case Elarion.Ease.OutBack:
					return EaseOutBackD(start, end, value);
				case Elarion.Ease.InOutBack:
					return EaseInOutBackD(start, end, value);
				case Elarion.Ease.InElastic:
					return EaseInElasticD(start, end, value);
				case Elarion.Ease.OutElastic:
					return EaseOutElasticD(start, end, value);
				case Elarion.Ease.InOutElastic:
					return EaseInOutElasticD(start, end, value);
				default:
					return LinearD(start, end, value);
			}
		}

		private const float NATURAL_LOG_OF_2 = 0.693147181f;

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

		//
		// These are derived functions that the motor can use to get the speed at a specific time.
		//
		// The easing functions all work with a normalized time (0 to 1) and the returned value here
		// reflects that. Values returned here should be divided by the actual time.
		//
		// TODO: These functions have not had the testing they deserve. If there is odd behavior around
		//       dash speeds then this would be the first place I'd look.

		public static float LinearD(float start, float end, float value) {
			return end - start;
		}

		public static float EaseInQuadD(float start, float end, float value) {
			return 2f * (end - start) * value;
		}

		public static float EaseOutQuadD(float start, float end, float value) {
			end -= start;
			return -end * value - end * (value - 2);
		}

		public static float EaseInOutQuadD(float start, float end, float value) {
			value /= .5f;
			end -= start;

			if(value < 1) {
				return end * value;
			}

			value--;

			return end * (1 - value);
		}

		public static float EaseInCubicD(float start, float end, float value) {
			return 3f * (end - start) * value * value;
		}

		public static float EaseOutCubicD(float start, float end, float value) {
			value--;
			end -= start;
			return 3f * end * value * value;
		}

		public static float EaseInOutCubicD(float start, float end, float value) {
			value /= .5f;
			end -= start;

			if(value < 1) {
				return (3f / 2f) * end * value * value;
			}

			value -= 2;

			return (3f / 2f) * end * value * value;
		}

		public static float EaseInQuartD(float start, float end, float value) {
			return 4f * (end - start) * value * value * value;
		}

		public static float EaseOutQuartD(float start, float end, float value) {
			value--;
			end -= start;
			return -4f * end * value * value * value;
		}

		public static float EaseInOutQuartD(float start, float end, float value) {
			value /= .5f;
			end -= start;

			if(value < 1) {
				return 2f * end * value * value * value;
			}

			value -= 2;

			return -2f * end * value * value * value;
		}

		public static float EaseInQuintD(float start, float end, float value) {
			return 5f * (end - start) * value * value * value * value;
		}

		public static float EaseOutQuintD(float start, float end, float value) {
			value--;
			end -= start;
			return 5f * end * value * value * value * value;
		}

		public static float EaseInOutQuintD(float start, float end, float value) {
			value /= .5f;
			end -= start;

			if(value < 1) {
				return (5f / 2f) * end * value * value * value * value;
			}

			value -= 2;

			return (5f / 2f) * end * value * value * value * value;
		}

		public static float EaseInSineD(float start, float end, float value) {
			return (end - start) * 0.5f * Mathf.PI * Mathf.Sin(0.5f * Mathf.PI * value);
		}

		public static float EaseOutSineD(float start, float end, float value) {
			end -= start;
			return (Mathf.PI * 0.5f) * end * Mathf.Cos(value * (Mathf.PI * 0.5f));
		}

		public static float EaseInOutSineD(float start, float end, float value) {
			end -= start;
			return end * 0.5f * Mathf.PI * Mathf.Cos(Mathf.PI * value);
		}
		public static float EaseInExpoD(float start, float end, float value) {
			return (10f * NATURAL_LOG_OF_2 * (end - start) * Mathf.Pow(2f, 10f * (value - 1)));
		}

		public static float EaseOutExpoD(float start, float end, float value) {
			end -= start;
			return 5f * NATURAL_LOG_OF_2 * end * Mathf.Pow(2f, 1f - 10f * value);
		}

		public static float EaseInOutExpoD(float start, float end, float value) {
			value /= .5f;
			end -= start;

			if(value < 1) {
				return 5f * NATURAL_LOG_OF_2 * end * Mathf.Pow(2f, 10f * (value - 1));
			}

			value--;

			return (5f * NATURAL_LOG_OF_2 * end) / (Mathf.Pow(2f, 10f * value));
		}

		public static float EaseInCircD(float start, float end, float value) {
			return ((end - start) * value) / Mathf.Sqrt(1f - value * value);
		}

		public static float EaseOutCircD(float start, float end, float value) {
			value--;
			end -= start;
			return (-end * value) / Mathf.Sqrt(1f - value * value);
		}

		public static float EaseInOutCircD(float start, float end, float value) {
			value /= .5f;
			end -= start;

			if(value < 1) {
				return (end * value) / (2f * Mathf.Sqrt(1f - value * value));
			}

			value -= 2;

			return (-end * value) / (2f * Mathf.Sqrt(1f - value * value));
		}

		public static float EaseInBounceD(float start, float end, float value) {
			end -= start;
			float d = 1f;

			return EaseOutBounceD(0, end, d - value);
		}

		public static float EaseOutBounceD(float start, float end, float value) {
			value /= 1f;
			end -= start;

			if(value < (1 / 2.75f)) {
				return 2f * end * 7.5625f * value;
			} else if(value < (2 / 2.75f)) {
				value -= (1.5f / 2.75f);
				return 2f * end * 7.5625f * value;
			} else if(value < (2.5 / 2.75)) {
				value -= (2.25f / 2.75f);
				return 2f * end * 7.5625f * value;
			} else {
				value -= (2.625f / 2.75f);
				return 2f * end * 7.5625f * value;
			}
		}

		public static float EaseInOutBounceD(float start, float end, float value) {
			end -= start;
			float d = 1f;

			if(value < d * 0.5f) {
				return EaseInBounceD(0, end, value * 2) * 0.5f;
			} else {
				return EaseOutBounceD(0, end, value * 2 - d) * 0.5f;
			}
		}

		public static float EaseInBackD(float start, float end, float value) {
			// Since the motor only cares about final speed, we only consider that part of the bounce function.
			float s = 1.70158f;

			return 3f * (s + 1f) * (end - start) * value * value - 2f * s * (end - start) * value;
		}

		public static float EaseOutBackD(float start, float end, float value) {
			float s = 1.70158f;
			end -= start;
			value = (value) - 1;

			return end * ((s + 1f) * value * value + 2f * value * ((s + 1f) * value + s));
		}

		public static float EaseInOutBackD(float start, float end, float value) {
			float s = 1.70158f;
			end -= start;
			value /= .5f;

			if((value) < 1) {
				s *= (1.525f);
				return 0.5f * end * (s + 1) * value * value + end * value * ((s + 1f) * value - s);
			}

			value -= 2;
			s *= (1.525f);
			return 0.5f * end * ((s + 1) * value * value + 2f * value * ((s + 1f) * value + s));
		}

		public static float EaseInElasticD(float start, float end, float value) {
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s;
			float a = 0;

			if(a == 0f || a < Mathf.Abs(end)) {
				a = end;
				s = p / 4;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			float c = 2 * Mathf.PI;

			// From an online derivative calculator, kinda hoping it is right.
			return ((-a) * d * c * Mathf.Cos((c * (d * (value - 1f) - s)) / p)) / p -
				5f * NATURAL_LOG_OF_2 * a * Mathf.Sin((c * (d * (value - 1f) - s)) / p) *
				Mathf.Pow(2f, 10f * (value - 1f) + 1f);
		}

		public static float EaseOutElasticD(float start, float end, float value) {
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s;
			float a = 0;

			if(a == 0f || a < Mathf.Abs(end)) {
				a = end;
				s = p * 0.25f;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			return (a * Mathf.PI * d * Mathf.Pow(2f, 1f - 10f * value) *
				Mathf.Cos((2f * Mathf.PI * (d * value - s)) / p)) / p - 5f * NATURAL_LOG_OF_2 * a *
				Mathf.Pow(2f, 1f - 10f * value) * Mathf.Sin((2f * Mathf.PI * (d * value - s)) / p);
		}

		public static float EaseInOutElasticD(float start, float end, float value) {
			end -= start;

			float d = 1f;
			float p = d * .3f;
			float s;
			float a = 0;

			if(a == 0f || a < Mathf.Abs(end)) {
				a = end;
				s = p / 4;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
			}

			if(value < 1) {
				value -= 1;

				return -5f * NATURAL_LOG_OF_2 * a * Mathf.Pow(2f, 10f * value) * Mathf.Sin(2 * Mathf.PI * (d * value - 2f) / p) -
					a * Mathf.PI * d * Mathf.Pow(2f, 10f * value) * Mathf.Cos(2 * Mathf.PI * (d * value - s) / p) / p;
			}

			value -= 1;

			return a * Mathf.PI * d * Mathf.Cos(2f * Mathf.PI * (d * value - s) / p) / (p * Mathf.Pow(2f, 10f * value)) -
				5f * NATURAL_LOG_OF_2 * a * Mathf.Sin(2f * Mathf.PI * (d * value - s) / p) / (Mathf.Pow(2f, 10f * value));
		}

		public static float SpringD(float start, float end, float value) {
			value = Mathf.Clamp01(value);
			end -= start;

			// Damn... Thanks http://www.derivative-calculator.net/
			return end * (6f * (1f - value) / 5f + 1f) * (-2.2f * Mathf.Pow(1f - value, 1.2f) *
				Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + Mathf.Pow(1f - value, 2.2f) *
				(Mathf.PI * (2.5f * value * value * value + 0.2f) + 7.5f * Mathf.PI * value * value * value) *
				Mathf.Cos(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + 1f) -
				6f * end * (Mathf.Pow(1 - value, 2.2f) * Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + value
				/ 5f);

		}
	}


}
