using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Units.Core.Parser.Semantic;
using Units.Core.Parser.Semantic.Models;
using Units.Core.Parser.State;
#nullable enable
namespace Units.Core.Parser
{
    public class SemanticUnitsListener : UnitsGrammarBaseListener
    {
        private readonly SemanticClassManager _manager;
        private List<List<string>>? _argsList;
        private List<string>? _args;
        private string? _arg;
        private OperatorDef? _operator;
        private Left? _left;
        public SemanticUnitsListener(ParserState state)
        {
            _manager = new SemanticClassManager(state);
        }
        #region args
        public override void ExitArg(UnitsGrammarParser.ArgContext context)
        {
            _arg ??= string.Empty;
            _arg += context.GetText();
        }
        public override void ExitArg_esc(UnitsGrammarParser.Arg_escContext context)
        {
            _arg ??= string.Empty;
            _arg += context.GetText();
        }
        public override void EnterArgs(UnitsGrammarParser.ArgsContext context)
        {

            _args ??= new List<string>();
            if (_arg is string)
                _args.Add(_arg);
            _arg = null;
        }
        public override void EnterArgs_esc(UnitsGrammarParser.Args_escContext context)
        {
            _args ??= new List<string>();
            if (_arg is string)
                _args.Add(_arg);
            _arg = null;
        }
        public override void EnterArgslist([NotNull] UnitsGrammarParser.ArgslistContext context)
        {
            if (_args is { } && !string.IsNullOrEmpty(_arg))
                _args.Add(_arg);
            _arg = null;
            _argsList ??= new List<List<string>>();
            if (_args is { Count: { } count } && count > 0)
                _argsList.Add(_args);
            _args = new List<string>();
        }
        public override void ExitArgslist([NotNull] UnitsGrammarParser.ArgslistContext context)
        {
            if (_args is { } && !string.IsNullOrEmpty(_arg))
                _args.Add(_arg);
            _arg = null;
            _argsList ??= new List<List<string>>();
            if (_args is { Count: { } count } && count > 0)
                _argsList.Add(_args);
            _args = new List<string>();
        }
        public override void EnterStatments([NotNull] UnitsGrammarParser.StatmentsContext context)
        {
            _argsList ??= new List<List<string>>();
            _argsList.Clear();
        }
        #endregion
        #region operator
        public override void EnterOperator(UnitsGrammarParser.OperatorContext context)
        {
            _operator = new OperatorDef();
        }
        public override void ExitOperatorDef_Binary(UnitsGrammarParser.OperatorDef_BinaryContext context)
        {
            if (_operator is null)
                return;
            var letters = context.LETTER().Select(i => i.GetText()[0]).ToArray();
            var res = letters[0];
            var left = letters[1];
            var sym = context.VALID_OPERATORS().GetText()[0];
            var right = letters[2];
            _operator.Binaries.Add(new OperatorDef_Binary(res, left, sym, right));
        }
        public override void ExitOperatorDef_Unary([NotNull] UnitsGrammarParser.OperatorDef_UnaryContext context)
        {
            if (_operator is null)
                return;
            var letters = context.LETTER().Select(i => i.GetText()[0]).ToArray();
            var res = letters[0];
            var sym = context.WORD().GetText();
            var right = letters[1];
            _operator.Unaries.Add(new OperatorDef_Unary(res, sym, right));
        }
        public override void ExitOperator(UnitsGrammarParser.OperatorContext context)
        {
            if (_operator is null)
                return;
            _manager["Operator"]["*", 4].Invoke(context.GetChild(2).GetText(), _operator.GetOrder(), _operator.Binaries, _operator.Unaries);
        }
        #endregion
        public override void ExitCommand(UnitsGrammarParser.CommandContext context)
        {
            var name = context.GetRuleContext<UnitsGrammarParser.UnitNameContext>(0).GetText();
            if (_argsList == null || _argsList.Count == 0)
            {
                _manager[name]["*", 0].Invoke();
                return;
            }
            foreach (var args in _argsList)
            {
                if (args.Count > 1)
                {
                    _manager[name][args[0], args.Count - 1].Invoke(args.ToArray()[1..]);
                }
                else if (args.Count == 1)
                {
                    _manager[name][args[0], 0].Invoke();
                }
                else
                {
                    _manager[name]["*", 0].Invoke();
                }
            }
        }
        public override void ExitLeft([NotNull] UnitsGrammarParser.LeftContext context)
        {
            _left = new Left(
                context.GetChild<UnitsGrammarParser.UnitNameContext>(0).GetText(),
                context.GetChild<UnitsGrammarParser.UnitNameContext>(1).GetText());
        }
        public override void ExitExpr(UnitsGrammarParser.ExprContext context)
        {
            if (_argsList is null || _left is null)
                return;
            _manager[_left.ClassName][_left.MethodName].InvokeRaw(_left.MethodName, _argsList.ToArray());
        }
        public override void ExitNewUnit_Binary(UnitsGrammarParser.NewUnit_BinaryContext context)
        {
            _manager["NewUnit"]["Binary"].Invoke(context.WORD(0).GetText(), context.WORD(1).GetText(), context.VALID_OPERATORS().GetText(), context.WORD(2).GetText());
        }
        public override void ExitNewUnit_Unary(UnitsGrammarParser.NewUnit_UnaryContext context)
        {
            _manager["NewUnit"]["Unary"].Invoke(context.WORD(0).GetText(), context.WORD(1).GetText(), context.WORD(2).GetText());
        }
        public override void VisitErrorNode(Antlr4.Runtime.Tree.IErrorNode node)
        {
            base.VisitErrorNode(node);
            Console.WriteLine(node.GetText());
            Console.WriteLine(node.ToStringTree());
        }
    }
}
