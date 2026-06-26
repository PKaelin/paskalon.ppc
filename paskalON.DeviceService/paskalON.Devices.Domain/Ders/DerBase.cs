using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Domain.Ders
{
    /// <summary>
    /// Base class for all distributed energy resources (DER).
    /// </summary>
    public abstract class DerBase
    {
        /// <summary>
        /// Name base configuration of this Distributed Energy Resource.
        /// </summary>
        private readonly NameBase _nameBase;


        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Id of the Distributed Energy Resource.
        /// </summary>
        public int Id { get => _nameBase.Id; }


        /// <summary>
        /// Name of the Distributed Energy Resource.
        /// </summary>
        public string Name { get => _nameBase.Name; }


        /// <summary>
        /// Constructor of <see cref="DerBase"/>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="nameBase">Name base configuration.</param>
        public DerBase(ILogger logger, NameBase nameBase)
        {
            _logger = logger;
            _nameBase = nameBase;
        }
    }
}
