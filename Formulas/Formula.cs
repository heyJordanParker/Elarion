using System;
using UnityEngine;

namespace Elarion {

	[Serializable]
	public class Formula {

		public string name;
		public string[] parameters; //useless with the new parser, replace with parametersCount
		public string uncompiledFormula;
		public Func<float[], float> dDelegate;

		public static float Apply(string formulaName, params float[] arguments) {
			return Managers.Resources.Get<FormulaDatabase>().ApplyFormula(formulaName, arguments);
		}

		private static FormulaParser _parser;

		public float Apply(params float[] arguments) {
			if(parameters.Length != arguments.Length)
				throw new ArgumentException("The amount of arguments for the Eval call of the formula " + name + " is insufficient. The supplied arguments are " + arguments.Length + " but " + parameters.Length + " are needed.");
			return dDelegate(arguments);
		}

		public void Compile() {
			if(_parser == null) _parser = new FormulaParser();
			dDelegate = _parser.Compile(uncompiledFormula);
		}
	}
}
