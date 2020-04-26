namespace Units.Core.Parser.State
{
    public interface IUnit
    {
        string Name { get; }
        IUnit Simplify();
        /// <summary>
        /// Get the name defined only by base units
        /// </summary>
        /// <returns>Si name</returns>
        /// <remarks>
        ///     e.g.
        ///
        ///     We have 2 base units defined:
        ///
        ///     <code>
        ///Units(Base) := Length | Time
        ///     </code>
        ///     And 2 composit units:
        ///     <code>
        ///Velocity := Length / Time
        ///Acceleration := Velocity / Time
        ///     </code>
        ///     The SiNames will be:
        ///     <table>
        ///     <thead>
        ///         <tr>
        ///             <th>Unit</th>
        ///             <th>SiName</th>
        ///         </tr>
        ///     </thead>
        ///     <tbody>
        ///         <tr>  
        ///             <td>Length</td>
        ///             <td>Length</td>
        ///         </tr>
        ///         <tr>
        ///             <td>Time</td>
        ///             <td>Time</td>
        ///         </tr>
        ///         <tr>
        ///             <td>Velocity</td>
        ///             <td>LengthOverTime</td>
        ///         </tr>
        ///         <tr>
        ///             <td>Acceleration</td>
        ///             <td>LengthOverTime2</td>
        ///         </tr>
        ///     </tbody>
        ///     </table>
        /// </remarks>
        string SiName();
        IUnit WithSiName();
        bool IsInfered { get; }
    }
    public interface IUnit<T> : IUnit where T : IUnit<T>
    {
        G WithSiName<G>(bool cache) where G : T;
    }
}
