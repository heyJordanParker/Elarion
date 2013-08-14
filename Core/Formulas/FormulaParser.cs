using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Elarion {
	public class FormulaParser {
		private readonly Stack<Expression> _expressionStack = new Stack<Expression>();
		private readonly Stack<char> _operatorStack = new Stack<char>();
		private readonly List<string> _parameters = new List<string>();

		private Func<float[], float> Parse(string expression, out string[] parameters) {
			parameters = new string[0];
			if(string.IsNullOrEmpty(expression.Replace(" ", "")))
				return s => 0;

			var arrayParameter = Expression.Parameter(typeof(float[]), "args");

			_parameters.Clear();
			_operatorStack.Clear();
			_expressionStack.Clear();

			using(var reader = new StringReader(expression)) {
				int peek;
				while((peek = reader.Peek()) > -1) {
					var next = (char)peek;

					if(char.IsDigit(next)) {
						_expressionStack.Push(ReadOperand(reader));
						continue;
					}

					if(char.IsLetter(next)) {
						_expressionStack.Push(ReadParameterOrFunction(reader, arrayParameter));
						continue;
					}

					if(Operation.IsDefined(next)) {
						var currentOperation = ReadOperation(reader);

						EvaluateWhile(() => _operatorStack.Count > 0 && _operatorStack.Peek() != '(' &&
							currentOperation.Precedence <= ((Operation)_operatorStack.Peek()).Precedence);

						_operatorStack.Push(next);
						continue;
					}

					if(next == '(') {
						reader.Read();
						_operatorStack.Push('(');
						continue;
					}

					if(next == ')') {
						reader.Read();
						//backtrack, to the previous ',' or Peek().Containing '('
						//evaluate everything this far
						//if the last evaluated one was a ',' repeat
						//pass the formula the list of parameters and run its expression

						//evaluate backwards until ',' or Peek.Contains('(') is reached
						EvaluateWhile(() => _operatorStack.Count > 0 && _operatorStack.Peek() != '('); //Peek.Contains('(');
						//remove the '(' from the Peek, than get the function by name from the static function array, than evaluate the function with last expressionStat variable
						_operatorStack.Pop();
						continue;
					}

					if(next == ' ') {
						reader.Read();
					} else {
						throw new ArgumentException(string.Format("Encountered invalid character {0}", next),
							"expression");
					}
				}
			}

			EvaluateWhile(() => _operatorStack.Count > 0);

			var lambda = Expression.Lambda<Func<float[], float>>(_expressionStack.Pop(), arrayParameter);
			var compiled = lambda.Compile();
			parameters = _parameters.ToArray();
			return compiled;
		}

		public Func<float[], float> Compile(string expression) {
			string[] parameters;
			var compiled = Parse(expression, out parameters);
			int parametersCount = parameters.Length;
			Func<float[], float> result = arguments => Execute(compiled, arguments, parametersCount);
			return result;
		}

		public Func<float[], float> Compile(string expression, out string[] parameters) {
			var compiled = Parse(expression, out parameters);
			int parametersCount = parameters.Length;
			Func<float[], float> result = arguments => Execute(compiled, arguments, parametersCount);
			return result;
		}

		public float Evaluate(string expression, float[] arguments) {
			string[] parameters;
			var compiled = Parse(expression, out parameters);
			return Execute(compiled, arguments, parameters.Length);
		}

		private float Execute(Func<float[], float> compiled, float[] arguments, int parametersCount) {
			if(parametersCount != arguments.Length) {
				throw new ArgumentException(string.Format("Expression contains {0} parameters but got only {1}",
					parametersCount, arguments.Length));
			}

			return compiled(arguments);
		}


		private void EvaluateWhile(Func<bool> condition) {
			while(condition()) {
				var right = _expressionStack.Pop();
				var left = _expressionStack.Pop();

				_expressionStack.Push(((Operation)_operatorStack.Pop()).Apply(left, right));
			}
		}

		private static Expression ReadOperand(TextReader reader) {
			var operand = string.Empty;

			int peek;

			while((peek = reader.Peek()) > -1) {
				var next = (char)peek;

				if(char.IsDigit(next) || next == '.') {
					reader.Read();
					operand += next;
				} else {
					break;
				}
			}

			return Expression.Constant(float.Parse(operand));
		}

		private static Operation ReadOperation(TextReader reader) {
			var operation = (char)reader.Read();
			return (Operation)operation;
		}

		//Finish
		private Expression ReadParameterOrFunction(TextReader reader, Expression arrayParameter) {
			var parameter = string.Empty;

			int peek;

			while((peek = reader.Peek()) > -1) {
				var next = (char)peek;

				if(char.IsLetter(next)) {
					reader.Read();
					parameter += next;
				} else {
//					if(next == '(') this is a function - evaluate as one
					break;
				}
			}

			if(!_parameters.Contains(parameter)) {
				_parameters.Add(parameter);
			}

			return Expression.ArrayIndex(arrayParameter, Expression.Constant(_parameters.IndexOf(parameter)));
		}


		private static bool IsNumeric(Type type) {
			switch(Type.GetTypeCode(type)) {
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
			}
			return false;
		}

		private sealed class Function {

			//support for multiple arguments ( at least one )
			//parenthesis and other functions - exp(), pow() etc
			//finds the text ending with '(' than finds the closing ')' than evaluates till one expression remains, than evaluates this expression with the function expression
			//parenthesis will have a blank function expression
			//parameters

		}

		private sealed class Operation {
			private readonly int _precedence;
			private readonly Func<Expression, Expression, Expression> _operation;

			public static readonly Operation Addition = new Operation(1, Expression.Add);
			public static readonly Operation Subtraction = new Operation(1, Expression.Subtract);
			public static readonly Operation Multiplication = new Operation(2, Expression.Multiply);
			public static readonly Operation Division = new Operation(2, Expression.Divide);
			public static readonly Operation Modulo = new Operation(2, Expression.Modulo);
			public static readonly Operation Power = new Operation(3, Expression.Power);

			private static readonly Dictionary<char, Operation> Operations = new Dictionary<char, Operation>
			{
				{ '+', Addition },
				{ '-', Subtraction },
				{ '*', Multiplication},
				{ '/', Division },
				{ '%', Modulo },
				{ '^', Power }
			};

			private Operation(int precedence, Func<Expression, Expression, Expression> operation) {
				_precedence = precedence;
				_operation = operation;
			}

			public int Precedence {
				get { return _precedence; }
			}

			public static explicit operator Operation(char operation) {
				Operation result;

				if(Operations.TryGetValue(operation, out result))
					return result;
				throw new InvalidCastException();
			}

			public Expression Apply(Expression left, Expression right) {
				return _operation(left, right);
			}

			public static bool IsDefined(char operation) {
				return Operations.ContainsKey(operation);
			}
		}

	}

}