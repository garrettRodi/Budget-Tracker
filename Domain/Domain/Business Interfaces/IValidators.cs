using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates the entity and throws an exception if validation fails.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        void Validate(T entity);
    }
}
