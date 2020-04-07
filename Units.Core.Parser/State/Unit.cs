using System;

namespace Units.Core.Parser.State
{
    /// <summary>
    /// Base unit
    /// </summary>
    /// <remarks>
    /// Maybe should be called BaseeUnit or BaseDimension or SiUnit...
    /// </remarks>
    public class Unit : IEquatable<Unit>
    {
        /// <summary>
        /// Name of the unit
        /// </summary>
        public string Name { get; set; }
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
        public virtual string SiName()
        {
            return Name;
        }
        /// <summary>
        /// Clone the unit
        /// </summary>
        /// <returns>The same unit but different reference</returns>
        public virtual Unit Clone()
        {
            return new Unit
            {
                Name = $"{Name}"
            };
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Unit);
        }

        public bool Equals(Unit other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
