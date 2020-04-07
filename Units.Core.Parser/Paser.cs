using System;
using System.Linq;
using Units.Core.Parser.State;

namespace Units.Core.Parser
{
    /// <summary>
    /// Parse your lines
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Given the <paramref name="lines"/> this method will handle
        /// </summary>
        /// <param name="lines">Lines of the code that can be handled by one of the handlers implementing <see cref="Handlers.IHandler"/></param>
        /// <returns>State of the parser after handling all lines</returns>
        /// <remarks>
        ///     Given the lines like this:
        ///     <code>
        /// Base(Unit) := Mass | Length | Time | Temperature | ElectricCurent | AmountOfSubstance | LuninusIntensity
        /// Operations := (*, Times) | (/, Per)
        /// SelfOps := (==, Eq, bool) | (!=, Ne, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)
        /// Real(Types) := (float, RealFloat)
        /// 
        /// Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
        /// Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
        /// 
        /// Speed := Length / Time
        /// Acceleration := Speed / Time
        /// Area := Length * Length
        /// Volume := Area * Length
        /// Force := Mass * Acceleration
        /// Pressure := Force / Area
        /// Energy := Force * Length
        /// 
        /// Infer
        /// 
        /// Unit(Mass) := gram(i, g)
        /// Unit(Length) := meter(i, m)
        /// Unit(Time) := second(i, s) | hour(i/3600, h) | minute(i/60, min)
        /// Unit(Temperature) := kelvin(i, K) | celsius(i + 273, ˙C)
        /// Unit(Force) := newton(i, N)
        ///</code>
        ///It will generate the state that has:
        ///
        ///         <see cref="ParserState.Units"/>
        ///
        ///         <see cref="ParserState.GraphEdges"/>
        ///
        ///         <see cref="ParserState.MesurmentUnits"/>
        ///
        ///         <see cref="ParserState.Operators"/>
        ///
        ///         <see cref="ParserState.RealDefs"/>
        ///
        ///         <see cref="ParserState.SelfOps"/>
        ///
        ///
        ///that can be converted to c# core -maybe code of other languages.
        /// </remarks>
        public static ParserState Parse(string[] lines)
        {
            var state = new ParserState();
            var handlers = Helpers.GetHandlers().ToList();
            var i = 0;
            foreach (var line in lines)
            {
                i++;
                var s = line;
                var handler = handlers.FirstOrDefault(j => j.MatchRegex(state).IsMatch(s));
                if (handler is null)
                {
                    Console.WriteLine($"Error on line {i}: {s} is not valid");
                    continue;
                }
                try
                {
                    handler.Handle(state, s);
                }
                catch (HandleException hx)
                {
                    Console.WriteLine($"[line {i}][error #{hx.ErrorCode}]: {hx.Message}\n    {line}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on line {i}: {ex}");
                }
            }
            state.Units.Remove(new Scalar());
            return state;
        }

    }
}
