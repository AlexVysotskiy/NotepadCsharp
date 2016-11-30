using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notepad.Models.Query;

namespace Notepad.Models.Repository
{
    /**
     * Interface for repositories, defined basic mandatory operations
     */ 
    public interface IRepository<T>
    {
        /**
         * Initial configurations of repo
         */ 
        void Init();

        /**
         * Request for list of entities.
         * Response must return defined count of entities with offset.
         * Ordering and search options is not mandatory, but it invokes if passed.
         */ 
        List<T> SelectEntries(int page = 0, int count = 30, ConditionCollection conditions = null, Ordering order = null);

        /**
         * Look for entity with passed identifier
         */
        T Find(int id);

        /**
         * Add new entity
         */ 
        bool Store(T newEntry);

        /**
         * Update existed entity in repository
         */ 
        bool Update(T entry);

        /**
         * Remove entities with passed identifiers.
         */ 
        bool DeleteEntries(int[] ids);

        /**
         * Clear used resource 
         */ 
        void Dispose();


    }
}
