using System;

namespace TBA.Api
{
    /// <inheritdoc />
    public sealed class MomentDto : IMomentDto
    {
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="id">The unique ID</param>
        /// <param name="journalId">The associated Journal ID</param>
        /// <param name="type">The content type -- TEXT, PHOTO, VIDEO</param>
        /// <param name="caption">Optional caption</param>
        /// <param name="localDate">The local date for this content, does not factor timezone</param>
        /// <param name="url">URL for the resource</param>
        public MomentDto(long id, long journalId, string type, string caption, DateTime localDate, string url)
        {
            // note: ensure a value is always assigned, so JSON property will be generated
            Id = id;
            JournalId = journalId;
            Type = type?.Trim() ?? string.Empty;
            Caption = caption?.Trim() ?? string.Empty;
            LocalDate = localDate.Date;
            Url = url?.Trim() ?? string.Empty;
        }

        /// <inheritdoc />
        public long Id { get; }

        /// <inheritdoc />
        public long JournalId { get; }

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public string Caption { get; }

        /// <inheritdoc />
        public DateTime LocalDate { get; }

        /// <inheritdoc />
        public string Url { get; }

        // public static Moment From
    }
}