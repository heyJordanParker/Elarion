using System;
using System.Collections.Generic;

namespace Elarion {

	[Serializable]
	public class FormulaDatabase : Database {

		public List<Formula> formulas;
		private SerializableDictionary<string, Formula> _formulaIndex;

		public override void Initialize() {
			if(formulas != null) foreach(var formula in formulas) formula.Compile();
			_formulaIndex = new SerializableDictionary<string, Formula>();
			foreach(var formula in formulas) {
				_formulaIndex.Add(formula.name, formula);
			}
		}

		public float ApplyFormula(string formula, float[] arguments) {
			if(_formulaIndex == null) throw new NullReferenceException("Formula Database has not been initialized. Evaluating formula failed.");
			var formulaObject = _formulaIndex.Get(formula);
			if(formulaObject == null) throw new ArgumentNullException("No formula with name " + formula + " found.");
			return formulaObject.Apply(arguments);
		}
	}
}