using System;

namespace GoogleParser.Core.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Дата создания UTC
        /// </summary>
        public DateTime CreatedUtcDate { get; set; }

        /// <summary>
        /// Дата обновления UTC
        /// </summary>
        public DateTime? UpdatedUtcDate { get; set; }
    }
}
