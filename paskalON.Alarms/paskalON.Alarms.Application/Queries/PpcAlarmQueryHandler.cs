// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.Queries
{
    /// <summary>
    /// Reads the PPC alarms from the database.
    /// </summary>
    /// <remarks>
    /// The handler is scoped and only reads, it does not take the write lock of the alarm writers.
    /// The saved alarms are read, an alarm that is evaluated at the same time is listed with the next request.
    /// </remarks>
    public class PpcAlarmQueryHandler : IPpcAlarmQueryHandler
    {
        /// <summary>
        /// Repository of the alarms.
        /// </summary>
        private readonly IPpcAlarmRepository _alarmRepository;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmQueryHandler"/>.
        /// </summary>
        /// <param name="alarmRepository">Repository of the alarms.</param>
        public PpcAlarmQueryHandler(IPpcAlarmRepository alarmRepository)
        {
            ArgumentNullException.ThrowIfNull(alarmRepository);

            _alarmRepository = alarmRepository;
        }


        /// <inheritdoc/>
        public async Task<List<PpcAlarmDto>> GetAlarmsAsync(PpcAlarmFilterDto filter, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);

            List<PpcAlarm> alarms = await _alarmRepository.GetAsync(PpcAlarmDtoFactory.CreateFilter(filter), cancellationToken);

            return alarms.Select(PpcAlarmDtoFactory.Create).ToList();
        }
    }
}
